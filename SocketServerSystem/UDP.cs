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
        int Port;
        public UDP(int _Port = 4862)
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
        public void DataReset(int _Port = 4862)
        {
            Port = _Port;
            is_ServerPlay = false;
            Thread.Sleep(1000);
            foreach (Packet.User data in userList)
            {
                data.user.Close();
            }
            tr_Accept.Interrupt();
            tr_Accept.Abort();
            Thread.Sleep(1000);
            DataSet();
        }
        /// <summary>
        /// 모든 데이터를 재설정 할때 호출
        /// </summary>
        private void DataSet()
        {
            userList = new List<UDPUser>();
            Server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            Server.Bind(new IPEndPoint(IPAddress.Any, Port));
            tr_Accept = new Thread(() => Accept());
        }
        #endregion

        List<UDPUser> userList;
        Thread tr_Accept;
        bool is_ServerPlay;

        #region Start
        /// <summary>
        /// 서버를 실행함
        /// </summary>
        public void ServerStart()
        {
            is_ServerPlay = true;
            tr_Accept.Start();
        }
        /// <summary>
        /// 유저들의 접속을 처리해줌.
        /// </summary>
        void Accept()
        {
            while(is_ServerPlay)
            {
                userList.Add(new UDPUser(Server.Accept()));
            }
        }
        #endregion
    }
    class UDPUser : Packet.User
    {
        string cid;
        uint sid;
        public UDPUser(Socket _user, uint _sid = 0, string _cid = "") : base(_user)
        {
            cid = _cid;
            sid = _sid;
            Console.WriteLine("새유저가 들어왔습니다");
        }
    }
}
//메모
//UDP와 TCP를 둘다 사용할지 아니면 TCP만 사용할지 고민해서 작성할것
//UDP 통신방식이 완전히 다름 다시 공부할것
