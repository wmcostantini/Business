using System.Runtime.Serialization;
using WorldMatch.Casino.Common;

namespace WorldMatch.Casino.ExternalWallet.Alea
{
    [DataContract]
    internal class AuthHttpPostData : IHttpPostResultData
    {
        [DataMember(Name = "userid")]
        public string UserID { get; set; }

        [DataMember(Name = "token")]
        public string Token { get; set; }

        [DataMember(Name = "username")]
        public string UserName { get; set; }

        [DataMember(Name = "licensee")]
        public string Licensee { get; set; }

        [DataMember(Name = "skin")]
        public string Skin { get; set; }

        [DataMember(Name = "language")]
        public string Language { get; set; }

        [DataMember(Name = "currency")]
        public string Currency { get; set; }

        public AuthHttpPostData()
        {
            this.UserID = string.Empty;
            this.Token = string.Empty;
            this.UserName = string.Empty;
            this.Licensee = string.Empty;
            this.Skin = string.Empty;
            this.Language = string.Empty;
            this.Currency = string.Empty;
        }

        public override string ToString()
        {
            return this.Token.SafeTrim();
        }
    }
}
