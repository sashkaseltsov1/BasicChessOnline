using System.Runtime.Serialization;

namespace GameSessionEstablishmentService
{
    [DataContract]
    public class GameDescription
    {
        [DataMember]
        public int Id { get; set; } 

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string StateDerscription { get; set; }

        [DataMember]
        public string TurnDescription { get; set; }

        [DataMember]
        public Player WhitePlayer { get; set; }

        [DataMember]
        public Player BlackPlayer { get; set; }

        [DataMember]
        public string Result { get; set; }

        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                if (_password == null) HasPassword = false; else HasPassword = true;
            }
        }

        [DataMember]
        public bool HasPassword { get; set; }

        private string _password;      
    }
}
