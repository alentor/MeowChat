using System.Drawing;
using System.Net;

namespace MeowChatServerLibrary
{
    public delegate void TabPagePrivateServerDoActionHandler(string clientName0, string clientName1, string message, TabPagePrivateChatServer.TabCommand command);

    public delegate void FrmServerImagesChangeNameHandler(string tabname, string tabNameNew);

    public delegate void ServerNetworkEngineEngineServerStartedHandler();

    public delegate void ServerNetworkEngineServerStopBeganHandler(int clientsCount);

    public delegate void ServerNetworkEngineServerStopTickHandler(string currentDisconnectintClientName);

    public delegate void ServerNetworkEngineServerStoppedHandler();

    public delegate void ServerNetworkEngineEngineClientToAddHandler(string clientNameToAdd, IPEndPoint clientNameToAddIpEndPoint);

    public delegate void ServerNetworkEngineClientToRemoveHandler(string clientNameToRemove);

    public delegate void ServerNetworkEngineSendPublicMessageHandler(string clientName, Color clientColor, string message);

    public delegate void ServerNetworkEngineClientColorChangedHandler(string clientName, Color newClientColor);

    public delegate void ServerNetworkEngineClientNameChangedHandler(string clientName, string newClientName);

    public delegate void ServerNetworkEnginePrivateChatStartedHandler(string clientName, string clientNamePrivate);

    public delegate void ServerNetworkEnginePrivateChatMessageHandler(string clientName, string clientNamePrivate, string message);

    public delegate void ServerNetworkEnginePrivateChatStoppedHandler(string clientName, string clientNamePrivate);

    public delegate void ServerNetworkEngineImageMessageHandler(Image img, string clientName, string clientNamePrivate);

    //public delegate void ServerNetworkEngineSendPublicMessageHandler(Client client, string message);

    public delegate void ServerDateEngineRefreshMessagesDbHandler();

    public delegate void ServerDataEngineRefreshClientsDbHandler();
}