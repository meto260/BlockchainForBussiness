using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlockchainForBussiness {
    public class Contract {
        /// <summary>
        /// Address Public Key
        /// </summary>
        public string FromWallet { get; set; }
        public string FromWalletSecret { get; set; }

        /// <summary>
        /// Address Public Key
        /// </summary>
        public string ToWallet { get; set; }
        public bool Spendable { get; set; } = false;
        public decimal Amount { get; set; } = 0.00000001M;
        public string Symbol { get; set; }
        public string Index { get; set; }
        public string Primer { get; set; }
        public string ContentType { get; set; }
        public string DataBag { get; set; }
        public string TxDescription { get; set; }

        DB db;
        public Contract() {
            db = DB.Create("_blocks");
        }
        public Block CreateBlock() {
            long lastNonce = db.MaxNonce();
            long lastBlockNonce = 1;
            Block foundBlock = null;
            if (lastNonce > 0) {
                foundBlock = db.FindOne<Block>(Query.EQ("Nonce", lastNonce));
                if (!string.IsNullOrEmpty(foundBlock.Index))
                    lastBlockNonce = db.BlockMaxNonce(foundBlock.Index);
            }
            var fromWallet = db.FindOne<Address>(Query.EQ("PublicKey", FromWallet));
            var block = new Block {
                PreviousId = foundBlock?.Id,
                OwnerWallet = fromWallet.EncryptObject(),
                Nonce = lastNonce + 1,
                BlockNonce = lastBlockNonce + 1,
                Spendable = Spendable,
                Amount = Amount,
                Symbol = Symbol,
                CreateByContract = this,
                Transaction = new Transaction {
                    Id = ObjectId.NewObjectId(),
                    From = FromWallet,
                    To = ToWallet,
                    Description = TxDescription
                },
                Timestamp = DateTimeOffset.UtcNow,
                Index = Index,
                Primer = Primer,
                ContentType = ContentType,
                DataBag = DataBag
            };
            var collection = db.Insert(block);
            collection.EnsureIndex(x => x.Index);
            collection.EnsureIndex(x => x.Primer);
            collection.EnsureIndex(x => x.Nonce);
            collection.EnsureIndex(x => x.BlockNonce);
            collection.EnsureIndex(x => x.Symbol);
            collection.EnsureIndex(x => x.Timestamp);
            return block;
        }

        public List<Block> Sync() {
            return db.FindAll<Block>();
        }

        public Block Get(string __Index) {
            return db.FindOne<Block>(Query.EQ("Index", __Index));
        }

        public Block Get(string __Symbol, string __Primer) {
            return db.FindOne<Block>(Query.And(Query.EQ("Symbol", __Symbol), Query.EQ("Primer", __Primer)));
        }

        public List<Block> GetAssets(string __Wallet) {
            return db.Find<Block>(Query.EQ("OwnerWallet", __Wallet));
        }

        public List<Transaction> GetTransactions(string __Wallet) {
            return db.Find<Block>(Query.EQ("Wallet", __Wallet)).Select(x => x.Transaction).ToList();
        }

        public Transaction GetTransaction(string __Tx) {
            return db.FindOne<Block>(Query.EQ("Transaction.Id", new ObjectId(__Tx)))?.Transaction;
        }

        /// <summary>
        /// Transfers must be approved by authority or consensus
        /// </summary>
        /// <param name="__Id">Block Id</param>
        /// <param name="__Sender">Sender Wallet Address</param>
        /// <param name="__Receiver">Receiver Wallet Address Public Key</param>
        /// <param name="__Amount">Transfer Quantity</param>
        /// <param name="__NewDataBag">Optional Anything for save data as text value</param>
        /// <param name="__Description">Optional Description for Transaction</param>
        /// <returns></returns>
        public Block MakeTransfer(
            ObjectId __Id,
            Address __Sender,
            string __Receiver,
            decimal __Amount,
            string __NewDataBag = null,
            string __Description = null,
            string __Primer = null,
            string __ContentType = null,
            string __Index = null) {

            Block result = new Block();
            var foundBlock = db.FindOne<Block>(Query.EQ("Id", __Id));
            var foundSender = db.FindOne<Address>(Query.EQ("PublicKey", __Sender.PublicKey));
            var foundReceiver = db.FindOne<Address>(Query.EQ("PublicKey", __Receiver));
            if (foundBlock.OwnerWallet.EncryptObject() == __Sender.EncryptObject()) {
                if (__Sender.PrivateKey.EncryptHash().Equals(foundSender.PrivateKey) &&
                    foundBlock.Amount >= __Amount) {

                    foundBlock.OwnerWallet = foundReceiver.EncryptObject();
                    foundBlock.PreviousId = __Id;
                    foundBlock.Transaction = new Transaction() {
                        Id = ObjectId.NewObjectId(),
                        From = __Sender.PublicKey,
                        To = __Receiver,
                        Description = __Description,
                        Timestamp = DateTimeOffset.UtcNow
                    };
                    foundBlock.DataBag = __NewDataBag;
                    foundBlock.Primer = __Primer;
                    foundBlock.ContentType = __ContentType;
                    foundBlock.Index = __Index;
                    var collection = db.Insert(foundBlock);
                    collection.EnsureIndex(x => x.Index);
                    collection.EnsureIndex(x => x.Symbol);
                    collection.EnsureIndex(x => x.OwnerWallet);
                    collection.EnsureIndex(x => x.Timestamp);
                    result = foundBlock;
                }
            }
            return result;
        }
    }
}
