using System;
using WorldMatch.Casino.Common;
using WorldMatch.Casino.Transactions;

namespace WorldMatch.Casino.ExternalWallet.Alea
{
    internal class CancelHttpPostResult : HttpPostResult<CancelHttpPostResultData, ExternalTransfer>
    {
        public CancelHttpPostResult() : base()
        {
        }

        public override ExternalResult<ExternalTransfer> ToExternalResult(params object[] args)
        {
            var transfer = new ExternalTransfer();
            
            if (this.Error == ExternalError.DebitNotFound)
            {
                // ExternalError.OK => Transaction Found on Alea side and rollbacked
                // ExternalError.DebitNotFound => Transaction Found on Alea, never got the Debit Request
                this.Result = (int)ExternalError.OK;
                this.Data = new CancelHttpPostResultData { };
                this.Data.TransactionID = !args.IsNullOrEmpty() && !args[0].IsNull() ? args[0].ToString() : string.Empty;
            }

            if (!args.IsNullOrEmpty() && !args[0].IsNull())
            {
                transfer.TransactionID = Convert.ToInt64(args[0].ToString().Split('-')[1]);
            }

            if (!this.Data.IsNull())
            {
                transfer.ExternalReference = this.Data.TransactionID.ToString();
            }

            switch (this.Error)
            {
                case ExternalError.OK:
                    transfer.ExternalState = TransactionState.Cancelled;
                    break;

                // Reversible error
                case ExternalError.ReversibleError:
                    transfer.ExternalState = TransactionState.Aborted;
                    break;

                // Blocking error
                default:
                    transfer.ExternalState = TransactionState.Denied;
                    break;
            }

            return new ExternalResult<ExternalTransfer>(this.ErrorMessage, this.ErrorCode, transfer);
        }
    }
}
