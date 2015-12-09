using System.Collections.Generic;

namespace MeowChatClientLibrary
{
    //Stores the clients name and it's messages history
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