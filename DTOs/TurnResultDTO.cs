using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace HangmanServer.DTOs
{
    [DataContract]
    public class TurnResultDTO
    {
        [DataMember]
        public bool IsCorrect { get; set; }

        [DataMember]
        public char GuessedLetter { get; set; }

        [DataMember]
        public int[] CorrectPositions { get; set; } 

        [DataMember]
        public int CurrentMistakes { get; set; } 
    }
}