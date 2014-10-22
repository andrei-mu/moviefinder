using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System.Threading.Tasks;
using System.Net;
using System.Text;
using System.Linq;
using System.Linq.Expressions;

namespace MovieLapsusTest
{
    //public class MovieDBQueriesMock : MovieLapsus.IMovieDBQueries
    //{
    //    public string actorSearch { get; set; }
    //    public string actorInfo { get; set; }

    //    async Task<string> MovieLapsus.IMovieDBQueries.GetActorInfoFromID(string actorID)
    //    {
    //        return await Task.Factory.StartNew(() => { return actorInfo; });
    //    }

    //    async Task<string> MovieLapsus.IMovieDBQueries.SearchForActor(string actorName)
    //    {
    //        return await Task.Factory.StartNew(() => { return actorSearch; });
    //    }
    //}

    [TestClass]
    public class UnitTest_MovieDBAPI
    {
        private static string BRAD_PITT_ID = "287";

        [TestMethod]
        public async Task Test_GetActorList()
        {
            var api = new MovieLapsus.MovieDBAPI();
            var queries = new MovieLapsus.MovieDBQueries();

            var actorList = await api.SearchForActor(queries, "pitt");

            Assert.AreEqual(actorList.results.Count, 20);
            Assert.AreEqual(actorList.total_pages, 7);
            Assert.AreEqual(actorList.total_results, 121);

            MovieLapsus.SearchActor_ActorInfo actorInfo = (
                        from ai in actorList.results
                        where ai.name == "Brad Pitt"
                        select ai).First();
        }

        [TestMethod]
        public async Task Test_GetActorList_BradPitt()
        {
            var api = new MovieLapsus.MovieDBAPI();
            var queries = new MovieLapsus.MovieDBQueries();

            var actorList = await api.SearchForActor(queries, "brad pitt");

            Assert.AreEqual(actorList.results.Count, 1);
            Assert.AreEqual(actorList.total_pages, 1);
            Assert.AreEqual(actorList.total_results, 1);

            MovieLapsus.SearchActor_ActorInfo actorInfo = (
                        from ai in actorList.results
                        where ai.name == "Brad Pitt"
                        select ai).First();
        }

        [TestMethod]
        public async Task Test_GetActorInfo_FromID()
        {
            var api = new MovieLapsus.MovieDBAPI();
            var queries = new MovieLapsus.MovieDBQueries();

            var actorInfo = await api.GetActorInfoFromID(queries, BRAD_PITT_ID);

            Assert.AreEqual(287, actorInfo.id);
            Assert.AreEqual(actorInfo.cast.Count, 55);

            MovieLapsus.MovieInfoByID_Cast castInfo = (
                        from ci in actorInfo.cast
                        where ci.id == 10917
                        select ci).First();

            Assert.AreEqual("Billy Canton", castInfo.character);
            Assert.AreEqual("Too Young to Die?", castInfo.original_title);
        }

        [TestMethod]
        public async Task Test_GetActorImages_FromID()
        {
            var api = new MovieLapsus.MovieDBAPI();
            var queries = new MovieLapsus.MovieDBQueries();

            var actorInfo = await api.GetActorImagesFromID(queries, BRAD_PITT_ID);

            Assert.AreEqual(287, actorInfo.id);
            Assert.IsTrue(actorInfo.profiles.Count > 0);
        }


        [TestMethod]
        public async Task Test_GetConfiguration()
        {
            var api = new MovieLapsus.MovieDBAPI();
            var queries = new MovieLapsus.MovieDBQueries();

            var config = await api.GetConfiguration(queries);

            Assert.IsTrue(config.images.base_url.Contains("tmdb.org"));
        }
    }
}
