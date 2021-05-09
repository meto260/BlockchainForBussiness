using LiteDB;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlockchainForBussiness {
    public class Block {

        public ObjectId Id { get; set; }
        public ObjectId PreviousId { get; set; }
        public long Nonce { get; set; }
        public long BlockNonce { get; set; } = 1;

        /// <summary>
        /// Wallet Hash String
        /// </summary>
        public string OwnerWallet { get; set; }

        /// <summary>
        /// For token blocks
        /// </summary>
        public bool Spendable { get; set; }
        public decimal Amount { get; set; }
        public string Symbol { get; set; }

        /// <summary>
        /// TxCode
        /// </summary>
        public Transaction Transaction { get; set; }

        public Contract CreateByContract { get; set; }
        public DateTimeOffset Timestamp { get; set; }

        /// <summary>
        /// Unique index for searchs
        /// </summary>
        public string Index { get; set; }

        /// <summary>
        /// For grouping
        /// </summary>
        public string Primer { get; set; }

        public string ContentType { get; set; }

        /// <summary>
        /// storage for any data
        /// </summary>
        public string DataBag { get; set; }
    }

    public static class ContentTypes {
        public static readonly string Contract = "Contract";
        public static readonly string Document = "Document";
        public static readonly string Image = "Image";
        public static readonly string File = "File";
        public static readonly string Json = "Json";
        public static readonly string Xml = "Xml";
    }
}
