using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System.Runtime.Serialization.Json;
using System.IO;

namespace MovieLapsusTest
{
    [TestClass]
    public class UnitTest_JsonParserStructures
    {
        [TestMethod]
        public void Test_KnownForStructure()
        {
            string jsonData = "{\"adult\":false,";
            jsonData += "\"backdrop_path\":\"/4H6vE7fvB0gvaaSRJyguUu67FvF.jpg\",";
            jsonData += "\"id\":86838,";
            jsonData += "\"original_title\":\"Seven Psychopaths\",";
            jsonData += "\"release_date\":\"2012-10-12\",";
            jsonData += "\"poster_path\":\"/3Zs6Ne6QAEOzQRz85VToH8ml6ab.jpg\",";
            jsonData += "\"popularity\":0.926313979510703,";
            jsonData += "\"title\":\"Seven Psychopaths\",";
            jsonData += "\"vote_average\":6.3,";
            jsonData += "\"vote_count\":329,";
            jsonData += "\"media_type\":\"movie\"}";

            DataContractJsonSerializer js = new DataContractJsonSerializer(typeof(MovieLapsus.SearchActor_MovieInfo));
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonData));
            var kf = (MovieLapsus.SearchActor_MovieInfo)js.ReadObject(stream);

            Assert.IsFalse(kf.adult);
            Assert.AreEqual("/4H6vE7fvB0gvaaSRJyguUu67FvF.jpg", kf.backdrop_path);
            Assert.AreEqual(86838, kf.id);
            Assert.AreEqual("Seven Psychopaths", kf.original_title);
            Assert.AreEqual("2012-10-12", kf.release_date);
            Assert.AreEqual("/3Zs6Ne6QAEOzQRz85VToH8ml6ab.jpg", kf.poster_path);
            Assert.AreEqual("Seven Psychopaths", kf.title);
            Assert.AreEqual(329, kf.vote_count);
            Assert.AreEqual("movie", kf.media_type);
        }

        [TestMethod]
        public void Test_SearchInfoStructure_OneMovie()
        {
            string jsonData = "{";
            jsonData += "\"adult\":false,";
            jsonData += "\"id\":1172925,";
            jsonData += "\"known_for\":[";
            jsonData += "{";
            jsonData += "\"adult\":false,";
            jsonData += "\"backdrop_path\":null,";
            jsonData += "\"id\":192619,";
            jsonData += "\"original_title\":\"The Curse of the Gothic Symphony\",";
            jsonData += "\"release_date\":\"2011-06-30\",";
            jsonData += "\"poster_path\":\"/kcZlx0bpLBIR1cVyHtNSHSpeJFo.jpg\",";
            jsonData += "\"popularity\":0.0101634763682439,";
            jsonData += "\"title\":\"The Curse of the Gothic Symphony\",";
            jsonData += "\"vote_average\":0.0,";
            jsonData += "\"vote_count\":0,";
            jsonData += "\"media_type\":\"movie\"";
            jsonData += "}";
            jsonData += "],";
            jsonData += "\"name\":\"Kimberley Pitt\",";
            jsonData += "\"popularity\":5.31440999313696e-07,";
            jsonData += "\"profile_path\":null";
            jsonData += "}";

            DataContractJsonSerializer js = new DataContractJsonSerializer(typeof(MovieLapsus.SearchActor_ActorInfo));
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonData));
            var kf = (MovieLapsus.SearchActor_ActorInfo)js.ReadObject(stream);

            Assert.IsFalse(kf.adult);
            Assert.AreEqual(1172925, kf.id);
            Assert.AreEqual("Kimberley Pitt", kf.name);
            Assert.AreEqual(1, kf.known_for.Count);
        }

        [TestMethod]
        public void Test_SearchInfoStructure_ListMovie()
        {
            string jsonData = "{";
            jsonData += "\"adult\":true,";
            jsonData += "\"id\":1172935,";
            jsonData += "\"known_for\":[";
            jsonData += "{";
            jsonData += "\"adult\":false,";
            jsonData += "\"backdrop_path\":null,";
            jsonData += "\"id\":192619,";
            jsonData += "\"original_title\":\"The Curse of the Gothic Symphony\",";
            jsonData += "\"release_date\":\"2011-06-30\",";
            jsonData += "\"poster_path\":\"/kcZlx0bpLBIR1cVyHtNSHSpeJFo.jpg\",";
            jsonData += "\"popularity\":0.0101634763682439,";
            jsonData += "\"title\":\"The Curse of the Gothic Symphony\",";
            jsonData += "\"vote_average\":0.0,";
            jsonData += "\"vote_count\":0,";
            jsonData += "\"media_type\":\"movie\"";
            jsonData += "},";
            jsonData += "{";
            jsonData += "\"adult\":false,";
            jsonData += "\"backdrop_path\":null,";
            jsonData += "\"id\":1926191,";
            jsonData += "\"original_title\":\"The Curse of the Gothic Symphony2\",";
            jsonData += "\"release_date\":\"2011-06-30\",";
            jsonData += "\"poster_path\":\"/kcZlx0bpLBIR1cVyHtNSHSpeJFo.jpg\",";
            jsonData += "\"popularity\":0.0101634763682439,";
            jsonData += "\"title\":\"The Curse of the Gothic Symphony\",";
            jsonData += "\"vote_average\":0.0,";
            jsonData += "\"vote_count\":0,";
            jsonData += "\"media_type\":\"movie\"";
            jsonData += "}";
            jsonData += "],";
            jsonData += "\"name\":\"Kimberley Pitt\",";
            jsonData += "\"popularity\":5.31440999313696e-07,";
            jsonData += "\"profile_path\":null";
            jsonData += "}";

            DataContractJsonSerializer js = new DataContractJsonSerializer(typeof(MovieLapsus.SearchActor_ActorInfo));
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonData));
            var kf = (MovieLapsus.SearchActor_ActorInfo)js.ReadObject(stream);

            Assert.IsTrue(kf.adult);
            Assert.AreEqual(1172935, kf.id);
            Assert.AreEqual("Kimberley Pitt", kf.name);
            Assert.AreEqual(2, kf.known_for.Count);
        }
    }
}
