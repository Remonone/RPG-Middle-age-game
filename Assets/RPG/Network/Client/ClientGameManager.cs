using System.Threading.Tasks;
using RPG.Creatures.Player;

namespace RPG.Network.Client {
    public class ClientGameManager {

        private string _serverId;
        private PlayerModel _playerModel;

        public PlayerModel Model => _playerModel;
        
        public async Task InitClient() {
            
        }

        public void SetCredentials(string serverId, PlayerModel model) {
            _serverId = serverId;
            _playerModel = model;
        }
    }
}
