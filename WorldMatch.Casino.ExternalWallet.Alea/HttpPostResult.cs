using System.Runtime.Serialization;
using WorldMatch.Casino.Common;

namespace WorldMatch.Casino.ExternalWallet.Alea
{
    [DataContract]
    internal abstract class HttpPostResult<TData, TResult> : IHttpPostResult<TData>
        where TData : class, IHttpPostResultData, new()
        where TResult : class, IValidObject, new()
    {
        [DataMember(Name = "result")]
        public int Result { get; set; }


        [DataMember(Name = "message")]
        public string Message { get; set; }

        [DataMember(Name = "data")]
        public TData Data { get; set; }

        public ExternalError Error
        {
            get
            {
                try
                {
                    return (ExternalError)this.Result;
                }
                catch
                {
                    return ExternalError.Undefined;
                }
            }
        }
        
        public string ErrorCode
        {
            get { return ((int)this.Error).ToString(); }
        }

        public string ErrorMessage
        {
            get { return this.Error.Equals(ExternalError.Undefined) ? string.Concat("Unknown external error. ", this.Message.SafeTrim()) : this.Message.SafeTrim(); }
        }

        public HttpPostResult()
        {
            this.Result = (int)ExternalError.Undefined;
            this.Message = string.Empty;
            this.Data = new TData();
        }

        public abstract ExternalResult<TResult> ToExternalResult(params object[] args);

        public override string ToString()
        {
            return this.Error.ToString();
        }
    }
}
