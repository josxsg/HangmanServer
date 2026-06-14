using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace HangmanServer.DTOs
{
    [DataContract]
    public class GameEndDTO
    {
        [DataMember]
        public int WinnerId { get; set; }

        [DataMember]
        public MatchEndReason Reason { get; set; } 

        [DataMember]
        public int PenalizedUserId { get; set; } 
    }

    [DataContract]
    public enum MatchEndReason
    {
        [EnumMember] WordGuessed,
        [EnumMember] MaxMistakesReached,
        [EnumMember] Abandoned,
        [EnumMember] Timeout
    }
}