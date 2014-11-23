using System;

namespace MovieLapsus
{
    namespace TMDB
    {
        public interface IMovieDBQueries
        {
            System.Threading.Tasks.Task<string> GetActorInfoFromID(string actorID);
            System.Threading.Tasks.Task<string> SearchForActor(string actorName);
            System.Threading.Tasks.Task<string> GetActorImagesFromID(string actorID);
            System.Threading.Tasks.Task<string> GetConfiguration();
        }
    }
}
