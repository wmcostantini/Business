namespace WorldMatch.Casino.ExternalWallet.Alea
{
    internal enum ExternalError
    {
        Undefined = -1,
        OK = 0,
        ReversibleError = 1,
        BlockingError = 2,
        DebitNotFound = 404
    }
}
