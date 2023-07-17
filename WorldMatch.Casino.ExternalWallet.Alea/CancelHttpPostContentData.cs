using System.Runtime.Serialization;
using WorldMatch.Casino.Common;

namespace WorldMatch.Casino.ExternalWallet.Alea
{
    [DataContract]
    internal class CancelHttpPostContentData : IValidObject
    {
        [DataMember(Name = "transactionid")]
        public string TransactionID { get; set; }

        public bool IsValid
        {
            get { return !string.IsNullOrEmpty(this.TransactionID); }
        }

        public CancelHttpPostContentData(string transactionID)
        {
            this.TransactionID = transactionID;
        }

        public override string ToString()
        {
            return this.TransactionID.ToString();
        }
    }
}
