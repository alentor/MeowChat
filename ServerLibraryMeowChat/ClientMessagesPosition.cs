using System.Collections.Generic;

namespace MeowChatServerLibrary
{/// <summary>
/// class which keeps track on each messages position in order to chnage thier color if color changed
/// </summary>
    public class ClientMessagesPosition
    {
        //Name of the Client
        public string ClientName;

        //A list which saves all the positions of the messages which been sent by the lined Name
        public readonly List<int[]> Messages = new List<int[]>();

        //Default Constructor
        public ClientMessagesPosition(string name)
        {
            ClientName = name;
        }
    }
}