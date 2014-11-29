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
            public int CastId {get; set; }
            public string Character {get; set; }
            public int ActorId {get; set; }
            public string Name {get; set; }
            public string ImagePath {get; set; }

            private TMDB_Character(int actorId)
            {
                ActorId = actorId;
            }

            public static TMDB_Character CreateCharacter(int actorId)
            {
                var character = new TMDB_Character(actorId);

                return character;
            }
        }
    };
};
