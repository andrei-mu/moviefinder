﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace MovieLapsus
{
    [DataContract]
    public class ActorBiography
    {
        [DataMember]
        public int id = 0;
        [DataMember]
        public bool adult = false;
        [DataMember]
        public string biography = "";
        [DataMember]
        public string birthday = "";
        [DataMember]
        public string imdb_id = "";
        [DataMember]
        public string name = "";
        [DataMember]
        public float popularity = 0.0f;
        [DataMember]
        public string profile_path = "";
    }

    [DataContract]
    public class MovieDescription
    {
        [DataMember]
        public int id = 0;
        [DataMember]
        public bool adult = false;
        [DataMember]
        public string imdb_id = "";
        [DataMember]
        public string original_title = "";
        [DataMember]
        public string poster_path = "";
        [DataMember]
        public string overview = "";
        [DataMember]
        public string tagline = "";
        [DataMember]
        public string release_date = "";
        [DataMember]
        public float popularity = 0.0f;
        [DataMember]
        public float vote_average = 0.0f;
    }

    [DataContract]
    public class SearchActor_MovieInfo
    {
        [DataMember]
        public bool adult = false;
        [DataMember]
        public string backdrop_path = "";
        [DataMember]
        public int id = 0;
        [DataMember]
        public string original_title = "";
        [DataMember]
        public string release_date = "";
        [DataMember]
        public string poster_path = "";
        [DataMember]
        public double popularity = 0.0;
        [DataMember]
        public string title = "";
        [DataMember]
        public string vote_average = "";
        [DataMember]
        public int vote_count = 0;
        [DataMember]
        public string media_type;
    };

    [DataContract]
    public class SearchActor_ActorInfo
    {
        [DataMember]
        public bool adult = false;
        [DataMember]
        public int id = 0;
        [DataMember]
        public List<SearchActor_MovieInfo> known_for = new List<SearchActor_MovieInfo>();
        [DataMember]
        public string name = "";
        [DataMember]
        public double popularity = 0.0;
        [DataMember]
        public string profile_path = "";

        public override string ToString()
        {
            return name;
        }
    }

    [DataContract]
    public class SearchMovie_Result
    {
        [DataMember]
        public bool adult = false;
        [DataMember]
        public int id = 0;
        [DataMember]
        public string original_title = "";
        [DataMember]
        public string release_date = "";
        [DataMember]
        public string poster_path = "";
        [DataMember]
        public float popularity = 0.0f;
        [DataMember]
        public string title = "";

        public override string ToString()
        {
            return title;
        }
    }

    [DataContract]
    public class SearchMovie_Page
    {
        [DataMember]
        public int page = 0;

        [DataMember]
        public List<SearchMovie_Result> results = new List<SearchMovie_Result>(); 
    }

    [DataContract]
    public class SearchActor_Result
    {
        [DataMember]
        public int page = 0;
        [DataMember]
        public List<SearchActor_ActorInfo> results = new List<SearchActor_ActorInfo>();
        [DataMember]
        public int total_pages = 0;
        [DataMember]
        public int total_results = 0;
    }

    [DataContract]
    public class MovieInfoByID_Cast
    {
        [DataMember]
        public bool adult = false;
        [DataMember]
        public string character = "";
        [DataMember]
        public string credit_id = "";
        [DataMember]
        public int id = 0;
        [DataMember]
        public string original_title = "";
        [DataMember]
        public string original_name = "";
        [DataMember]
        public string poster_path = "";
        [DataMember]
        public string release_date = "";
        [DataMember]
        public string media_type = "";

        public string Name()
        {
            if (media_type == "tv")
            {
                return original_name;
            }

            return original_title;
        }

        public string Description()
        {
            DateTime dt;
            if (DateTime.TryParse(release_date, out dt))
            {
                string date = dt.ToString("D");
                return date;
            }

            return "";
        }

        public string ImageUrl()
        {
            return poster_path;
        }

        public string ID()
        {
            return id.ToString();
        }
    }

    [DataContract]
    public class MovieCredits_Cast
    {
        [DataMember]
        public int cast_id = 0;
        [DataMember]
        public string character = "";
        [DataMember]
        public string credit_id = "";
        [DataMember]
        public int id = 0;
        [DataMember]
        public string name = "";
        [DataMember]
        public string profile_path = "";
    }

    [DataContract]
    public class MovieCredits
    {
        [DataMember]
        public int id = 0;
        [DataMember]
        public List<MovieCredits_Cast> cast = new List<MovieCredits_Cast>();
    }

    [DataContract]
    public class ActorMoviesByID
    {
        [DataMember]
        public int id = 0;
        [DataMember]
        public List<MovieInfoByID_Cast> cast = new List<MovieInfoByID_Cast>();
    }

    [DataContract]
    public class ActorImageProfile
    {
        [DataMember]
        public string file_path = "";
        [DataMember]
        public int width = 0;
        [DataMember]
        public int height = 0;
    }
    
    [DataContract]
    public class ActorImagesByID
    {
        [DataMember]
        public int id = 0;
        [DataMember]
        public List<ActorImageProfile> profiles = new List<ActorImageProfile>();
    }

    [DataContract]
    public class DBImages
    {
        [DataMember]
        public string base_url = "";
        [DataMember]
        public List<string> backdrop_sizes = new List<string>();
        [DataMember]
        public List<string> logo_sizes = new List<string>();
        [DataMember]
        public List<string> poster_sizes = new List<string>();
        [DataMember]
        public List<string> profile_sizes = new List<string>();
        [DataMember]
        public List<string> still_sizes = new List<string>();
    }

    [DataContract]
    public class DBConfig
    {
        [DataMember]
        public DBImages images;
    }
}
