using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace HangmanServer.DTOs
{
    [DataContract]
    public class WordDTO
    {
        [DataMember]
        public int WordId { get; set; }
        [DataMember]
        public string WordText { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public int Length { get; set; }
    }
}