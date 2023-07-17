namespace WorldMatch.Casino.ExternalWallet.Alea
{
    internal class AuthToken
    {
        public IExternalUserIdentity User { get; set; }
        public string Provider { get; set; }

        public AuthToken(string provider, IExternalUserIdentity user)
        {
            this.User = user;
            this.Provider = provider;
        }
    }
}
