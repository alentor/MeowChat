using System.Collections.Generic;

namespace MeowChatClientLibrary
{/// <summary>
/// Stores the information of all the clients which are connected to the server and their messages
/// </summary>
    public class ClientChatHistory
    {
        public string Name;
        public readonly List<int[]> Messages = new List<int[]>();

        //Constarctor
        public ClientChatHistory(string name)
        {
            Name = name;
        }
    }
}