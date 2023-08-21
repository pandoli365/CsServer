namespace Server.System
{
    public class ErrorResp : Resp
    {
        public string message;
        public ErrorResp(RuntimeException ex)
        {
            this.status = (int)ex.status;
            this.message = ex.Message;
        }
        public ErrorResp()
        {
            this.status = -1;
            this.message = "Unknown Error";
        }

    }

    public class RuntimeException : Exception 
    { 
        public Error status;

        public RuntimeException(string message = "", Error status = Error.RuntimeException) : base(message) 
        {
            this.status = status;
        }
    }
}
