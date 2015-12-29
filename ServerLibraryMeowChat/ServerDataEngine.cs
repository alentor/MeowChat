using System.Data;
using System.Data.SqlClient;

namespace MeowChatServerLibrary
{
    public static class ServerDataEngine
    {
        public static ServerDataEngineRefreshClientsDbHandler ServerDataEngineRefreshClientsDbEvent;
        public static ServerDateEngineRefreshMessagesDbHandler ServerDateEngineRefreshMessagesDbEvent;
        private static readonly string sr_sqlConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|SERVERDATABASE.MDF;Integrated Security=True";
        private static readonly SqlConnection _SqlConn = new SqlConnection(sr_sqlConnectionString);
        private static SqlDataAdapter _Adp = new SqlDataAdapter();
        private static SqlDataAdapter _sqlDataAdapterC;
        private static DataSet _DataSet = new DataSet("ServerDataSet");
        private static SqlCommand s_sqlCommSelectAllCleintsDb;
        private static SqlCommand s_sqlCommSelectAllMessagesDb;

        public static void StartDataServer()
        {
            if ((_SqlConn.State & ConnectionState.Open) != 0)
            {
                _SqlConn.Close();
            }
            _SqlConn.Open();
            _DataSet.Tables.Add("ClientsDB");
            _DataSet.Tables.Add("MessagesDB");
        }

        public static DataSet GetDataSet()
        {
            _DataSet.Tables["ClientsDB"].Clear();
            _DataSet.Tables["MessagesDB"].Clear();
            // ClientsDB
            s_sqlCommSelectAllCleintsDb = new SqlCommand("select * from ClientsDB", _SqlConn);
            _Adp.SelectCommand = s_sqlCommSelectAllCleintsDb;
            _Adp.Fill(_DataSet.Tables["ClientsDB"]);
            // MessagesDB
            s_sqlCommSelectAllMessagesDb = new SqlCommand("select * from MessagesDB", _SqlConn);
            _Adp.SelectCommand = s_sqlCommSelectAllMessagesDb;
            _Adp.Fill(_DataSet.Tables["MessagesDB"]);
            return _DataSet;
        }

        public static DataTable GetClientsDbTable()
        {
            _DataSet.Tables["ClientsDB"].Clear();
            s_sqlCommSelectAllCleintsDb = new SqlCommand("select * from ClientsDB", _SqlConn);
            _Adp.SelectCommand = s_sqlCommSelectAllCleintsDb;
            _Adp.Fill(_DataSet.Tables["ClientsDB"]);
            return _DataSet.Tables["ClientsDB"];
        }

        public static DataTable GetMessagesDbTable()
        {
            _DataSet.Tables["MessagesDB"].Clear();
            s_sqlCommSelectAllCleintsDb = new SqlCommand("select * from MessagesDB", _SqlConn);
            _Adp.SelectCommand = s_sqlCommSelectAllCleintsDb;
            _Adp.Fill(_DataSet.Tables["MessagesDB"]);
            return _DataSet.Tables["MessagesDB"];
        }

        public static string Register(string userName, string firstName, string lastConnected, string status)
        {
            SqlCommand sqlCommCheckIfRegistered = new SqlCommand("select userName from ClientsDB where userName = @userName", _SqlConn);
            sqlCommCheckIfRegistered.Parameters.AddWithValue("@userName", userName);
            object checkIfRegistered = sqlCommCheckIfRegistered.ExecuteScalar();
            if (checkIfRegistered != null)
            {
                return "That User Name already taken, try another";
            }
            SqlCommand sqlCommRegister = new SqlCommand("insert ClientsDB values (@userName, @firstName, @lastConnected, @status)", _SqlConn);
            sqlCommRegister.Parameters.AddWithValue("@userName", userName);
            sqlCommRegister.Parameters.AddWithValue("@firstName", firstName);
            sqlCommRegister.Parameters.AddWithValue("@lastConnected", lastConnected);
            sqlCommRegister.Parameters.AddWithValue("@status", status);
            sqlCommRegister.ExecuteNonQuery();
            ServerDataEngineRefreshClientsDbEvent?.Invoke();
            return "Registerted";
        }

