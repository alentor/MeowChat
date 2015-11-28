using System.Collections.Generic;
using System.Net.Sockets;

namespace ServerLibrary
{
    public class Client
    {
        //Socket of the established connection(client)
        public Socket ClientSocket;

        //Name of the established connection(cleint)
        public string ClientName;

        //The color of the client => not being used but being saved save/tracked anyways
        public string Color;

        //A list which saves all the public messages of the client
        public readonly List<int[]> Messages = new List<int[]>();
    }
}