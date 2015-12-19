using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowChatServerLibrary {
    public class ClientMessagesPosition {
        //Name of the Client
        public string ClientName;

        //A list which saves all the positions of the messages which been sent by the lined Name
        public readonly List <int[]> Messages = new List <int[]>();

        //Default Constructor
        public ClientMessagesPosition(string name) {
            ClientName = name;
        }
    }
}