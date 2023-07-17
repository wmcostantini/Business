using System.Runtime.Serialization;
using WorldMatch.Casino.Common;

namespace WorldMatch.Casino.ExternalWallet.Alea
{
    [DataContract]
    internal class TransferHttpPostResultData : IHttpPostResultData
    {
        [DataMember(Name = "transactionid")]
        public string TransactionID { get; set; }

        [DataMember(Name = "balance")]
        public decimal Balance { get; set; }

        [DataMember(Name = "currency")]
        public string Currency { get; set; }

        public TransferHttpPostResultData() : base()
        {
            this.TransactionID = string.Empty;
            this.Balance = 0.00M;
            this.Currency = string.Empty;
        }

        public override string ToString()
        {
            return this.TransactionID.SafeTrim();
        }
    }
}
