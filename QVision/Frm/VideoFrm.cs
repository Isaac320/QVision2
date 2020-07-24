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

        ImgProcess.ImgProcess myprocess = new ImgProcess.ImgProcess();

        bool isRunCamera = true;
        bool needSnap = false;

        bool light1 = true;
        bool light2 = true;

        Thread th1;

        delegate void  delegateShow(string s);

        public VideoFrm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (light1)
            {
                QVision.Tools.SerPort.SendData("SA0000#");
                light1 = false;
            }
            else
            {
                QVision.Tools.SerPort.SendData("SA0100#");
                light1 = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

            needSnap = true;
            //Run();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (light2)
            {
                QVision.Tools.SerPort.SendData("SD0000#");
                light2 = false;
            }
            else
            {
                QVision.Tools.SerPort.SendData("SD0100#");
                light2 = true;
            }
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
                    // hSmartWindowControl1.HalconWindow.SetPart(0, 0, -2, -2);
                    hSmartWindowControl1.HalconWindow.DispObj(image111);
                    //Thread.Sleep(100);
                }
                else
                {

                HObject image111 = cam.Snap();
                //       // hSmartWindowControl1.HalconWindow.SetPart(0, 0, -2, -2);
                     hSmartWindowControl1.HalconWindow.DispObj(image111);
                string time = DateTime.Now.ToString("HHmmss");
                    Light1ON();
                    HObject image1 = cam.Snap();
                    
                    hSmartWindowControl2.HalconWindow.DispObj(image1);
                    hSmartWindowControl2.HalconWindow.SetPart(0, 0, -2, -2);
                    //  image1.WriteImage("bmp", 0, time + "1" + ".bmp");
                    Light2ON();
                    HObject image2 = cam.Snap();
                    
                    hSmartWindowControl3.HalconWindow.DispObj(image2);
                    hSmartWindowControl3.HalconWindow.SetPart(0, 0, -2, -2);
                    // image2.WriteImage("bmp", 0, time + "2" + ".bmp");
                    LightAllON();
                    HObject image3 = cam.Snap();
                    
                    hSmartWindowControl4.HalconWindow.DispObj(image3);
                    hSmartWindowControl4.HalconWindow.SetPart(0, 0, -2, -2);
                    // image3.WriteImage("bmp", 0, time + "3" + ".bmp");
                    runImage(image1, image2, image3);
                    needSnap = false;
                }
            }

        }



        private void runImage(HObject image1, HObject image2, HObject image3)
        {
            listboxClear();
            if (!myprocess.isInit)
            {
                myprocess.Init();
            }
            HRegion region = new HRegion(69.6906, 41.8878, 2944.91, 2522.69);

            //HObject region;
            //HOperatorSet.GenEmptyObj(out region);

            //region.Dispose();
            //HOperatorSet.GenRectangle1(out region, 69.6906, 41.8878, 2944.91, 2522.69);

            myprocess.Run(image1, image2, image3, region, out HObject xld1, out HObject xld2, out HObject xld3);

            listboxShow("1号芯片:");
            foreach(string s in myprocess.myList)
            {
                listboxShow(s);
            }
            listboxShow(" ");
            hSmartWindowControl2.HalconWindow.SetColor("red");
            hSmartWindowControl2.HalconWindow.DispObj(xld1);

            hSmartWindowControl3.HalconWindow.SetColor("green");
            hSmartWindowControl3.HalconWindow.DispObj(xld2);

            hSmartWindowControl4.HalconWindow.SetColor("red");
            hSmartWindowControl4.HalconWindow.DispObj(xld3);


            HRegion region2 = new HRegion(132.082, 1313.52, 2965.7, 4035.26);
            myprocess.Run(image1, image2, image3, region2, out HObject xld4, out HObject xld5, out HObject xld6);

            listboxShow("2号芯片:");
            foreach (string s in myprocess.myList)
            {
                listboxShow(s);
            }
            listboxShow(" ");
            hSmartWindowControl2.HalconWindow.SetColor("red");
            hSmartWindowControl2.HalconWindow.DispObj(xld4);

            hSmartWindowControl3.HalconWindow.SetColor("green");
            hSmartWindowControl3.HalconWindow.DispObj(xld5);

            hSmartWindowControl4.HalconWindow.SetColor("red");
            hSmartWindowControl4.HalconWindow.DispObj(xld6);

        }


        private void VideoFrm_Load(object sender, EventArgs e)
        {
            th1 = new Thread(Run);
            th1.IsBackground = true;
            th1.Start();

            hSmartWindowControl1.MouseWheel += hSmartWindowControl1.HSmartWindowControl_MouseWheel;
            hSmartWindowControl2.MouseWheel += hSmartWindowControl2.HSmartWindowControl_MouseWheel;
            hSmartWindowControl3.MouseWheel += hSmartWindowControl3.HSmartWindowControl_MouseWheel;
            hSmartWindowControl4.MouseWheel += hSmartWindowControl4.HSmartWindowControl_MouseWheel;

        }

        private void listboxShow(string s)
        {
            if(listBox1.InvokeRequired)
            {
                BeginInvoke(new delegateShow(listboxShow), new object[] { s });
            }
            else
            {
                listBox1.Items.Add(s);
            }
        }

        private void listboxClear()
        {
            if(listBox1.InvokeRequired)
            {
                BeginInvoke(new Action(listboxClear), new object[] { });
            }
            else
            {
                listBox1.Items.Clear();
            }
        }

        private void hSmartWindowControl1_Load(object sender, EventArgs e)
        {
            
        }

        private void hSmartWindowControl2_Load(object sender, EventArgs e)
        {
            //hSmartWindowControl2.MouseWheel += hSmartWindowControl2.HSmartWindowControl_MouseWheel;
        }
    }
}
