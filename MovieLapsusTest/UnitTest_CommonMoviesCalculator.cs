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
    public class UnitTest_CommonMoviesCalculator
    {
        private static string BEN_STILLER_ID = "7399";
        private static string OWEN_WILSON_ID = "887";

        private MovieLapsus.TMDB.TMDBQueries queries;
        private MovieLapsus.TMDB.TMDBAPI api;

        UnitTest_CommonMoviesCalculator()
        {
            queries = new MovieLapsus.TMDB.TMDBQueries();
            api = new MovieLapsus.TMDB.TMDBAPI(queries);
        }

        [TestInitialize]
        public void Initialize()
        {
            MovieLapsus.ACMOV.TMDB_Actor.ClearCache();
            MovieLapsus.ACMOV.TMDB_Movie.ClearCache();
        }

        [TestMethod]
        public async Task Test_CommonMoviesSimple()
        {
            await api.GetConfiguration();
            var calc = new MovieLapsus.CommonCalculator(api);
            var result = await calc.CalculateCommonMovies(BEN_STILLER_ID, OWEN_WILSON_ID);

            Assert.AreEqual(11, result.Count());

            Assert.AreEqual("181533", result.First().ItemID());
            Assert.AreEqual("9894", result.Last().ItemID());

            {
                var movie = (
                    from ch in result
                    where ch.ItemID() == "22318"
                    select ch).First();

                Assert.IsNotNull(movie);
                Assert.AreEqual(movie.ItemName(), "Permanent Midnight");
            }
            {
                var movie = (
                    from ch in result
                    where ch.ItemID() == "9384"
                    select ch).First();

                Assert.IsNotNull(movie);
                Assert.AreEqual(movie.ItemName(), "Starsky & Hutch");
            }
            {
                var movie = (
                    from ch in result
                    where ch.ItemID() == "9398"
                    select ch).First();

                Assert.IsNotNull(movie);
                Assert.AreEqual(movie.ItemName(), "Zoolander");
            }

            return;
        }
    }
}