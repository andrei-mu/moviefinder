using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieLapsus
{
    namespace ACMOV
    {
        public class Actor
        {
            private static Dictionary<string, Actor> _actorCache = new Dictionary<string, Actor>();

            public static void ClearCache()
            {
                _actorCache.Clear();
            }

            public static async Task<Actor> GetActorFromID(string id, MovieLapsus.TMDB.TMDBAPI api)
            {
                try
                {
                    Actor actor = _actorCache[id];

                    return actor;
                }
                catch(Exception)
                {

                }

                Actor act = new Actor(id);
                _actorCache[id] = act;
                await act.Load(api);

                return act;
            }

            public string ID { get; private set; }
            public string Name { get; private set; }
            public string ImdbID { get; private set; }
            public string PictureURL { get; private set; }

            private Actor(string id)
            {
                ID = id;
            }

            private async Task Load(MovieLapsus.TMDB.TMDBAPI api)
            {
                ActorBiography actorInfo = await api.GetActorBiographyFromID(ID);

                this.Name = actorInfo.name;
                this.ImdbID = actorInfo.imdb_id;

                await api.GetConfiguration();
                this.PictureURL = api.MakeActorPosterPath(actorInfo.profile_path);
            }
        }
    }
}
