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
    class TCP
    {
        #region System
        public static TCP script;
        Socket Server;
        int Port;
        public TCP(int _Port = 4863)
        {
            if (script == null)
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
        public void DataReset(int _Port = 4863)
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
            //userList = new List<TCPUser>();
            //Server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //Server.Bind(new IPEndPoint(IPAddress.Any, Port));
            //Server.Listen(10);
            //tr_Accept = new Thread(() => Accept());
        }
        #endregion

        List<TCPUser> userList;
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
            while (is_ServerPlay)
            {
                userList.Add(new TCPUser(Server.Accept()));
            }
        }

        #endregion
    }
    class TCPUser : Packet.User
    {
        string cid;
        uint sid;
        public TCPUser(Socket _user, uint _sid = 0, string _cid = "") : base(_user)
        {
            cid = _cid;
            sid = _sid;
        }
    }
}
