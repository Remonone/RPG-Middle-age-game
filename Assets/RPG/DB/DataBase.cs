using Realms;
using RPG.Utils;

namespace RPG.DB {
    public class DataBase {
        private static DataBase _db;

        private Realm _connection;
        
        public static DataBase Instance => _db;

        private DataBase(string credentials) {
            _connection = Realm.GetInstance(credentials);
        }

        public static void CreateConnection() {
            var credentials = (string)SecretDataFetcher.FetchProperty("database");
            _db = new DataBase(credentials);
        }
        
    }
}
