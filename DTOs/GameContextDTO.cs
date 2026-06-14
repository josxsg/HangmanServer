using System.Runtime.Serialization;

namespace HangmanServer.DTOs
{
    [DataContract]
    public class GameContextDTO
    {
        [DataMember]
        public int MatchId { get; set; }

        [DataMember]
        public int WordLength { get; set; } 

        [DataMember]
        public string CategoryName { get; set; }

        [DataMember]
        public string WordDescription { get; set; }

        [DataMember]
        public int ChallengerId { get; set; }

        [DataMember]
        public int CreatorId { get; set; }

        [DataMember] 
        public string SecretWord { get; set; }
    }
}