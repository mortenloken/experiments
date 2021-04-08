using JetBrains.Annotations;

namespace Snapshot {
	[PublicAPI]
	public enum SnapshotState {
		Empty,
		Loading,
		Loaded,
		Failed
	}
}