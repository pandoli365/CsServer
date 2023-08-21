using Newtonsoft.Json;

namespace Server.System
{
    public abstract class AbstractService
    {
        public abstract Protocol ProtocolValue();

        public abstract string Process();

        public abstract Req Requst(string json);
    }

    public abstract class Req
    {
        public Protocol cmd;

        public virtual bool IsReceivedAllField()
        {
            return true;
        }
    }

    public abstract class Resp
    {
        public int status;
        public virtual string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
