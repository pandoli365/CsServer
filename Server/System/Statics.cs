namespace Server.System
{
    public static class STATICS
    {
        #region Dev 
        #if DEBUG
        public static readonly string SQL_URL = "Server=myServerAddress;Port=myPort;Database=myDatabase;Uid=myUsername;Pwd=myPassword;";
#endif
        #endregion

        public static readonly string PATTERN = "[^a-zA-Z0-9가-힣 ]";
    }

}
