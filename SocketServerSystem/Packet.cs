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
        public class Vector3
        {
            public float x;
            public float y;
            public float z;

            public Vector3(float _x, float _y)
            {
                x = _x;
                y = _y;
                z = 0;
            }
            public Vector3(float _x, float _y, float _z)
            {
                x = _x;
                y = _y;
                z = _z;
            }
            public string ToString()
            {
                return "(" + x.ToString() + ", " + y.ToString() + ", " + z.ToString() +")";
            }

            #region static
            public static Vector3 right { get { return new Vector3(1, 0, 0); } }
            public static Vector3 left { get { return new Vector3(-1, 0, 0); } }
            public static Vector3 up { get { return new Vector3(0, 1, 0); } }
            public static Vector3 back { get { return new Vector3(0, 0, -1); } }
            public static Vector3 forward { get { return new Vector3(0, 0, 1); } }
            public static Vector3 one { get { return new Vector3(1, 1, 1); } }
            public static Vector3 zero { get { return new Vector3(0, 0, 0); } }
            public static Vector3 down { get { return new Vector3(0, -1, 0); } }
            public static Vector3 operator +(Vector3 a, Vector3 b)
            {
                return new Vector3(a.x + b.x, a.y + b.y, a.z + b.y);
            }
            public static Vector3 operator -(Vector3 a)
            {
                return new Vector3(a.x * -1, a.y * -1, a.z * -1);
            }
            public static Vector3 operator -(Vector3 a, Vector3 b)
            {
                return new Vector3(a.x - b.x, a.y - b.y, a.z - b.y);
            }
            public static Vector3 operator *(float d, Vector3 a)
            {
                return new Vector3(a.x * d, a.y * d, a.z * d);
            }
            public static Vector3 operator *(Vector3 a, Vector3 b)
            {
                return new Vector3(a.x * b.x, a.y * b.y, a.z * b.y);
            }
            public static Vector3 operator *(Vector3 a, float d)
            {
                return new Vector3(a.x * d, a.y * d, a.z * d);
            }
            public static Vector3 operator /(Vector3 a, float d)
            {
                return new Vector3(a.x / d, a.y / d, a.z / d);
            }
            public static Vector3 operator /(float d, Vector3 a)
            {
                return new Vector3(d / a.x, d / a.y, d / a.z);
            }
            public static Vector3 operator /(Vector3 a, Vector3 b)
            {
                return new Vector3(a.x / b.x, a.y / b.y, a.z / b.z);
            }
            public static bool operator ==(Vector3 a, Vector3 b)
            {
                return (a.x == b.x) && (a.y == b.y) && (a.z == b.z);
            }
            public static bool operator !=(Vector3 a, Vector3 b)
            {
                return (a.x != b.x) || (a.y != b.y) || (a.z != b.z);
            }
            #endregion
        }
    }
}
