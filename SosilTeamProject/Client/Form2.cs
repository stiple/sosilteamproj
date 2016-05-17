using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using userLibrary;

namespace Client
{
    public partial class Form2 : Form
    {
        Form1 refForm;
        string Id;
        string password;
        string nickname;
        public Form2(Form1 referenceForm)
        {
            this.refForm = referenceForm;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Id = IdTextBox.Text;
            password = PasswordTextBox.Text;
            nickname = NickNameTextBox.Text;
            SignIn signininfo = new SignIn(Id, password, nickname);
            try
            {
                refForm.client = new TcpClient();
                refForm.client.Connect("127.0.0.1", 5000);
            }
            catch
            {
                MessageBox.Show("서버 접속에 실패했습니다.");
                return;
            }

            refForm.c_NetStream = refForm.client.GetStream();

            Packet.Serialize(signininfo).CopyTo(refForm.sendBuffer, 0);
            refForm.Send();

            try
            {
                int nRead = 0;
                nRead = refForm.c_NetStream.Read(refForm.readBuffer, 0, 1024 * 4);
            }
            catch (Exception e2)
            {
                return;
            }

            SignIn packet = (SignIn)Packet.Deserialize(refForm.readBuffer);

            if(packet.SignInDone)
            {
                MessageBox.Show("회원가입이 완료되었습니다.");
                this.Close();
            }
            else
            {
                MessageBox.Show("회원가입 실패(중복된 아이디가 있습니다.)");
                IdTextBox.Clear();
                PasswordTextBox.Clear();
                NickNameTextBox.Clear();
            }

            refForm.c_NetStream.Close();
            refForm.client.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
