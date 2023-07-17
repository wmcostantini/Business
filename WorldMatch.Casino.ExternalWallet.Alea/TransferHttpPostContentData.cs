using System.Runtime.Serialization;
using WorldMatch.Casino.Common;

namespace WorldMatch.Casino.ExternalWallet.Alea
{
    [DataContract]
    internal class TransferHttpPostContentData : IValidObject
    {
        private const string TRANSFER_UID_FORMAT = "{0}-{1:0000000000}";

        [DataMember(Name = "usertoken")]
        public string UserToken { get; set; }

        [DataMember(Name = "sessiontoken")]
        public string SessionToken { get; set; }

        [DataMember(Name = "gameidentity")]
        public string GameIdentity { get; set; }

        [DataMember(Name = "transactionid")]
        public string TransactionID { get; set; }

        [DataMember(Name = "roundid")]
        public long RoundID { get; set; }

        [DataMember(Name = "amount")]
        public decimal Amount { get; set; }

        [DataMember(Name = "jackpot")]
        public decimal Jackpot { get; set; }

        [DataMember(Name = "currency")]
        public string Currency { get; set; }

        [DataMember(Name = "reference")]
        public string Reference { get; set; }

        [DataMember(Name = "closed")]
        public int ClosedRound { get; set; }

        [DataMember(Name = "promo")]
        public int IsPromo { get; set; }

        public bool IsValid
        {
            get { return !string.IsNullOrEmpty(this.TransactionID) && this.RoundID > 0L && this.Amount >= 0.00M; }
        }

        public TransferHttpPostContentData(ExternalSeamlessTransaction transaction)
        {
            var gameBetParentID = transaction.GameBetParentID.GetValueOrDefault();

            this.UserToken = string.Empty;
            this.SessionToken = string.Empty;
            this.GameIdentity = string.Empty;
            this.TransactionID = string.Empty;
            this.RoundID = ConfigurationService.InvalidIdentifier;
            this.Amount = 0.00M;
            this.Jackpot = 0.00M;
            this.Currency = string.Empty;
            this.ClosedRound = transaction.NoBetCount == 0 ? 1 : 0;
            this.IsPromo = (transaction.Type == ObjectModel.SessionType.FreeRoundMin || transaction.Type == ObjectModel.SessionType.FreeRoundMax) ? 1 : 0;

            if (!transaction.IsNull())
            {
                var token = transaction.Token;
                var gameCode = transaction.Game.GetInfo().Code;
                var transfer = transaction.Transfer;
                var externalReference = transaction.ExternalReference.SafeTrim();

                if (!token.IsNull() && !gameCode.IsNull() && !transfer.IsNull())
                {
                    var transactionID = transaction.IsDeposit ? transaction.DepositTransactionID.GetValueOrDefault() : transaction.WithdrawalTransactionID.GetValueOrDefault();
                    var transferUID = string.Format(TRANSFER_UID_FORMAT, transaction.GameBetID.GetValueOrDefault(), transactionID);

                    this.UserToken = token.User.HashCode;
                    this.SessionToken = token.HashCode;
                    this.GameIdentity = gameCode;
                    this.TransactionID = transferUID;
                    this.RoundID = gameBetParentID > 0L ? gameBetParentID : transaction.GameBetID.GetValueOrDefault();
                    this.Amount = transaction.Transfer.Amount;
                    this.Currency = transaction.Transfer.Currency.ISOCode;
                    this.Reference = externalReference;
                }
            }
        }

        public override string ToString()
        {
            return this.TransactionID.ToString();
        }
    }
}
