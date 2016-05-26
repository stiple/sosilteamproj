using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using userLibrary;

//mysql namespace 사용
using MySql.Data.MySqlClient;
using MySql.Data.Types;
using System.Data;

namespace SosilTeamProject
{
    class Program
    {
        public static List<Socket> AllOnlineUser = new List<Socket>(); //접속중인 모든 유저의 소켓
        public static List<Socket> InLobby = new List<Socket>(); //대기방에 접속중인 유저의 소켓
        public static List<string> AllOnlineUserName = new List<string>(); //접속중인 모든 유저 이름
        public static List<string> InLobbyUserName = new List<string>(); //대기방에 접속중인 유저 이름
        public static List<string> InLobbyUserNickName = new List<string>(); //대기방에 접속중인 유저 닉네임
        public static List<NetworkStream> InLobbyStream = new List<NetworkStream>();
        public static List<AvailableGameInfo> AvailableGames = new List<AvailableGameInfo>(); //대기방목록
        public static List<Gamez> gHandle = new List<Gamez>(); //실제 게임
        public static int gamenumber = 1;


        static void Main(string[] args)
        {
            mySQLConnect myDB = new mySQLConnect();
            myDB.SQLconnection();
                          
            TcpListener listener = new TcpListener(5000);

            listener.Start();

            while(true)
            {
                try
                {
                    Socket clntSock = listener.AcceptSocket(); //연결요청대기 블록
                    Console.WriteLine("연결요청을 처리합니다");
                    Protocol protocol = new Protocol(clntSock,myDB);
                    
                    Thread thread = new Thread(new ThreadStart(protocol.handleclient));
                    thread.Start();
                }
                catch(Exception e)
                {
                    Console.WriteLine("Exception =" + e.Message);
                }
            }

        }

        //mysql과 연결하는 클래스
        class mySQLConnect
        {
            private string strCnn; //DB 커넥션 스트링
            private MySqlConnection Conn = null; //커넥션 개체를 담음
            private MySqlCommand Comd;
            private MySqlDataReader Dreader; 

