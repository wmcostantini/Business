using System.Runtime.Serialization;

namespace WorldMatch.Casino.ExternalWallet.Alea
{
    [DataContract]
    internal class TransferHttpPostAuth : HttpPostContent
    {
        public TransferHttpPostAuth(AuthToken token) : base(token)
        {
        }
    }
}
