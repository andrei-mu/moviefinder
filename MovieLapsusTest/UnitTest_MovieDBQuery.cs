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

        [TestMethod]
        public async Task Test_SearchForActor_List()
        {
            var api = new MovieLapsus.MovieDBQueries();

            string ret = await api.SearchForActor("pitt");

            Assert.IsTrue(ret.Contains("Brad Pitt"));
            Assert.IsTrue(ret.Contains("Jacob Pitts"));
            Assert.IsTrue(ret.Contains("Michael Pitt"));
            Assert.IsTrue(ret.Contains("Ernst Pittschau"));
        }

        [TestMethod]
        public async Task Test_SearchForActor_ID()
        {
            var api = new MovieLapsus.MovieDBQueries();

            string ret = await api.SearchForActor("brad pitt");

            Assert.IsTrue(ret.Contains("\"id\":287"));
            Assert.IsTrue(ret.Contains("Fight Club"));
        }

        [TestMethod]
        public async Task Test_ActorMovieListFromID()
        {
            var api = new MovieLapsus.MovieDBQueries();

            string ret = await api.GetActorInfoFromID(BRAD_PITT_ID);

            Assert.IsTrue(ret.Contains("Fight Club"));
            Assert.IsTrue(ret.Contains("Legends of the Fall"));
            Assert.IsTrue(ret.Contains("Too Young to Die?"));
            Assert.IsTrue(ret.Contains("The Curious Case of Benjamin Button"));
            Assert.IsTrue(ret.Contains("Sinbad: Legend of the Seven Seas"));
            Assert.IsTrue(ret.Contains("Johnny Suede"));
        }

        [TestMethod]
        public async Task Test_GetActorImages()
        {
            var api = new MovieLapsus.MovieDBQueries();

            string ret = await api.GetActorImagesFromID(BRAD_PITT_ID);

            Assert.IsTrue(ret.Contains("file_path"));
            Assert.IsTrue(ret.Contains("height"));
            Assert.IsTrue(ret.Contains("width"));
            Assert.IsTrue(ret.Contains(".jpg"));
            Assert.IsTrue(ret.Contains("profiles"));
        }

        [TestMethod]
        public async Task Test_GetConfig()
        {
            var api = new MovieLapsus.MovieDBQueries();

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
