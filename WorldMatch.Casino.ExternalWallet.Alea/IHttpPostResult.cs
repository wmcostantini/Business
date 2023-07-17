namespace WorldMatch.Casino.ExternalWallet.Alea
{
    internal interface IHttpPostResult
    {
        int Result { get; set; }
        string Message { get; set; }
    }

    internal interface IHttpPostResult<TData> : IHttpPostResult
        where TData : class, IHttpPostResultData, new()
    {
        TData Data { get; set; }
    }
}
