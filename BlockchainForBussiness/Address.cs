using LiteDB;
using NBitcoin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlockchainForBussiness {
    public class Address {
        public ObjectId Id { get; set; }

        /// <summary>
        /// Wallet address
        /// </summary>
        public string PublicKey { get; set; }

        /// <summary>
        /// Hashed secret key
        /// </summary>
        public string PrivateKey { get; set; }
        public string Titles { get; set; }

        DB db;

        public Address() {
            db = DB.Create("_addresses");
        }

        public Address Create(string __Titles = null) {
            Key key = new Key();
            var adr = new Address {
                PublicKey = key.PubKey.GetAddress(ScriptPubKeyType.Legacy, Network.Main).ToString(),
                PrivateKey = key.GetBitcoinSecret(Network.Main).ToString(),
                Titles = __Titles
            };
            var insertAdr = new Address {
                PublicKey = adr.PublicKey.ToString(),
                PrivateKey = adr.PrivateKey.EncryptHash(),
                Titles = adr.Titles
            };
            db.Insert(insertAdr).EnsureIndex(x => x.PublicKey);
            adr.Id = insertAdr.Id;
            return adr;
        }

        public Address Get(string __PublicKey) {
            return db.Find<Address>(Query.EQ("PublicKey", __PublicKey)).FirstOrDefault();
        }
    }
}
