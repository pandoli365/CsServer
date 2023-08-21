using Server.System;
using Newtonsoft.Json;

namespace Server.Service
{
    public class Test : AbstractService
    {
        private TestReq req;
        public override string Process()
        {
            return makeResp();
        }

        public override Protocol ProtocolValue() => Protocol.Test;

        public override Req Requst(string json)
        {
            req = JsonConvert.DeserializeObject<TestReq>(json);
            return req;
        }

        private string makeResp()
        {
            TestResp resp = new TestResp();
            resp.status = 200;
            return resp.ToJson();
        }


    }

    public class TestReq : Req
    {
        public override bool IsReceivedAllField()
        {
            return true;
        }
    }

    public class TestResp : Resp
    {

    }
}
