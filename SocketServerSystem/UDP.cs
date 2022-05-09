using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SocketServerSystem
{
    class UDP
    {
        #region System
        public static UDP script;
        Socket Server;
        IPEndPoint sender;
        EndPoint remote;
        int Port;
        Queue<DataInfo> GetData;
        public UDP(int _Port = 4861)
        {
            if(script == null)
            {
                script = this;
                is_ServerPlay = false;
                Port = _Port;
                DataSet();
            }
        }
        /// <summary>
        /// 모든 데이터를 초기화 할때 호출
        /// </summary>
        public void DataReset(int _Port = 4861)
        {
            Port = _Port;
            is_ServerPlay = false;
            Thread.Sleep(1000);
            tr_Accept.Interrupt();
            tr_Processing.Interrupt();
            tr_Accept.Abort();
            tr_Processing.Abort();
            Thread.Sleep(1000);
            DataSet();
        }
        /// <summary>
        /// 모든 데이터를 재설정 할때 호출
        /// </summary>
        private void DataSet()
        {
            userList = new List<UDPUser>();
            LobbyList = new List<Lobby>();
            Server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            sender = new IPEndPoint(IPAddress.Any, Port);
            Server.Bind(sender);
            remote = sender;
            GetData = new Queue<DataInfo>();
            tr_Accept = new Thread(() => Accept());
            tr_Processing = new Thread(() => Processing());
        }
        #endregion

        List<UDPUser> userList;
        List<Lobby> LobbyList;
        Thread tr_Accept;
        Thread tr_Processing;
        bool is_ServerPlay;

        #region Start
        /// <summary>
        /// 서버를 실행함
        /// </summary>
        public void ServerStart()
        {
            is_ServerPlay = true;
            tr_Accept.Start();
            tr_Processing.Start();
        }
        /// <summary>
        /// 유저들의 접속을 처리해줌.
        /// </summary>
        void Accept()
        {
            byte[] _data;
            while (is_ServerPlay)
            {
                _data = new byte[1024];
                Server.ReceiveFrom(_data, ref remote);
                GetData.Enqueue(new DataInfo(Encoding.Default.GetString(_data), remote));//Null문자 제거해서 들어갈수 있게 만들기
                //Console.WriteLine("{0} : {1}", remote.ToString(), Encoding.Default.GetString(_data));
                //remote < 이정보자체가 어떤 연결을 통신할지 결정하는 키가됨.
            }
        }
        /// <summary>
        /// 들어왔던 데이터들을 한번 정리해줌
        /// </summary>
        void Processing()
        {
            DataInfo di;
            int index;
            while (is_ServerPlay)
            {
                if(GetData.Count.Equals(0))
                {
                    Thread.Sleep(100);
                }
                else
                {
                    di = GetData.Dequeue();
                    string[] cut = di.data.Split('\n');
                    switch(cut[0])
                    {
                        case "NewUser":
                            userList.Add(new UDPUser(di.remote, cut[1]));
                            break;
                        case "JoinLobby":
                            index = userList.FindIndex(n => n.cid.Equals(di.remote));
                            if (userList[index].pos.Equals(UDPUser.ePos.Server) || userList[index].pos.Equals(UDPUser.ePos.Room))
                            {
                                userList[index].pos = UDPUser.ePos.Lobby;
                                userList[index].lobbyName = cut[1];
                                int lobbyCount = LobbyList.FindIndex(n => n.lobbyName.Equals(cut[1]));
                                if (lobbyCount.Equals(-1))
                                {
                                    LobbyList.Add(new Lobby(cut[1]));
                                    lobbyCount = LobbyList.FindIndex(n => n.lobbyName.Equals(cut[1]));
                                }
                                LobbyList[lobbyCount].AddUser(userList[index].cid);
                            }
                            break;
                        case "OutLobby":
                            index = userList.FindIndex(n => n.cid.Equals(di.remote));
                            if (userList[index].pos.Equals(UDPUser.ePos.Lobby))
                            {
                                int lobbyCount = LobbyList.FindIndex(n => n.lobbyName.Equals(userList[index].lobbyName));
                                LobbyList[lobbyCount].RemoveUser(userList[index].cid);
                                userList[index].pos  = UDPUser.ePos.Server;
                                userList[index].lobbyName = "";
                            }
                            break;
                        case "JoinRoom":
                            index = userList.FindIndex(n => n.cid.Equals(di.remote));
                            if (userList[index].pos.Equals(UDPUser.ePos.Lobby))
                            {
                                userList[index].pos = UDPUser.ePos.Room;
                                userList[index].roomName = cut[1];
                                userList[index].is_RoomPrivate = cut[2].Equals(true);
                                int lobbyCount = LobbyList.FindIndex(n => n.lobbyName.Equals(userList[index].lobbyName));
                                int roomCount = LobbyList[lobbyCount].roomList.FindIndex(n => n.roomName.Equals(cut[1]));
                                if(roomCount.Equals(-1))
                                {
                                    LobbyList[lobbyCount].roomList.Add(new Room(cut[1], cut[2].Equals(true)));
                                    roomCount = LobbyList[lobbyCount].roomList.FindIndex(n => n.roomName.Equals(cut[1]));
                                }
                                LobbyList[lobbyCount].roomList[roomCount].AddUser(userList[index].cid);
                            }
                            break;
                        case "OutRoom":
                            index = userList.FindIndex(n => n.cid.Equals(di.remote));
                            if(userList[index].pos.Equals(UDPUser.ePos.Room))
                            {
                                int lobbyCount = LobbyList.FindIndex(n => n.lobbyName.Equals(userList[index].lobbyName));
                                int roomCount = LobbyList[lobbyCount].roomList.FindIndex(n => n.roomName.Equals(userList[index].roomName));
                                LobbyList[lobbyCount].roomList[roomCount].RemoveUser(userList[index].cid);
                                userList[index].pos = UDPUser.ePos.Lobby;
                                userList[index].roomName = "";
                                userList[index].is_RoomPrivate = false;
                            }
                            break;
                    }
                }
            }
        }

        public bool SendData(string _cid, byte[] _data)
        {
            int index = userList.FindIndex(n => n.cid.Equals(_cid));
            if(index.Equals(-1))
            {
                return false;
            }
            else
            {
                Server.SendTo(_data, _data.Length, SocketFlags.None, remote);
                return true;
            }    
        }
        #endregion
    }
    class DataInfo
    {
        public string data;
        public EndPoint remote;
        public DataInfo(string _data, EndPoint _remote)
        {
            data = _data;
            remote = _remote;
        }
    }
    class UDPUser
    {
        public enum ePos
        {
            Error = -1,
            Null = 0,
            Server = 1,
            Lobby = 2,
            Room = 3
        }

        public string cid;//유저의 닉네임
        public EndPoint sid;
        public ePos pos;//유저의 현재 위치
        public string lobbyName;
        public string roomName;
        public bool is_RoomPrivate;

        public UDPUser(EndPoint _sid,string _cid = "")
        {
            sid = _sid;
            cid = _cid;
            pos = ePos.Server;
            Console.WriteLine("새유저가 들어왔습니다");
        }
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
                if(!UDP.script.SendData(room_User_Cid[n], data))
                {
                    room_User_Cid.RemoveAt(n);
                    n--;
                }
            }
        }
    }
}
//메모
//UDP와 TCP를 둘다 사용할지 아니면 TCP만 사용할지 고민해서 작성할것
//UDP 통신방식이 완전히 다름 다시 공부할것
//내가 현재 이해하고 있는 UDP
//UDP의 데이터는 누구나 접속이 가능함.
//photon에서 처럼 특정 1인에 대한 메세지 송수신이 어려움.
//처리하기 좋은 시스템 : 유저의 현재 위치, 유저가 접속하고 있는 월드의 정보, 각 월드별 정보(Lobby World)
//테스트 해봐야 하는것 : 과연 매초마다 데이터를 요청할경우 서버는 어떤 데이터를 반환하는가
//각 유저별로 연결종료가 가능한가?
//아이디어 : 유저가 설정한 닉네임 + 현재시간을 더해 암호화를 한뒤 그것을 cid로 사용
//UDPUser 설계 계획 : 유저가 들어오면 cid를 전송,리스트에 없다면 새로 등록 있다면 재접속한것
//일정 시간마다 유저의 정보가 들어오지 않는다면 유저정보 삭제(아마 많은 버그가 생길것으로 예상)
//실시간 체팅은 TCP로 주고받을예정? UDP로 받을예정? 고민해볼것
//체팅 기록은 HTTP로 만들예정
//전송 데이터는 절대 1024byte가 넘지 않게 할것!
//기존 코딩 방식대로 제한을 걸까 했지만 접속 데이터가 정확하지 않음.
//룸마다 쓰레드 나누기
//DDos 공격은 사전에 막는것이 중요 ip를 숨길수 있게 다양한 방법을 찾아보기.
//코드로 특정 ip를 차단할수 있는지 알아보기
/*            
while (true)
{
    Thread.Sleep(1);
    _data = Encoding.Default.GetBytes((n++).ToString());
    sck.SendTo(_data, _data.Length, SocketFlags.None, remote);
}
*/
