using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net.Sockets;

namespace userLibrary
{
    public enum PacketType
    {
        로그인요청 = 0,
        플레이어정보,
        대기실정보,
        게임방입장,
        회원가입,
        메시지,
        방생성,
        방퇴장,
        게임유저,
        무의미
    }

    public enum LobbyInAndOut
    {
        입장 = 0,
        퇴장
    }


    public enum PacketSendError
    {
        정상 =0,
        에러
    }

    public enum GameInfo
    {
        입장가능=0,
        입장불가능
    }

    [Serializable]
    public class gameInfo
    {
        //public List<Socket> clntList; //같은 게임을 진행중인 client의 리스트
        public List<string> clntName; //같은 게임을 진행하는 client들의 이름 
        public int gameNumber; //방 번호
        public gameInfo()
        {
            //clntList = new List<Socket>();
            clntName = new List<string>();
        }
    }

    //로비에 입장했을 때 게임의 정보를 담는 클래스
    [Serializable]
    public class AvailableGameInfo
    {
        public int GameNumber;
        public string GameName;
        public int NumOfPlayer;
        public GameInfo GState;
    }

    //유저 정보를 담는 클래스
    [Serializable]
    public class userInfo
    {
        public string Id;
        public string password;
        public string NickName;
        public int numof1st;
        public int numofgame;

        public userInfo(string id, string password, string nickname, int numof1st, int numofgame)
        {
            this.Id = id;
            this.password = password;
            this.NickName = nickname;
            this.numof1st = numof1st;
            this.numofgame = numofgame;
        }
        public userInfo()
        {
            Id = null;
            password = null;
            NickName = null;
            numof1st = 0;
            numofgame = 0;
        }
    }


    [Serializable]
    public class Packet
    {
        public int Length;
        public int Type;

        public Packet()
        {
            Length = 0;
            Type = 0;
        }

        public static byte[] Serialize(Object o)
        {
            MemoryStream ms = new MemoryStream(1024 * 4);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(ms, o);
            return ms.ToArray();
        }

        public static Object Deserialize(byte[] bt)
        {
            MemoryStream ms = new MemoryStream(1024 * 4);
            foreach (byte b in bt)
            {
                ms.WriteByte(b);
            }

            ms.Position = 0;
            BinaryFormatter bf = new BinaryFormatter();
            Object obj = bf.Deserialize(ms);
            ms.Close();
            return obj;
        }
    }

    //클라이언트에서 서버에게 로그인 요청을 보낼 때 사용하는 패킷
    //Id와 PassWord를 전송
    [Serializable]
    public class LogIn : Packet
    {
        public string Id;
        public string password;
        public LogIn(string Id, string password)
        {
            this.Id = Id;
            this.password = password;
            Type = (int)PacketType.로그인요청;
        }
    }

    //클라이언트가 성공적으로 로그인을 완료했을때 서버가 보내주는 패킷
    //ID,1위횟수,총플레이게임수,닉네임을 전송
    [Serializable]
    public class PlayerInfo : Packet
    {
        public userInfo userinfo;
        public bool done;
        public PlayerInfo(userInfo userinfo, bool done)
        {
            this.userinfo = userinfo;
            this.done = done;
            Type = (int)PacketType.플레이어정보;
        }
    }

    //클라이언트가 로비 입장시 서버가 클라이언트에게 보내주는 패킷
    //로비에 있는 다른 유저들의 닉네임과, 현재 존재하는 대기방 정보를 보내줌
    [Serializable]
    public class LobbyInfo : Packet
    {
        public List<AvailableGameInfo> GameList;
        public List<string> InLobbyUserName;
        public LobbyInAndOut InOrOut;

        public LobbyInfo(List<AvailableGameInfo> GameList, List<String> InLobbyUserName)
        {
            this.GameList = GameList;
            this.InLobbyUserName = InLobbyUserName;
            Type = (int)PacketType.대기실정보;
        }
    }

    //클라이언트가 서버에게 게임방 입장요청을 보내거나 서버가 그에 응답할때 사용하는 패킷
    //전송자가 클라이언트라면 선택한 게임방 정보만 보내줌
    //전송자가 서버라면 추가로 방안의 다른 사람들의 정보를 보내주고, 입장이 가능한지 불가능한지 여부를 전송
    [Serializable]
    public class Join : Packet
    {
        //방번호
        public int Rnumber;

        //입장이 가능하면 1
        //입장이 불가능하면 0
        public int Available;

        public List<string> userName;

        public Join(int Rnumber)
        {
            this.Rnumber = Rnumber;
            Available = -1;
            Type = (int)PacketType.게임방입장;
        }

        public Join (int Available, List<string>UserName, int Rnumber)
        {
            this.Available = Available;
            this.userName = UserName;
            this.Rnumber = Rnumber;
            Type = (int)PacketType.게임방입장;
        }
    }


    //회원가입 요청을 하고 응답할때 쓰이는 클래스
    //클라이언트는 ID password 닉네임을 
    [Serializable]
    public class SignIn : Packet
    {
        public string Id;
        public string password;
        public string NickName;
        public bool SignInDone;

        public SignIn(string Id,string password, string NickName)
        {
            this.Id = Id;
            this.password = password;
            this.NickName = NickName;
            SignInDone = false;
            Type = (int)PacketType.회원가입;
        }
        public SignIn(bool SignInDone)
        {
            Id = null;
            password = null;
            NickName = null;
            this.SignInDone = SignInDone;
            Type = (int)PacketType.회원가입;
        }

    }

    //메시지 전송을 위해 쓰이는 패킷
    [Serializable]
    public class userMessage : Packet
    {
        public string message;
        public string userNickname;
        public bool isLobby; //로비에있으면 true, 방안에있으면 false
        public int Gnumber;

        public userMessage(string message, string userNickname)
        {
            isLobby = false;
            this.message = message;
            this.userNickname = userNickname;
            this.Gnumber = -1;
            Type = (int)PacketType.메시지;
        }
        public userMessage(string message, string userNickname, int Gnumber)
        {
            isLobby = false;
            this.message = message;
            this.userNickname = userNickname;
            this.Gnumber = Gnumber;
            Type = (int)PacketType.메시지;
        }

    }

    //방 생성 요청, 응답
    [Serializable]
    public class RoomCreate : Packet
    {
        public RoomCreate(string Rname)
        {
            this.RName = Rname;
            Type = (int)PacketType.방생성;
        }
        public RoomCreate(string Rname, int Rnumber)
        {
            this.RName = Rname;
            this.Rnumber = Rnumber;
            Type = (int)PacketType.방생성;
        }
        public string RName; //방 이름- client에서 전달
        public int Rnumber; // 방 번호 - server에서 전달, 생성 실패시 -1 return;
        public gameInfo gameinfo; //게임정보를 담고있는 클래스 서버 -> 클라이언트시
    }


    //방 퇴장시
    [Serializable]
    public class RoomOut : Packet
    {
        public int Rnumber; //방번호
        public bool isSuperUser; //방장이었는지

        public RoomOut(int Rnumber,bool isSuperUser )
        {
            this.Rnumber = Rnumber;
            this.isSuperUser = isSuperUser;
            Type = (int)PacketType.방퇴장;
        }
    }

    [Serializable]
    public class InGamePlayerInfo : Packet
    {
        public List<string> userList;
        public InGamePlayerInfo(List<string> userList)
        {
            this.userList = userList;
            Type = (int)PacketType.게임유저;
        }
              


    }
}

