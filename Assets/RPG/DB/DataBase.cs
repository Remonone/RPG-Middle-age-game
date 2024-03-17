using System.Threading.Tasks;
using Realms;
using RPG.Utils;

namespace RPG.DB {
    public class DataBase {
        private static DataBase _db;

        private Realm _connection;

        private static bool _isConnected = false;
        
        public static DataBase Instance => _db;
        
        public Realm Connection => _connection;

        private DataBase() {}

        public static async void CreateConnection() {
            if (_isConnected) return;
            var credentials = (string)SecretDataFetcher.FetchProperty("database");
            _db = new DataBase {
                _connection = await GetRealmInstance(credentials)
            };
            _isConnected = true;
        }

        private static async Task<Realm> GetRealmInstance(string credentials) {
            RealmConfigurationBase baseConfig = new RealmConfiguration(credentials);
            var instance = await Realm.GetInstanceAsync(baseConfig);
            return instance;
        }
    }
}
