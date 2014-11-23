using System;
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
        public int id;
        [DataMember]
        public bool adult;
        [DataMember]
        public string biography;
        [DataMember]
        public string birthday;
        [DataMember]
        public string imdb_id;
        [DataMember]
        public string name;
        [DataMember]
        public float popularity;
        [DataMember]
        public string profile_path;
    }

    [DataContract]
    public class MovieDescription
    {
        [DataMember]
        public int id;
        [DataMember]
        public bool adult;
        [DataMember]
        public string imdb_id;
        [DataMember]
        public string original_title;
        [DataMember]
        public string poster_path;
        [DataMember]
        public string overview;
        [DataMember]
        public string tagline;
        [DataMember]
        public string release_date;
        [DataMember]
        public float popularity;
    }

    [DataContract]
    public class SearchActor_MovieInfo
    {
        [DataMember]
        public bool adult;
        [DataMember]
        public string backdrop_path;
        [DataMember]
        public int id;
        [DataMember]
        public string original_title;
        [DataMember]
        public string release_date;
        [DataMember]
        public string poster_path;
        [DataMember]
        public double popularity;
        [DataMember]
        public string title;
        [DataMember]
        public string vote_average;
        [DataMember]
        public int vote_count;
        [DataMember]
        public string media_type;
    };

    [DataContract]
    public class SearchActor_ActorInfo
    {
        [DataMember]
        public bool adult;
        [DataMember]
        public int id;
        [DataMember]
        public List<SearchActor_MovieInfo> known_for;
        [DataMember]
        public string name;
        [DataMember]
        public double popularity;
        [DataMember]
        public string profile_path;

        public override string ToString()
        {
            return name;
        }

    }

    [DataContract]
    public class SearchActor_Result
    {
        [DataMember]
        public int page;
        [DataMember]
        public List<SearchActor_ActorInfo> results;
        [DataMember]
        public int total_pages;
        [DataMember]
        public int total_results;
    }

    [DataContract]
    public class MovieInfoByID_Cast
    {
        [DataMember]
        public bool adult;
        [DataMember]
        public string character;
        [DataMember]
        public string credit_id;
        [DataMember]
        public int id;
        [DataMember]
        public string original_title;
        [DataMember]
        public string poster_path;
        [DataMember]
        public string release_date;
        [DataMember]
        public string title;
    }

    [DataContract]
    public class ActorMoviesByID
    {
        [DataMember]
        public int id;
        [DataMember]
        public List<MovieInfoByID_Cast> cast;
    }

    [DataContract]
    public class ActorImageProfile
    {
        [DataMember]
        public string file_path;
        [DataMember]
        public int width;
        [DataMember]
        public int height;
    }
    
    [DataContract]
    public class ActorImagesByID
    {
        [DataMember]
        public int id;
        [DataMember]
        public List<ActorImageProfile> profiles;
    }

    [DataContract]
    public class DBImages
    {
        [DataMember]
        public string base_url;
        [DataMember]
        public List<string> backdrop_sizes;
        [DataMember]
        public List<string> logo_sizes;
        [DataMember]
        public List<string> poster_sizes;
        [DataMember]
        public List<string> profile_sizes;
        [DataMember]
        public List<string> still_sizes;
    }

    [DataContract]
    public class DBConfig
    {
        [DataMember]
        public DBImages images;
    }
}
