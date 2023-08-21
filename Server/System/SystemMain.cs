using System.Reflection;
using NLog;

namespace Server.System {
    public class ProtocolProcessor {
        private static Dictionary<Protocol, AbstractService> SERVICE_DIC = new Dictionary<Protocol, AbstractService>();

        private static readonly NLog.ILogger logger = LogManager.GetCurrentClassLogger();


        public static void addProtocol(AbstractService abstractService) {
            if (SERVICE_DIC.ContainsKey(abstractService.ProtocolValue())) {
                logger.Error("중복된 프로토콜 : " + abstractService.ProtocolValue());
                throw new Exception("중복된 프로토콜 : " + abstractService.ProtocolValue());
            }
               
            SERVICE_DIC.Add(abstractService.ProtocolValue(), abstractService);
        }

        public static void init() {
            // 현재 실행 중인 어셈블리를 가져옴
            var assembly = Assembly.GetExecutingAssembly();

            // 'AbstractService'의 하위 클래스를 모두 찾음
            var serviceTypes = assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(AbstractService)) && !t.IsAbstract);

            // 각 클래스의 인스턴스를 생성합니다. 생성자에서 자동으로 등록됩니다.
            foreach (var type in serviceTypes) 
                addProtocol((AbstractService)Activator.CreateInstance(type));

            logger.Info("Server Start");
        }

        public static string Process(HttpContext context) {
            AbstractService abstractService;
            string Response = "";
            try {
                Protocol cmd = (Protocol)int.Parse(context.Request.Headers["cmd"]);
                SERVICE_DIC.TryGetValue(cmd, out abstractService);
                if (abstractService == null)
                    throw new RuntimeException("Not Found", Error.notFound);

                string body = Request(context.Request).GetAwaiter().GetResult();

                logger.Info("GetRequst : " + body);

                Req req = abstractService.Requst(body);

                if (req == null)
                    throw new RuntimeException("", Error.nodata);
                else if (!req.IsReceivedAllField())
                    throw new RuntimeException("Internal Server Error", Error.unknown);

                Response = abstractService.Process();

                logger.Info("GetResponse : " + Response);
            }
            catch (RuntimeException ex) {
                ErrorResp error = new ErrorResp(ex);
                Response = error.ToJson();
                logger.Error("GetErrorResponse : " + Response);
            }
            catch (Exception ex) {
                ErrorResp error = new ErrorResp();
                Response = error.ToJson();
                logger.Error("GetErrorResponse : " + ex.ToString());
            }
            return Response;

        }

        private static async Task<string> Request(HttpRequest request) {
            using var reader = new StreamReader(request.Body);
            return await reader.ReadToEndAsync();
        }
    }
}