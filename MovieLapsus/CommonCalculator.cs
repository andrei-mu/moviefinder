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

            var orderedMovies = commonMovies.OrderByDescending(m => m.ItemPriority());

            return orderedMovies;
        }

        public async Task<System.Collections.Generic.IEnumerable<IResultsListItem>> CalculateCommonActors(string id1, string id2)
        {
            var tasks = new Task<MovieCredits>[2];
            tasks[0] = dbApi.GetMovieCreditsFromID(id1);
            tasks[1] = dbApi.GetMovieCreditsFromID(id2);

            await Task.WhenAll(tasks);

            var credits1 = await tasks[0];
            var credits2 = await tasks[1];

            var commonList = from act1 in credits1.cast
                             join act2 in credits2.cast on act1.id equals act2.id
                             select act1;

            var c = commonList.Count();

            var infoTasks = new System.Collections.Generic.List<Task<ACMOV.TMDB_Actor>>();
            foreach (MovieCredits_Cast ai in commonList)
            {
                infoTasks.Add(ACMOV.TMDB_Actor.GetActorFromID(ai.id.ToString(), dbApi));
            }

            await Task.WhenAll(infoTasks);

            var commonActors = new System.Collections.Generic.List<IResultsListItem>();
            foreach (Task<ACMOV.TMDB_Actor> t in infoTasks)
            {
                var actor = await t;

                commonActors.Add(actor as IResultsListItem);
            }

            var orderedActors = commonActors.OrderByDescending(m => m.ItemPriority());

            return orderedActors;
        }
    }
}
