using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Net.Sockets;

namespace MeowChatServerLibrary
{
    public class Client
    {
        // A list which saves all the public messages of the client
        public readonly List<int[]> Messages = new List<int[]>();

        // Name of the established connection(cleint)
        public string Name;

        // Socket of the established connection(client)
        public Socket Socket;

        // The color of the client => not being used but being saved save/tracked anyways
        public Color Color;

        // The IP address of the client
        public IPEndPoint IpEndPoint;
    }
}