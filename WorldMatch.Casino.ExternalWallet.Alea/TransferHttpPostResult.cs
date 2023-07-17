using System;
using WorldMatch.Casino.Common;
using WorldMatch.Casino.ObjectModel;
using WorldMatch.Casino.Transactions;

namespace WorldMatch.Casino.ExternalWallet.Alea
{
    internal class TransferHttpPostResult : HttpPostResult<TransferHttpPostResultData, ExternalTransfer>
    {
        public TransferHttpPostResult() : base()
        {
        }

        public override ExternalResult<ExternalTransfer> ToExternalResult(params object[] args)
        {
            var transfer = new ExternalTransfer();
   
            switch (this.Error)
            {
                // Success
                case ExternalError.OK:
                    transfer.ExternalState = TransactionState.Granted;
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

            if (!args.IsNullOrEmpty() && !args[0].IsNull())
            {
                var id = 0L;

                if (Int64.TryParse(args[0].ToString(), out id))
                {
                    transfer.TransactionID = id;
                }
            }

            if (!this.Data.IsNull() && this.Error == ExternalError.OK)
            {
                transfer.ExternalBalance = new Money(this.Data.Balance, this.Data.Currency.SafeTrim());
                transfer.ExternalReference = this.Data.TransactionID.SafeTrim();
            }

            return new ExternalResult<ExternalTransfer>(this.ErrorMessage, this.ErrorCode, transfer);
        }
    }
}
