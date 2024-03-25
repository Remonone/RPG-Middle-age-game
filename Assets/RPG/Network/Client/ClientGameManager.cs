using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using RPG.Creatures.Player;

namespace RPG.Network.Client {
    public class ClientGameManager {

        private string _jwtData;

        public string Credentials => _jwtData;
        
        public async Task InitClient() {
            
        }

        public void SetData(string data) {
            _jwtData = data;
        }
    }
}
