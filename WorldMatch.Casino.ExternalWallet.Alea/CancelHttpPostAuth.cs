using System.Runtime.Serialization;

namespace WorldMatch.Casino.ExternalWallet.Alea
{
    [DataContract]
    internal class CancelHttpPostAuth : HttpPostContent
    {
        public CancelHttpPostAuth(AuthToken token) : base(token)
        {
        }
    }
}
