using WorldMatch.Casino.Common;
using WorldMatch.Casino.ObjectModel;

namespace WorldMatch.Casino.ExternalWallet.Alea
{
    internal class BalanceHttpPostResult : HttpPostResult<BalanceHttpPostData, ExternalBalance>
    {
        public BalanceHttpPostResult() : base()
        {
        }

        public override ExternalResult<ExternalBalance> ToExternalResult(params object[] args)
        {
            var wallet = new ExternalBalance();

            if (this.Error == ExternalError.OK && !this.Data.IsNull())
            {
                wallet.Balance = new Money(this.Data.Amount, this.Data.Currency.SafeTrim());

                if (!args.IsNullOrEmpty() && !args[0].IsNull())
                {
                    wallet.ExternalReference = args[0].ToString();
                }
            }

            return new ExternalResult<ExternalBalance>(this.ErrorMessage, this.ErrorCode, wallet);
        }
    }
}
