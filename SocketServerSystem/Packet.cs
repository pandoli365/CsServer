using LitJson;
using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace SocketServerSystem
{
    class Packet
    {
        public enum eSendMessage
        {
            Null = 0,
            StartMessage,
            Chatting_Message,
            EndMessage,
            SystemMessage,//서버에서 보내는 메세지 혹은 클라이언트가 서버에 요청하는 메세지 내용
        }
        public class Message
        {
            public string cid;//유저가 지정하는 ClientID 닉네임을 넣어도 좋음.
            public uint sid;//서버에서 지정하는 ServerID
            public eSendMessage type; //메세지 타입
            public string chatting; //데이터 정보
            
            public Message(string _cid = "", uint _sid = 0, eSendMessage _type = eSendMessage.Null, string _chatting = "")
            {
                cid = _cid;
                sid = _sid;
                type = _type;
                chatting = _chatting;
            }

            /// <summary>
            /// 딱히 문제가 없다면 true를 반환
            /// </summary>
            /// <param name="jd">Json Data</param>
            /// <returns></returns>
            public bool Setting(JsonData _jd)
            {
                cid = _jd["cid"].ToString();
                sid = uint.Parse(_jd["sid"].ToString());
                type = (eSendMessage)(int)_jd["type"];
                chatting = (string)_jd["chatting"];
                return true;
            }
            /// <summary>
            /// 정상적인 데이터를 받으면 true를 반환
            /// 비정상적인 데이터라면 false를 반환
            /// </summary>
            /// <param name="_sjd">string 형식의 Json Data</param>
            /// <returns></returns>
            public bool Setting(string _sjd)
            {
                JsonData jd = JsonMapper.ToObject(_sjd);
                try
                {
                    cid = jd["cid"].ToString();
                    sid = uint.Parse(jd["sid"].ToString());
                    type = (eSendMessage)(int)jd["type"];
                    chatting = (string)jd["chatting"];
                    return true;
                }
                catch
                {
                    Console.WriteLine("JsonDataError : " + _sjd);
                    return false;
                }
            }
        }
        public class User
        {
            public Queue<Message> getMassageList;
            public Queue<Message> outMassageList;
            public Socket user; //소켓
            public string ip;//유저의 ip주소
            public User(Socket _user)
            {
                user = _user;
                getMassageList = new Queue<Message>();
                outMassageList = new Queue<Message>();
            }

        }
    }
}
