
using RPG.Utils;
using UnityEngine;

namespace RPG.DB {
    public class DataBase {
        private static DataBase _db;

        
        public static DataBase Instance => _db;

        private DataBase(string credentials) {
        }

        public static void CreateConnection() {
            var credentials = (string)SecretDataFetcher.FetchProperty("database");
            _db = new DataBase(credentials);
        }
        
    }
}
