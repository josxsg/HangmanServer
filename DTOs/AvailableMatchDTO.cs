using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace HangmanServer.DTOs
{
    [DataContract]
    public class AvailableMatchDTO
    {
        [DataMember]
        public int MatchId { get; set; }
        [DataMember]
        public string CreatorUsername { get; set; }
        [DataMember]
        public string CategoryName { get; set; }
        [DataMember]
        public string ChallengerUsername { get; set; }
        [DataMember]
        public DateTime CreationDate { get; set; }
        [DataMember]
        public int StatusId { get; set; }
    }
}