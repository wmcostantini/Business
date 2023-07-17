using System.Runtime.Serialization;

namespace WorldMatch.Casino.ExternalWallet.Alea
{
    [DataContract]
    internal class CancelHttpPostContent : IHttpPostContent
    {
        [DataMember(Name = "auth")]
        public CancelHttpPostAuth Auth { get; set; }

        [DataMember(Name = "data")]
        public CancelHttpPostContentData Data { get; set; }

        public string UserID
        {
            get { return this.Auth.UserID; }
            set { this.Auth.UserID = value; }
        }

        public string Token
        {
            get { return this.Auth.Token; }
            set { this.Auth.Token = value; }
        }

        public string Skin
        {
            get { return this.Auth.Skin; }
            set { this.Auth.Skin = value; }
        }

        public bool IsValid
        {
            get { return this.Auth.IsValid && this.Data.IsValid; }
        }

        public CancelHttpPostContent(AuthToken token, string transactionID)
        {
            this.Auth = new CancelHttpPostAuth(token);
            this.Data = new CancelHttpPostContentData(transactionID);
        }

        public override string ToString()
        {
            return this.Data.ToString();
        }
    }
}
