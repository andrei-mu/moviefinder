using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;

namespace MovieLapsus
{
    namespace TMDB
    {
        public class TMDBAPI
        {
            private static DBConfig _configuration = null;
            private IMovieDBQueries queryInterface = null;

            public TMDBAPI(TMDB.IMovieDBQueries rawInterface)
            {
                queryInterface = rawInterface;
            }

            public string MakeActorPath(string path)
            {
                string fullPath = _configuration.images.base_url +
                                           _configuration.images.profile_sizes.First() +
                                           path;

                return fullPath;
            }

            public string MakeMoviePath(string path)
            {
                string fullPath = _configuration.images.base_url +
                                           _configuration.images.poster_sizes.First() +
                                           path;

                return fullPath;
            }

            public async Task<SearchActor_Result> SearchForActor(string actorName)
            {
                string searchResultString = await queryInterface.SearchForActor(actorName);

                DataContractJsonSerializer js = new DataContractJsonSerializer(typeof(SearchActor_Result));
                MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(searchResultString));
                var resultObject = (SearchActor_Result)js.ReadObject(stream);
                return resultObject;
            }

            public async Task<ActorInfoByID> GetActorInfoFromID(string actorID)
            {
                string actorInfoQuery = await queryInterface.GetActorInfoFromID(actorID);

                DataContractJsonSerializer js = new DataContractJsonSerializer(typeof(ActorInfoByID));
                MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(actorInfoQuery));
                var resultObject = (ActorInfoByID)js.ReadObject(stream);
                return resultObject;
            }

            public async Task<string> GetActorImageFromID(string actorID)
            {
                DBConfig conf = await GetConfiguration();

                var images = await GetActorImageListFromID(actorID);
                var imagePath = images.profiles.First();

                string path = MakeActorPath(imagePath.file_path);
                return path;
            }

            public async Task<ActorImagesByID> GetActorImageListFromID(string actorID)
            {
                string query = await queryInterface.GetActorImagesFromID(actorID);

                DataContractJsonSerializer js = new DataContractJsonSerializer(typeof(ActorImagesByID));
                MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(query));
                var resultObject = (ActorImagesByID)js.ReadObject(stream);

                return resultObject;
            }

            public async Task<DBConfig> GetConfiguration()
            {
                if (_configuration == null)
                {
                    string query = await queryInterface.GetConfiguration();

                    DataContractJsonSerializer js = new DataContractJsonSerializer(typeof(DBConfig));
                    MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(query));
                    _configuration = (DBConfig)js.ReadObject(stream);
                }
                return _configuration;
            }
        }
    }
}
