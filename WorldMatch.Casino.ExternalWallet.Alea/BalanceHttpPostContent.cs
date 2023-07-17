using System.Runtime.Serialization;

namespace WorldMatch.Casino.ExternalWallet.Alea
{
    [DataContract]
    internal class BalanceHttpPostContent : HttpPostContent
    {
        public BalanceHttpPostContent(AuthToken token) : base(token)
        {
        }
    }
}
