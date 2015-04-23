using System;

namespace MovieLapsus
{
    namespace TMDB
    {
        public interface IMovieRawApi
        {
            System.Threading.Tasks.Task<string> SearchForActor(string actorName);
            System.Threading.Tasks.Task<string> SearchForMovie(string movieName, bool autocomplete);
            System.Threading.Tasks.Task<string> GetMovieDescriptionFromID(string movieID); 
            System.Threading.Tasks.Task<string> GetActorMoviesFromID(string actorID);
            System.Threading.Tasks.Task<string> GetActorImagesFromID(string actorID);
            System.Threading.Tasks.Task<string> GetActorBiographyFromID(string actorID);
            System.Threading.Tasks.Task<string> GetMovieCreditsFromID(string actorID);
            System.Threading.Tasks.Task<string> GetConfiguration();
        }
    }
}
