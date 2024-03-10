using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using RPG.Creatures.Player;
using RPG.DB;
using RPG.Network.Client;
using RPG.Utils;
using UnityEngine;

namespace RPG.Network.Server {
    public class ServerGameManager {

        public ServerGameManager() {
            CreateTcpServer();
        }
        
        private void CreateTcpServer() {
            Debug.Log($"Starting TCP server on port: {PropertyConstants.PORT}");

            TcpListener listener = new TcpListener(IPAddress.Any, PropertyConstants.PORT);
            listener.Start();
            listener.BeginAcceptTcpClient(OnTcpReceived, listener);
        }
        
        private void OnTcpReceived(IAsyncResult ar) {
            TcpListener listener = (TcpListener)ar.AsyncState;
            listener.BeginAcceptTcpClient(OnTcpReceived, ar.AsyncState);
            
            using (TcpClient client = listener.EndAcceptTcpClient(ar))
            using (var stream = client.GetStream()) {
                var payload = ServerUtils.ReadData(stream);
                if (payload.Length == 0) {
                    var failResponse = "{\"message\": \"Error during authentication\"}";
                    var responsePayload = Encoding.ASCII.GetBytes(failResponse);
                    stream.Write(responsePayload, 0, responsePayload.Length);
                }
                else {
                    var requestJson = Encoding.ASCII.GetString(payload);
                    var loginRequest = JsonUtility.FromJson<AuthCredentials>(requestJson);
                    byte[] hashedPassword;
                    var model = DataBase.Instance.Connection.All<PlayerModel>().ToList().Find(model => model.PlayerLogin == loginRequest.Login);
                    using (HashAlgorithm algotithm = SHA256.Create()) {
                        hashedPassword = algotithm.ComputeHash(Encoding.UTF8.GetBytes(loginRequest.Password));
                    }

                    if (model.HashedPassword != Encoding.ASCII.GetString(hashedPassword)) {
                        var failResponse = "{\"message\": \"Wrong credentials.\"}";
                        var responsePayload = Encoding.ASCII.GetBytes(failResponse);
                        stream.Write(responsePayload, 0, responsePayload.Length);
                    }
                    else {
                        var jsonResponse = JsonUtility.ToJson(model);
                        var jsonPayload = Encoding.ASCII.GetBytes(jsonResponse);
                        stream.Write(payload, 0, payload.Length);
                    }
                    
                }
            }
        }
    }
}
