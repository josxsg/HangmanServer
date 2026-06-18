using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace HangmanServer.DTOs
{
    [DataContract]
    public class MatchHistoryDTO
    {
        [DataMember]
        public int MatchId { get; set; }
        [DataMember]
        public DateTime Date { get; set; }
        [DataMember]
        public string WordText { get; set; }
        [DataMember]
        public string RivalUsername { get; set; }
        [DataMember]
        public string Result { get; set; }

        [DataMember]
        public int Points { get; set; }
    }
}