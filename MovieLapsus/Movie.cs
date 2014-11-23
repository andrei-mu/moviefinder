using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieLapsus
{
    namespace ACMOV
    {
        public class Movie
        {
            private static Dictionary<string, Movie> _movieCache = new Dictionary<string, Movie>();

            public static void ClearCache()
            {
                _movieCache.Clear();
            }

            public static async Task<Movie> GetMovieFromID(string id, MovieLapsus.TMDB.TMDBAPI api)
            {
                try
                {
                    Movie item = _movieCache[id];

                    return item;
                }
                catch (Exception)
                {

                }

                Movie newItem = new Movie(id);
                _movieCache[id] = newItem;
                await newItem.Load(api);

                return newItem;
            }

            public string ID { get; private set; }
            public string Name { get; private set; }
            public string ImdbID { get; private set; }
            public string PictureURL { get; private set; }

            private Movie(string id)
            {
                ID = id;
            }

            private async Task Load(MovieLapsus.TMDB.TMDBAPI api)
            {
                var actorInfo = await api.GetActorBiographyFromID(ID);

            }
        }
    }
}
