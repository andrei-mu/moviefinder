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

        private async Task<IEnumerable<IResultsListItem>> GetMoviesFromList(IEnumerable<MovieInfoByID_Cast> moviesEnum)
        {
            var movieInfoTasks = new System.Collections.Generic.List<Task<ACMOV.TMDB_Movie>>();
            foreach (MovieInfoByID_Cast ai in moviesEnum)
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

        public async Task<System.Collections.Generic.IEnumerable<IResultsListItem>> CalculateCommonMovies(string id)
        {
            var movies = await dbApi.GetActorMoviesFromID(id);

            return await GetMoviesFromList(movies.cast);
        }

        public async Task<System.Collections.Generic.IEnumerable<IResultsListItem>> CalculateCommonMovies(string id1, string id2)
        {
            if (id1 == null)
            {
                return await CalculateCommonMovies(id2);
            }
            if (id2 == null)
            {
                return await CalculateCommonMovies(id1);
            }

            var tasks = new Task<ActorMoviesByID>[2];
            tasks[0] = dbApi.GetActorMoviesFromID(id1);
            tasks[1] = dbApi.GetActorMoviesFromID(id2);

            await Task.WhenAll(tasks);

            var actorInfo1 = await tasks[0];
            var actorInfo2 = await tasks[1];

            var commonList = from mov1 in actorInfo1.cast
                             join mov2 in actorInfo2.cast on mov1.id equals mov2.id
                             select mov1;

            return await GetMoviesFromList(commonList);
        }

        public async Task<IEnumerable<IResultsListItem>> GetActorsFromList(IEnumerable<MovieCredits_Cast> moviesEnum)
        {
            var infoTasks = new System.Collections.Generic.List<Task<ACMOV.TMDB_Actor>>();
            foreach (MovieCredits_Cast ai in moviesEnum)
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

        public async Task<IEnumerable<IResultsListItem>> CalculateCommonActors(string id)
        {
            var movies = await dbApi.GetMovieCreditsFromID(id);

            return await GetActorsFromList(movies.cast);
        }
        public async Task<IEnumerable<IResultsListItem>> CalculateCommonActors(string id1, string id2)
        {
            if (id1 == null)
            {
                return await CalculateCommonActors(id2);
            }
            if (id2 == null)
            {
                return await CalculateCommonActors(id1);
            }

            var tasks = new Task<MovieCredits>[2];
            tasks[0] = dbApi.GetMovieCreditsFromID(id1);
            tasks[1] = dbApi.GetMovieCreditsFromID(id2);

            await Task.WhenAll(tasks);

            var credits1 = await tasks[0];
            var credits2 = await tasks[1];

            var commonList = from act1 in credits1.cast
                             join act2 in credits2.cast on act1.id equals act2.id
                             select act1;

            return await GetActorsFromList(commonList);
        }
    }
}
