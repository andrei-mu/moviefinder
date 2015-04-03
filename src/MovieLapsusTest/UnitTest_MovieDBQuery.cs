using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System.Threading.Tasks;
using System.Net;

namespace MovieLapsusTest
{
    [TestClass]
    public class UnitTest_MovieDBQuery
    {
        private static string BRAD_PITT_ID = "287";
        private static string PULP_FICTION_ID = "680";

        [TestMethod]
        public async Task Test_SearchForActor_List()
        {
            var api = new MovieLapsus.TMDB.TMDBQueries();

            string ret = await api.SearchForActor("pitt");

            Assert.IsTrue(ret.Contains("Brad Pitt"));
            Assert.IsTrue(ret.Contains("Michael Pitt"));
        }

        [TestMethod]
        [Ignore]
        public async Task Test_SearchForMovie_List()
        {
            var api = new MovieLapsus.TMDB.TMDBQueries();

            string ret = await api.SearchForMovie("fight", true);

            Assert.IsTrue(ret.Contains("Fight Club"));
            Assert.IsTrue(ret.Contains("The Fighter"));
        }

        [TestMethod]
        public async Task Test_SearchForActor_ID()
        {
            var api = new MovieLapsus.TMDB.TMDBQueries();

            string ret = await api.SearchForActor("brad pitt");

            Assert.IsTrue(ret.Contains("\"id\":287"));
            Assert.IsTrue(ret.Contains("Fight Club"));
        }

        [TestMethod]
        public async Task Test_ActorMovieListFromID()
        {
            var api = new MovieLapsus.TMDB.TMDBQueries();

            string ret = await api.GetActorMoviesFromID(BRAD_PITT_ID);

            Assert.IsTrue(ret.Contains("Fight Club"));
            Assert.IsTrue(ret.Contains("Legends of the Fall"));
            Assert.IsTrue(ret.Contains("Too Young to Die?"));
            Assert.IsTrue(ret.Contains("The Curious Case of Benjamin Button"));
            Assert.IsTrue(ret.Contains("Sinbad: Legend of the Seven Seas"));
            Assert.IsTrue(ret.Contains("Johnny Suede"));
        }

        [TestMethod]
        public async Task Test_ActorBiographyFromID()
        {
            var api = new MovieLapsus.TMDB.TMDBQueries();

            string ret = await api.GetActorBiographyFromID(BRAD_PITT_ID);

            Assert.IsTrue(ret.Contains("Brad Pitt"));
            Assert.IsTrue(ret.Contains("biography"));
            Assert.IsTrue(ret.Contains("imdb_id"));
            Assert.IsTrue(ret.Contains("nm0000093"));
            Assert.IsTrue(ret.Contains("name"));
            Assert.IsTrue(ret.Contains("popularity"));
            Assert.IsTrue(ret.Contains("profile_path"));
        }

        [TestMethod]
        public async Task Test_GetActorImages()
        {
            var api = new MovieLapsus.TMDB.TMDBQueries();

            string ret = await api.GetActorImagesFromID(BRAD_PITT_ID);

            Assert.IsTrue(ret.Contains("file_path"));
            Assert.IsTrue(ret.Contains("height"));
            Assert.IsTrue(ret.Contains("width"));
            Assert.IsTrue(ret.Contains(".jpg"));
            Assert.IsTrue(ret.Contains("profiles"));
        }

        [TestMethod]
        public async Task Test_MovieDescFromID()
        {
            var api = new MovieLapsus.TMDB.TMDBQueries();

            string ret = await api.GetMovieDescriptionFromID(PULP_FICTION_ID);

            Assert.IsTrue(ret.Contains("original_title"));
            Assert.IsTrue(ret.Contains("poster_path"));
            Assert.IsTrue(ret.Contains("imdb_id"));
            Assert.IsTrue(ret.Contains("release_date"));
            Assert.IsTrue(ret.Contains("tagline"));
            Assert.IsTrue(ret.Contains("popularity"));
        }

        [TestMethod]
        public async Task Test_MovieCreditsFromID()
        {
            var api = new MovieLapsus.TMDB.TMDBQueries();

            string ret = await api.GetMovieCreditsFromID(PULP_FICTION_ID);

            Assert.IsTrue(ret.Contains("\"id\""));
            Assert.IsTrue(ret.Contains("\"cast\""));
            Assert.IsTrue(ret.Contains("\"cast_id\""));
            Assert.IsTrue(ret.Contains("\"character\""));
            Assert.IsTrue(ret.Contains("\"profile_path\""));
            Assert.IsTrue(ret.Contains("\"crew\""));
        }
        [TestMethod]
        public async Task Test_GetMovieImages()
        {
            var api = new MovieLapsus.TMDB.TMDBQueries();

            string ret = await api.GetMovieImagesFromID(PULP_FICTION_ID);

            Assert.IsTrue(ret.Contains("id"));
            Assert.IsTrue(ret.Contains("backdrops"));
            Assert.IsTrue(ret.Contains("posters"));
            Assert.IsTrue(ret.Contains(".jpg"));
            Assert.IsTrue(ret.Contains("file_path"));
        }

        [TestMethod]
        public async Task Test_GetConfig()
        {
            var api = new MovieLapsus.TMDB.TMDBQueries();

            string ret = await api.GetConfiguration();

            Assert.IsTrue(ret.Contains("images"));
            Assert.IsTrue(ret.Contains("base_url"));
            Assert.IsTrue(ret.Contains("logo_sizes"));
            Assert.IsTrue(ret.Contains("poster_sizes"));
            Assert.IsTrue(ret.Contains("profile_sizes"));
            Assert.IsTrue(ret.Contains("still_sizes"));
            Assert.IsTrue(ret.Contains("change_keys"));
        }

    }
}
