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
    public class MovieDBAPI
    {
        public MovieDBAPI()
        {

        }

        public async Task<SearchActor_Result> SearchForActor(IMovieDBQueries rawInterface, string actorName)
        {
            string searchResultString = await rawInterface.SearchForActor(actorName);

            DataContractJsonSerializer js = new DataContractJsonSerializer(typeof(SearchActor_Result));
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(searchResultString));
            var resultObject = (SearchActor_Result)js.ReadObject(stream);
            return resultObject;
        }

        public async Task<ActorInfoByID> GetActorInfoFromID(IMovieDBQueries rawInterface, string actorID)
        {
            string actorInfoQuery = await rawInterface.GetActorInfoFromID(actorID);

            DataContractJsonSerializer js = new DataContractJsonSerializer(typeof(ActorInfoByID));
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(actorInfoQuery));
            var resultObject = (ActorInfoByID)js.ReadObject(stream);
            return resultObject;
        }

        public async Task<ActorImagesByID> GetActorImagesFromID(IMovieDBQueries rawInterface, string actorID)
        {
            string query = await rawInterface.GetActorImagesFromID(actorID);

            DataContractJsonSerializer js = new DataContractJsonSerializer(typeof(ActorImagesByID));
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(query));
            var resultObject = (ActorImagesByID)js.ReadObject(stream);

            return resultObject;
        }

        public async Task<DBConfig> GetConfiguration(IMovieDBQueries rawInterface)
        {
            string query = await rawInterface.GetConfiguration();

            DataContractJsonSerializer js = new DataContractJsonSerializer(typeof(DBConfig));
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(query));
            var resultObject = (DBConfig)js.ReadObject(stream);
            return resultObject;
        }
    }
}
