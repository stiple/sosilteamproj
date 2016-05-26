using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using userLibrary;

namespace Client
{
    public partial class Form1 : Form
    {
        string myId;
        public TcpClient client;
        public NetworkStream c_NetStream;
        public byte[] sendBuffer = new byte[1024*4];
        public byte[] readBuffer = new byte[1024 * 4];
        bool connection = false;
        Thread recvThread;
        public userInfo myInfo;
        Form3 form3;
        

        public Form1()
        {
            InitializeComponent();

            RoomList.View = View.Details;
            RoomList.BeginUpdate();
            RoomList.Columns.Add("번호");
            RoomList.Columns.Add("이름");
            RoomList.Columns.Add("인원");
            RoomList.EndUpdate();
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            myId = IdTextBox.Text;
            string password = PasswordTextBox.Text;
            try
            {
                client = new TcpClient();
                client.Connect("1.228.8.99", 5000);
            }
            catch
            {
                MessageBox.Show("서버 접속에 실패했습니다.");
                return;
            }

            c_NetStream = client.GetStream();

            LogIn login = new LogIn(myId, password);

            Packet.Serialize(login).CopyTo(sendBuffer, 0);
            Send();

            recvThread = new Thread(new ThreadStart(Recvfunc));
            recvThread.Start();
            //read 아래부분 스레드로 처리하도록 분리
 
        }

        public void Send()
        {
            c_NetStream.Write(this.sendBuffer, 0, sendBuffer.Length);
            this.c_NetStream.Flush();
            for(int i = 0; i < 1024*4; i++)
            {
                this.sendBuffer[i] = 0;
            }
            
        }

        //서버로부터 수신 후 기능을 실행하는 메소드
        //스레드로 동작
        public void Recvfunc()
        {
            int nRead = 0;

            try
            {
                nRead = 0;
                nRead = this.c_NetStream.Read(this.readBuffer, 0, 1024 * 4);
            }
            catch (Exception ex)
            {

                return;
            }

            PlayerInfo upacket = (PlayerInfo)Packet.Deserialize(readBuffer);

            if (upacket.done)
            {
                MessageBox.Show("로그인 성공");
            }
            else
            {
                MessageBox.Show("로그인 실패");
                return;
            }

            myInfo = upacket.userinfo;
            UserNicknameText.Invoke((MethodInvoker)(() =>
                UserNicknameText.Text = myInfo.NickName
            ));
            UserNumof1stText.Invoke((MethodInvoker)(() =>
                UserNumof1stText.Text = "우승횟수: " + myInfo.numof1st.ToString()
            ));
            UserNumofGameText.Invoke((MethodInvoker)(() =>
                UserNumofGameText.Text = "총 게임수: " + myInfo.numofgame.ToString()
            ));

            LoginButton.Invoke((MethodInvoker)(() =>
                LoginButton.Enabled = false
            ));

            SignInButton.Invoke((MethodInvoker)(() =>

                SignInButton.Enabled = false
            ));
            

            connection = true;

            while(connection)
            {
                try
                {
                    nRead = 0;
                    nRead = c_NetStream.Read(this.readBuffer, 0, 1024 * 4);

                    Packet packet = (Packet)Packet.Deserialize(readBuffer);

                    switch((int)packet.Type)
                    {
                        case (int)PacketType.플레이어정보:
                            {
                                
                                break;
                            }
                        case (int)PacketType.메시지:
                            {
                                userMessage recvMsg = (userMessage)packet;

                                ChatTextBox.Invoke((MethodInvoker)(() => ChatTextBox.AppendText(recvMsg.userNickname + ": " + recvMsg.message + "\n")));

                                break;
                            }
                        case (int)PacketType.대기실정보:
                            {
                                //MessageBox.Show("대기실 정보를 받아옵니다.");
                                LobbyInfo lobby = (LobbyInfo)packet;
                                UserList.Invoke((MethodInvoker)(() =>
                                    UserList.Items.Clear()
                                ));
                                RoomList.Invoke((MethodInvoker)(() =>
                                    RoomList.Items.Clear()                               
                                ));
                                
                                foreach (string nickname in lobby.InLobbyUserName)
                                {
                                    UserList.Invoke((MethodInvoker)(() =>
                                        UserList.Items.Add(nickname)
                                    ));
                                }
                                foreach(AvailableGameInfo thisGame in lobby.GameList)
                                {
                                    string[] arr = new string[] { thisGame.GameNumber.ToString(), thisGame.GameName, thisGame.NumOfPlayer + "/4" };
                                    RoomList.Invoke((MethodInvoker)(() =>

                                        RoomList.Items.Add(new ListViewItem(arr))
                                    ));
                                }
                                //방 정보 추가하는 메서드 추가 필요함! 추가
                                break;
                            }
                            //방생성 응답받았을 시
                        case (int)PacketType.방생성:
                            {
                                //MessageBox.Show("게임방을 생성합니다.");
                                RoomCreate R_C = (RoomCreate)packet;
                                InGame InGameForm = new InGame(this);
                                InGameForm.isSuperUser = true;
                                InGameForm.myInGameUserName = R_C.gameinfo.clntName;
                                //InGameForm.igNetStream = c_NetStream;
                                InGameForm.isSuperUser = true;
                                InGameForm.Rnumber = R_C.Rnumber;
                                InGameForm.myname = myInfo.NickName;
                                InGameForm.createRoom();
                                InGameForm.ShowDialog();

                                break;
                            }
                        //방 입장 응답받았을 시
                        case (int)PacketType.게임방입장:
                            {
                                //MessageBox.Show("게임방에 입장합니다!");
                                Join join = (Join)packet;
                                InGame InGameForm = new InGame(this);
                                InGameForm.isSuperUser = false;
                                InGameForm.myInGameUserName = join.userName;
                                //InGameForm.igNetStream = c_NetStream;
                                InGameForm.Rnumber = join.Rnumber;
                                InGameForm.myname = myInfo.NickName;
                                InGameForm.createRoom();
                                InGameForm.ShowDialog();
                                break;
                            }
                        default:
                            {
                                break;
                            }
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.StackTrace+"\n");
                    Console.WriteLine(e.ToString());
                    MessageBox.Show(e.StackTrace + "\n" + e.ToString());
                    
                    return;
                }
            }
        }



