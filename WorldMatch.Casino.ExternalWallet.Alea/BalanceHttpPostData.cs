using System.Runtime.Serialization;
using WorldMatch.Casino.Common;
using WorldMatch.Casino.ObjectModel;

namespace WorldMatch.Casino.ExternalWallet.Alea
{
    [DataContract]
    internal class BalanceHttpPostData : IHttpPostResultData
    {
        [DataMember(Name = "amount")]
        public decimal Amount { get; set; }

        [DataMember(Name = "currency")]
        public string Currency { get; set; }

        public BalanceHttpPostData()
        {
            this.Amount = 0.00M;
            this.Currency = string.Empty;
        }

        public override string ToString()
        {
            return new Money(this.Amount, this.Currency.SafeTrim()).ToString();
        }
    }
}
