using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieLapsus
{
    namespace ACMOV
    {
        public class TMDB_Actor
        {
            private static Dictionary<string, TMDB_Actor> _actorCache = new Dictionary<string, TMDB_Actor>();

            public static void ClearCache()
            {
                _actorCache.Clear();
            }

            public static async Task<TMDB_Actor> GetActorFromID(string id, MovieLapsus.TMDB.TMDBAPI api)
            {
                try
                {
                    TMDB_Actor actor = _actorCache[id];

                    return actor;
                }
                catch(Exception)
                {

                }

                TMDB_Actor act = new TMDB_Actor(id);
                _actorCache[id] = act;
                await act.Load(api);

                return act;
            }


            public static async Task<TMDB_Actor> GetActorFromString(string name, MovieLapsus.TMDB.TMDBAPI api)
            {
                var search = await api.SearchForActor(name);

                var match = search.results.First();

                return await GetActorFromID(match.id.ToString(), api);
            }

            public string ID { get; private set; }
            public string Name { get; private set; }
            public string ImdbID { get; private set; }
            public string PictureURL { get; private set; }
            public string Birthday { get; private set; }
            public float Popularity { get; private set; }

            public List<TMDB_Character> Characters { get; private set; }

            private TMDB_Actor(string id)
            {
                ID = id;
                Characters = new List<TMDB_Character>();
            }

            private async Task Load(MovieLapsus.TMDB.TMDBAPI api)
            {
                ActorBiography actorInfo = await api.GetActorBiographyFromID(ID);

                this.Name = actorInfo.name;
                this.ImdbID = actorInfo.imdb_id;
                this.Popularity = actorInfo.popularity;
                this.Birthday = actorInfo.biography;

                await api.GetConfiguration();
                this.PictureURL = api.MakeActorPosterPath(actorInfo.profile_path);
            }

            public async Task LoadCharacters(MovieLapsus.TMDB.TMDBAPI api)
            {
                if (Characters.Count > 0)
                    return;

                var actorInfo = await api.GetActorMoviesFromID(ID);

                foreach (var role in actorInfo.cast)
                {
                    var character = TMDB_Character.CreateCharacter(this.ID);

                    character.ActorImage = this.PictureURL;
                    character.ActorName = this.Name;
                    character.CastId = "";

                    character.CharacterName = role.character;
                    character.MovieId = role.id.ToString();
                    character.MovieImage = role.poster_path;
                    character.MovieName = role.original_title;

                    Characters.Add(character);
                }
            }
        }
    }
}
