using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Snapshot {
	/// <summary>
	/// A snapshot is a point-in-time representation of something loaded from some data factory.
	/// </summary>
	/// <typeparam name="TData">The type of the data to load.</typeparam>
	[PublicAPI]
	public interface ISnapshot<TData> {
		SnapshotState State { get; }
		SnapshotData<TData> Data { get; }
		Task LoadAsync();
	}

	[PublicAPI]
	public class Snapshot<TData> : ISnapshot<TData> {
		private readonly Func<Task<TData>> _dataFactory;
		private readonly SemaphoreSlim _semaphore;
		
		#region Constructor methods
		public Snapshot(Func<Task<TData>> dataFactory, TData initialData) {
			_dataFactory = dataFactory;
			_semaphore = new SemaphoreSlim(1);
			
			State = SnapshotState.Empty;
			Data = new SnapshotData<TData>(initialData);
		}
		#endregion
		
		#region ISnapshot implementation
		public SnapshotState State { get; private set; }

		public SnapshotData<TData> Data { get; private set; }

		public async Task LoadAsync() {
			//wait for the semaphore
			await _semaphore.WaitAsync();

			try {
				//state management
				if(State == SnapshotState.Loading) return;
				State = SnapshotState.Loading;

				//load data from the factory
				Data = new SnapshotData<TData>(await _dataFactory());
				State = SnapshotState.Loaded;
				
				//release the semaphore
				_semaphore.Release();
			}
			catch {
				//set state
				State = SnapshotState.Failed;
				
				//release the semaphore
				_semaphore.Release();

				//leak exception to caller
				throw;
			}
		}
		#endregion
	}
}