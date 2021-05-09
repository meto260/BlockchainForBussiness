using LiteDB;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlockchainForBussiness {
    public class Transaction {
        public ObjectId Id { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Description { get; set; }
        public DateTimeOffset Timestamp { get; set; }
    }
}
