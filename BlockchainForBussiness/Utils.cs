using LiteDB;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace BlockchainForBussiness {
    internal static class Utils {
        internal static string ToJson(this object source, Formatting formatting = Formatting.None) {
            if (formatting == Formatting.None)
                return JsonConvert.SerializeObject(source);
            else
                return JsonConvert.SerializeObject(source, formatting);
        }

        internal static T FromJson<T>(this string source) {
            return JsonConvert.DeserializeObject<T>(source);
        }

        internal static string EncryptHash(this string rawData) {
            using (SHA256 sha256Hash = SHA256.Create()) {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++) {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        internal static string EncryptObject(this object source) {
            using (SHA256 sha256Hash = SHA256.Create()) {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(source.ToJson()));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++) {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        internal static List<string> ClearUrlParams(string dirty) {
            return dirty.Split('/').Where(x => !string.IsNullOrEmpty(x)).ToList();
        }
    }

    internal class DB {
        public static LiteDatabase Connection;
        static object locker = new object();
        static DB dbclass;
        static string personalPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        public static DB Create(string dbname) {
            if (dbclass == null) {
                lock (locker) {
                    dbclass = new DB();
                    ConnectionString connectionString = new ConnectionString(personalPath + "/" + dbname + ".db") {
                        Upgrade = true,
                        Password = "metinyakar.net",
                        Connection = ConnectionType.Shared,
                        Collation = Collation.Default
                    };
                    Connection = new LiteDatabase(connectionString) { UtcDate = true };
                }
            }
            return dbclass;
        }
        public List<T> Find<T>(BsonExpression filter) {
            var collection = Connection.GetCollection<T>(typeof(T).Name);
            List<T> result = collection.Find(filter).ToList<T>();
            //Close();
            return result;
        }
        public T FindOne<T>(BsonExpression filter) {
            var collection = Connection.GetCollection<T>(typeof(T).Name);
            T result = collection.FindOne(filter);
            //Close();
            return result;
        }
        public List<T> FindAll<T>() {
            var collection = Connection.GetCollection<T>(typeof(T).Name);
            List<T> result = collection.FindAll().ToList<T>();
            //Close();
            return result;
        }
        public ILiteCollection<T> Insert<T>(T newDocument) {
            var collection = Connection.GetCollection<T>(typeof(T).Name);
            collection.Insert(newDocument);
            return collection;
        }
        public long MaxNonce() {
            long result = 0;
            var collection = Connection.GetCollection<Block>("Block");
            if (collection.Count() > 0) {
                result = collection.Max(x => x.Nonce);
            }
            //Close();
            return result;
        }
        public long BlockMaxNonce(string __BlockIndex) {
            long result = 0;
            var collection = Connection.GetCollection<Block>("Block");
            if (collection.Count() > 0) {
                result = collection.Find(x => x.Index.Equals(__BlockIndex)).Max(x => x.BlockNonce);
            }
            //Close();
            return result;
        }
        public void Close() {
            Connection.Dispose();
        }
    }
}
