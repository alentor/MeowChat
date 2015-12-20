using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibraryMeowChat {
    public enum Command {
        Regiter,
        AttemptLogin,
        Login,
        Logout,
        Message,
        List,
        NameChange,
        ColorChanged,
        Disconnect,
        PrivateStarted,
        PrivateMessage,
        PrivateStopped,
        ServerMessage,
        ImageMessage,
        Null //No command, only used in MessageStructure constarctor
    }

    public class MessageStructure {
        //Constructor
        public MessageStructure() {
            Command = Command.Null;
            Color = null;
            ClientName = null;
            Private = null;
            Message = null;
            ImgByte = null;
        }

        public Command Command; //Command type (Login, Logout, Message etc...)
        public string ClientName; //The name by which the server and client recognizes the establised connection(client) also the name which is displayed in the UI
        public string Color; //Reserved for Color of the message
        public string Private; // Reserved for if the message is private
        public string Message; //The message itself, it can be anything, any kind of information which can fit into a string
        public byte[] ImgByte;

        //Convert bytes[] into MessageStructure object
        public MessageStructure(byte[] data) {
            Command = (Command) BitConverter.ToInt32(data, 0);
            //Next four bytes store the length of the clientName
            int clientNameLen = BitConverter.ToInt32(data, 4);
            //Next four bytes store the length of the color
            int colorLen = BitConverter.ToInt32(data, 8);
            //Next four bytes store the length of the Private
            int privateLen = BitConverter.ToInt32(data, 12);
            //Next four bytes store the length of the message
            int messageLen = BitConverter.ToInt32(data, 16);
            //Make sure that clientNameLen has been passed in the bytes array
            ClientName = clientNameLen > 0 ? Encoding.UTF8.GetString(data, 24, clientNameLen) : null;
            //Make sure that colorLen has been passed in the bytes array
            Color = colorLen > 0 ? Encoding.UTF8.GetString(data, 24 + clientNameLen, colorLen) : null;
            //Make sure that colorLen has been passed in the bytes array
            Private = privateLen > 0 ? Encoding.UTF8.GetString(data, 24 + clientNameLen + colorLen, privateLen) : null;
            //Make sure that messageLen has been passed in the bytes array
            Message = messageLen > 0 ? Encoding.UTF8.GetString(data, 24 + clientNameLen + colorLen + privateLen, messageLen) : null;
            //Check if it's an image command, if it's add the representative bytes, if not leave it empty
            ImgByte = Command == Command.ImageMessage ? data.Skip(24 + clientNameLen + colorLen + privateLen + messageLen).ToArray() : null;
        }

        //Convert MessageStructure object into bytes[]
        public byte[] ToByte() {
            //emptyByte for usage in LINQ expression
            byte[] emptyByte = {};
            //create list bytes to which the object MessageStructure will be converted
            List <byte> bytesList = new List <byte>();
            //First add command to the bytesList
            bytesList.AddRange(BitConverter.GetBytes((int) Command));
            //add ClientName length to the bytesList, add zero bytes if clintName is null
            bytesList.AddRange(ClientName != null ? BitConverter.GetBytes(ClientName.Length) : BitConverter.GetBytes(0));
            //add Color length to the bytesList, add zero bytes if clintName is null
            bytesList.AddRange(Color != null ? BitConverter.GetBytes(Color.Length) : BitConverter.GetBytes(0));
            //add Private length to the bytesList, add zero bytes if clintName is null
            bytesList.AddRange(Private != null ? BitConverter.GetBytes(Private.Length) : BitConverter.GetBytes(0));
            //add Message length to the bytes bytesList, add zero bytes if message is null
            bytesList.AddRange(Message != null ? BitConverter.GetBytes(Message.Length) : BitConverter.GetBytes(0));
            //add ImageMessage length to the bytes bytesList, add zero bytes if message is null
            bytesList.AddRange(ImgByte != null ? BitConverter.GetBytes(ImgByte.Length) : BitConverter.GetBytes(0));
            //Add ClientName to the bytesList
            bytesList.AddRange(ClientName != null ? Encoding.UTF8.GetBytes(ClientName) : emptyByte);
            //Add Color to the bytesList
            bytesList.AddRange(Color != null ? Encoding.UTF8.GetBytes(Color) : emptyByte);
            //Add Private to the bytesList
            bytesList.AddRange(Private != null ? Encoding.UTF8.GetBytes(Private) : emptyByte);
            //Add Message to the bytesList
            bytesList.AddRange(Message != null ? Encoding.UTF8.GetBytes(Message) : emptyByte);
            //Add ImageMessage to the bytesList
            bytesList.AddRange(ImgByte != null ? ImgByte : emptyByte);
            // The above can be also done using the following expression
            //bytesList.AddRange(ImgByte ?? BitConverter.GetBytes(0));

            //convert List to array of byte since you can send only arrays of bytes thro the TCP protocol.
            return bytesList.ToArray();
        }
    }
}