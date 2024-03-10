using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using RPG.Creatures.Player;
using RPG.Utils;
using UnityEngine;

namespace RPG.Network.Client {
    public static class AuthenticationWrapper {
        
        public static AuthState AuthState { get; private set; } = AuthState.NotAuthenticated;

        
        
        public static async Task<AuthState> DoAuth(AuthCredentials credentials, int capacity = 10) {
            if (AuthState == AuthState.Authenticated) return AuthState;

            AuthState = AuthState.Authenticating;
            for (int tries = 0; tries < capacity; tries++) {
                PlayerModel response = await OnLogin(credentials);
                if(response == null) continue;
                if (response.PlayerID == "") {
                    AuthState = AuthState.Error;
                    return AuthState;
                }

                AuthState = AuthState.Authenticated;
                return AuthState;
            }


            AuthState = AuthState.TimeOut;
            return AuthState;
        }
        
        private static async Task<PlayerModel> OnLogin(AuthCredentials credentials) {
            using (TcpClient client = new TcpClient(credentials.ServerID, PropertyConstants.PORT))
            using (var stream = client.GetStream()) {
                var request = JsonUtility.ToJson(credentials);
                var payload = Encoding.ASCII.GetBytes(request);
                stream.Write(payload, 0, payload.Length);

                var responsePayload = ServerUtils.ReadData(stream);
                var jsonResponse = Encoding.ASCII.GetString(responsePayload);
                var response = JsonUtility.FromJson<PlayerModel>(jsonResponse);
                
                return response;
            }
        }

    }
    
    public class AuthCredentials {
        public string ServerID;
        public int Port = 8888;
        public string Login;
        public string Password;
    }
    
    public enum AuthState {
        NotAuthenticated,
        Authenticating,
        Authenticated,
        Error,
        TimeOut,
    }
}
