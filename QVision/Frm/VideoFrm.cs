using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace QVision.Frm
{
    public partial class VideoFrm : Form
    {
        public VideoFrm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            QVision.Tools.SerPort.SendData("aaaaaa");
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }
    }
}
