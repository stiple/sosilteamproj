using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using userLibrary;

namespace Client
{
    public partial class Form3 : Form
    {
        Form1 refForm;
 
        public Form3(Form1 refFormz)
        {
            this.refForm = refFormz;
            InitializeComponent();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            RoomCreate R_C = new RoomCreate(NameTextBox.Text, -1);
            Packet.Serialize(R_C).CopyTo(refForm.sendBuffer, 0);
            refForm.Send();
            this.Close();
        }
    }
}
