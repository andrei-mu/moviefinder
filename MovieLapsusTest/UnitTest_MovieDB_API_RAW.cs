using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System.Threading.Tasks;
using System.Net;

namespace imdbmov_testapp
{
    [TestClass]
    public class UnitTest_MovieDB_API_RAW
    {
        [TestMethod]
        public async Task Test_SearchForActor_List()
        {
            var api = new imdbmov.MovieDB_API_RAW();

            string ret = await api.SearchForActor("pitt");

            Assert.IsTrue(ret.Contains("Brad Pitt"));
            Assert.IsTrue(ret.Contains("Jacob Pitts"));
            Assert.IsTrue(ret.Contains("Michael Pitt"));
            Assert.IsTrue(ret.Contains("Ernst Pittschau"));
        }

        [TestMethod]
        public async Task Test_SearchForActor_ID()
        {
            var api = new imdbmov.MovieDB_API_RAW();

            string ret = await api.SearchForActor("brad pitt");

            Assert.IsTrue(ret.Contains("\"id\":287"));
            Assert.IsTrue(ret.Contains("Fight Club"));
        }

        [TestMethod]
        public async Task Test_ActorMovieList()
        {
            var api = new imdbmov.MovieDB_API_RAW();

            string ret = await api.GetActorInfoFromID("287");

            Assert.IsTrue(ret.Contains("Fight Club"));
            Assert.IsTrue(ret.Contains("Legends of the Fall"));
            Assert.IsTrue(ret.Contains("Too Young to Die?"));
            Assert.IsTrue(ret.Contains("The Curious Case of Benjamin Button"));
            Assert.IsTrue(ret.Contains("Sinbad: Legend of the Seven Seas"));
            Assert.IsTrue(ret.Contains("Johnny Suede"));
            
            
        }
    }
}
