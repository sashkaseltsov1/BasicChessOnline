using System.Runtime.Serialization;

namespace GameSessionEstablishmentService
{
    [DataContract]
    public class OperationResult
    {
        [DataMember]
        public bool Result { get; set; }

        [DataMember]
        public string Message { get; set; }

        public OperationResult(bool result, string message)
        {
            Result = result;
            Message = message;
        }
    }
}
