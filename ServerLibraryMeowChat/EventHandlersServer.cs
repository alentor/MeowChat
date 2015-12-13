namespace MeowChatServerLibrary
{
    public delegate void TabPagePrivateChatReceiveServerHandler(string tabName0, string tabName1, string message, int caseId);

    public delegate void FrmServerImagesChangeNameHandler(string tabname, string tabNameNew);
}