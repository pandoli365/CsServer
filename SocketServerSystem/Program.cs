using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SocketServerSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            #region UDPSample
            ////client
            //new SampleClientUDP(4868, "175.124.197.128");
            //SampleClientUDP.script.ClientStart();
            //string SendData = "NewUser\npandoli";
            //SampleClientUDP.DataInfo sd = new SampleClientUDP.DataInfo(SendData, null);
            //SampleClientUDP.script.SendData(sd);
            //Console.ReadLine();

            ////server
            //new SampleServerUDP(4868);
            //SampleServerUDP.script.ServerStart();
            #endregion

        }
    }

    /// <summary>
    /// 샘플 코드 입니다 아래와같이 class를 새로 만들어 활용하셔도 좋고 udp를 직접 호출하셔서 사용하셔도 좋습니다
    /// </summary>
    class SampleServerUDP : UDP
    {
        //로비 List
        List<Lobby> LobbyList;
        //참조를 선언한 경우라면 가급적 udp의 직접 접근은 삼가해 주세요
        public static SampleServerUDP script;

        /// <summary>
        /// base는 참조 선언한 클레스에 초기화 파라미터들을 전송해줄때 사용합니다.
        /// _is_Server(true) : 현재 동작하는 시스템이 서버인지 클라이언트인지 구분하는 파라미터 (true : 서버 falst : 클라이언트)
        /// _maxSize(1024) : 패킷의 최대 사이즈 1024가 기본값으로 설정되어 있습니다
        /// _Port(_Port) : 접속을 허용할 혹은 접속할 네트워크 포트번호
        /// _ip("127.0.0.1") : 접속하는 서버의 ip주소 (클라이언트로 선언했을때만 사용됩니다)
        /// 생초보용 서버 세팅 방법
        /// 공유기 사용시1 : 포트포워드 > 외부포트,내부포트를 이곳의 입력한 _Port와 같은번호로 설정한뒤 내부 ip주소를 실행하고 싶은 컴퓨터의 ip주소를 입력
        /// 그뒤 클라이언트에 ip를 입력할때 서버컴퓨터와 연결되어있는 공유기 기본정보 외부ip입력 만약 찾기가 어렵다면 https://url.kr/web_tools/ip/ 접속
        /// 모뎀 직접연결시 : https://url.kr/web_tools/ip/ 접속 나오는 ip를 클라이언트 접속ip에 입력
        /// 
        /// 만약 위의 방법으로 접속이 되지 않는다면 구글 검색창에 "방화벽 포트 열기"검색
        /// 포트를 열어보고 다시시도
        /// </summary>
        /// <param name="_Port"></param>
        public SampleServerUDP(int _Port = 4861) : base(true, 1024, _Port)
        {
            if (script == null)
            {
                script = this;
            }
            else
            {
                //에러 글자색 변경을 위한 코드
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("에러 : 중복된 선언을 하셨습니다");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        /// <summary>
        /// 데이터를 받았을 경우위 처리
        /// 따로 호출하지 않아도 상대방이 데이터를 받으면 자동으로 동작합니다.
        /// </summary>
        /// <param name="di"></param>
        public override void v_Processing(DataInfo di)
        {
            UDPUser user;
            string[] cut = di.data.Split('\n');
            Console.WriteLine(cut[0]);
            switch (cut[0])
            {
                case "NewUser":
                    AddUser(new UDPUser(di.remote, cut[1]));
                    Console.WriteLine("새로운 유저가 접속 했습니다 : {0}",cut[1]);
                    SendData(cut[1], Encoding.Default.GetBytes("새로운 서버에 오신것을 환영 합니다."));
                    break;
                case "JoinLobby":
                    user = UserInfo(di.remote);
                    if (user == null)
                    {
                        Console.WriteLine("유저를 찾을수 없습니다");
                        break;
                    }
                    if (user.pos.Equals(UDPUser.ePos.Server) || user.pos.Equals(UDPUser.ePos.Room))
                    {
                        user.pos = UDPUser.ePos.Lobby;
                        user.lobbyName = cut[1];
                        int lobbyCount = LobbyList.FindIndex(n => n.lobbyName.Equals(cut[1]));
                        if (lobbyCount.Equals(-1))
                        {
                            LobbyList.Add(new Lobby(cut[1]));
                            lobbyCount = LobbyList.FindIndex(n => n.lobbyName.Equals(cut[1]));
                        }
                        LobbyList[lobbyCount].AddUser(user.cid);
                    }
                    break;
                case "OutLobby":
                    user = UserInfo(di.remote);
                    if (user == null)
                    {
                        Console.WriteLine("유저를 찾을수 없습니다");
                        break;
                    }
                    if (user.pos.Equals(UDPUser.ePos.Lobby))
                    {
                        int lobbyCount = LobbyList.FindIndex(n => n.lobbyName.Equals(user.lobbyName));
                        LobbyList[lobbyCount].RemoveUser(user.cid);
                        user.pos = UDPUser.ePos.Server;
                        user.lobbyName = "";
                    }
                    break;
                case "JoinRoom":
                    user = UserInfo(di.remote);
                    if (user == null)
                    {
                        Console.WriteLine("유저를 찾을수 없습니다");
                        break;
                    }
                    if (user.pos.Equals(UDPUser.ePos.Lobby))
                    {
                        user.pos = UDPUser.ePos.Room;
                        user.roomName = cut[1];
                        user.is_RoomPrivate = cut[2].Equals(true);
                        int lobbyCount = LobbyList.FindIndex(n => n.lobbyName.Equals(user.lobbyName));
                        int roomCount = LobbyList[lobbyCount].roomList.FindIndex(n => n.roomName.Equals(cut[1]));
                        if (roomCount.Equals(-1))
                        {
                            LobbyList[lobbyCount].roomList.Add(new Room(cut[1], cut[2].Equals(true)));
                            roomCount = LobbyList[lobbyCount].roomList.FindIndex(n => n.roomName.Equals(cut[1]));
                        }
                        LobbyList[lobbyCount].roomList[roomCount].AddUser(user.cid);
                    }
                    break;
                case "OutRoom":
                    user = UserInfo(di.remote);
                    if (user.pos.Equals(UDPUser.ePos.Room))
                    {
                        int lobbyCount = LobbyList.FindIndex(n => n.lobbyName.Equals(user.lobbyName));
                        int roomCount = LobbyList[lobbyCount].roomList.FindIndex(n => n.roomName.Equals(user.roomName));
                        LobbyList[lobbyCount].roomList[roomCount].RemoveUser(user.cid);
                        user.pos = UDPUser.ePos.Lobby;
                        user.roomName = "";
                        user.is_RoomPrivate = false;
                    }
                    break;
            }
        }

        public override void v_DataSet()
        {
            //로비 리스트 초기화
            LobbyList = new List<Lobby>();
        }
        class Room
        {
            public string roomName; //방이름 비공계 방일경우 다음과 같이 생성됨 방이름:비밀번호
            public bool is_RoomPrivate; //방이 비공계인지 아닌지 구분을함.
            List<string> room_User_Cid;
            public Room(string _roomName, bool _is_RoomPrivate)
            {
                is_RoomPrivate = _is_RoomPrivate;
                roomName = _roomName;
                room_User_Cid = new List<string>();
            }
            public void AddUser(string _cid)
            {
                room_User_Cid.Add(_cid);
            }
            public void RemoveUser(string _cid)
            {
                room_User_Cid.Remove(_cid);
            }
            public void Send(string _data)
            {
                byte[] data = Encoding.Default.GetBytes(_data);
                for (int n = 0; n < room_User_Cid.Count; n++)
                {
                    if (!UDP.script.SendData(room_User_Cid[n], data))
                    {
                        room_User_Cid.RemoveAt(n);
                        n--;
                    }
                }
            }
        }
        class Lobby
        {
            public List<Room> roomList;
            public string lobbyName;
            List<string> room_User_Cid;
            public Lobby(string _lobbyName)
            {
                lobbyName = _lobbyName;
                roomList = new List<Room>();
                room_User_Cid = new List<string>();
            }
            public void AddUser(string _cid)
            {
                room_User_Cid.Add(_cid);
            }
            public void RemoveUser(string _cid)
            {
                room_User_Cid.Remove(_cid);
            }
            public void Send(string _data)
            {
                byte[] data = Encoding.Default.GetBytes(_data);
                for (int n = 0; n < room_User_Cid.Count; n++)
                {
                    if (!UDP.script.SendData(room_User_Cid[n], data))
                    {
                        room_User_Cid.RemoveAt(n);
                        n--;
                    }
                }
            }
        }
    }
    class SampleClientUDP : UDP
    {
        public static SampleClientUDP script;
        public SampleClientUDP(int _port, string _ip = "127.0.0.1") : base(false, 1024, _port, _ip)
        {
            if (script == null)
            {
                script = this;
            }
            else
            {
                //에러 글자색 변경을 위한 코드
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("에러 : 중복된 선언을 하셨습니다");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
    }
}
