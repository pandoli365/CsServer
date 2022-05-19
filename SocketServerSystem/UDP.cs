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
        public class DataInfo
        {
            public string data;
            public EndPoint remote;
            public DataInfo(string _data, EndPoint _remote)
            {
                data = _data.Trim('\0');
                remote = _remote;
            }
        }

        public class UDPUser
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

            public UDPUser(EndPoint _sid, string _cid = "")
            {
                sid = _sid;
                cid = _cid;
                pos = ePos.Server;
                Console.WriteLine("새유저가 들어왔습니다");
            }
        }

        #region System
        public static UDP script;
        Socket server;
        Socket client;
        IPEndPoint sender;
        EndPoint remote;
        int port;
        Queue<DataInfo> GetData;
        Queue<DataInfo> OutData;
        bool is_Server;
        string ip;
        int maxSize;
        /// <summary>
        /// udp 기본 세팅
        /// </summary>
        /// <param name="_is_Server">현재 동작하는 시스템이 서버인지 클라이언트인지 확인</param>
        /// <param name="_maxSize">송수신하는 패킷의 크기를 설정 기본값 : 1024</param>
        /// <param name="_port">개방혹은 접속할 포트번호</param>
        /// <param name="_ip">클라이언트가 접속을 희망하는 ip주소</param>
        public UDP(bool _is_Server,int _maxSize = 1024, int _port = 4861, string _ip = "127.0.0.1")
        {
            if (script == null)
            {
                script = this;
                is_SocketPlay = false;
                maxSize = _maxSize;
                port = _port;
                is_Server = _is_Server;
                ip = _ip;
                DataSet();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("에러 : 중복된 선언을 하셨습니다");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
        /// <summary>
        /// 모든 데이터를 초기화 할때 호출
        /// </summary>
        public void DataReset(int _Port = 4861)
        {
            port = _Port;
            is_SocketPlay = false;
            if (is_Server)
            {
                Thread.Sleep(1000);
                tr_Accept.Interrupt();
                tr_Processing.Interrupt();
                Thread.Sleep(1000);
                tr_Accept.Abort();
                tr_Processing.Abort();
                Thread.Sleep(1000);
            }
            else
            {
                Thread.Sleep(1000);
                tr_Accept.Interrupt();
                tr_Processing.Interrupt();
                tr_Send.Interrupt();
                Thread.Sleep(1000);
                tr_Accept.Abort();
                tr_Processing.Abort();
                tr_Send.Abort();
                Thread.Sleep(1000);
            }
            DataSet();
            is_EndSocket = true;
        }
        /// <summary>
        /// 모든 데이터를 재설정 할때 호출
        /// </summary>
        private void DataSet()
        {
            userList = new List<UDPUser>();
            if (is_Server)
            {
                server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                sender = new IPEndPoint(IPAddress.Any, port);
            }
            else
            {
                client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                sender = new IPEndPoint(IPAddress.Parse(ip), port);
            }
            remote = sender;
            GetData = new Queue<DataInfo>();
            OutData = new Queue<DataInfo>();
            tr_Accept = new Thread(() => Accept());
            tr_Processing = new Thread(() => Processing());
            tr_Send = new Thread(() => Send());
            v_DataSet();
        }
        /// <summary>
        /// 모든 데이터를 재설정 할때 호출
        /// </summary>
        public virtual void v_DataSet()
        {

        }
        #endregion

        List<UDPUser> userList;
        Thread tr_Accept;
        Thread tr_Processing;
        Thread tr_Send;
        bool is_SocketPlay;
        bool is_EndSocket = true;
        /// <summary>
        /// 서버가 실행중인지 아닌지 확인 정상적으로 종료가 완료되면 true를 반환
        /// </summary>
        public bool ServerPlay { get { return is_EndSocket; } }

        /// <summary>
        /// 서버를 실행함
        /// </summary>
        public void ServerStart()
        {
            server.Bind(sender);
            is_SocketPlay = true;
            is_EndSocket = false;
            tr_Accept.Start();
            tr_Processing.Start();
        }
        public void ClientStart()
        {
            client.Connect(sender);
            is_SocketPlay = true;
            is_EndSocket = false;
            tr_Accept.Start();
            tr_Processing.Start();
            tr_Send.Start();
        }

        /// <summary>
        /// 받은 데이터를 처리함.
        /// </summary>
        void Accept()
        {
            byte[] _data;
            while (is_SocketPlay)
            {
                _data = new byte[maxSize];
                if (is_Server)
                    server.ReceiveFrom(_data, maxSize, SocketFlags.None, ref remote);
                else
                    client.ReceiveFrom(_data, maxSize, SocketFlags.None, ref remote);
                GetData.Enqueue(new DataInfo(Encoding.UTF8.GetString(_data), remote));
            }
        }

        /// <summary>
        /// 들어왔던 데이터들을 한번 정리해줌
        /// </summary>
        void Processing()
        {
            while (is_SocketPlay)
            {
                if (GetData.Count.Equals(0))
                {
                    Thread.Sleep(100);
                }
                else
                {
                    v_Processing(GetData.Dequeue());
                }
            }
        }

        /// <summary>
        /// 클라이언트에서 데이터를 보낼때 사용
        /// </summary>
        void Send()
        {
            byte[] data = new byte[maxSize];
            while (is_SocketPlay)
            {
                if (OutData.Count.Equals(0))
                {
                    Thread.Sleep(100);
                }
                else
                {
                    try
                    {
                        data = new byte[maxSize];
                        DataInfo di = OutData.Dequeue();
                        data = Encoding.UTF8.GetBytes(di.data);
                        if (data.Length > maxSize)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("시스템에 설정된 패킷보다 더 높은용량을 송신합니다.");
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                        else
                        {
                            Console.WriteLine("전송완료");
                            client.SendTo(data, data.Length, SocketFlags.None, di.remote);
                        }
                    }
                    catch (SocketException ex)
                    {
                        Console.WriteLine("{0} : {1}", ex.SocketErrorCode, ex.Message);
                    }
                }
            }
        }

        /// <summary>
        /// 들어온 데이터들을 유저가 처리할 수 있게 함.
        /// </summary>
        /// <param name="di"></param>
        public virtual void v_Processing(DataInfo di)
        {
            Console.WriteLine(di.data);
        }

        /// <summary>
        /// 서버가 유저에게 송신할대 사용
        /// </summary>
        /// <param name="_cid">유저의 id</param>
        /// <param name="_data">보내고 싶은 정보</param>
        /// <returns></returns>
        public bool SendData(string _cid, byte[] _data)
        {
            int index = userList.FindIndex(n => n.cid.Equals(_cid));
            if (index.Equals(-1))
            {
                return false;
            }
            else
            {
                try
                {
                    if (_data.Length > maxSize)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("시스템에 설정된 패킷보다 더 높은용량을 송신합니다.");
                        Console.ForegroundColor = ConsoleColor.White;
                        return false;
                    }
                    else
                    {
                        server.SendTo(_data, _data.Length, SocketFlags.None, userList[index].sid);
                        return true;
                    }
                }
                catch (SocketException ex)
                {

                    Console.WriteLine("{0} : {1}", ex.SocketErrorCode, ex.Message);
                    return false;
                }

            }
        }

        /// <summary>
        /// 유저가 서버에게 송신할때 사용
        /// </summary>
        /// <param name="di"></param>
        public void SendData(DataInfo di)
        {
            OutData.Enqueue(di);
        }

        /// <summary>
        /// 유저가 퇴장 명령을 보낼때 사용
        /// </summary>
        public void Exit(string NickName)
        {
            byte[] data = new byte[maxSize];
            data = Encoding.UTF8.GetBytes("RemoveUser\n" + NickName);
            client.SendTo(data, data.Length, SocketFlags.None, remote);
        }

        public void AddUser(UDPUser NewUser)
        {
            userList.Add(NewUser);
        }
        public void RemoveUser(string _cid)
        {
            if(userList.Count.Equals(1))
            {
                userList.Clear();
            }
            else
            { 
                int index = userList.FindIndex(n => n.cid.Equals(_cid));
                if (!index.Equals(-1))
                    userList.RemoveAt(index);
            }
        }

        public UDPUser UserInfo(EndPoint _sid)
        {
            int index = userList.FindIndex(n => n.sid.Equals(_sid));
            if (index.Equals(-1))
            {
                return null;
            }
            else
            {
                return userList[index];
            }
        }

        public UDPUser UserInfo(string _cid)
        {
            int index = userList.FindIndex(n => n.cid.Equals(_cid));
            if (index.Equals(-1))
            {
                return null;
            }
            else
            {
                return userList[index];
            }
        }

   
    }
}
//메모
//UDP와 TCP를 둘다 사용할지 아니면 TCP만 사용할지 고민해서 작성할것
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
//다시 만들기 단순한 udp와 서버 시스템이랑 분리할것
//String형식에서 obj[] 형식으로 전송할 수 있게 변경할것
//유저가 일정 시간동안 반응이 없다면 자동으로 연결을 끊게 만들기
