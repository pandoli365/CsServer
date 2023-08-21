using Server.System;
using Newtonsoft.Json;

namespace Server.Service
{
    public class awaketest : AbstractService
    {
        private awaketestReq req;
        public override string Process()
        {
            return makeResp();
        }

        public override Protocol ProtocolValue() => Protocol.Test;
        
        public override Req Requst(string json)
        {
            req = JsonConvert.DeserializeObject<awaketestReq>(json);
            return req;
        }

        private string makeResp()
        {
            awaketestResp resp = new awaketestResp();
            resp.status = 200;
            return resp.ToJson();
        }


    }

    public class awaketestReq : Req
    {
        public override bool IsReceivedAllField()
        {
            return true;
        }
    }

    public class awaketestResp : Resp
    {

    }
}
