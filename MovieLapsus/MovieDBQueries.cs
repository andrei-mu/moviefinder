using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;


namespace MovieLapsus
{
    public class MovieDBQueries : IMovieDBQueries
    {
        private static string API_KEY = "7d3315bb7234145c8d3b6e4b89e6ec55";
        private static string ACTOR_MOVIES = "http://api.themoviedb.org/3/person/{ACTOR_ID}/movie_credits?api_key={APIKEY}";
        private static string ACTOR_IMAGES = "http://api.themoviedb.org/3/person/{ACTOR_ID}/images?api_key={APIKEY}"; 
        private static string MOVIE_IMAGES = "http://api.themoviedb.org/3/movie/{MOVIE_ID}/images?api_key={APIKEY}"; 
        
        private static string SEARCH_ACTOR_QUERY = "http://api.themoviedb.org/3/search/person?api_key={APIKEY}&query={ACTOR_NAME}";
        private static string CONFIG = "http://api.themoviedb.org/3/configuration?api_key={APIKEY}";

        private string ActorSearchQuery
        {
            get
            {
                return SEARCH_ACTOR_QUERY;
            }
        }

        private string ActorInfoQuery
        {
            get
            {
                return ACTOR_MOVIES;
            }
        }

        private string ActorImagesQuery
        {
            get
            {
                return ACTOR_IMAGES;
            }
        }

        private string MovieImagesQuery
        {
            get
            {
                return MOVIE_IMAGES;
            }
        }

        private string ConfigurationQuery
        {
            get
            {
                return CONFIG;
            }
        }

        private async Task<string> GenericHTTPQuery(string query)
        {
            var link = query.Replace("{APIKEY}", API_KEY);

            var request = System.Net.WebRequest.Create(link) as System.Net.HttpWebRequest;

            request.Method = "GET";
            string responseContent = "";
            string responseError = "";

            try
            {
                var reqStreamTask = request.GetResponseAsync();
                using (var response = await reqStreamTask)
                {
                    using (var reader = new System.IO.StreamReader(response.GetResponseStream()))
                    {
                        responseContent = reader.ReadToEnd();
                    }
                }
            }
            catch (WebException e)
            {
                responseError += "\nWeb exception raised!\n";
                responseError += "Message: ";
                responseError += e.Message;
                responseError += "\nStatus: ";
                responseError += e.Status;
                responseError += "\n";

                System.Diagnostics.Debug.WriteLine(responseError);
            }
            catch (Exception e)
            {
                responseError += "\nException raised!\n";
                responseError += "\nMessage: ";
                responseError += e.Message;
                responseError += "\n";

                System.Diagnostics.Debug.WriteLine(responseError);
            }

            return responseContent;

        }

        public async Task<string> SearchForActor(string actorName)
        {
            actorName = actorName.Replace(" ", "+");
            string query = ActorSearchQuery.Replace("{ACTOR_NAME}", actorName);

            string response = await this.GenericHTTPQuery(query);
            return response;
        }

        public async Task<string> GetActorInfoFromID(string actorID)
        {
            string query = ActorInfoQuery.Replace("{ACTOR_ID}", actorID);

            string response = await this.GenericHTTPQuery(query);
            return response;
        }

        public async Task<string> GetActorImagesFromID(string actorID)
        {
            string query = ActorImagesQuery.Replace("{ACTOR_ID}", actorID);

            string response = await this.GenericHTTPQuery(query);
            return response;
        }

        public async Task<string> GetMovieImagesFromID(string movieID)
        {
            string query = MovieImagesQuery.Replace("{MOVIE_ID}", movieID);

            string response = await this.GenericHTTPQuery(query);
            return response;
        }

        public async Task<string> GetConfiguration()
        {
            string query = ConfigurationQuery;

            string response = await this.GenericHTTPQuery(query);
            return response;
        }

    }
}