            //처음 SQL과 연결하는 함수
            public void SQLconnection()
            {
                Conn = new MySqlConnection();
                strCnn = "Server=127.0.0.1;Database=client_info;Uid=root;pwd=1324";

                try
                {
                    Conn.ConnectionString = strCnn;
                    Conn.Open();
                    Console.WriteLine("DB와 연결 성공");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            public bool SQLSignIn(string inputId, string inputpassword, string inputNickName)
            {
                try
                {
                    Comd = new MySqlCommand("select ID from client where ID ='" + inputId + "'");
                    Comd.Connection = Conn;
                    Dreader = Comd.ExecuteReader();
                    

                    if (Dreader.Read())
                    {
                        Dreader.Close();
                        return false;
                    }
                    else
                    {
                        Dreader.Close();
                        Comd = new MySqlCommand("INSERT INTO client_info.client (ID, Password, NickName) VALUES ('"+ inputId+"','" + inputpassword+ "','" + inputNickName+ "');",Conn);
                        Dreader = Comd.ExecuteReader();
                        Dreader.Close();
                        return true;
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                    return false;
                }
            }

            public bool SQLLogIn(string inputid, string inputpassword)
            {
                try
                {
                    Comd = new MySqlCommand("select password from client where ID ='" + inputid + "'",Conn);
                    Dreader = Comd.ExecuteReader();
                    string passresult;

                    if (Dreader.Read())
                    {
                        passresult = Dreader.GetString("password");
                        
                    }
                    else
                    {
                        Dreader.Close();
                        return false;
                    }
                    
                    if (passresult == inputpassword)
                    {
                        Dreader.Close();
                        return true;
                    }
                    else
                    {
                        Dreader.Close();
                        return false;
                    }


                }
                catch (Exception e)
                {
                    Console.Write(e.Message);
                    return false;
                }
            }

            //유저정보를 검색해주는 메소드
            public userInfo SQLuserSearch(string inputId)
            {
                try
                {
                    userInfo userinfo;
                    Comd = new MySqlCommand("select * from client where ID ='" + inputId + "'", Conn);
                    Dreader = Comd.ExecuteReader();

                    Dreader.Read();
                    
                    userinfo = new userInfo(Dreader.GetString("ID"), Dreader.GetString("password"), Dreader.GetString("Nickname"), Dreader.GetInt32("numof1st"), Dreader.GetInt32("numofgame"));
                        
                    
                    Dreader.Close();
                    return userinfo;
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                    return null;
                }
            }

        }

        public class Gamez
        {
            public List<NetworkStream> InGameUserStream = new List<NetworkStream>(); //같은 게임에 있는 유저 스트림
            public List<string> InGameUserName = new List<string>(); //같은 게임에 있는 유저 이름
            public int Gnumber; //게임 번호;
            public byte[] sendBuffer = new byte[1024 * 4];

            public Gamez(int Gnum)
            {
                this.Gnumber = Gnum;
            }

            public void Gallsend()
            {
                foreach (NetworkStream nws in InGameUserStream)
                {
                    nws.Write(this.sendBuffer, 0, sendBuffer.Length);
                    nws.Flush();
                }
                for (int i = 0; i < 1024 * 4; i++)
                {
                    this.sendBuffer[i] = 0;
                }
            }
        }
        


        class Protocol
        {
            private Socket clntSock; //클라이언트의 소켓 저장
            private enum clientState
            {
                inLobby = 0,
                inGame = 1
            } //유저상태 나타내는 
            private clientState cState; //현재 클라이언트의 상태를 나타냄
            private gameInfo cGame; //현재 클라이언트가 진행중인 게임의 정보
            private NetworkStream S_NetStream; //네트워크 스트림;
            //private List<NetworkStream> InGameUserStarem = new List<NetworkStream>(); //같은 게임에 있는 유저 스트림
            //public List<Socket> clntList = new List<Socket>(); //같은 게임을 진행중인 client의 리스트
            byte[] readBuffer = new byte[1024 * 4];
            byte[] sendBuffer = new byte[1024 * 4];
            bool clientOn = false;
            mySQLConnect MyDB;
            userInfo myInfo;
            
            public Protocol(Socket clntSock, mySQLConnect MyDB) //생성자
            {
                this.clntSock = clntSock;
                this.MyDB = MyDB;
            }

            //스레드 종료하기전 호출해줄 함수
            public void threadClose()
            {
                clientOn = false;
                InLobbyStream.Remove(S_NetStream);
                S_NetStream.Close();
                Console.WriteLine("연결종료");
                AllOnlineUser.Remove(clntSock);
                InLobby.Remove(clntSock);
                if (myInfo != null)
                {
                    try
                    {
                        AllOnlineUserName.Remove(myInfo.Id);
                        InLobbyUserName.Remove(myInfo.Id);
                        InLobbyUserNickName.Remove(myInfo.NickName);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
                clntSock.Close();
                //여기에 DB와 연동해서 플레이어의 정보를 저장할 수 있는 메서드를 추가해줘야함

                return;
            }

            //전송함수
            public void Send()
            {
                S_NetStream.Write(this.sendBuffer, 0, sendBuffer.Length);
                this.S_NetStream.Flush();
                for (int i = 0; i < 1024 * 4; i++)
                {
                    this.sendBuffer[i] = 0;
                }

            }

            //다른 네트워크스트림으로 여러 클라이언트에게 전송할때
            public void SendLobby()
            {
                foreach (NetworkStream nws in InLobbyStream)
                {
                    nws.Write(this.sendBuffer, 0, sendBuffer.Length);
                    nws.Flush();
                }
                for(int i =0; i< 1024*4; i++)
                {
                    this.sendBuffer[i] = 0;
                }

                return;
            }

            //로비정보 전송
            public void LobbyInfoSend(Packet packet)
            {
                Console.WriteLine("대기실 정보를 전송합니다.");
                LobbyInfo recv = (LobbyInfo)packet;
                LobbyInfo lobby;
                if (recv.InOrOut == LobbyInAndOut.입장)
                {
                    InLobbyUserName.Add(myInfo.Id);
                    InLobbyStream.Add(S_NetStream);
                    InLobby.Add(clntSock);
                    InLobbyUserNickName.Add(myInfo.NickName);
                    cState = clientState.inLobby;
                    lobby = new LobbyInfo(AvailableGames, InLobbyUserNickName);
                }
                else
                {
                    InLobby.Remove(clntSock);
                    InLobbyStream.Remove(S_NetStream);
                    InLobbyUserName.Remove(myInfo.Id);
                    InLobbyUserNickName.Remove(myInfo.NickName);
                    cState = clientState.inGame;
                    lobby = new LobbyInfo(AvailableGames, InLobbyUserNickName);
                }
                Packet.Serialize(lobby).CopyTo(sendBuffer, 0);
                SendLobby();

            }

            //회원가입을 처리하는 메소드
            public void Sign(SignIn signin)
            {
                bool done = MyDB.SQLSignIn(signin.Id, signin.password, signin.NickName);
                SignIn OK = new SignIn(done);

                Packet.Serialize(OK).CopyTo(sendBuffer, 0);
                Send();
            }

            //로그인 처리 메소드
            public void DoLogin(LogIn login)
            {
                //접속중인지 체크
                string a = AllOnlineUserName.Find((c) => c == login.Id);

                //iDpassword체크
                bool done = MyDB.SQLLogIn(login.Id, login.password);
                if(a!=null)
                {
                    done = false;
                } 
                if(done)
                {
                    Console.WriteLine("성공");
                    myInfo = MyDB.SQLuserSearch(login.Id);
                    PlayerInfo pinfo = new PlayerInfo(myInfo, true);
                    Packet.Serialize(pinfo).CopyTo(sendBuffer, 0);
                    Send();
                    AllOnlineUserName.Add(login.Id);
                    AllOnlineUser.Add(clntSock);
                    InLobby.Add(clntSock);
                    InLobbyUserName.Add(login.Id);
                    InLobbyStream.Add(S_NetStream);
                    InLobbyUserNickName.Add(myInfo.NickName);
                    cState = clientState.inLobby;
                    LobbyInfo lobby = new LobbyInfo(AvailableGames, InLobbyUserNickName);
                    Packet.Serialize(lobby).CopyTo(sendBuffer, 0);
                    SendLobby();
                }
                else
                {
                    Console.WriteLine("실패");
                    PlayerInfo pinfo = new PlayerInfo(new userInfo(), false);
                    Packet.Serialize(pinfo).CopyTo(sendBuffer, 0);
                    Send();
                }
            }

            //로비에서 나갈때 이걸 호출하자 로비정보전송은 따로해줘야함
            public void LobbyOut()
            {
                InLobby.Remove(clntSock);
                InLobbyStream.Remove(S_NetStream);
                InLobbyUserName.Remove(myInfo.Id);
                InLobbyUserNickName.Remove(myInfo.NickName);
            }

            //로비로 들어올 때 이거 호출
            public void LobbyIn()
            {
                InLobby.Add(clntSock);
                InLobbyStream.Add(S_NetStream);
                InLobbyUserName.Add(myInfo.Id);
                InLobbyUserNickName.Add(myInfo.NickName);
            }


            //클라이언트 요청을 처리하는 메소드
            //스레드로 동작하고 각 클라이언트마다 하나씩 존재
            public void handleclient() 
            {
                Console.WriteLine("연결 완료");
                S_NetStream = new NetworkStream(clntSock);

                int nRead = 0;
                clientOn = true;

                while (clientOn)
                {
                    try
                    {
                        nRead = 0;
                        nRead = this.S_NetStream.Read(this.readBuffer, 0, 1024 * 4);
                    }
                    catch(Exception e)
                    {
                        threadClose();
                        return;
                    }


                    Packet packet = (Packet)Packet.Deserialize(readBuffer);

                    switch ((int)packet.Type)
                    {
                        case (int)PacketType.회원가입:
                            {
                                Console.WriteLine("회원가입 요청을 받았습니다.");
                                SignIn signin = (SignIn)Packet.Deserialize(readBuffer);
                                Sign(signin);
                                break;
                            }
                        case (int)PacketType.로그인요청:
                            {
                                Console.WriteLine("로그인 요청을 받았습니다.");
                                LogIn login = (LogIn)Packet.Deserialize(readBuffer);
                                //아래로 로그인처리하는 메소드 만들기 OK
                                //DB와 연동해서 로그인이 제대로 됐는지 확인한다. OK
                                //로그인성공시 유저리스트에 소켓과 ID 추가해주는 메소드만들기 OK
                                //CState는 로그인 성공시 inLobby가 됨
                                DoLogin(login);
                                Console.WriteLine(login.Id + " " + login.password);
                                break;
                            }
                            //누군가 대기실에 들어갈때나 나갈때, 이 패킷이 요청됨
                            //모든 로비유저에게 뿌려줌
                        case (int)PacketType.대기실정보:
                            {
                                LobbyInfoSend(packet);
                                break;
                            }
                        case (int)PacketType.게임방입장:
                            {
                                Join joinRequest = (Join)packet;
                                Console.WriteLine("입장요청을 받았습니다");
                                //방번호를 받아서 검색을 합니다.
                                
                                //입장가능한지 체크하고 가능하면 인원수 +1
                                foreach(AvailableGameInfo temp in AvailableGames)
                                {
                                    if(temp.GameNumber == joinRequest.Rnumber)
                                    {
                                        if(temp.NumOfPlayer == 4)
                                        {
                                            Console.WriteLine("입장 불가능");
                                            //불가능하다고 전송하고 Out
                                            Join response = new Join(0, null,0);
                                            Packet.Serialize(response).CopyTo(sendBuffer, 0);
                                            Send();
                                            break;
                                        }
                                        temp.NumOfPlayer++;
                                    }
                                }

                                //방번호가 같으면 내가 입장한 게임과 매칭시킵니다.
                                Console.WriteLine("게임 매칭중");
                                foreach (Gamez tempz in gHandle)
                                {
                                    if(tempz.Gnumber == joinRequest.Rnumber)
                                    {
                                        cGame = new gameInfo();
                                        cGame.gameNumber = tempz.Gnumber;
                                        tempz.InGameUserName.Add(myInfo.NickName);
                                        cGame.clntName = tempz.InGameUserName;
                                        Console.WriteLine("게임정보 재전송");
                                        InGamePlayerInfo responsez = new InGamePlayerInfo(tempz.InGameUserName);
                                        Packet.Serialize(responsez).CopyTo(tempz.sendBuffer, 0);
                                        tempz.Gallsend();
                                        tempz.InGameUserStream.Add(S_NetStream);
                                        cState = clientState.inGame;
                                        //게임정보를 재전송합니다. 같은방에 있는 모든 유저에게 재전송
                                        Console.WriteLine("방입장 응답");
                                        Join response = new Join(1, tempz.InGameUserName,joinRequest.Rnumber);
                                        Packet.Serialize(response).CopyTo(sendBuffer,0);
                                        Send();
                                        Console.WriteLine("로비이탈중");
                                        //로비이탈처리를 합니다
                                        LobbyOut();

                                        Console.WriteLine("로비정보 재전송");
                                        //로비정보 재전송합니다.
                                        LobbyInfo lobbyinfoz = new LobbyInfo(AvailableGames, InLobbyUserNickName);
                                        Packet.Serialize(lobbyinfoz).CopyTo(sendBuffer, 0);
                                        SendLobby();
                                        break;
                                    }

                                }

                                //입장불가능시
                                Join response2 = new Join(0, null,0);
                                Packet.Serialize(response2).CopyTo(sendBuffer, 0);
                                Send();



                                break;
                            }
                        case (int)PacketType.메시지:
                            {
                                
                                userMessage recvMsg = (userMessage)packet;
                                //2가지... 로비에서 전달, 방안에서전달
                                if (cState == clientState.inLobby)
                                {
                                    Console.WriteLine("로비 이용자들에게 메시지를 전송합니다.");
                                    userMessage SendMsg = new userMessage(recvMsg.message, recvMsg.userNickname);
                                    SendMsg.isLobby = true;
                                    Packet.Serialize(SendMsg).CopyTo(sendBuffer, 0);
                                    SendLobby();
                                }
                                else if (cState == clientState.inGame)
                                {
                                    foreach (Gamez temp in gHandle)
                                    {
                                        if(temp.Gnumber == recvMsg.Gnumber)
                                        {
                                            Console.WriteLine("게임 이용자들에게 메시지를 전송합니다.");
                                            userMessage SendMsg = new userMessage(recvMsg.message, recvMsg.userNickname);
                                            SendMsg.isLobby = false;
                                            Packet.Serialize(SendMsg).CopyTo(temp.sendBuffer, 0);
                                            temp.Gallsend();
                                        }
                                    }
                                    //요고요고 수정하도록!
                                }

                                break;
                            }
                        case (int)PacketType.방생성:
                            {
                                Console.WriteLine("대기 방 생성 요청을 받았습니다.");
                                RoomCreate R_C = (RoomCreate)packet;
                                AvailableGameInfo newRoom = new AvailableGameInfo();
                                newRoom.GameName = R_C.RName;
                                newRoom.GameNumber = gamenumber;
                                gamenumber++;
                                newRoom.NumOfPlayer = 1;
                                newRoom.GState = GameInfo.입장가능;
                                AvailableGames.Add(newRoom);
                                Console.WriteLine("방 생성 요청에 응답했습니다. 로비 이탈 처리를 합니다.");
                                LobbyOut();
                                Console.WriteLine("로비이탈처리를했습니다. 방 생성 입장 처리를 합니다");
                                cGame = new gameInfo();
                                //clntList.Add(clntSock);
                                cGame.clntName.Add(myInfo.NickName);
                                cGame.gameNumber = newRoom.GameNumber;
                                cState = clientState.inGame;
                                //InGameUserStarem.Add(S_NetStream);
                                Console.Write("새 게임을 만듭니다.");
                                Gamez newGame = new Gamez(newRoom.GameNumber);
                                newGame.InGameUserStream.Add(S_NetStream);
                                newGame.InGameUserName.Add(myInfo.NickName);
                                gHandle.Add(newGame);
                                Console.WriteLine("방 생성 요청에 응답합니다");
                                RoomCreate responseR_C = new RoomCreate(R_C.RName, newRoom.GameNumber);
                                responseR_C.gameinfo = cGame;

                                Packet.Serialize(responseR_C).CopyTo(sendBuffer, 0);
                                Send();
                                Console.WriteLine("로비이탈처리를 했습니다. 로비정보를 재전송합니다");
                                LobbyInfo lobbyinfoz = new LobbyInfo(AvailableGames, InLobbyUserNickName);
                                Packet.Serialize(lobbyinfoz).CopyTo(sendBuffer, 0);
                                SendLobby();
                                break;
                            }
                        case (int)PacketType.방퇴장:
                            {
                                bool Boom = false; //방 폭파를 위한 bool
                                Console.WriteLine("방 퇴장 요청을 받았습니다.");
                                RoomOut R_O = (RoomOut)packet;

                                AvailableGameInfo thisGameInfo = new AvailableGameInfo();
                                Gamez thisGamez = new Gamez(-1);
                                //방 인원수 체크
                                foreach (AvailableGameInfo temp in AvailableGames)
                                {
                                    if (temp.GameNumber == R_O.Rnumber)
                                    {
                                        thisGameInfo = temp;
                                        if (temp.NumOfPlayer == 1)
                                        {
                                            //방을 폭파시키기위한 준비.
                                            Boom = true;
                                        }
                                        else
                                        {
                                            //아닐시 사람수 하나 줄여줌
                                            thisGameInfo.NumOfPlayer--;
                                        }
                                    }
                                        
                                }

                                foreach (Gamez tempz in gHandle)
                                {
                                    if (tempz.Gnumber == R_O.Rnumber)
                                    {
                                        thisGamez = tempz;
                                        //방이 폭파되지 않을경우
                                        if (Boom == false)
                                        {
                                            Console.WriteLine("현재 유저만 방에서 이탈");
                                            tempz.InGameUserName.Remove(myInfo.NickName);
                                            tempz.InGameUserStream.Remove(S_NetStream);
                                            Console.WriteLine(tempz.InGameUserStream.Count);
                                            cState = clientState.inLobby;
                                            //게임정보를 재전송합니다. 같은방에 있는 모든 유저에게 재전송
                                            //이부분 수정필요 JOIN말고 다른걸 써야함
                                            Console.WriteLine("게임정보 재전송");
                                            InGamePlayerInfo response = new InGamePlayerInfo(tempz.InGameUserName);
                                            Packet.Serialize(response).CopyTo(tempz.sendBuffer, 0);
                                            tempz.Gallsend();
                                            Console.WriteLine("로비진입중");

                                        }
                                    }

                                }

                                //방이 폭파되지 않더라도 cGame초기화
                                cGame = new gameInfo();

                                //방폭파처리
                                if (Boom == true)
                                {
                                    AvailableGames.Remove(thisGameInfo);
                                    gHandle.Remove(thisGamez);
                                }
                                //로비진입처리
                                LobbyIn(); //이거 바꿔준다
                                cState = clientState.inLobby;
                                Console.WriteLine("로비정보 재전송");
                                //로비정보 재전송합니다.
                                LobbyInfo lobbyinfoz = new LobbyInfo(AvailableGames, InLobbyUserNickName);
                                Packet.Serialize(lobbyinfoz).CopyTo(sendBuffer, 0);
                                SendLobby();

                                lobbyinfoz = new LobbyInfo(AvailableGames, InLobbyUserNickName);
                                Packet.Serialize(lobbyinfoz).CopyTo(sendBuffer, 0);
                                SendLobby();


                                break;
                            }
                        default:
                            {
                                threadClose();
                                return;
                                break;
                            }
                    }
                 }
            }

        }
    }


    


}
