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
        public async Task Test_ActorFromID()
        {
            var actor = await MovieLapsus.ACMOV.TMDB_Actor.GetActorFromID(BRAD_PITT_ID, api);

            Assert.AreEqual(actor.ID, BRAD_PITT_ID);
            Assert.AreEqual(actor.Name, "Brad Pitt");
        }

        [TestMethod]
        public async Task Test_ActorFromName()
        {
            var actor = await MovieLapsus.ACMOV.TMDB_Actor.GetActorFromString("brad pitt", api);

            Assert.AreEqual(actor.ID, BRAD_PITT_ID);
            Assert.AreEqual(actor.Name, "Brad Pitt");
        }

        [TestMethod]
        public async Task Test_ActorMovies()
        {
            var actor = await MovieLapsus.ACMOV.TMDB_Actor.GetActorFromID(BRAD_PITT_ID, api);

            await actor.LoadCharacters(api);

            {
                var character = (
                    from ch in actor.Characters
                    where ch.MovieId == "16869"
                    select ch).First();

                Assert.IsNotNull(character);
                Assert.AreEqual(BRAD_PITT_ID.ToString(), character.ActorId);
                Assert.AreEqual("Brad Pitt", character.ActorName);
                Assert.IsTrue(character.CharacterName.IndexOf("Lieutenant Aldo") >= 0);

                Assert.AreEqual(character.MovieId, "16869");
                Assert.AreEqual(character.MovieName, "Inglourious Basterds");
            }
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
                    where ch.CastId == "1"
                    select ch).First();

                Assert.IsNotNull(character);
                Assert.AreEqual(character.ActorId, "62");
                Assert.AreEqual(character.ActorName, "Bruce Willis");
                Assert.AreEqual(character.CharacterName, "Butch Coolidge");

                Assert.AreEqual(character.MovieId, PULP_FICTION_ID.ToString());
                Assert.AreEqual(character.MovieName, "Pulp Fiction");
            }

            {
                var character = (
                    from ch in movie.Characters
                    where ch.CastId == "2"
                    select ch).First();

                Assert.IsNotNull(character);
                Assert.AreEqual(character.ActorId, "8891");
                Assert.AreEqual(character.ActorName, "John Travolta");
                Assert.AreEqual(character.CharacterName, "Vincent Vega");

                Assert.AreEqual(character.MovieId, PULP_FICTION_ID.ToString());
                Assert.AreEqual(character.MovieName, "Pulp Fiction");
            } 
        }
    }
}