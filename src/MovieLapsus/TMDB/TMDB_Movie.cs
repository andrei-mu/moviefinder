﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieLapsus
{
    namespace ACMOV
    {
        public class TMDB_Movie : IResultsListItem
        {
            private static Dictionary<string, TMDB_Movie> _movieCache = new Dictionary<string, TMDB_Movie>();

            public static void ClearCache()
            {
                _movieCache.Clear();
            }

            public static async Task<TMDB_Movie> GetMovieFromID(string id, MovieLapsus.TMDB.TMDBAPI api)
            {
                if (_movieCache.ContainsKey(id))
                {
                    TMDB_Movie item = _movieCache[id];

                    return item;
                }

                TMDB_Movie newItem = new TMDB_Movie(id);
                _movieCache.Add(id, newItem);
                await newItem.Load(api);

                return newItem;
            }

            public static async Task<TMDB_Movie> GetMovieFromString(string movieName, MovieLapsus.TMDB.TMDBAPI api)
            {
                var movieSearch = await api.SearchForMovie(movieName, false);

                var movieMatch = movieSearch.results.First();

                return await GetMovieFromID(movieMatch.id.ToString(), api);
            }

            public string ID { get; private set; }
            public string Name { get; private set; }
            public float Popularity { get; private set; }
            public float VoteAverage { get; private set; }
            public string ImdbID { get; private set; }
            public string PictureURL { get; private set; }
            public string ReleaseDate { get; private set; }

            public List<TMDB_Character> Characters;// { get; private set; }

            private TMDB_Movie(string id)
            {
                ID = id;
                Characters = new List<TMDB_Character>();
            }

            private async Task Load(MovieLapsus.TMDB.TMDBAPI api)
            {
                var movieDesc = await api.GetMovieDescriptionFromID(ID);

                this.ID = movieDesc.id.ToString();
                this.Name = movieDesc.original_title;
                this.Popularity = movieDesc.popularity;
                this.ImdbID = movieDesc.imdb_id;
                this.PictureURL = api.MakeMoviePosterPath(movieDesc.poster_path);
                this.ReleaseDate = movieDesc.release_date;
                this.VoteAverage = movieDesc.vote_average;
            }

            public async Task LoadCharacters(MovieLapsus.TMDB.TMDBAPI api)
            {
                if (this.Characters.Count() > 0)
                {
                    return;
                }

                var movieCredits = await api.GetMovieCreditsFromID(ID);

                foreach (var cast in movieCredits.cast)
                {
                    var newChar = TMDB_Character.CreateCharacter(cast.id);

                    newChar.CastId = cast.cast_id.ToString();
                    newChar.CharacterName = cast.character;
                    newChar.ActorName = cast.name;
                    newChar.ActorImage = api.MakeActorPosterPath(cast.profile_path);

                    newChar.MovieId = this.ID;
                    newChar.MovieName = this.Name;
                    newChar.MovieImage = this.PictureURL;

                    Characters.Add(newChar);
                }
            }

            public string ItemName()
            {
                return Name;
            }

            public string ItemDescription()
            {
                return ReleaseDate;
            }

            public string ItemSubtitle()
            {
                
                return string.Format("Vote: {0} / 10", VoteAverage.ToString());
            }

            public string ItemImageUrl()
            {
                return PictureURL;
            }

            public string ItemID()
            {
                return ID;
            }

            public float ItemPriority()
            {
                DateTime dt;
                if (DateTime.TryParse(ReleaseDate, out dt))
                {
                    float rating = (float)(dt.DayOfYear + dt.Year * 400);

                    return rating;
                }

                return 0.0f;
            }
        }
    }
}
