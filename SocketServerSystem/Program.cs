using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketServerSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            //new TCP(4860);
            new SampleUDP(4868);
            SampleUDP.script.ServerStart();
            Console.Read();
        }
    }
    class SampleUDP : UDP
    {
        List<Lobby> LobbyList;
        //참조를 선언한 경우라면 가급적 udp의 직접 접근은 삼가해 주세요
        public static SampleUDP script;
        public SampleUDP(int _Port = 4861) : base(_Port)
        {
            if (script == null)
            {
                script = this;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("에러 : 중복된 선언을 하셨습니다");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
        public override void v_Processing(DataInfo di)
        {
            UDPUser user;
            string[] cut = di.data.Split('\n');
            switch (cut[0])
            {
                case "NewUser":
                    AddUser(new UDPUser(di.remote, cut[1]));
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
}
