using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System.Threading.Tasks;
using System.Net;
using System.Text;
using System.Linq;
using System.Linq.Expressions;

namespace MovieLapsusTest
{
    [TestClass]
    public class UnitTest_ACMOV
    {
        private static string BRAD_PITT_ID = "287";

        private MovieLapsus.TMDB.TMDBQueries queries;
        private MovieLapsus.TMDB.TMDBAPI api;

        UnitTest_ACMOV()
        {
            queries = new MovieLapsus.TMDB.TMDBQueries();
            api = new MovieLapsus.TMDB.TMDBAPI(queries);
        }

        [TestMethod]
        public async Task Test_GetNameNotExisting()
        {
            MovieLapsus.ACMOV.Actor.ClearCache();
            var actor = await MovieLapsus.ACMOV.Actor.GetActorFromID(BRAD_PITT_ID, api);

            Assert.AreEqual(actor.ID, BRAD_PITT_ID);
            Assert.AreEqual(actor.Name, "Brad Pitt");
        }
    }
}