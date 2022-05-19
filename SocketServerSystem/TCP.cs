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
        class TCPUser
        {
            string cid;
            uint sid;
            public Queue<Packet.Message> getMassageList;
            public Queue<Packet.Message> outMassageList;
            public Socket user; //소켓
            public string ip;//유저의 ip주소
            public TCPUser(Socket _user, uint _sid = 0, string _cid = "")
            {
                cid = _cid;
                sid = _sid;
                user = _user;
                getMassageList = new Queue<Packet.Message>();
                outMassageList = new Queue<Packet.Message>();
            }
        }
        #region System
        public static TCP script;
        Socket Server;
        int Port;
        public TCP(int _Port = 4860)
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
        public void DataReset(int _Port = 4860)
        {
            Port = _Port;
            is_ServerPlay = false;
            Thread.Sleep(1000);
            foreach (TCPUser data in userList)
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
            Server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
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
}
//간단히 파일전송 만들기
//파일전송 설계
//1. 소켓 연결요청
//2. 파일을 받을지 전송할지 메세지를 보냄
//3. 파일 공유가 끝나면 end메세지전송
//4. 소켓을 끊음.(서버에서 받는경우 끊는 메세지를 보낼것 클라에서 받는경우 전송완료 메세지를 받으면 바로 소켓종료)
//메세지 기능도 넣을 예정이지만 다수에게 보내기 어렵게 만들 예정
//tcp 주요용도 : Web열람、메일의 송수신、파일전송、공유
//udp 주요용도 : 음성통화、Video스트리밍、멀티캐스터 통신, Broadcast 통신、소량의 데이터전송