using System;

namespace MovieLapsus
{
    public interface IMovieDBQueries
    {
        System.Threading.Tasks.Task<string> GetActorInfoFromID(string actorID);
        System.Threading.Tasks.Task<string> SearchForActor(string actorName);
    }
}
