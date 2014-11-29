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
        private static string PULP_FICTION_ID = "680";

        private MovieLapsus.TMDB.TMDBQueries queries;
        private MovieLapsus.TMDB.TMDBAPI api;

        UnitTest_ACMOV()
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
        public async Task Test_GetNameNotExisting()
        {
            var actor = await MovieLapsus.ACMOV.TMDB_Actor.GetActorFromID(BRAD_PITT_ID, api);

            Assert.AreEqual(actor.ID, BRAD_PITT_ID);
            Assert.AreEqual(actor.Name, "Brad Pitt");

            return;
        }

        [TestMethod]
        public async Task Test_MovieFromID()
        {
            var movie = await MovieLapsus.ACMOV.TMDB_Movie.GetMovieFromID(PULP_FICTION_ID.ToString(), api);

            Assert.AreEqual(movie.ID, PULP_FICTION_ID);
            Assert.AreEqual(movie.Name, "Pulp Fiction");
            Assert.AreEqual(movie.ReleaseDate, "1994-10-14");
        }

        [TestMethod]
        public async Task Test_MovieFromString()
        {
            var movie = await MovieLapsus.ACMOV.TMDB_Movie.GetMovieFromString("pulp fiction", api);

            Assert.AreEqual(movie.ID, PULP_FICTION_ID);
            Assert.AreEqual(movie.Name, "Pulp Fiction");
            Assert.AreEqual(movie.ReleaseDate, "1994-10-14");
        }

        [TestMethod]
        public async Task Test_Movie_Characters()
        {
            await api.GetConfiguration();

            var movie = await MovieLapsus.ACMOV.TMDB_Movie.GetMovieFromID(PULP_FICTION_ID.ToString(), api);
            await movie.LoadCharacters(api);

            {
                var character = (
                    from ch in movie.Characters
                    where ch.CastId == 1
                    select ch).First();

                Assert.IsNotNull(character);
                Assert.AreEqual(character.ActorId, 62);
                Assert.AreEqual(character.Name, "Bruce Willis");
                Assert.AreEqual(character.Character, "Butch Coolidge");
            }

            {
                var character = (
                    from ch in movie.Characters
                    where ch.CastId == 2
                    select ch).First();

                Assert.IsNotNull(character);
                Assert.AreEqual(character.ActorId, 8891);
                Assert.AreEqual(character.Name, "John Travolta");
                Assert.AreEqual(character.Character, "Vincent Vega");
            } 
        }
    }
}