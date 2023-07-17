using System.Runtime.Serialization;

namespace WorldMatch.Casino.ExternalWallet.Alea
{
    [DataContract]
    internal class TransferHttpPostContent : IHttpPostContent
    {
        [DataMember(Name = "auth")]
        public TransferHttpPostAuth Auth { get; set; }

        [DataMember(Name = "data")]
        public TransferHttpPostContentData Data { get; set; }

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

        public TransferHttpPostContent(AuthToken token, ExternalSeamlessTransaction transaction)
        {
            this.Auth = new TransferHttpPostAuth(token);
            this.Data = new TransferHttpPostContentData(transaction);
        }

        public override string ToString()
        {
            return this.Data.ToString();
        }
    }
}