        public static string CheckStatus(string userName)
        {
            SqlCommand sqlCommCheckStatus = new SqlCommand("select status from ClientsDB where UserName = @userName", _SqlConn);
            sqlCommCheckStatus.Parameters.AddWithValue("@userName", userName);
            object checkStatus = sqlCommCheckStatus.ExecuteScalar();
            string returnString = (string)checkStatus;
            return returnString;
        }

        public static int CheckIfRegistered(string userName)
        {
            SqlCommand sqlCommCheckIfRegistered = new SqlCommand("select * from ClientsDB where UserName = @userName", _SqlConn);
            sqlCommCheckIfRegistered.Parameters.AddWithValue("@userName", userName);
            object checkIfRegistered = sqlCommCheckIfRegistered.ExecuteScalar();
            int returnInt = 1;
            if (checkIfRegistered == null)
            {
                returnInt = 0;
            }
            return returnInt;
        }

        public static void ResetStatus()
        {
            SqlCommand sqlCommUpdateStatus = new SqlCommand("update ClientsDB set Status ='Offline'", _SqlConn);
            sqlCommUpdateStatus.ExecuteNonQuery();
            ServerDataEngineRefreshClientsDbEvent?.Invoke();
        }

        public static void UpdateStatus(string userName, string clientName, string status)
        {
            SqlCommand sqlCommUpdateStatus = new SqlCommand("update ClientsDB set Status = @status, [Logged/Last Logged] = @clientName where UserName = @userName", _SqlConn);
            sqlCommUpdateStatus.Parameters.AddWithValue("@userName", userName);
            sqlCommUpdateStatus.Parameters.AddWithValue("@status", status);
            sqlCommUpdateStatus.Parameters.AddWithValue("@clientName", clientName);
            sqlCommUpdateStatus.ExecuteNonQuery();
            ServerDataEngineRefreshClientsDbEvent?.Invoke();
        }

        public static void UpdateDate(string userName, string dateTime)
        {
            SqlCommand sqlCommUpdateDate = new SqlCommand("update ClientsDB set LastConnected =@dateTime where UserName = @userName", _SqlConn);
            sqlCommUpdateDate.Parameters.AddWithValue("@userName", userName);
            sqlCommUpdateDate.Parameters.AddWithValue("@dateTime", dateTime);
            sqlCommUpdateDate.ExecuteNonQuery();
            ServerDataEngineRefreshClientsDbEvent?.Invoke();
        }

        public static void UpdateLoggedLastLogged(string userName, string clientName)
        {
            SqlCommand sqlCommUpdateLoggedLastLogged = new SqlCommand("update ClientsDB set [Logged/Last Logged] = @clientName where UserName = @userName", _SqlConn);
            sqlCommUpdateLoggedLastLogged.Parameters.AddWithValue("@userName", userName);
            sqlCommUpdateLoggedLastLogged.Parameters.AddWithValue("@clientName", clientName);
            sqlCommUpdateLoggedLastLogged.ExecuteNonQuery();
            ServerDataEngineRefreshClientsDbEvent?.Invoke();
        }

        public static void AddMessage(string userName, string message, string timeDate, string isPrivate, string isImage)
        {
            SqlCommand sqlCommGetID = new SqlCommand("select ID from ClientsDB where UserName = @userName", _SqlConn);
            sqlCommGetID.Parameters.AddWithValue("@userName", userName);
            object getIDobj = sqlCommGetID.ExecuteScalar();
            int id = (int)getIDobj;

            SqlCommand sqlCommInsertMessage = new SqlCommand("insert MessagesDB values (@id, @userName, @message, @timeDate, @isPrivate, @isImage)", _SqlConn);
            sqlCommInsertMessage.Parameters.AddWithValue("@id", id);
            sqlCommInsertMessage.Parameters.AddWithValue("@userName", userName);
            sqlCommInsertMessage.Parameters.AddWithValue("@message", message);
            sqlCommInsertMessage.Parameters.AddWithValue("@timeDate", timeDate);
            sqlCommInsertMessage.Parameters.AddWithValue("@isPrivate", isPrivate);
            sqlCommInsertMessage.Parameters.AddWithValue("@isImage", isImage);
            sqlCommInsertMessage.ExecuteNonQuery();
            ServerDateEngineRefreshMessagesDbEvent?.Invoke();
        }

