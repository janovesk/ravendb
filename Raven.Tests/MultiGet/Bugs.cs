using Raven.Client.Document;
using Raven.Client.Linq;
using Raven.Tests.Bugs;
using Xunit;
using System.Linq;

namespace Raven.Tests.MultiGet
{
	public class Bugs : RemoteClientTest
	{
		[Fact]
		public void CanUseStats()
		{
			using (GetNewServer())
			using (var docStore = new DocumentStore { Url = "http://localhost:8080" }.Initialize())
			{
				using (var session = docStore.OpenSession())
				{
					session.Store(new User { Name = "Ayende" });
					session.Store(new User { Name = "Oren" });
					session.SaveChanges();
				}


				using (var session = docStore.OpenSession())
				{
					RavenQueryStatistics stats;
					session.Query<User>()
						.Statistics(out stats)
						.ToArray();

					session.Advanced.Eagerly.ExecuteAllPendingLazyOperations();

					Assert.Equal(2, stats.TotalResults);
				}
			}
		}
	}
}