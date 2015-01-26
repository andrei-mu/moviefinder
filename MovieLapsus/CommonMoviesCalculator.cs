using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieLapsus
{
    public class CommonCalculator
    {
        private TMDB.TMDBAPI dbApi;
        public CommonCalculator(TMDB.TMDBAPI _dbApi)
        {
            this.dbApi = _dbApi;
        }

        public async Task<System.Collections.Generic.IEnumerable<IResultsListItem>> CalculateCommonMovies(string id1, string id2)
        {
            var tasks = new Task<ActorMoviesByID>[2];
            tasks[0] = dbApi.GetActorMoviesFromID(id1);
            tasks[1] = dbApi.GetActorMoviesFromID(id2);

            await Task.WhenAll(tasks);

            var actorInfo1 = await tasks[0];
            var actorInfo2 = await tasks[1];

            var commonList = from mov1 in actorInfo1.cast
                             join mov2 in actorInfo2.cast on mov1.id equals mov2.id
                             select mov1;

            var c = commonList.Count();

            var movieInfoTasks = new System.Collections.Generic.List<Task<ACMOV.TMDB_Movie>>();
            foreach (MovieInfoByID_Cast ai in commonList)
            {
                if (ai.media_type == "tv")
                    continue;
                movieInfoTasks.Add(ACMOV.TMDB_Movie.GetMovieFromID(ai.ID(), dbApi));
            }

            await Task.WhenAll(movieInfoTasks);

            var commonMovies = new System.Collections.Generic.List<IResultsListItem>();
            foreach (Task<ACMOV.TMDB_Movie> t in movieInfoTasks)
            {
                var movie = await t;

                commonMovies.Add(movie as IResultsListItem);
            }

            var orderedMovies = commonMovies.OrderByDescending(m => m.ItemDescription());

            return orderedMovies;
        }

        public async Task<System.Collections.Generic.IEnumerable<IResultsListItem>> CalculateCommonActors(string id1, string id2)
        {
            var tasks = new Task<ActorMoviesByID>[2];
            tasks[0] = dbApi.GetActorMoviesFromID(id1);
            tasks[1] = dbApi.GetActorMoviesFromID(id2);

            await Task.WhenAll(tasks);

            var actorInfo1 = await tasks[0];
            var actorInfo2 = await tasks[1];

            var commonList = from mov1 in actorInfo1.cast
                             join mov2 in actorInfo2.cast on mov1.id equals mov2.id
                             select mov1;

            var c = commonList.Count();

            var movieInfoTasks = new System.Collections.Generic.List<Task<ACMOV.TMDB_Movie>>();
            foreach (MovieInfoByID_Cast ai in commonList)
            {
                if (ai.media_type == "tv")
                    continue;
                movieInfoTasks.Add(ACMOV.TMDB_Movie.GetMovieFromID(ai.ID(), dbApi));
            }

            await Task.WhenAll(movieInfoTasks);

            var commonMovies = new System.Collections.Generic.List<IResultsListItem>();
            foreach (Task<ACMOV.TMDB_Movie> t in movieInfoTasks)
            {
                var movie = await t;

                commonMovies.Add(movie as IResultsListItem);
            }

            var orderedMovies = commonMovies.OrderByDescending(m => m.ItemDescription());

            return orderedMovies;
        }
    }
}
