using System.Runtime.Serialization;

namespace WorldMatch.Casino.ExternalWallet.Alea
{
    [DataContract]
    internal class CancelHttpPostResultData : IHttpPostResultData
    {
        [DataMember(Name = "transactionid")]
        public string TransactionID { get; set; }

        public CancelHttpPostResultData() : base()
        {
            this.TransactionID = string.Empty;
        }

        public override string ToString()
        {
            return this.TransactionID.ToString();
        }
    }
}
