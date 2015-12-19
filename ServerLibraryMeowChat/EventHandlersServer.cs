using System.Drawing;
using System.Net;

namespace MeowChatServerLibrary {
    public delegate void TabPagePrivateServerDoActionHandler(string clientName0, string clientName1, string message, TabPagePrivateChatServer.TabCommand command);

    public delegate void FrmServerImagesChangeNameHandler(string tabname, string tabNameNew);

    public delegate void ServerEngineServerStartedHandler();

    public delegate void ServerEngineServerStopBeganHandler(int clientsCount);

    public delegate void ServerEngineStopTickHandler(string currentDisconnectintClientName);

    public delegate void ServerEngineStoppedHandler();

    public delegate void ServerEngineClientToAddHandler(string clientNameToAdd, IPEndPoint clientNameToAddIpEndPoint);

    public delegate void ServerEngineClientToRemoveHandler(string clientNameToRemove);

    public delegate void ServerEngineSendPublicMessageHandler(string clientName, Color clientColor, string message);

    public delegate void ServerEngineClientColorChangedHandler(string clientName, Color newClientColor);

    public delegate void ServerEngineClientNameChangedHandler(string clientName, string newClientName);

    public delegate void ServerEnginePrivateChatStartedHandler(string clientName, string clientNamePrivate);

    public delegate void ServerEnginePrivateChatMessageHandler(string clientName, string clientNamePrivate, string message);

    public delegate void ServerEnginePrivateChatStoppedHandler(string clientName, string clientNamePrivate);

    //public delegate void ServerEngineSendPublicMessageHandler(Client client, string message);
}