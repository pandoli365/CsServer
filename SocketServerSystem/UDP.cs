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
            //while(is_ServerPlay)
            //{
            //    userList.Add(new UDPUser(Server.Accept()));
            //}
        }
        #endregion
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
        public ePos pos;//유저의 현재 위치
        public string LobbyName;
        public string RoomName;
        public bool RoomType;

        public UDPUser(string _cid = "")
        {
            cid = _cid;
            pos = ePos.Null;
            Console.WriteLine("새유저가 들어왔습니다");
        }
        /// <summary>
        /// 오브젝트 분석해서 변경할것
        /// </summary>
        /// <param name="Msg"></param>
        public void GetUserMessage(string Msg)
        {
            string[] cut = Msg.Split('\n');
            switch(cut[0])
            {
                case "":
                    break;
            }
        }
        //필요한정보
        //Lobby에 있다면 어떤로비에 있는가?
        //Room에 있다면 해당 룸은 어떤룸인가?(공개방인가 비공계방인가? 비공계라면 암호는 어떻게 되어있는가)


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
