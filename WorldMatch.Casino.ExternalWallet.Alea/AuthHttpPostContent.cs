using System.Runtime.Serialization;
using WorldMatch.Casino.Common;

namespace WorldMatch.Casino.ExternalWallet.Alea
{
    [DataContract]
    internal class AuthHttpPostContent : HttpPostContent
    {
        [DataMember(Name = "ip")]
        public string IPAddress { get; set; }

        public AuthHttpPostContent(AuthToken token) : base(token)
        {
            if (!token.User.IsNull())
            {
                this.IPAddress = token.User.IPAddress ?? string.Empty;
            }
            else
            {
                this.IPAddress = string.Empty;
            }
        }
    }
}
