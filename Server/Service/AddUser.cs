using Server.System;
using Newtonsoft.Json;

namespace Server.Service
{
    public class AddUser : AbstractService
    {
        private AddUserReq req;
        public override string Process()
        {
            return makeResp();
        }

        public override Protocol ProtocolValue() => Protocol.AddUser;

        public override Req Requst(string json)
        {
            req = JsonConvert.DeserializeObject<AddUserReq>(json);
            return req;
        }

        private string makeResp()
        {
            AddUserResp resp = new AddUserResp();
            resp.status = 200;
            return resp.ToJson();
        }


    }

    public class AddUserReq : Req
    {
        public override bool IsReceivedAllField()
        {
            return true;
        }
    }

    public class AddUserResp : Resp
    {

    }
}
