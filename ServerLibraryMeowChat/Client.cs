using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Net.Sockets;

namespace MeowChatServerLibrary {
    public class Client {
        // The username of the the established connection(cleint) in the DataBase, a unique modifer
        public string UserName;

        // Name of the established connection(cleint)
        public string Name;

        // Socket of the established connection(client)
        public Socket Socket;

        // The color of the client => assigned, but not being used
        public Color Color;

        // The IP address of the client
        public IPEndPoint IpEndPoint;
    }
}