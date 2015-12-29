using System.Drawing;

namespace MeowChatClientLibrary
{
    public delegate void FrmStatisticsUpdateHandler(StatisticsEntry staticsEntry);

    public delegate void FrmClientImagesChangeNameHandler(string tabname, string tabNameNew);

    public delegate void ClientNetworkEngineRegisterHandler(string message);

    public delegate void ClientNetworkEngineAttemptLoginErrorHandler(string message);

    public delegate void ClientNetworkEngineLoggedinHandler();

    public delegate void ClientNetworkEngineLoginHandler(string clientName, string messsage);

    public delegate void ClientNetworkEngineClientsListHandler(string clientsList);

    public delegate void ClientNetworkEngineDisconnectHandler();

    public delegate void ClientNetworkEngineLogoutHandler(string clientName, string message);

    public delegate void ClientNetworkEngineMessageHandler(string clientName, string message, Color color);

    public delegate void ClientNetworkEngineNameChangeHandler(string clientName, string clientNameNew, Color color);

    public delegate void ClientNetworkEngineColorChangedHandler(string clientName, Color color);

    public delegate void ClientNetworkEnginePrivateChatStartHandler(string clientName);

    public delegate void ClientNetworkEnginePrivateMessageHandler(string clientName, string clientNamePrivate, string message);

    public delegate void ClientNetworkEnginePrivateStoppedHandler(string clientNamePrivate);

    public delegate void ClientNetworkEngineServerMessageHandler(string message);

    public delegate void ClientNetworkEngineImageMessageHandler(string clientName, string clientNamePrivate, Image image);

    public delegate void TabPagePrivateChatSendClietHandler(string clientName, string message);

    public delegate void TabPagePrivateChatSendImageClietHandler(string clientName);

    public delegate void TabPagePrivateChatReceiveClientHandler(string tabName, string privateName, string message, TabPagePrivateChatClient.TabCommand tabCommand);
}