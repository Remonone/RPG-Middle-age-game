using System.IO;
using System.Net.Sockets;

namespace RPG.Utils {
    public static class ServerUtils {

        public static byte[] ReadData(NetworkStream stream) {
            byte[] buffer = new byte[128];
            using (MemoryStream memStream = new MemoryStream()) {
                do {
                    stream.Read(buffer, 0, buffer.Length);
                    memStream.Write(buffer, 0, buffer.Length);
                } while (stream.DataAvailable);

                return memStream.ToArray();
            }
        }
    }
}
