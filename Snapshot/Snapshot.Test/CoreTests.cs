using System;
using System.Threading.Tasks;
using Snapshot.Test.Mocks;
using Xunit;

namespace Snapshot.Test {
	public class CoreTests {
		private const string InitialData = nameof(InitialData);
		private const string LoadedData = nameof(LoadedData);
		
		[Fact]
		public async Task StringData() {
			var snapshot = new Snapshot<string>(
				async () => {
					await Task.Delay(1000);
					return LoadedData;
				},
				InitialData
			);

			Assert.Equal(SnapshotState.Empty, snapshot.State);
			Assert.Equal(InitialData, snapshot.Data.Value);
			await snapshot.LoadAsync();
			Assert.Equal(SnapshotState.Loaded, snapshot.State);
			Assert.Equal(LoadedData, snapshot.Data.Value);
		}
		
		[Fact]
		public async Task PersonService() {
			var snapshot = new Snapshot<Person>(
				() => new PersonService().GetPersonAsync(),
				new Person("No such", "Person")
			);

			Assert.Equal(SnapshotState.Empty, snapshot.State);
			await snapshot.LoadAsync();
			Assert.Equal(SnapshotState.Loaded, snapshot.State);
			var person = snapshot.Data.Value;
			Assert.Equal("Morten", person.FirstName);
			Assert.Equal("Løken", person.LastName);
		}
		
		[Fact]
		public async Task PersonServiceFailing() {
			var snapshot = new Snapshot<Person>(
				() => new PersonService().GetPersonFailingAsync(),
				new Person("No such", "Person")
			);

			Assert.Equal(SnapshotState.Empty, snapshot.State);
			await Assert.ThrowsAsync<Exception>(() => snapshot.LoadAsync());
			var person = snapshot.Data.Value;
			Assert.Equal(SnapshotState.Failed, snapshot.State);
			Assert.Equal("No such", person.FirstName);
			Assert.Equal("Person", person.LastName);
		}

	}
}