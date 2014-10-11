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
    public struct SearchActor_MovieInfo
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
    public struct SearchActor_ActorInfo
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
    }

    [DataContract]
    public struct SearchActor_Result
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
    public struct MovieInfoByID_Cast
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
    public struct ActorInfoByID
    {
        [DataMember]
        public List<MovieInfoByID_Cast> cast;
        [DataMember]
        public int id;
    }

}
