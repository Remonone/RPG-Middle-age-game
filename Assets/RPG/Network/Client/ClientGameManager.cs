using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using RPG.Creatures.Player;

namespace RPG.Network.Client {
    public class ClientGameManager {

        private JToken _playerModel;

        public JToken Model => _playerModel;
        
        public async Task InitClient() {
            
        }

        public void SetData(JToken model) {
            _playerModel = model;
        }
    }
}
