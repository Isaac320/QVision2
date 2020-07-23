using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HalconDotNet;
using System.Threading;

namespace QVision.Frm
{
    public partial class VideoFrm : Form
    {
        Cameras.GrayPoint.GPCam cam = new Cameras.GrayPoint.GPCam();

        bool isRunCamera = true;
        bool needSnap = false;

        Thread th1;

        public VideoFrm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            QVision.Tools.SerPort.SendData("SA0100#");
        }

        private void button2_Click(object sender, EventArgs e)
        {

            needSnap = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            QVision.Tools.SerPort.SendData("SA0000#");
        }


        void Light1ON()
        {
            QVision.Tools.SerPort.SendData("SA0200#");
            QVision.Tools.SerPort.SendData("SD0000#");           
        }

        void Light2ON()
        {
            QVision.Tools.SerPort.SendData("SD0200#");
            QVision.Tools.SerPort.SendData("SA0000#");
        }

        void LightAllON()
        {
            QVision.Tools.SerPort.SendData("SA0100#");
            QVision.Tools.SerPort.SendData("SD0100#");
        }

        void Run()
        {

            while (isRunCamera)
            {
                if (!needSnap)
                {                    
                    HImage image111 = cam.Snap();
                    hSmartWindowControl1.HalconWindow.DispObj(image111);
                }
                else
                {


                    string time = DateTime.Now.ToString("HHmmss");

                    Light1ON();
                    HImage image1 = cam.Snap();
                    hSmartWindowControl2.HalconWindow.DispObj(image1);

                  //  image1.WriteImage("bmp", 0, time + "1" + ".bmp");

                    Light2ON();
                    HImage image2 = cam.Snap();
                    hSmartWindowControl3.HalconWindow.DispObj(image2);

                   // image2.WriteImage("bmp", 0, time + "2" + ".bmp");

                    LightAllON();
                    HImage image3 = cam.Snap();
                    hSmartWindowControl4.HalconWindow.DispObj(image3);

                   // image3.WriteImage("bmp", 0, time + "3" + ".bmp");
                    needSnap = false;
                }
            }

        }

        private void VideoFrm_Load(object sender, EventArgs e)
        {
            th1 = new Thread(Run);
            th1.IsBackground = true;
            th1.Start();
        }
    }
}
