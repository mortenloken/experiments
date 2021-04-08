using System;
using System.Threading.Tasks;

namespace Snapshot.Test.Mocks {
	public class PersonService {
		public Task<Person> GetPersonAsync() => Task.FromResult(new Person("Morten", "Løken"));
		public Task<Person> GetPersonFailingAsync()  => throw new Exception("Cannot get person");
	}
}