        public static DataTable SearchID(int id)
        {
            _DataSet.Tables["ClientsDB"].Clear();
            SqlCommand sqlCommSearchID = new SqlCommand("select * from ClientsDB where ID = @id", _SqlConn);
            sqlCommSearchID.Parameters.AddWithValue("@id", id);
            _Adp.SelectCommand = sqlCommSearchID;
            _Adp.Fill(_DataSet.Tables["ClientsDB"]);
            return _DataSet.Tables["ClientsDB"];
        }

        public static DataTable SearchUserName(string userName)
        {
            _DataSet.Tables["ClientsDB"].Clear();
            SqlCommand sqlCommSearchUserName = new SqlCommand("select * from ClientsDB where UserName = @userName", _SqlConn);
            sqlCommSearchUserName.Parameters.AddWithValue("@userName", userName);
            _Adp.SelectCommand = sqlCommSearchUserName;
            _Adp.Fill(_DataSet.Tables["ClientsDB"]);
            return _DataSet.Tables["ClientsDB"];
        }

        public static DataTable SearchLoggedLastLogged(string clientName)
        {
            _DataSet.Tables["ClientsDB"].Clear();
            SqlCommand sqlCommSearchLoggedLastLogged = new SqlCommand("select * from ClientsDB where [Logged/Last Logged] = @clientName", _SqlConn);
            sqlCommSearchLoggedLastLogged.Parameters.AddWithValue("@clientName", clientName);
            _Adp.SelectCommand = sqlCommSearchLoggedLastLogged;
            _Adp.Fill(_DataSet.Tables["ClientsDB"]);
            return _DataSet.Tables["ClientsDB"];
        }

        public static DataTable SearchMessagesByKeyword(string keyWord)
        {
            _DataSet.Tables["MessagesDB"].Clear();
            SqlCommand sqlCommSearchKeyWord = new SqlCommand("select * from MessagesDB where Message like @keyWord", _SqlConn);
            sqlCommSearchKeyWord.Parameters.AddWithValue("@keyWord", "%" + keyWord + "%");
            _Adp.SelectCommand = sqlCommSearchKeyWord;
            _Adp.Fill(_DataSet.Tables["MessagesDB"]);
            return _DataSet.Tables["MessagesDB"];
        }

        public static DataTable SearchByDate(string date)
        {
            _DataSet.Tables["MessagesDB"].Clear();
            SqlCommand sqlCommSearchDate = new SqlCommand("select * from MessagesDB where Date like @date", _SqlConn);
            sqlCommSearchDate.Parameters.AddWithValue("@date", "%" + date + "%");
            _Adp.SelectCommand = sqlCommSearchDate;
            _Adp.Fill(_DataSet.Tables["MessagesDB"]);
            return _DataSet.Tables["MessagesDB"];
        }

        public static DataTable SearchByPrivate(string userName)
        {
            _DataSet.Tables["MessagesDB"].Clear();
            SqlCommand sqlCommSearchUserName = new SqlCommand("select * from MessagesDB where IsPrivate = @userName", _SqlConn);
            sqlCommSearchUserName.Parameters.AddWithValue("@userName", userName);
            _Adp.SelectCommand = sqlCommSearchUserName;
            _Adp.Fill(_DataSet.Tables["MessagesDB"]);
            return _DataSet.Tables["MessagesDB"];
        }
    }
}