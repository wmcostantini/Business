using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using WorldMatch.Casino.Common;
using WorldMatch.Casino.Data;
using WorldMatch.Casino.ExternalWallet.Data;
using WorldMatch.Casino.ObjectModel;
using WorldMatch.Casino.Security;
using WorldMatch.Casino.Transactions;

namespace WorldMatch.Casino.ExternalWallet.Alea
{
    // GAMECODE
    public class AleaSeamlessProvider : ExternalSeamlessProvider
    {
        private const string SUCCESS_MESSAGE = "OK";
        private const string SUCCESS_CODE = "0";
        private const string ERROR_CODE = "2";
        private const string ERROR_MESSAGE = "ERROR";
        private const string TRANSFER_UID_FORMAT = "{0}-{1:0000000000}";

        public static readonly string OPERATION_AUTH = "auth";
        public static readonly string OPERATION_BALANCE = "balance";
        public static readonly string OPERATION_ENDROUND = "endround";
        public static readonly string OPERATION_DEBIT = "debit";
        public static readonly string OPERATION_CREDIT = "credit";
        public static readonly string OPERATION_CANCEL = "cancel";
        public static readonly string OPERATION_INSTANT = "instant";

        public override string Name
        {
            get { return "Alea"; }
        }

        public override bool ExternalRoundEnabled
        {
            get { return true; }
        }

        protected Logger JsonLogger { get; private set; }

        public AleaSeamlessProvider()
        {
            this.JsonLogger = new Logger(this.Name);
        }

        public virtual string FormatUrl(string baseUrl, string operation)
        {
            return string.Concat(baseUrl, "/", operation);
        }

        private object[] GetFormatArgs<TContent>(string operationUrl, TContent content, string responseContent, string errorMessage = "")
        {
            return new[]
            {
                operationUrl,
                JsonConvert.SerializeObject(content),
                responseContent,
                errorMessage
            };
        }

        public string GetEndpointUrl(string skinCode)
        {
            var skin = CasinoContext.Current.GetSkin(skinCode);

            if (skin.IsNull())
            {
                throw new Exception(string.Format("The skin {0} is invalid", skinCode));
            }

            var serviceUrl = string.Empty;
            var skinBaseUrl = string.Concat(ConfigurationService.GetValueOrDefault(this.ConfigurationPrefix + "SkinBaseUrl" + skin.ID, string.Empty)).TrimEnd('?');

            serviceUrl = skinBaseUrl;
            if (string.IsNullOrEmpty(skinBaseUrl))
            {
                serviceUrl = string.Concat(ConfigurationService.GetValueOrDefault(this.ConfigurationPrefix + "BaseUrl", string.Empty)).TrimEnd('?');
            }

            return serviceUrl.TrimEnd('/');
        }

