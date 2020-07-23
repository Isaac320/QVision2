using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpinnakerNET;
using SpinnakerNET.GenApi;
using HalconDotNet;

namespace QVision.Cameras.GrayPoint
{
    class GPCam
    {
        IManagedCamera cam;
        public GPCam()
        {
            Init();
        }

        private void Init()
        {
            ManagedSystem system = new ManagedSystem();
            cam = system.GetCameras()[0];
            cam.Init();
        }

        public HImage Snap()
        {
            HImage hImage;
            cam.BeginAcquisition();
            using (IManagedImage rawImage = cam.GetNextImage())
            {
                hImage= new HImage("byte", (int)rawImage.Width, (int)rawImage.Height, rawImage.DataPtr);                               
            }
            cam.EndAcquisition();
            return hImage;
        }
    }
}
