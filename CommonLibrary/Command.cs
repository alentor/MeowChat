namespace CommonLibrary
{
    //These are the commands which are being exchanged between the server and client
    public enum Command
    {
        Login,
        Logout,
        Message,
        List,
        NameChange,
        ColorChange,
        Disconnect,
        PrivateStart,
        PrivateMessage,
        PrivateStop,
        ServerMessage,
        Null //No command, only used in MessageStracture constarctor
    }
}