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
    public partial class InGame : Form
    {
        Form1 refForm;
        public List<string> myInGameUserName; //대기방에 접속중인 유저 이름
        public bool isSuperUser = false; // 방장인지 표시
       // public NetworkStream igNetStream; //네트워크 스트림 받아옴
        public int Rnumber; //현재 방번호
        public byte[] readBuffer = new byte[1024 * 4];
        Thread recvThread;
        public string myname;

        public InGame(Form1 refFormz)
        {
            this.refForm = refFormz;

            InitializeComponent();
            //refForm.Enabled = true;
            ChatBox.AppendText("게임방에 입장했습니다.\n");
            //createRoom();
            //UserNameList.Items.Clear();
            //foreach (string st in InGameUserName)
            //{
            //  UserNameList.Items.Add(st);
            //}
            //createRoom();
            recvThread = new Thread(new ThreadStart(Recvfunc));
            recvThread.Start();
        }

        public void createRoom()
        {
            //UserNameList.Items.Clear();
            try
            {
                foreach (string st in myInGameUserName)
                {
                    UserNameList.Items.Add(st);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString() + "\n");
                MessageBox.Show(e.StackTrace);
                ChatBox.AppendText("에러 발생");
            }

            //MessageBox.Show("잘했어요.");

            //     foreach(string st in myInGameUserName)
            //     {
            //         ChatBox.AppendText(st);
            //     }
        }

        //유저 입장시 실행될 메소드
        public void EnterHandler(string UserName)
        {
            myInGameUserName.Add(UserName);
        }

        //채팅창에 메시지 추가
        public void addText(userMessage usermessage)
        {
            ChatBox.AppendText(usermessage.userNickname + " : " + usermessage.message + "\n");
        }

        //유저 퇴장시 실행될 메소드
        public void ExitHandler(string UserName)
        {
            myInGameUserName.Add(UserName);
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            recvThread.Abort();
            this.Close();
        }

        private void InGame_FormClosing(object sender, FormClosingEventArgs e)
        {
            RoomOut roomout = new RoomOut(this.Rnumber, this.isSuperUser);
            Packet.Serialize(roomout).CopyTo(refForm.sendBuffer, 0);
            refForm.Send();
            return;
        }

        public void Recvfunc()
        {
            int nRead = 0;


            while (true)
            {
                try
                {
                    nRead = 0;
                    nRead = refForm.c_NetStream.Read(this.readBuffer, 0, 1024 * 4);

                    Packet packet = (Packet)Packet.Deserialize(readBuffer);

                    switch ((int)packet.Type)
                    {
                        case (int)PacketType.게임유저:
                            {
                                InGamePlayerInfo igPI = (InGamePlayerInfo)packet;
                                myInGameUserName = igPI.userList;
                                UserNameList.Invoke((MethodInvoker)(() =>
                                    UserNameList.Items.Clear()
                                ));
                                //UserNameList.Items.Clear();
                                foreach (string st in myInGameUserName)
                                {
                                    UserNameList.Invoke((MethodInvoker)(() =>

                                        UserNameList.Items.Add(st)
                                    ));
                                    //UserNameList.Items.Add(st);
                                }

                                break;
                            }
                        case (int)PacketType.메시지:
                            {
                                userMessage recvMsg = (userMessage)packet;

                                if (recvMsg.isLobby == false) //게임 중
                                {
                                    ChatBox.Invoke((MethodInvoker)(() => ChatBox.AppendText(recvMsg.userNickname + ": " + recvMsg.message + "\n")));
                                }
                                break;
                            }

                        default:
                            {
                                break;
                            }
                    }
                }
                catch (Exception e5)
                {
                    MessageBox.Show(e5.ToString() + "\n");
                    MessageBox.Show(e5.StackTrace);
                    //ChatBox.AppendText("에러 발생");
                    return;
                }


            }
        }

        private void EnterButton_Click(object sender, EventArgs e)
        {
            userMessage SendMsg = new userMessage(WriteTextBox.Text, myname,Rnumber);
            WriteTextBox.Clear();
            Packet.Serialize(SendMsg).CopyTo(refForm.sendBuffer, 0);
            refForm.Send();
        }

        private void WriteTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                userMessage SendMsg = new userMessage(WriteTextBox.Text, myname,Rnumber);
                WriteTextBox.Clear();
                Packet.Serialize(SendMsg).CopyTo(refForm.sendBuffer, 0);
                refForm.Send();
            }
        }
    }

}