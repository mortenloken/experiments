using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Snapshot {
	[PublicAPI]
	public interface ISnapshot<out TData> {
		SnapshotState State { get; }
		TData Data { get; }
		Task LoadAsync();
	}	

	[PublicAPI]
	public class Snapshot<TData> : ISnapshot<TData> {
		private readonly Func<Task<TData>> _dataFactory;
		private object _lock = new object();

		#region Constructor methods
		public Snapshot(Func<Task<TData>> dataFactory, TData initialData) {
			_dataFactory = dataFactory;
			State = SnapshotState.Empty;
			Data = initialData;
		}
		#endregion
		
		#region ISnapshot implementation
		public SnapshotState State { get; private set; }

		public TData Data { get; private set; }

		public async Task LoadAsync() {
			//state management
			lock (_lock) {
				if(State == SnapshotState.Loading) return;
				State = SnapshotState.Loading;
			}

			//load data from the factor
			try {
				Data = await _dataFactory();
				State = SnapshotState.Loaded;
			}
			catch (Exception) {
				State = SnapshotState.Failed;
			}
		}
		#endregion
	}
}