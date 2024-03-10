using Realms;
using RPG.Utils;

namespace RPG.DB {
    public class DataBase {
        private static DataBase _db;

        private Realm _connection;

        private static bool _isConnected = false;
        
        public static DataBase Instance => _db;
        
        public Realm Connection => _connection;

        private DataBase(string credentials) {
            _connection = Realm.GetInstance(credentials);
        }

        public static void CreateConnection() {
            if (_isConnected) return;
            var credentials = (string)SecretDataFetcher.FetchProperty("database");
            _db = new DataBase(credentials);
            _isConnected = true;
        }
        
    }
}