        private void SignInButton_Click(object sender, EventArgs e)
        {
            Form form2 = new Form2(this);
            form2.ShowDialog();
        }

        private void SendTextButton_Click(object sender, EventArgs e)
        {
            if (connection == true)
            {
                //MessageBox.Show("보낸다!");
                userMessage SendMsg = new userMessage(WriteChatBox.Text, myInfo.NickName);
                WriteChatBox.Clear();
                Packet.Serialize(SendMsg).CopyTo(sendBuffer, 0);
                Send();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (connection == true)
                {
                    LobbyInfo lobby = new LobbyInfo(null, null);
                    lobby.InOrOut = LobbyInAndOut.퇴장;
                    Packet.Serialize(lobby).CopyTo(sendBuffer, 0);
                    Send();
                    Packet dummy = new Packet();
                    dummy.Type = (int)PacketType.무의미;
                    Packet.Serialize(dummy).CopyTo(sendBuffer, 0);
                    Send();
                    recvThread.Abort();
                    c_NetStream.Close();
                    client.Close();
                    connection = false;
                }
            }
            catch (Exception ex)
            {
                return;
            }
        }

        private void WriteChatBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (connection == true)
            {
                if (e.KeyCode == Keys.Enter)
                {
                    userMessage SendMsg = new userMessage(WriteChatBox.Text, myInfo.NickName);
                    WriteChatBox.Clear();
                    Packet.Serialize(SendMsg).CopyTo(sendBuffer, 0);
                    Send();
                }
            }
        }

        private void CreateRoomButton_Click(object sender, EventArgs e)
        {
            if (connection == true)
            {
                form3 = new Form3(this);
                form3.ShowDialog();
            }
        }

        private void RoomList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (connection == true)
            {
                if(RoomList.SelectedItems.Count == 1)
                {
                    ListView.SelectedListViewItemCollection items = RoomList.SelectedItems;
                    ListViewItem lvItem = items[0];
                    if(lvItem.SubItems[2].Text == "4/4")
                    {
                        MessageBox.Show("방이 가득 찼습니다.");
                        return;
                    }
                    string Rnumber = lvItem.SubItems[0].Text;
                    string Rname = lvItem.SubItems[1].Text;
                    Join joinRequest = new Join(int.Parse(Rnumber));
                    Packet.Serialize(joinRequest).CopyTo(sendBuffer, 0);
                    Send();
                }
                
            }
        }
    }
}
