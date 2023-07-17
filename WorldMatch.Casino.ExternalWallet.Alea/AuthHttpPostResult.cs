using WorldMatch.Casino.Common;

namespace WorldMatch.Casino.ExternalWallet.Alea
{
    internal class AuthHttpPostResult : HttpPostResult<AuthHttpPostData, ExternalUser>
    {
        public AuthHttpPostResult() : base()
        {
        }

        public override ExternalResult<ExternalUser> ToExternalResult(params object[] args)
        {
            var user = new ExternalUser();

            if (this.Error == ExternalError.OK && !this.Data.IsNull())
            {
                user.ExternalUserID = this.Data.UserID.SafeTrim();
                user.ExternalAuthKey = this.Data.Token.SafeTrim();
                user.UserName = this.Data.UserName.SafeTrim();
                user.LicenseeCode = this.Data.Licensee.SafeTrim();
                user.SkinCode = this.Data.Skin.SafeTrim();
                user.LanguageCode = this.Data.Language.SafeTrim().ToUpperInvariant();
                user.CurrencyCode = this.Data.Currency.SafeTrim().ToUpperInvariant();
            }

            return new ExternalResult<ExternalUser>(this.ErrorMessage, this.ErrorCode, user);
        }
    }
}
