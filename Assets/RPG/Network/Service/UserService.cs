using Newtonsoft.Json.Linq;

namespace RPG.Network.Service {
    public static class UserService {
        public static string ConvertUserToForm(string login, string username, string password) {
            JObject objToPush = new JObject();
            objToPush["login"] = login;
            objToPush["username"] = username;
            objToPush["password"] = password;
            return objToPush.ToString();
        }
    }
}
