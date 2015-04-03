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
    public class UnitTest_CommonCalculator
    {
        private static string BEN_STILLER_ID = "7399";
        private static string OWEN_WILSON_ID = "887";

        private static string STARSKY_HUTCH_ID = "9384";
        private static string ZOOLANDER_ID = "9398";

        private MovieLapsus.TMDB.TMDBQueries queries;
        private MovieLapsus.TMDB.TMDBAPI api;

        UnitTest_CommonCalculator()
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
        [Ignore]
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

        [TestMethod]
        [Ignore]
        public async Task Test_CommonActorsSimple()
        {
            await api.GetConfiguration();
            var calc = new MovieLapsus.CommonCalculator(api);
            var result = await calc.CalculateCommonActors(STARSKY_HUTCH_ID, ZOOLANDER_ID);

            Assert.AreEqual(4, result.Count());

            Assert.AreEqual("23659", result.First().ItemID());
            Assert.AreEqual("887", result.Last().ItemID());

            {
                var actor = (
                    from ch in result
                    where ch.ItemID() == "7399"
                    select ch).First();

                Assert.IsNotNull(actor);
                Assert.AreEqual(actor.ItemName(), "Ben Stiller");
            }

            {
                var actor = (
                    from ch in result
                    where ch.ItemID() == "887"
                    select ch).First();

                Assert.IsNotNull(actor);
                Assert.AreEqual(actor.ItemName(), "Owen Wilson");
            }

            {
                var actor = (
                    from ch in result
                    where ch.ItemID() == "4937"
                    select ch).First();

                Assert.IsNotNull(actor);
                Assert.AreEqual(actor.ItemName(), "Vince Vaughn");
            }

            {
                var actor = (
                    from ch in result
                    where ch.ItemID() == "23659"
                    select ch).First();

                Assert.IsNotNull(actor);
                Assert.AreEqual(actor.ItemName(), "Will Ferrell");
            }
        }

    }
}