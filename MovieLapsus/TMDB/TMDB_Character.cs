using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieLapsus
{
    namespace ACMOV
    {
        public class TMDB_Character
        {
            public string CastId {get; set; }
            public string CharacterName {get; set; }
            public string ActorId {get; set; }
            public string ActorName { get; set; }
            public string ActorImage { get; set; }

            public string MovieId { get; set; }
            public string MovieName { get; set; }
            public string MovieImage { get; set; }

            private TMDB_Character(string actorId)
            {
                ActorId = actorId.ToString();
            }

            public static TMDB_Character CreateCharacter(int actorId)
            {
                return CreateCharacter(actorId.ToString());
            }

            public static TMDB_Character CreateCharacter(string actorId)
            {
                var character = new TMDB_Character(actorId);

                return character;
            }
        }
    };
};
