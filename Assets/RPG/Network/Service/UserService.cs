using Newtonsoft.Json.Linq;

namespace RPG.Network.Service {
    public static class UserService {
        public static string ConvertUserToForm(string login, string username, string password, string serverName) {
            JObject objToPush = new JObject();
            objToPush["login"] = login;
            objToPush["username"] = username;
            objToPush["password"] = password;
            objToPush["server_name"] = serverName;
            
            return objToPush.ToString();
        }
    }
}
