﻿using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System.Threading.Tasks;
using System.Net;
using System.Text;
using System.Linq;
using System.Linq.Expressions;

namespace MovieLapsusTest
{
    [TestClass]
    public class UnitTest_MovieDBAPI
    {
        private static int BRAD_PITT_ID_INT = 287;
        private static int JENNA_DAVIS_ID_INT = 1181646;
        private static string BRAD_PITT_ID = BRAD_PITT_ID_INT.ToString();
        private static string JENNA_DAVIS_ID = JENNA_DAVIS_ID_INT.ToString();

        private static int PULP_FICTION_ID_INT = 680;

        private static int AVATAR_ID_INT = 19995;

        private MovieLapsus.TMDB.TMDBQueries queries;
        private MovieLapsus.TMDB.TMDBAPI api;

        UnitTest_MovieDBAPI()
        {
            queries = new MovieLapsus.TMDB.TMDBQueries();
            api = new MovieLapsus.TMDB.TMDBAPI(queries);
        }

        [TestMethod]
        public async Task Test_GetActorBiography()
        {
            var actorBio = await api.GetActorBiographyFromID(BRAD_PITT_ID_INT.ToString());

            Assert.AreEqual(actorBio.adult, false);
            Assert.AreEqual(actorBio.birthday, "1963-12-18");
            Assert.AreEqual(actorBio.id, BRAD_PITT_ID_INT);
            Assert.AreEqual(actorBio.imdb_id, "nm0000093");
            Assert.AreEqual(actorBio.name, "Brad Pitt");
        }

        [TestMethod]
        public async Task Test_GetMovieDescription()
        {
            var movieDesc = await api.GetMovieDescriptionFromID(PULP_FICTION_ID_INT.ToString());

            Assert.AreEqual(movieDesc.adult, false);
            Assert.AreEqual(movieDesc.imdb_id, "tt0110912");
            Assert.AreEqual(movieDesc.id, PULP_FICTION_ID_INT);
            Assert.AreEqual(movieDesc.original_title, "Pulp Fiction");
            Assert.AreEqual(movieDesc.poster_path, "/dM2w364MScsjFf8pfMbaWUcWrR.jpg");
            Assert.AreEqual(movieDesc.release_date, "1994-10-14");
        }

        [TestMethod]
        public async Task Test_GetMovieCredits()
        {
            var movieCredits = await api.GetMovieCreditsFromID(PULP_FICTION_ID_INT.ToString());

            Assert.AreEqual(movieCredits.id, PULP_FICTION_ID_INT);

            {
                MovieLapsus.MovieCredits_Cast actorInfo = (
                from ai in movieCredits.cast
                where ai.cast_id == 1
                select ai).First();

                Assert.AreEqual(actorInfo.name, "Bruce Willis");
                Assert.AreEqual(actorInfo.id, 62);
            }

            {
                MovieLapsus.MovieCredits_Cast actorInfo = (
                from ai in movieCredits.cast
                where ai.character == "Marsellus Wallace"
                select ai).First();

                Assert.AreEqual(actorInfo.name, "Ving Rhames");
                Assert.AreEqual(actorInfo.id, 10182);
            }
        }

        [TestMethod]
        public async Task Test_SearchForActor()
        {
            var actorList = await api.SearchForActor("pitt");

            Assert.AreEqual(20, actorList.results.Count);
            Assert.AreEqual(8, actorList.total_pages);
            Assert.IsTrue(actorList.total_results > 121);

            MovieLapsus.SearchActor_ActorInfo actorInfo = (
                        from ai in actorList.results
                        where ai.name == "Brad Pitt"
                        select ai).First();
        }

        [TestMethod]
        public async Task Test_SearchForActor_BradPitt()
        {
            var actorList = await api.SearchForActor("brad pitt");

            Assert.AreEqual(actorList.results.Count, 1);
            Assert.AreEqual(actorList.total_pages, 1);
            Assert.AreEqual(actorList.total_results, 1);

            MovieLapsus.SearchActor_ActorInfo actorInfo = (
                        from ai in actorList.results
                        where ai.name == "Brad Pitt"
                        select ai).First();
        }

        [TestMethod]
        public async Task Test_SearchForMovie()
        {
            var movieList = await api.SearchForMovie("avatar", true);

            var actorInfo = (
                        from ai in movieList.results
                        orderby ai.popularity descending
                        select ai).First();

            Assert.AreEqual(actorInfo.id, AVATAR_ID_INT);
        }

        [TestMethod]
        public async Task Test_GetActorMovies_FromID()
        {
            var actorInfo = await api.GetActorMoviesFromID(BRAD_PITT_ID);

            Assert.AreEqual(287, actorInfo.id);
            Assert.IsTrue(actorInfo.cast.Count > 70);

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
            var actorInfo = await api.GetActorImageListFromID(BRAD_PITT_ID);

            Assert.AreEqual(BRAD_PITT_ID_INT, actorInfo.id);
            Assert.IsTrue(actorInfo.profiles.Count > 0);
        }

        [TestMethod]
        public async Task Test_GetActorImages_FromIDWithoutImages()
        {
            var actorInfo = await api.GetActorImageListFromID(JENNA_DAVIS_ID);

            Assert.AreEqual(JENNA_DAVIS_ID_INT, actorInfo.id);
            Assert.IsTrue(actorInfo.profiles.Count == 0);
        }

        [TestMethod]
        public async Task Test_GetActorImage_FromID()
        {
            var imgPath = await api.GetActorImageFromID(BRAD_PITT_ID);

            Assert.AreEqual("http://image.tmdb.org/t/p/w45/kc3M04QQAuZ9woUvH3Ju5T7ZqG5.jpg", imgPath);
        }

        [TestMethod]
        public async Task Test_GetActorImage_FromIDWithoutImage()
        {
            var imgPath = await api.GetActorImageFromID(JENNA_DAVIS_ID);

            Assert.IsTrue(imgPath.Length == 0);
        }

        [TestMethod]
        public async Task Test_GetConfiguration()
        {
            var queries = new MovieLapsus.TMDB.TMDBQueries();
            var api = new MovieLapsus.TMDB.TMDBAPI(queries);

            var config = await api.GetConfiguration();

            Assert.IsTrue(config.images.base_url.Contains("tmdb.org"));
        }

        [TestMethod]
        public async Task Test_MakeActorPath()
        {
            await api.GetConfiguration();
            string actorPath = api.MakeActorPosterPath("/AAA");

            Assert.AreEqual(@"http://image.tmdb.org/t/p/w45/AAA", actorPath);
        }

        [TestMethod]
        public async Task Test_MakeMoviePath()
        {
            await api.GetConfiguration();
            string moviePath = api.MakeMoviePosterPath("/BBB");

            Assert.AreEqual(@"http://image.tmdb.org/t/p/w92/BBB", moviePath);
        }

    }
}
