using System.Threading.Tasks;
using Xunit;

namespace Snapshot.Test {
	public class CoreTests {
		private const string InitialData = nameof(InitialData);
		private const string LoadedData = nameof(LoadedData);
		
		[Fact]
		public async Task Test1() {
			var snapshot = new Snapshot<string>(
				async () => {
					await Task.Delay(1000);
					return LoadedData;
				},
				InitialData
			);

			Assert.Equal(InitialData, snapshot.Data.Value);
			await snapshot.LoadAsync();
			Assert.Equal(LoadedData, snapshot.Data.Value);
		}
	}
}