        private TResult PostAsJson<TContent, TResult>(ExternalLogType logType, AuthToken token, TContent content, ExternalSeamlessTransaction transaction = null, int timeout = 60)
            where TContent : IHttpPostContent
            where TResult : IHttpPostResult, new()
        {
            var result = new TResult();
            var baseUrl = this.GetEndpointUrl(content.Skin);
            var responseContent = string.Empty;
            var errorMessage = string.Empty;
            string operation = string.Empty;

            switch (logType)
            {
                case ExternalLogType.Debit:
                    operation = OPERATION_DEBIT;
                    break;
                case ExternalLogType.EndRound:
                case ExternalLogType.Credit:
                    operation = OPERATION_CREDIT;
                    break;
                case ExternalLogType.Cancel:
                    operation = OPERATION_CANCEL;
                    break;
                case ExternalLogType.Auth:
                    operation = OPERATION_AUTH;
                    break;
                case ExternalLogType.Balance:
                    operation = OPERATION_BALANCE;
                    break;
            }

            var operationUrl = this.FormatUrl(baseUrl, operation);

            try
            {
                if (content.IsValid)
                {
                    if (baseUrl.Length > 0)
                    {
                        var formatter = new JsonMediaTypeFormatter();
                        var mediaType = JsonMediaTypeFormatter.DefaultMediaType.MediaType;
                        var request = HttpWebRequest.Create(operationUrl) as HttpWebRequest;
                        var encoding = new UTF8Encoding(false);

                        request.Method = "POST";
                        request.ContentType = mediaType;
                        request.Accept = string.Concat(mediaType, "; charset=utf-8");
                        request.Timeout = timeout * 1000;
                        request.Proxy = null;
                        request.KeepAlive = false;

                        using (var requestStream = request.GetRequestStream())
                        {
                            formatter.WriteToStream(typeof(TContent), content, requestStream, encoding);
                        }

                        using (var response = request.GetResponse())
                        {
                            using (var responseStream = response.GetResponseStream())
                            {
                                using (var reader = new StreamReader(responseStream))
                                {
                                    responseContent = reader.ReadToEnd();
                                }
                            }
                        }

                        using (var stringContent = new StringContent(responseContent, encoding, mediaType))
                        {
                            result = stringContent.ReadAsAsync<TResult>().Result;
                        }

                        if (result.IsNull())
                        {
                            throw new WebException("Invalid response content");
                        }
                    }
                    else
                    {
                        throw new WebException("Unknown base URL");
                    }
                }
                else
                {
                    throw new WebException("Invalid request content");
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Innerest().Message;
                throw ex;
            }
            finally
            {
                var log = new ExternalLog
                {
                    LicenseeID = this.Licensee.ID,
                    Url = operationUrl,
                    Command = operation,
					CommandType = logType,
                    User = token.User,
                    SecureToken = content.Token,
                    Transaction = transaction,
                    Request = content.IsNull() ? "null" : JsonConvert.SerializeObject(content),
                    Response = responseContent,
                    IsSuccess = result.Result == (int)ExternalError.OK,
                    ErrorCode = result.Result.ToString(),
                    ErrorMessage = string.IsNullOrEmpty(result.Message) ? errorMessage : result.Message, 
                };

                this.InsertLog(this.JsonLogger, log);
            }

            return result;
        }

        public override ExternalResult<ExternalUser> ValidateUser(IExternalUserIdentity user, GameIdentity game)
        {
            var token = new AuthToken(this.ConfigurationPrefix, user);
            var content = new AuthHttpPostContent(token);
            var result = new AuthHttpPostResult();

            result = this.PostAsJson<AuthHttpPostContent, AuthHttpPostResult>(ExternalLogType.Auth, token, content);

            return result.ToExternalResult();
        }

        public override ExternalResult<ExternalBalance> GetCash(IExternalUserIdentity user, GameIdentity game)
        {
            var token = new AuthToken(this.ConfigurationPrefix, user);
            var content = new BalanceHttpPostContent(token);
            var result = new BalanceHttpPostResult();

            result = this.PostAsJson<BalanceHttpPostContent, BalanceHttpPostResult>(ExternalLogType.Balance, token, content);

            return result.ToExternalResult(user.ExternalUserID);
        }

        public override ExternalResult<ExternalFreeRoundCollection> GetFreeRounds(IExternalUserIdentity user, GameIdentity game)
        {
            return this.GetWalletFreeRounds(user, game);
        }

        protected override ExternalResult<ExternalTransfer> TryCancel(IExternalUserIdentity user, long transactionID, int retry)
        {
            var result = new ExternalResult<ExternalTransfer>(ERROR_MESSAGE, ERROR_CODE, new ExternalTransfer { ExternalState = TransactionState.Denied });
            var debitTransaction = this.GetTransaction(transactionID);
            var token = new AuthToken(this.ConfigurationPrefix, user);

            debitTransaction.Flags = ExternalSeamlessFlag.Task;

            if (debitTransaction.SafeValidate())
            {
                var transferUID = string.Format(TRANSFER_UID_FORMAT, debitTransaction.GameBetID, transactionID);

                var content = new CancelHttpPostContent(token, transferUID);

                var cancelResult = this.PostAsJson<CancelHttpPostContent, CancelHttpPostResult>(ExternalLogType.Cancel, token, content, debitTransaction);

                result = cancelResult.ToExternalResult(transferUID);
            }

            return result;
        }

        protected override ExternalResult<ExternalTransfer> TryTransferIn(IExternalUserIdentity user, ExternalSeamlessTransaction transaction, int retry)
        {
            var result = this.BetRound(user, transaction);

            if (result.Error == ExternalError.OK)
            {
                // Insert Pending Round for MultiShot games
                this.InsertPendingRound(user, transaction);
            }

            return result.ToExternalResult(transaction.DepositTransactionID);
        }

        private void CheckPendingRound(string method, ExternalSeamlessTransaction transaction)
        {
            // If I'm going to close the parent round be sure there are not win freespins pending tasks
            var seamlessTaskID = this.GetPendingParentTask(transaction);

            if (seamlessTaskID > 0L)
            {
                throw new Exception($"{nameof(AleaSeamlessProvider)}.{method} - Round Parent {transaction.GameBetID} has one or more pending Seamless Task ({seamlessTaskID}) - GameBetID ({transaction.GameBetID})");
            }
        }

        private TransferHttpPostResult TransferRound(IExternalUserIdentity user, ExternalSeamlessTransaction transaction, ExternalLogType logType)
        {
            var token = new AuthToken(this.ConfigurationPrefix, user);
            var content = new TransferHttpPostContent(token, transaction);
            var jsonResult = new TransferHttpPostResult();

            if (transaction.IsDeposit)
            {
                content.Data.Jackpot = this.GetJackpotBetAmount(this.Licensee, transaction);
            }

            if (transaction.IsWithdrawal)
            {
                content.Data.Jackpot = this.GetJackpotWinAmount(this.Licensee, transaction);
            }

            return this.PostAsJson<TransferHttpPostContent, TransferHttpPostResult>(logType, token, content, transaction, this.Timeout);
        }

        private TransferHttpPostResult BetRound(IExternalUserIdentity user, ExternalSeamlessTransaction transaction)
        {
            return this.TransferRound(user, transaction, ExternalLogType.Debit);
        }

        private TransferHttpPostResult WinRound(IExternalUserIdentity user, ExternalSeamlessTransaction transaction)
        {
            return this.TransferRound(user, transaction, ExternalLogType.Credit);
        }

        private TransferHttpPostResult CloseRound(IExternalUserIdentity user, ExternalSeamlessTransaction transaction)
        {
            return this.TransferRound(user, transaction, ExternalLogType.Credit);
        }

        protected override ExternalResult<ExternalTransfer> TryTransferOut(IExternalUserIdentity user, ExternalSeamlessTransaction transaction, int retry)
        {
            var result = new ExternalResult<ExternalTransfer>("", "-1", new ExternalTransfer { ExternalState = TransactionState.Aborted });
            var jsonResult = new TransferHttpPostResult();
            var resultOK = new ExternalResult<ExternalTransfer>(SUCCESS_MESSAGE, SUCCESS_CODE, new ExternalTransfer { ExternalState = TransactionState.Granted, TransactionID = transaction.TransactionID.GetValueOrDefault() });

            try
            {
                this.CheckPendingRound(nameof(TryTransferOut), transaction);

                if (transaction.IsWithdrawal)
                {
                    jsonResult = this.WinRound(user, transaction);

                    result = jsonResult.ToExternalResult(transaction.WithdrawalTransactionID);
                }
                else
                {
                    if (transaction.NoBetCount == 0)
                    {
                        jsonResult = this.CloseRound(user, transaction);

                        result = jsonResult.ToExternalResult(transaction.WithdrawalTransactionID);
                    }
                    else
                    {
                        result = resultOK;
                    }
                }
            }
            catch (Exception ex)
            {
                var args = new Dictionary<string, object>
                {
                    { "GameSessionID", transaction.GameSessionID },
                    { "GameBetParentID", transaction.GameBetParentID },
                    { "GameBetID", transaction.GameBetID }
                };

                ExceptionLogService.Write(nameof(AleaSeamlessProvider) + "." + nameof(TryTransferOut), args, ex);

                throw;
            }
            finally
            {
                this.InsertPendingRound(user, transaction);
            }

            return result;
        }

        protected override ExternalResult<ExternalSession> TryEndSession(IExternalUserIdentity user, ExternalSession session)
        {
            var resultOK = new ExternalResult<ExternalSession>(SUCCESS_MESSAGE, SUCCESS_CODE, session);
            var resultKO = new ExternalResult<ExternalSession>(ERROR_MESSAGE, ERROR_CODE);
            var result = resultKO;
            var jsonResult = new TransferHttpPostResult();

            var pendingRounds = this.GetPendingRounds(session);

            if (pendingRounds.IsNullOrEmpty())
            {
                result = resultOK;
            }
            else
            {
                result = null;

                foreach (var pendingRound in pendingRounds)
                {
                    jsonResult = this.CloseRound(user, pendingRound);

                    if (jsonResult.Error == ExternalError.OK)
                    {
                        result = (result == resultKO) ? resultKO : resultOK;

                        if (result == resultKO)
                        {
                            break;
                        }
                    }
                    else
                    {
                        result = resultKO;
                    }
                }
            }

            return result;
        }
    }
}
