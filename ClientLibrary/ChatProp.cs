using System.Collections.Generic;

namespace ClientLibrary
{
    //Stores the clients name and it's messages
    public class ChatLines
    {
        public string Name;
        public readonly List<int[]> Messages = new List<int[]>();

        //Constarctor
        public ChatLines(string name)
        {
            Name = name;
        }
    }
}