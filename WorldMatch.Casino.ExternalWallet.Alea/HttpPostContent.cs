using System.Runtime.Serialization;
using WorldMatch.Casino.Common;

namespace WorldMatch.Casino.ExternalWallet.Alea
{
    [DataContract]
    [KnownType(typeof(AuthHttpPostContent))]
    [KnownType(typeof(BalanceHttpPostContent))]
    [KnownType(typeof(TransferHttpPostAuth))]
    [KnownType(typeof(CancelHttpPostAuth))]
    internal abstract class HttpPostContent : IHttpPostContent
    {
        public string Secret 
        {
            get
            {
                return ConfigurationService.GetValueOrDefault($"{this.Provider.ToLowerInvariant()}Secret", string.Empty);
            }
        }

        public string Provider { get; set; }

        [DataMember(Name = "ID")]
        public string ID { get; set; }

        [DataMember(Name = "AuthToken")]
        public string AuthToken { get; set; }

        [DataMember(Name = "userid")]
        public string UserID { get; set; }

        [DataMember(Name = "token")]
        public string Token { get; set; }

        [DataMember(Name = "skin")]
        public string Skin { get; set; }

        public virtual bool IsValid
        {
            get
            {
                return
                    !string.IsNullOrEmpty(this.UserID) &&
                    !string.IsNullOrEmpty(this.Token) &&
                    !string.IsNullOrEmpty(this.Skin);
            }
        }

        public HttpPostContent(AuthToken token)
        {
            this.Provider = token.Provider;
            this.ID = System.Guid.NewGuid().ToString().ToUpper();
            this.AuthToken = $"{this.Secret}{this.ID}".ToMD5();

            if (!token.User.IsNull())
            {
                this.UserID = token.User.ExternalUserID ?? string.Empty;
                this.Token = token.User.ExternalAuthKey ?? string.Empty;
                this.Skin = token.User.SkinCode ?? string.Empty;
            }
            else
            {
                this.UserID = string.Empty;
                this.Token = string.Empty;
                this.Skin = string.Empty;
            }
        }

        public override string ToString()
        {
            return this.UserID.SafeTrim();
        }
    }
}
