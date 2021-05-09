using System;
using System.Collections.Generic;
using System.Text;

namespace BlockchainForBussiness {
    public class ServiceHeplers {

        public string PushToWork(List<string> UrlParams) {
            string result = "";
            if (UrlParams?.Count > 0) {
                if (UrlParams[0].ToLower().Equals("contract")) {
                    result = WorkWithContract(UrlParams);
                } else if (UrlParams[0].ToLower().Equals("address")) {
                    result = WorkWithAddress(UrlParams);
                }
            }
            return result;
        }

        public string PushToWork(string PostRequest) {
            return WorkWithContract(PostRequest);
        }

        /// <summary>
        /// /[contract]/[sync | get | asset | trans | tx]/[(ObjectId)Id | (string)Index | (string)Wallet | (string)Tx] | [(string)Symbol, (string)Primer]
        /// </summary>
        /// <param name="UrlParams">[required] <optional></param>
        /// <returns></returns>
        string WorkWithContract(List<string> UrlParams) {
            Contract contract = new Contract();
            try {
                if (UrlParams?.Count > 0) {
                    switch (UrlParams[1].ToLower()) {
                        case "sync":
                            return contract.Sync().ToJson();
                        case "get":
                            if (UrlParams.Count == 3) {
                                string index = UrlParams[2];
                                return contract.Get(index).ToJson();
                            } else if (UrlParams.Count > 3) {
                                string symbol = UrlParams[2];
                                string primer = UrlParams[3];
                                return contract.Get(symbol, primer).ToJson();
                            }
                            break;
                        case "asset":
                            if (UrlParams.Count == 3) {
                                string wallet = UrlParams[2];
                                return contract.GetAssets(wallet).ToJson();
                            }
                            break;
                        case "trans":
                            if (UrlParams.Count == 3) {
                                string wallet = UrlParams[2];
                                return contract.GetTransactions(wallet).ToJson();
                            }
                            break;
                        case "tx":
                            if (UrlParams.Count == 3) {
                                string tx = UrlParams[2];
                                return contract.GetTransaction(tx).ToJson();
                            }
                            break;
                    }
                }
            } catch {
                return "Bad request - " + UrlParams.ToJson();
            }
            return contract.ToJson();
        }

        string WorkWithContract(string PostRequest) {
            return PostRequest;
        }
        /// <summary>
        /// /[address]/[create|get]/<(string)titles | (string)publicKey>
        /// </summary>
        /// <param name="UrlParams">[required] <optional></param>
        /// <returns></returns>
        string WorkWithAddress(List<string> UrlParams) {
            Address adr = new Address();
            try {
                switch (UrlParams[1].ToLower()) {
                    case "create":
                        string titles = null;
                        if (UrlParams.Count > 2)
                            titles = UrlParams[2];
                        adr = adr.Create(titles);
                        break;
                    case "get":
                        string publicKey = null;
                        if (UrlParams.Count > 2)
                            publicKey = UrlParams[2];
                        adr = adr.Get(publicKey);
                        break;
                }
            } catch {
                return "Bad request - " + UrlParams.ToJson();
            }

            return adr.ToJson();
        }
    }
}
