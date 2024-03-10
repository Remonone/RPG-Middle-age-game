using System.Threading.Tasks;

namespace RPG.Network.Client {
    public class ClientGameManager {

        private string _playerCredentials;
        
        public async Task InitClient() {
            
        }

        public void SetCredentials(string credentials) {
            _playerCredentials = credentials;
        }
    }
}
