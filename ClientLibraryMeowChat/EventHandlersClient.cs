namespace MeowChatClientLibrary {
    public delegate void FrmLoginCloseHandler();

    public delegate void TabPagePrivateChatSendClietHandler(string clientName, string message);

    public delegate void TabPagePrivateChatReceiveClientHandler(string tabName, string privateName, string message, int caseId);

    public delegate void FrmStatisticsUpdateHandler(StatisticsEntry staticsEntry);

    public delegate void FrmClientImagesChangeNameHandler(string tabname, string tabNameNew);

    public delegate void ClientNetworkEngineLoggedinHandler();

    public delegate void ClientEngineLoginErrorHandler(string errorMessage);
}