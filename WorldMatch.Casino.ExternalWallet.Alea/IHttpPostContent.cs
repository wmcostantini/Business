using WorldMatch.Casino.Common;

namespace WorldMatch.Casino.ExternalWallet.Alea
{
    internal interface IHttpPostContent : IValidObject
    {
        string UserID { get; set; }
        string Token { get; set; }
        string Skin { get; set; }
    }
}
