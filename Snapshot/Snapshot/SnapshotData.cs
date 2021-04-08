using System;
using JetBrains.Annotations;

namespace Snapshot {
	[PublicAPI]
	public class SnapshotData<TData> {
		public TData Value { get; }
		public DateTime Timestamp { get; }
		
		#region Constructor methods
		public SnapshotData(TData value) {
			Value = value;
			Timestamp = DateTime.Now;
		}
		#endregion
	}
}