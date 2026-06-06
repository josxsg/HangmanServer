using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace HangmanServer.DTOs
{
    [DataContract]
    public class PlayerScoreDTO
    {
        [DataMember]
        public int TotalScore { get; set; }
        [DataMember]
        public int MatchesWon { get; set; }
        [DataMember]
        public int MatchesLost { get; set; }
        [DataMember]
        public int Penalties { get; set; }
    }
}