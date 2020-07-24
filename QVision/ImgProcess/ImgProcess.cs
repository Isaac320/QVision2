using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HalconDotNet;

namespace QVision.ImgProcess
{
    class ImgProcess
    {

        public bool isInit = false;

        HTuple hv_R1 = 1094;
        HTuple hv_C1 = 1170;
        HTuple hv_Ang1 = 0;

        HTuple hv_HeightMin = 15;
        HTuple hv_HeightMax = 25;
        HTuple hv_WidthMax = 100;


        HTuple MaxR = 200;

        HTuple hv_ModelID = null;

        public List<string> myList = new List<string>();

        public void Init()
        {
            try
            {
                HOperatorSet.ReadShapeModel("model.shm", out hv_ModelID);
                isInit = true;
            }
            catch(Exception ee)
            {

            }
        }



        public void Run(HObject image1, HObject image2, HObject image3,HObject region,out HObject xld1,out HObject xld2,out HObject xld3)
        {

            myList.Clear();
            xld1 = null;
            xld2 = null;
            xld3 = null;

            HOperatorSet.GenEmptyObj(out xld1);
            HOperatorSet.GenEmptyObj(out xld2);
            HOperatorSet.GenEmptyObj(out xld3);

            HHomMat2D homMat;

            GetMyImage(image1,region, image2,image3, out HObject ho_Regions, out HObject ho_ImageAffineTrans1,out HObject ho_ImageAffineTrans2, hv_ModelID, hv_R1, hv_C1, hv_Ang1,out HTuple flag3,out HTuple hommat2d);

            if (flag3 != 0)
            {

                homMat = new HHomMat2D(hommat2d);

                HObject ho_SelectedRegionsR;
                HObject ho_EmptyObjectR, ho_SelectedRegionsL, ho_EmptyObjectL;
                HObject ho_SelectedRegionsU, ho_EmptyObjectU, ho_SelectedRegionsD;
                HObject ho_EmptyObjectD, ho_SelectedRegions;

                HTuple hv_NumberR = null, hv_heightR = null;
                HTuple hv_PinWidthR = null, hv_NumberL = null, hv_heightL = null;
                HTuple hv_PinWidthL = null, hv_NumberU = null, hv_heightU = null;
                HTuple hv_PinWidthU = null, hv_NumberD = null, hv_heightD = null;
                HTuple hv_PinWidthD = null, hv_Radius = null, hv_flag = null;

                HOperatorSet.GenEmptyObj(out ho_SelectedRegionsR);
                HOperatorSet.GenEmptyObj(out ho_EmptyObjectR);
                HOperatorSet.GenEmptyObj(out ho_SelectedRegionsL);
                HOperatorSet.GenEmptyObj(out ho_EmptyObjectL);
                HOperatorSet.GenEmptyObj(out ho_SelectedRegionsU);
                HOperatorSet.GenEmptyObj(out ho_EmptyObjectU);
                HOperatorSet.GenEmptyObj(out ho_SelectedRegionsD);
                HOperatorSet.GenEmptyObj(out ho_EmptyObjectD);
                HOperatorSet.GenEmptyObj(out ho_SelectedRegions);

                ho_SelectedRegionsR.Dispose(); ho_EmptyObjectR.Dispose();
                ttestRight(ho_Regions, out ho_SelectedRegionsR, out ho_EmptyObjectR, out hv_NumberR,
            out hv_heightR, out hv_PinWidthR);
                ho_SelectedRegionsL.Dispose(); ho_EmptyObjectL.Dispose();
                ttestLeft(ho_Regions, out ho_SelectedRegionsL, out ho_EmptyObjectL, out hv_NumberL,
                    out hv_heightL, out hv_PinWidthL);
                ho_SelectedRegionsU.Dispose(); ho_EmptyObjectU.Dispose();
                ttestUp(ho_Regions, out ho_SelectedRegionsU, out ho_EmptyObjectU, out hv_NumberU,
                    out hv_heightU, out hv_PinWidthU);
                ho_SelectedRegionsD.Dispose(); ho_EmptyObjectD.Dispose();
                ttestDown(ho_Regions, out ho_SelectedRegionsD, out ho_EmptyObjectD, out hv_NumberD,
                    out hv_heightD, out hv_PinWidthD);


                HOperatorSet.AffineTransRegion(ho_SelectedRegionsR, out HObject regionR, homMat, "nearest_neighbor");
                HOperatorSet.AffineTransRegion(ho_SelectedRegionsL, out HObject regionL, homMat, "nearest_neighbor");
                HOperatorSet.AffineTransRegion(ho_SelectedRegionsU, out HObject regionU, homMat, "nearest_neighbor");
                HOperatorSet.AffineTransRegion(ho_SelectedRegionsD, out HObject regionD, homMat, "nearest_neighbor");

                xld1 =xld1.ConcatObj(regionR);
                xld1 = xld1.ConcatObj(regionL);
                xld1 = xld1.ConcatObj(regionU);
                xld1 = xld1.ConcatObj(regionD);


                ho_SelectedRegions.Dispose();
                checkX(ho_ImageAffineTrans1, out ho_SelectedRegions, out hv_Radius);

                HOperatorSet.AffineTransRegion(ho_SelectedRegions, out HObject regionM, homMat, "nearest_neighbor");

                xld2 = xld2.ConcatObj(regionM);


                testMiss(ho_ImageAffineTrans2, out HObject ho_RegionUnion, out HTuple hv_Number);

                HOperatorSet.AffineTransRegion(ho_RegionUnion, out HObject regionCKK, homMat, "nearest_neighbor");

                xld3 = xld3.ConcatObj(regionCKK);

                if(hv_Number!=0)
                {
                    myList.Add("破损");
                }
               


                if (hv_Radius > MaxR)
                {
                    myList.Add("打叉");
                }

                if(hv_NumberR!=16)
                {
                    myList.Add("右侧Pin脚数量不对");
                }

                CheckHeight(hv_heightR, hv_HeightMin, hv_HeightMax, out HTuple flag);
                if (flag != 0)
                {
                    myList.Add("右侧Pin脚歪斜");
                }

                testWidth(hv_PinWidthR, hv_WidthMax, out  flag);
                if (flag != 0)
                {
                    myList.Add("右侧Pin脚过长");
                }

                if (hv_NumberL != 16)
                {
                    myList.Add("左侧Pin脚数量不对");
                }

                CheckHeight(hv_heightL, hv_HeightMin, hv_HeightMax, out flag);
                if (flag != 0)
                {
                    myList.Add("左侧Pin脚歪斜");
                }

                testWidth(hv_PinWidthL, hv_WidthMax, out flag);
                if (flag != 0)
                {
                    myList.Add("左侧Pin脚过长");
                }

                if (hv_NumberU != 16)
                {
                    myList.Add("上侧Pin脚数量不对");
                }

                CheckHeight(hv_heightU, hv_HeightMin, hv_HeightMax, out flag);
                if (flag != 0)
                {
                    myList.Add("上侧Pin脚歪斜");
                }

                testWidth(hv_PinWidthU, hv_WidthMax, out flag);
                if (flag != 0)
                {
                    myList.Add("上侧Pin脚过长");
                }

                if (hv_NumberD != 16)
                {
                    myList.Add("下侧Pin脚数量不对");
                }

                CheckHeight(hv_heightD, hv_HeightMin, hv_HeightMax, out flag);
                if (flag != 0)
                {
                    myList.Add("下侧Pin脚歪斜");
                }

                testWidth(hv_PinWidthD, hv_WidthMax, out flag);
                if (flag != 0)
                {
                    myList.Add("下侧Pin脚过长");
                }

            }
            else
            {
                myList.Add("未找到芯片");
            }

        }


        public void GetMyImage(HObject ho_Image1, HObject ho_ROI_0, HObject ho_Image2,
      HObject ho_Image3, out HObject ho_Regions, out HObject ho_ImageAffineTrans1,
      out HObject ho_ImageAffineTrans2, HTuple hv_ModelID, HTuple hv_R1, HTuple hv_C1,
      HTuple hv_Ang1, out HTuple hv_flag, out HTuple hv_HomMat2DInvert)
        {




            // Local iconic variables 

            HObject ho_ImageReduced, ho_ImageAffineTrans = null;
            HObject ho_ImageReduced1 = null, ho_ImageReduced2 = null;

            // Local control variables 

            HTuple hv_Row1 = null, hv_Column1 = null, hv_Angle1 = null;
            HTuple hv_Score1 = null, hv_num = null, hv_HomMat2D = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Regions);
            HOperatorSet.GenEmptyObj(out ho_ImageAffineTrans1);
            HOperatorSet.GenEmptyObj(out ho_ImageAffineTrans2);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced);
            HOperatorSet.GenEmptyObj(out ho_ImageAffineTrans);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced1);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced2);
            hv_HomMat2DInvert = new HTuple();

            hv_flag = 0;
            ho_ImageReduced.Dispose();
            HOperatorSet.ReduceDomain(ho_Image1, ho_ROI_0, out ho_ImageReduced);
            HOperatorSet.FindShapeModel(ho_ImageReduced, hv_ModelID, -0.39, 0.79, 0.5, 1,
                0.5, "least_squares", 0, 0.9, out hv_Row1, out hv_Column1, out hv_Angle1,
                out hv_Score1);
            hv_num = new HTuple(hv_Score1.TupleLength());
            if ((int)(new HTuple(hv_num.TupleNotEqual(0))) != 0)
            {
                HOperatorSet.VectorAngleToRigid(hv_Row1, hv_Column1, hv_Angle1, hv_R1, hv_C1,
                    hv_Ang1, out hv_HomMat2D);
                ho_ImageAffineTrans.Dispose();
                HOperatorSet.AffineTransImage(ho_ImageReduced, out ho_ImageAffineTrans, hv_HomMat2D,
                    "constant", "false");
                ho_Regions.Dispose();
                HOperatorSet.Threshold(ho_ImageAffineTrans, out ho_Regions, 200, 255);

                ho_ImageReduced1.Dispose();
                HOperatorSet.ReduceDomain(ho_Image2, ho_ROI_0, out ho_ImageReduced1);
                ho_ImageAffineTrans1.Dispose();
                HOperatorSet.AffineTransImage(ho_ImageReduced1, out ho_ImageAffineTrans1, hv_HomMat2D,
                    "constant", "false");


                ho_ImageReduced2.Dispose();
                HOperatorSet.ReduceDomain(ho_Image3, ho_ROI_0, out ho_ImageReduced2);
                ho_ImageAffineTrans2.Dispose();
                HOperatorSet.AffineTransImage(ho_ImageReduced2, out ho_ImageAffineTrans2, hv_HomMat2D,
                    "constant", "false");

                HOperatorSet.HomMat2dInvert(hv_HomMat2D, out hv_HomMat2DInvert);

                hv_flag = 1;

            }
            else
            {
                hv_flag = 0;
            }


            ho_ImageReduced.Dispose();
            ho_ImageAffineTrans.Dispose();
            ho_ImageReduced1.Dispose();
            ho_ImageReduced2.Dispose();

            return;
        }



        public void ttestLeft(HObject ho_Regions, out HObject ho_SelectedRegions, out HObject ho_EmptyObject,
     out HTuple hv_Number, out HTuple hv_height, out HTuple hv_PinWidth)
        {



            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_RegionL, ho_RegionIntersection;
            HObject ho_RegionClosing, ho_RegionOpening, ho_ConnectedRegions;
            HObject ho_testL, ho_RegionDifference, ho_ConnectedRegions1;
            HObject ho_SortedRegions, ho_ObjectSelected = null;

            // Local control variables 

            HTuple hv_Number1 = null, hv_Index1 = null;
            HTuple hv_Row1 = null, hv_Column1 = null, hv_Row2 = null;
            HTuple hv_Column2 = null, hv_Row11 = null, hv_Column11 = null;
            HTuple hv_Row21 = null, hv_Column21 = null;
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions);
            HOperatorSet.GenEmptyObj(out ho_EmptyObject);
            HOperatorSet.GenEmptyObj(out ho_RegionL);
            HOperatorSet.GenEmptyObj(out ho_RegionIntersection);
            HOperatorSet.GenEmptyObj(out ho_RegionClosing);
            HOperatorSet.GenEmptyObj(out ho_RegionOpening);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
            HOperatorSet.GenEmptyObj(out ho_testL);
            HOperatorSet.GenEmptyObj(out ho_RegionDifference);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions1);
            HOperatorSet.GenEmptyObj(out ho_SortedRegions);
            HOperatorSet.GenEmptyObj(out ho_ObjectSelected);
            ho_RegionL.Dispose();
            HOperatorSet.GenRectangle1(out ho_RegionL, 719.084, 594.187, 1462.31, 788.936);

            ho_RegionIntersection.Dispose();
            HOperatorSet.Intersection(ho_Regions, ho_RegionL, out ho_RegionIntersection);
            ho_RegionClosing.Dispose();
            HOperatorSet.ClosingRectangle1(ho_RegionIntersection, out ho_RegionClosing, 20,
                1);
            ho_RegionOpening.Dispose();
            HOperatorSet.OpeningRectangle1(ho_RegionClosing, out ho_RegionOpening, 1, 3);


            ho_ConnectedRegions.Dispose();
            HOperatorSet.Connection(ho_RegionOpening, out ho_ConnectedRegions);
            ho_SelectedRegions.Dispose();
            HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_SelectedRegions, "area",
                "and", 1000, 5000);

            HOperatorSet.CountObj(ho_SelectedRegions, out hv_Number);
            ho_testL.Dispose();
            HOperatorSet.GenRectangle1(out ho_testL, 725.508, 1658, 1482.24, 1660);

            ho_testL.Dispose();
            HOperatorSet.GenRectangle1(out ho_testL, 712.37, 663, 1507.58, 666);

            ho_RegionDifference.Dispose();
            HOperatorSet.Difference(ho_testL, ho_SelectedRegions, out ho_RegionDifference
                );
            ho_ConnectedRegions1.Dispose();
            HOperatorSet.Connection(ho_RegionDifference, out ho_ConnectedRegions1);
            ho_SortedRegions.Dispose();
            HOperatorSet.SortRegion(ho_ConnectedRegions1, out ho_SortedRegions, "character",
                "true", "row");
            HOperatorSet.CountObj(ho_SortedRegions, out hv_Number1);

            ho_EmptyObject.Dispose();
            HOperatorSet.GenEmptyObj(out ho_EmptyObject);

            HTuple end_val22 = hv_Number1 - 1;
            HTuple step_val22 = 1;
            for (hv_Index1 = 2; hv_Index1.Continue(end_val22, step_val22); hv_Index1 = hv_Index1.TupleAdd(step_val22))
            {
                ho_ObjectSelected.Dispose();
                HOperatorSet.SelectObj(ho_SortedRegions, out ho_ObjectSelected, hv_Index1);

                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.ConcatObj(ho_EmptyObject, ho_ObjectSelected, out ExpTmpOutVar_0
                        );
                    ho_EmptyObject.Dispose();
                    ho_EmptyObject = ExpTmpOutVar_0;
                }
            }

            HOperatorSet.SmallestRectangle1(ho_EmptyObject, out hv_Row1, out hv_Column1,
                out hv_Row2, out hv_Column2);
            hv_height = hv_Row2 - hv_Row1;
            HOperatorSet.SmallestRectangle1(ho_SelectedRegions, out hv_Row11, out hv_Column11,
                out hv_Row21, out hv_Column21);
            hv_PinWidth = hv_Column21 - hv_Column11;
            ho_RegionL.Dispose();
            ho_RegionIntersection.Dispose();
            ho_RegionClosing.Dispose();
            ho_RegionOpening.Dispose();
            ho_ConnectedRegions.Dispose();
            ho_testL.Dispose();
            ho_RegionDifference.Dispose();
            ho_ConnectedRegions1.Dispose();
            ho_SortedRegions.Dispose();
            ho_ObjectSelected.Dispose();

            return;
        }

        public void ttestDown(HObject ho_Regions, out HObject ho_SelectedRegions, out HObject ho_EmptyObject,
            out HTuple hv_Number, out HTuple hv_height, out HTuple hv_PinWidth)
        {



            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_RegionD, ho_RegionIntersection;
            HObject ho_RegionClosing, ho_RegionOpening, ho_ConnectedRegions;
            HObject ho_testD, ho_RegionDifference, ho_ConnectedRegions1;
            HObject ho_SortedRegions, ho_ObjectSelected = null;

            // Local control variables 

            HTuple hv_Number1 = null, hv_Index1 = null;
            HTuple hv_Row1 = null, hv_Column1 = null, hv_Row2 = null;
            HTuple hv_Column2 = null, hv_Row11 = null, hv_Column11 = null;
            HTuple hv_Row21 = null, hv_Column21 = null;
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions);
            HOperatorSet.GenEmptyObj(out ho_EmptyObject);
            HOperatorSet.GenEmptyObj(out ho_RegionD);
            HOperatorSet.GenEmptyObj(out ho_RegionIntersection);
            HOperatorSet.GenEmptyObj(out ho_RegionClosing);
            HOperatorSet.GenEmptyObj(out ho_RegionOpening);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
            HOperatorSet.GenEmptyObj(out ho_testD);
            HOperatorSet.GenEmptyObj(out ho_RegionDifference);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions1);
            HOperatorSet.GenEmptyObj(out ho_SortedRegions);
            HOperatorSet.GenEmptyObj(out ho_ObjectSelected);
            ho_RegionD.Dispose();
            HOperatorSet.GenRectangle1(out ho_RegionD, 1499.11, 776.337, 1661.74, 1553.33);

            ho_RegionIntersection.Dispose();
            HOperatorSet.Intersection(ho_Regions, ho_RegionD, out ho_RegionIntersection);
            ho_RegionClosing.Dispose();
            HOperatorSet.ClosingRectangle1(ho_RegionIntersection, out ho_RegionClosing, 1,
                20);
            ho_RegionOpening.Dispose();
            HOperatorSet.OpeningRectangle1(ho_RegionClosing, out ho_RegionOpening, 3, 1);



            ho_ConnectedRegions.Dispose();
            HOperatorSet.Connection(ho_RegionOpening, out ho_ConnectedRegions);
            ho_SelectedRegions.Dispose();
            HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_SelectedRegions, "area",
                "and", 1000, 5000);

            HOperatorSet.CountObj(ho_SelectedRegions, out hv_Number);


            ho_testD.Dispose();
            HOperatorSet.GenRectangle1(out ho_testD, 1590, 759.941, 1593, 1575.84);

            ho_RegionDifference.Dispose();
            HOperatorSet.Difference(ho_testD, ho_SelectedRegions, out ho_RegionDifference
                );
            ho_ConnectedRegions1.Dispose();
            HOperatorSet.Connection(ho_RegionDifference, out ho_ConnectedRegions1);
            ho_SortedRegions.Dispose();
            HOperatorSet.SortRegion(ho_ConnectedRegions1, out ho_SortedRegions, "character",
                "true", "row");
            HOperatorSet.CountObj(ho_SortedRegions, out hv_Number1);

            ho_EmptyObject.Dispose();
            HOperatorSet.GenEmptyObj(out ho_EmptyObject);

            HTuple end_val23 = hv_Number1 - 1;
            HTuple step_val23 = 1;
            for (hv_Index1 = 2; hv_Index1.Continue(end_val23, step_val23); hv_Index1 = hv_Index1.TupleAdd(step_val23))
            {
                ho_ObjectSelected.Dispose();
                HOperatorSet.SelectObj(ho_SortedRegions, out ho_ObjectSelected, hv_Index1);

                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.ConcatObj(ho_EmptyObject, ho_ObjectSelected, out ExpTmpOutVar_0
                        );
                    ho_EmptyObject.Dispose();
                    ho_EmptyObject = ExpTmpOutVar_0;
                }
            }

            HOperatorSet.SmallestRectangle1(ho_EmptyObject, out hv_Row1, out hv_Column1,
                out hv_Row2, out hv_Column2);
            hv_height = hv_Column2 - hv_Column1;
            HOperatorSet.SmallestRectangle1(ho_SelectedRegions, out hv_Row11, out hv_Column11,
                out hv_Row21, out hv_Column21);
            hv_PinWidth = hv_Row21 - hv_Row11;
            ho_RegionD.Dispose();
            ho_RegionIntersection.Dispose();
            ho_RegionClosing.Dispose();
            ho_RegionOpening.Dispose();
            ho_ConnectedRegions.Dispose();
            ho_testD.Dispose();
            ho_RegionDifference.Dispose();
            ho_ConnectedRegions1.Dispose();
            ho_SortedRegions.Dispose();
            ho_ObjectSelected.Dispose();

            return;
        }

        public void ttestUp(HObject ho_Regions, out HObject ho_SelectedRegions, out HObject ho_EmptyObject,
            out HTuple hv_Number, out HTuple hv_height, out HTuple hv_PinWidth)
        {



            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_RegionU, ho_RegionIntersection;
            HObject ho_RegionClosing, ho_RegionOpening, ho_ConnectedRegions;
            HObject ho_testU, ho_RegionDifference, ho_ConnectedRegions1;
            HObject ho_SortedRegions, ho_ObjectSelected = null;

            // Local control variables 

            HTuple hv_Number1 = null, hv_Index1 = null;
            HTuple hv_Row1 = null, hv_Column1 = null, hv_Row2 = null;
            HTuple hv_Column2 = null, hv_Row11 = null, hv_Column11 = null;
            HTuple hv_Row21 = null, hv_Column21 = null;
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions);
            HOperatorSet.GenEmptyObj(out ho_EmptyObject);
            HOperatorSet.GenEmptyObj(out ho_RegionU);
            HOperatorSet.GenEmptyObj(out ho_RegionIntersection);
            HOperatorSet.GenEmptyObj(out ho_RegionClosing);
            HOperatorSet.GenEmptyObj(out ho_RegionOpening);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
            HOperatorSet.GenEmptyObj(out ho_testU);
            HOperatorSet.GenEmptyObj(out ho_RegionDifference);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions1);
            HOperatorSet.GenEmptyObj(out ho_SortedRegions);
            HOperatorSet.GenEmptyObj(out ho_ObjectSelected);
            ho_RegionU.Dispose();
            HOperatorSet.GenRectangle1(out ho_RegionU, 519.336, 781.758, 722.777, 1570.17);

            ho_RegionIntersection.Dispose();
            HOperatorSet.Intersection(ho_Regions, ho_RegionU, out ho_RegionIntersection);
            ho_RegionClosing.Dispose();
            HOperatorSet.ClosingRectangle1(ho_RegionIntersection, out ho_RegionClosing, 1,
                20);
            ho_RegionOpening.Dispose();
            HOperatorSet.OpeningRectangle1(ho_RegionClosing, out ho_RegionOpening, 3, 1);


            ho_ConnectedRegions.Dispose();
            HOperatorSet.Connection(ho_RegionOpening, out ho_ConnectedRegions);
            ho_SelectedRegions.Dispose();
            HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_SelectedRegions, "area",
                "and", 1000, 5000);

            HOperatorSet.CountObj(ho_SelectedRegions, out hv_Number);


            ho_testU.Dispose();
            HOperatorSet.GenRectangle1(out ho_testU, 589.784, 786.918, 594.868, 1574.53);


            ho_RegionDifference.Dispose();
            HOperatorSet.Difference(ho_testU, ho_SelectedRegions, out ho_RegionDifference
                );
            ho_ConnectedRegions1.Dispose();
            HOperatorSet.Connection(ho_RegionDifference, out ho_ConnectedRegions1);
            ho_SortedRegions.Dispose();
            HOperatorSet.SortRegion(ho_ConnectedRegions1, out ho_SortedRegions, "character",
                "true", "row");
            HOperatorSet.CountObj(ho_SortedRegions, out hv_Number1);

            ho_EmptyObject.Dispose();
            HOperatorSet.GenEmptyObj(out ho_EmptyObject);

            HTuple end_val23 = hv_Number1 - 1;
            HTuple step_val23 = 1;
            for (hv_Index1 = 2; hv_Index1.Continue(end_val23, step_val23); hv_Index1 = hv_Index1.TupleAdd(step_val23))
            {
                ho_ObjectSelected.Dispose();
                HOperatorSet.SelectObj(ho_SortedRegions, out ho_ObjectSelected, hv_Index1);

                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.ConcatObj(ho_EmptyObject, ho_ObjectSelected, out ExpTmpOutVar_0
                        );
                    ho_EmptyObject.Dispose();
                    ho_EmptyObject = ExpTmpOutVar_0;
                }
            }

            HOperatorSet.SmallestRectangle1(ho_EmptyObject, out hv_Row1, out hv_Column1,
                out hv_Row2, out hv_Column2);
            hv_height = hv_Column2 - hv_Column1;
            HOperatorSet.SmallestRectangle1(ho_SelectedRegions, out hv_Row11, out hv_Column11,
                out hv_Row21, out hv_Column21);
            hv_PinWidth = hv_Row21 - hv_Row11;
            ho_RegionU.Dispose();
            ho_RegionIntersection.Dispose();
            ho_RegionClosing.Dispose();
            ho_RegionOpening.Dispose();
            ho_ConnectedRegions.Dispose();
            ho_testU.Dispose();
            ho_RegionDifference.Dispose();
            ho_ConnectedRegions1.Dispose();
            ho_SortedRegions.Dispose();
            ho_ObjectSelected.Dispose();

            return;
        }

        public void ttestRight(HObject ho_Regions, out HObject ho_SelectedRegions, out HObject ho_EmptyObject,
            out HTuple hv_Number, out HTuple hv_height, out HTuple hv_PinWidth)
        {



            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_RegionR, ho_RegionIntersection;
            HObject ho_RegionClosing, ho_RegionOpening, ho_ConnectedRegions;
            HObject ho_testR, ho_RegionDifference, ho_ConnectedRegions1;
            HObject ho_SortedRegions, ho_ObjectSelected = null;

            // Local control variables 

            HTuple hv_Number1 = null, hv_Index1 = null;
            HTuple hv_Row1 = null, hv_Column1 = null, hv_Row2 = null;
            HTuple hv_Column2 = null, hv_Row11 = null, hv_Column11 = null;
            HTuple hv_Row21 = null, hv_Column21 = null;
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions);
            HOperatorSet.GenEmptyObj(out ho_EmptyObject);
            HOperatorSet.GenEmptyObj(out ho_RegionR);
            HOperatorSet.GenEmptyObj(out ho_RegionIntersection);
            HOperatorSet.GenEmptyObj(out ho_RegionClosing);
            HOperatorSet.GenEmptyObj(out ho_RegionOpening);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
            HOperatorSet.GenEmptyObj(out ho_testR);
            HOperatorSet.GenEmptyObj(out ho_RegionDifference);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions1);
            HOperatorSet.GenEmptyObj(out ho_SortedRegions);
            HOperatorSet.GenEmptyObj(out ho_ObjectSelected);
            ho_RegionR.Dispose();
            HOperatorSet.GenRectangle1(out ho_RegionR, 706.338, 1560.02, 1474.56, 1748.48);
            ho_RegionIntersection.Dispose();
            HOperatorSet.Intersection(ho_Regions, ho_RegionR, out ho_RegionIntersection);
            ho_RegionClosing.Dispose();
            HOperatorSet.ClosingRectangle1(ho_RegionIntersection, out ho_RegionClosing, 20,
                1);
            ho_RegionOpening.Dispose();
            HOperatorSet.OpeningRectangle1(ho_RegionClosing, out ho_RegionOpening, 1, 3);


            ho_ConnectedRegions.Dispose();
            HOperatorSet.Connection(ho_RegionOpening, out ho_ConnectedRegions);
            ho_SelectedRegions.Dispose();
            HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_SelectedRegions, "area",
                "and", 1000, 5000);

            HOperatorSet.CountObj(ho_SelectedRegions, out hv_Number);
            ho_testR.Dispose();
            HOperatorSet.GenRectangle1(out ho_testR, 725.508, 1658, 1482.24, 1660);

            ho_RegionDifference.Dispose();
            HOperatorSet.Difference(ho_testR, ho_SelectedRegions, out ho_RegionDifference
                );
            ho_ConnectedRegions1.Dispose();
            HOperatorSet.Connection(ho_RegionDifference, out ho_ConnectedRegions1);
            ho_SortedRegions.Dispose();
            HOperatorSet.SortRegion(ho_ConnectedRegions1, out ho_SortedRegions, "character",
                "true", "row");
            HOperatorSet.CountObj(ho_SortedRegions, out hv_Number1);

            ho_EmptyObject.Dispose();
            HOperatorSet.GenEmptyObj(out ho_EmptyObject);

            HTuple end_val19 = hv_Number1 - 1;
            HTuple step_val19 = 1;
            for (hv_Index1 = 2; hv_Index1.Continue(end_val19, step_val19); hv_Index1 = hv_Index1.TupleAdd(step_val19))
            {
                ho_ObjectSelected.Dispose();
                HOperatorSet.SelectObj(ho_SortedRegions, out ho_ObjectSelected, hv_Index1);
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.ConcatObj(ho_EmptyObject, ho_ObjectSelected, out ExpTmpOutVar_0
                        );
                    ho_EmptyObject.Dispose();
                    ho_EmptyObject = ExpTmpOutVar_0;
                }
            }

            HOperatorSet.SmallestRectangle1(ho_EmptyObject, out hv_Row1, out hv_Column1,
                out hv_Row2, out hv_Column2);
            hv_height = hv_Row2 - hv_Row1;
            HOperatorSet.SmallestRectangle1(ho_SelectedRegions, out hv_Row11, out hv_Column11,
                out hv_Row21, out hv_Column21);
            hv_PinWidth = hv_Column21 - hv_Column11;
            ho_RegionR.Dispose();
            ho_RegionIntersection.Dispose();
            ho_RegionClosing.Dispose();
            ho_RegionOpening.Dispose();
            ho_ConnectedRegions.Dispose();
            ho_testR.Dispose();
            ho_RegionDifference.Dispose();
            ho_ConnectedRegions1.Dispose();
            ho_SortedRegions.Dispose();
            ho_ObjectSelected.Dispose();

            return;
        }

        public void checkX(HObject ho_ImageAffineTrans1, out HObject ho_SelectedRegions,
            out HTuple hv_Radius)
        {



            // Local iconic variables 

            HObject ho_RegionCheck, ho_ImageReduced2, ho_Regions1;
            HObject ho_RegionIntersection, ho_ConnectedRegions, ho_ContCircle;

            // Local control variables 

            HTuple hv_Row2 = null, hv_Column2 = null;
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions);
            HOperatorSet.GenEmptyObj(out ho_RegionCheck);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced2);
            HOperatorSet.GenEmptyObj(out ho_Regions1);
            HOperatorSet.GenEmptyObj(out ho_RegionIntersection);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
            HOperatorSet.GenEmptyObj(out ho_ContCircle);
            ho_RegionCheck.Dispose();
            HOperatorSet.GenRegionRuns(out ho_RegionCheck, ((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((
                (new HTuple(734)).TupleConcat(735)).TupleConcat(736)).TupleConcat(737)).TupleConcat(
                738)).TupleConcat(739)).TupleConcat(740)).TupleConcat(741)).TupleConcat(742)).TupleConcat(
                743)).TupleConcat(744)).TupleConcat(745)).TupleConcat(746)).TupleConcat(747)).TupleConcat(
                748)).TupleConcat(749)).TupleConcat(750)).TupleConcat(751)).TupleConcat(752)).TupleConcat(
                753)).TupleConcat(754)).TupleConcat(755)).TupleConcat(756)).TupleConcat(757)).TupleConcat(
                758)).TupleConcat(759)).TupleConcat(760)).TupleConcat(761)).TupleConcat(762)).TupleConcat(
                763)).TupleConcat(764)).TupleConcat(765)).TupleConcat(766)).TupleConcat(767)).TupleConcat(
                768)).TupleConcat(769)).TupleConcat(770)).TupleConcat(771)).TupleConcat(772)).TupleConcat(
                773)).TupleConcat(774)).TupleConcat(775)).TupleConcat(776)).TupleConcat(777)).TupleConcat(
                778)).TupleConcat(779)).TupleConcat(780)).TupleConcat(781)).TupleConcat(782)).TupleConcat(
                783)).TupleConcat(784)).TupleConcat(785)).TupleConcat(786)).TupleConcat(787)).TupleConcat(
                788)).TupleConcat(789)).TupleConcat(790)).TupleConcat(791)).TupleConcat(792)).TupleConcat(
                793)).TupleConcat(794)).TupleConcat(795)).TupleConcat(796)).TupleConcat(797)).TupleConcat(
                798)).TupleConcat(799)).TupleConcat(800)).TupleConcat(801)).TupleConcat(802)).TupleConcat(
                803)).TupleConcat(804)).TupleConcat(805)).TupleConcat(806)).TupleConcat(807)).TupleConcat(
                808)).TupleConcat(809)).TupleConcat(810)).TupleConcat(811)).TupleConcat(812)).TupleConcat(
                813)).TupleConcat(814)).TupleConcat(815)).TupleConcat(816)).TupleConcat(817)).TupleConcat(
                818)).TupleConcat(819)).TupleConcat(820)).TupleConcat(821)).TupleConcat(822)).TupleConcat(
                823)).TupleConcat(824)).TupleConcat(825)).TupleConcat(826)).TupleConcat(827)).TupleConcat(
                828)).TupleConcat(829)).TupleConcat(830)).TupleConcat(831)).TupleConcat(832)).TupleConcat(
                833)).TupleConcat(834)).TupleConcat(835)).TupleConcat(836)).TupleConcat(837)).TupleConcat(
                838)).TupleConcat(839)).TupleConcat(840)).TupleConcat(841)).TupleConcat(842)).TupleConcat(
                843)).TupleConcat(844)).TupleConcat(845)).TupleConcat(846)).TupleConcat(847)).TupleConcat(
                848)).TupleConcat(849)).TupleConcat(850)).TupleConcat(851)).TupleConcat(852)).TupleConcat(
                853)).TupleConcat(854)).TupleConcat(855)).TupleConcat(856)).TupleConcat(857)).TupleConcat(
                858)).TupleConcat(859)).TupleConcat(860)).TupleConcat(861)).TupleConcat(862)).TupleConcat(
                863)).TupleConcat(864)).TupleConcat(865)).TupleConcat(866)).TupleConcat(867)).TupleConcat(
                868)).TupleConcat(869)).TupleConcat(870)).TupleConcat(871)).TupleConcat(872)).TupleConcat(
                873)).TupleConcat(874)).TupleConcat(875)).TupleConcat(876)).TupleConcat(877)).TupleConcat(
                878)).TupleConcat(879)).TupleConcat(880)).TupleConcat(881)).TupleConcat(882)).TupleConcat(
                883)).TupleConcat(884)).TupleConcat(885)).TupleConcat(886)).TupleConcat(887)).TupleConcat(
                888)).TupleConcat(889)).TupleConcat(890)).TupleConcat(891)).TupleConcat(892)).TupleConcat(
                893)).TupleConcat(894)).TupleConcat(895)).TupleConcat(896)).TupleConcat(897)).TupleConcat(
                898)).TupleConcat(899)).TupleConcat(900)).TupleConcat(901)).TupleConcat(902)).TupleConcat(
                903)).TupleConcat(904)).TupleConcat(905)).TupleConcat(906)).TupleConcat(907)).TupleConcat(
                908)).TupleConcat(909)).TupleConcat(910)).TupleConcat(911)).TupleConcat(912)).TupleConcat(
                913)).TupleConcat(914)).TupleConcat(915)).TupleConcat(916)).TupleConcat(917)).TupleConcat(
                918)).TupleConcat(919)).TupleConcat(920)).TupleConcat(921)).TupleConcat(922)).TupleConcat(
                923)).TupleConcat(924)).TupleConcat(925)).TupleConcat(926)).TupleConcat(927)).TupleConcat(
                928)).TupleConcat(929)).TupleConcat(930)).TupleConcat(931)).TupleConcat(932)).TupleConcat(
                933)).TupleConcat(934)).TupleConcat(935)).TupleConcat(936)).TupleConcat(937)).TupleConcat(
                938)).TupleConcat(939)).TupleConcat(940)).TupleConcat(941)).TupleConcat(942)).TupleConcat(
                943)).TupleConcat(944)).TupleConcat(945)).TupleConcat(946)).TupleConcat(947)).TupleConcat(
                948)).TupleConcat(949)).TupleConcat(950)).TupleConcat(951)).TupleConcat(952)).TupleConcat(
                953)).TupleConcat(954)).TupleConcat(955)).TupleConcat(956)).TupleConcat(957)).TupleConcat(
                958)).TupleConcat(959)).TupleConcat(960)).TupleConcat(961)).TupleConcat(962)).TupleConcat(
                963)).TupleConcat(964)).TupleConcat(965)).TupleConcat(966)).TupleConcat(967)).TupleConcat(
                968)).TupleConcat(969)).TupleConcat(970)).TupleConcat(971)).TupleConcat(972)).TupleConcat(
                973)).TupleConcat(974)).TupleConcat(975)).TupleConcat(976)).TupleConcat(977)).TupleConcat(
                978)).TupleConcat(979)).TupleConcat(980)).TupleConcat(981)).TupleConcat(982)).TupleConcat(
                983)).TupleConcat(984)).TupleConcat(985)).TupleConcat(986)).TupleConcat(987)).TupleConcat(
                988)).TupleConcat(989)).TupleConcat(990)).TupleConcat(991)).TupleConcat(992)).TupleConcat(
                993)).TupleConcat(994)).TupleConcat(995)).TupleConcat(996)).TupleConcat(997)).TupleConcat(
                998)).TupleConcat(999)).TupleConcat(1000)).TupleConcat(1001)).TupleConcat(
                1002)).TupleConcat(1003)).TupleConcat(1004)).TupleConcat(1005)).TupleConcat(
                1006)).TupleConcat(1007)).TupleConcat(1008)).TupleConcat(1009)).TupleConcat(
                1010)).TupleConcat(1011)).TupleConcat(1012)).TupleConcat(1013)).TupleConcat(
                1014)).TupleConcat(1015)).TupleConcat(1016)).TupleConcat(1017)).TupleConcat(
                1018)).TupleConcat(1019)).TupleConcat(1020)).TupleConcat(1021)).TupleConcat(
                1022)).TupleConcat(1023)).TupleConcat(1024)).TupleConcat(1025)).TupleConcat(
                1026)).TupleConcat(1027)).TupleConcat(1028)).TupleConcat(1029)).TupleConcat(
                1030)).TupleConcat(1031)).TupleConcat(1032)).TupleConcat(1033)).TupleConcat(
                1034)).TupleConcat(1035)).TupleConcat(1036)).TupleConcat(1037)).TupleConcat(
                1038)).TupleConcat(1039)).TupleConcat(1040)).TupleConcat(1041)).TupleConcat(
                1042)).TupleConcat(1043)).TupleConcat(1044)).TupleConcat(1045)).TupleConcat(
                1046)).TupleConcat(1047)).TupleConcat(1048)).TupleConcat(1049)).TupleConcat(
                1050)).TupleConcat(1051)).TupleConcat(1052)).TupleConcat(1053)).TupleConcat(
                1054)).TupleConcat(1055)).TupleConcat(1056)).TupleConcat(1057)).TupleConcat(
                1058)).TupleConcat(1059)).TupleConcat(1060)).TupleConcat(1061)).TupleConcat(
                1062)).TupleConcat(1063)).TupleConcat(1064)).TupleConcat(1065)).TupleConcat(
                1066)).TupleConcat(1067)).TupleConcat(1068)).TupleConcat(1069)).TupleConcat(
                1070)).TupleConcat(1071)).TupleConcat(1072)).TupleConcat(1073)).TupleConcat(
                1074)).TupleConcat(1075)).TupleConcat(1076)).TupleConcat(1077)).TupleConcat(
                1078)).TupleConcat(1079)).TupleConcat(1080)).TupleConcat(1081)).TupleConcat(
                1082)).TupleConcat(1083)).TupleConcat(1084)).TupleConcat(1085)).TupleConcat(
                1086)).TupleConcat(1087)).TupleConcat(1088)).TupleConcat(1089)).TupleConcat(
                1090)).TupleConcat(1091)).TupleConcat(1092)).TupleConcat(1093)).TupleConcat(
                1094)).TupleConcat(1095)).TupleConcat(1096)).TupleConcat(1097)).TupleConcat(
                1098)).TupleConcat(1099)).TupleConcat(1100)).TupleConcat(1101)).TupleConcat(
                1102)).TupleConcat(1103)).TupleConcat(1104)).TupleConcat(1105)).TupleConcat(
                1106)).TupleConcat(1107)).TupleConcat(1108)).TupleConcat(1109)).TupleConcat(
                1110)).TupleConcat(1111)).TupleConcat(1112)).TupleConcat(1113)).TupleConcat(
                1114)).TupleConcat(1115)).TupleConcat(1116)).TupleConcat(1117)).TupleConcat(
                1118)).TupleConcat(1119)).TupleConcat(1120)).TupleConcat(1121)).TupleConcat(
                1122)).TupleConcat(1123)).TupleConcat(1124)).TupleConcat(1125)).TupleConcat(
                1126)).TupleConcat(1127)).TupleConcat(1128)).TupleConcat(1129)).TupleConcat(
                1130)).TupleConcat(1131)).TupleConcat(1132)).TupleConcat(1133)).TupleConcat(
                1134)).TupleConcat(1135)).TupleConcat(1136)).TupleConcat(1137)).TupleConcat(
                1138)).TupleConcat(1139)).TupleConcat(1140)).TupleConcat(1141)).TupleConcat(
                1142)).TupleConcat(1143)).TupleConcat(1144)).TupleConcat(1145)).TupleConcat(
                1146)).TupleConcat(1147)).TupleConcat(1148)).TupleConcat(1149)).TupleConcat(
                1150)).TupleConcat(1151)).TupleConcat(1152)).TupleConcat(1153)).TupleConcat(
                1154)).TupleConcat(1155)).TupleConcat(1156)).TupleConcat(1157)).TupleConcat(
                1158)).TupleConcat(1159)).TupleConcat(1160)).TupleConcat(1161)).TupleConcat(
                1162)).TupleConcat(1163)).TupleConcat(1164)).TupleConcat(1165)).TupleConcat(
                1166)).TupleConcat(1167)).TupleConcat(1168)).TupleConcat(1169)).TupleConcat(
                1170)).TupleConcat(1171)).TupleConcat(1172)).TupleConcat(1173)).TupleConcat(
                1174)).TupleConcat(1175)).TupleConcat(1176)).TupleConcat(1177)).TupleConcat(
                1178)).TupleConcat(1179)).TupleConcat(1180)).TupleConcat(1181)).TupleConcat(
                1182)).TupleConcat(1183)).TupleConcat(1184)).TupleConcat(1185)).TupleConcat(
                1186)).TupleConcat(1187)).TupleConcat(1188)).TupleConcat(1189)).TupleConcat(
                1190)).TupleConcat(1191)).TupleConcat(1192)).TupleConcat(1193)).TupleConcat(
                1194)).TupleConcat(1195)).TupleConcat(1196)).TupleConcat(1197)).TupleConcat(
                1198)).TupleConcat(1199)).TupleConcat(1200)).TupleConcat(1201)).TupleConcat(
                1202)).TupleConcat(1203)).TupleConcat(1204)).TupleConcat(1205)).TupleConcat(
                1206)).TupleConcat(1207)).TupleConcat(1208)).TupleConcat(1209)).TupleConcat(
                1210)).TupleConcat(1211)).TupleConcat(1212)).TupleConcat(1213)).TupleConcat(
                1214)).TupleConcat(1215)).TupleConcat(1216)).TupleConcat(1217)).TupleConcat(
                1218)).TupleConcat(1219)).TupleConcat(1220)).TupleConcat(1221)).TupleConcat(
                1222)).TupleConcat(1223)).TupleConcat(1224)).TupleConcat(1225)).TupleConcat(
                1226)).TupleConcat(1227)).TupleConcat(1228)).TupleConcat(1229)).TupleConcat(
                1230)).TupleConcat(1231)).TupleConcat(1232)).TupleConcat(1233)).TupleConcat(
                1234)).TupleConcat(1235)).TupleConcat(1236)).TupleConcat(1237)).TupleConcat(
                1238)).TupleConcat(1239)).TupleConcat(1240)).TupleConcat(1241)).TupleConcat(
                1242)).TupleConcat(1243)).TupleConcat(1244)).TupleConcat(1245)).TupleConcat(
                1246)).TupleConcat(1247)).TupleConcat(1248)).TupleConcat(1249)).TupleConcat(
                1250)).TupleConcat(1251)).TupleConcat(1252)).TupleConcat(1253)).TupleConcat(
                1254)).TupleConcat(1255)).TupleConcat(1256)).TupleConcat(1257)).TupleConcat(
                1258)).TupleConcat(1259)).TupleConcat(1260)).TupleConcat(1261)).TupleConcat(
                1262)).TupleConcat(1263)).TupleConcat(1264)).TupleConcat(1265)).TupleConcat(
                1266)).TupleConcat(1267)).TupleConcat(1268)).TupleConcat(1269)).TupleConcat(
                1270)).TupleConcat(1271)).TupleConcat(1272)).TupleConcat(1273)).TupleConcat(
                1274)).TupleConcat(1275)).TupleConcat(1276)).TupleConcat(1277)).TupleConcat(
                1278)).TupleConcat(1279)).TupleConcat(1280)).TupleConcat(1281)).TupleConcat(
                1282)).TupleConcat(1283)).TupleConcat(1284)).TupleConcat(1285)).TupleConcat(
                1286)).TupleConcat(1287)).TupleConcat(1288)).TupleConcat(1289)).TupleConcat(
                1290)).TupleConcat(1291)).TupleConcat(1292)).TupleConcat(1293)).TupleConcat(
                1294)).TupleConcat(1295)).TupleConcat(1296)).TupleConcat(1297)).TupleConcat(
                1298)).TupleConcat(1299)).TupleConcat(1300)).TupleConcat(1301)).TupleConcat(
                1302)).TupleConcat(1303)).TupleConcat(1304)).TupleConcat(1305)).TupleConcat(
                1306)).TupleConcat(1307)).TupleConcat(1308)).TupleConcat(1309)).TupleConcat(
                1310)).TupleConcat(1311)).TupleConcat(1312)).TupleConcat(1313)).TupleConcat(
                1314)).TupleConcat(1315)).TupleConcat(1316)).TupleConcat(1317)).TupleConcat(
                1318)).TupleConcat(1319)).TupleConcat(1320)).TupleConcat(1321)).TupleConcat(
                1322)).TupleConcat(1323)).TupleConcat(1324)).TupleConcat(1325)).TupleConcat(
                1326)).TupleConcat(1327)).TupleConcat(1328)).TupleConcat(1329)).TupleConcat(
                1330)).TupleConcat(1331)).TupleConcat(1332)).TupleConcat(1333)).TupleConcat(
                1334)).TupleConcat(1335)).TupleConcat(1336)).TupleConcat(1337)).TupleConcat(
                1338)).TupleConcat(1339)).TupleConcat(1340)).TupleConcat(1341)).TupleConcat(
                1342)).TupleConcat(1343)).TupleConcat(1344)).TupleConcat(1345)).TupleConcat(
                1346)).TupleConcat(1347)).TupleConcat(1348)).TupleConcat(1349)).TupleConcat(
                1350)).TupleConcat(1351)).TupleConcat(1352)).TupleConcat(1353)).TupleConcat(
                1354)).TupleConcat(1355)).TupleConcat(1356)).TupleConcat(1357)).TupleConcat(
                1358)).TupleConcat(1359)).TupleConcat(1360)).TupleConcat(1361)).TupleConcat(
                1362)).TupleConcat(1363)).TupleConcat(1364)).TupleConcat(1365)).TupleConcat(
                1366)).TupleConcat(1367)).TupleConcat(1368)).TupleConcat(1369)).TupleConcat(
                1370)).TupleConcat(1371)).TupleConcat(1372)).TupleConcat(1373)).TupleConcat(
                1374)).TupleConcat(1375)).TupleConcat(1376)).TupleConcat(1377)).TupleConcat(
                1378)).TupleConcat(1379)).TupleConcat(1380)).TupleConcat(1381)).TupleConcat(
                1382)).TupleConcat(1383)).TupleConcat(1384)).TupleConcat(1385)).TupleConcat(
                1386)).TupleConcat(1387)).TupleConcat(1388)).TupleConcat(1389)).TupleConcat(
                1390)).TupleConcat(1391)).TupleConcat(1392)).TupleConcat(1393)).TupleConcat(
                1394)).TupleConcat(1395)).TupleConcat(1396)).TupleConcat(1397)).TupleConcat(
                1398)).TupleConcat(1399)).TupleConcat(1400)).TupleConcat(1401)).TupleConcat(
                1402)).TupleConcat(1403)).TupleConcat(1404)).TupleConcat(1405)).TupleConcat(
                1406)).TupleConcat(1407)).TupleConcat(1408)).TupleConcat(1409)).TupleConcat(
                1410)).TupleConcat(1411)).TupleConcat(1412)).TupleConcat(1413)).TupleConcat(
                1414)).TupleConcat(1415)).TupleConcat(1416)).TupleConcat(1417)).TupleConcat(
                1418)).TupleConcat(1419)).TupleConcat(1420)).TupleConcat(1421)).TupleConcat(
                1422)).TupleConcat(1423)).TupleConcat(1424)).TupleConcat(1425)).TupleConcat(
                1426)).TupleConcat(1427)).TupleConcat(1428)).TupleConcat(1429)).TupleConcat(
                1430)).TupleConcat(1431)).TupleConcat(1432)).TupleConcat(1433)).TupleConcat(
                1434)).TupleConcat(1435)).TupleConcat(1436)).TupleConcat(1437)).TupleConcat(
                1438)).TupleConcat(1439)).TupleConcat(1440)).TupleConcat(1441)).TupleConcat(
                1442)).TupleConcat(1443)).TupleConcat(1444)).TupleConcat(1445)).TupleConcat(
                1446)).TupleConcat(1447)).TupleConcat(1448)).TupleConcat(1449)).TupleConcat(
                1450)).TupleConcat(1451)).TupleConcat(1452)).TupleConcat(1453)).TupleConcat(
                1454)).TupleConcat(1455)).TupleConcat(1456)).TupleConcat(1457)).TupleConcat(
                1458)).TupleConcat(1459)).TupleConcat(1460)).TupleConcat(1461)).TupleConcat(
                1462)).TupleConcat(1463)).TupleConcat(1464)).TupleConcat(1465)).TupleConcat(
                1466)).TupleConcat(1467)).TupleConcat(1468)).TupleConcat(1469), ((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((
                (new HTuple(1189)).TupleConcat(1111)).TupleConcat(1032)).TupleConcat(953)).TupleConcat(
                875)).TupleConcat(827)).TupleConcat(811)).TupleConcat(802)).TupleConcat(802)).TupleConcat(
                802)).TupleConcat(802)).TupleConcat(802)).TupleConcat(802)).TupleConcat(802)).TupleConcat(
                802)).TupleConcat(802)).TupleConcat(802)).TupleConcat(802)).TupleConcat(802)).TupleConcat(
                802)).TupleConcat(802)).TupleConcat(802)).TupleConcat(802)).TupleConcat(802)).TupleConcat(
                802)).TupleConcat(802)).TupleConcat(802)).TupleConcat(801)).TupleConcat(801)).TupleConcat(
                801)).TupleConcat(801)).TupleConcat(801)).TupleConcat(801)).TupleConcat(801)).TupleConcat(
                801)).TupleConcat(801)).TupleConcat(801)).TupleConcat(801)).TupleConcat(801)).TupleConcat(
                801)).TupleConcat(801)).TupleConcat(801)).TupleConcat(801)).TupleConcat(801)).TupleConcat(
                801)).TupleConcat(801)).TupleConcat(801)).TupleConcat(801)).TupleConcat(801)).TupleConcat(
                801)).TupleConcat(801)).TupleConcat(801)).TupleConcat(801)).TupleConcat(801)).TupleConcat(
                801)).TupleConcat(801)).TupleConcat(801)).TupleConcat(801)).TupleConcat(801)).TupleConcat(
                801)).TupleConcat(801)).TupleConcat(801)).TupleConcat(801)).TupleConcat(801)).TupleConcat(
                801)).TupleConcat(801)).TupleConcat(801)).TupleConcat(800)).TupleConcat(800)).TupleConcat(
                800)).TupleConcat(800)).TupleConcat(800)).TupleConcat(800)).TupleConcat(800)).TupleConcat(
                800)).TupleConcat(800)).TupleConcat(800)).TupleConcat(800)).TupleConcat(800)).TupleConcat(
                800)).TupleConcat(800)).TupleConcat(800)).TupleConcat(800)).TupleConcat(800)).TupleConcat(
                800)).TupleConcat(800)).TupleConcat(800)).TupleConcat(800)).TupleConcat(800)).TupleConcat(
                800)).TupleConcat(800)).TupleConcat(800)).TupleConcat(800)).TupleConcat(800)).TupleConcat(
                800)).TupleConcat(800)).TupleConcat(800)).TupleConcat(800)).TupleConcat(800)).TupleConcat(
                800)).TupleConcat(800)).TupleConcat(800)).TupleConcat(800)).TupleConcat(800)).TupleConcat(
                800)).TupleConcat(800)).TupleConcat(800)).TupleConcat(799)).TupleConcat(799)).TupleConcat(
                799)).TupleConcat(799)).TupleConcat(799)).TupleConcat(799)).TupleConcat(799)).TupleConcat(
                799)).TupleConcat(799)).TupleConcat(799)).TupleConcat(799)).TupleConcat(799)).TupleConcat(
                799)).TupleConcat(799)).TupleConcat(799)).TupleConcat(799)).TupleConcat(799)).TupleConcat(
                799)).TupleConcat(799)).TupleConcat(799)).TupleConcat(799)).TupleConcat(799)).TupleConcat(
                799)).TupleConcat(799)).TupleConcat(799)).TupleConcat(799)).TupleConcat(799)).TupleConcat(
                799)).TupleConcat(799)).TupleConcat(799)).TupleConcat(799)).TupleConcat(799)).TupleConcat(
                799)).TupleConcat(799)).TupleConcat(799)).TupleConcat(799)).TupleConcat(799)).TupleConcat(
                799)).TupleConcat(799)).TupleConcat(798)).TupleConcat(798)).TupleConcat(798)).TupleConcat(
                798)).TupleConcat(798)).TupleConcat(798)).TupleConcat(798)).TupleConcat(798)).TupleConcat(
                798)).TupleConcat(798)).TupleConcat(798)).TupleConcat(798)).TupleConcat(798)).TupleConcat(
                798)).TupleConcat(798)).TupleConcat(798)).TupleConcat(798)).TupleConcat(798)).TupleConcat(
                798)).TupleConcat(798)).TupleConcat(798)).TupleConcat(798)).TupleConcat(798)).TupleConcat(
                798)).TupleConcat(798)).TupleConcat(798)).TupleConcat(798)).TupleConcat(798)).TupleConcat(
                798)).TupleConcat(798)).TupleConcat(798)).TupleConcat(798)).TupleConcat(798)).TupleConcat(
                798)).TupleConcat(798)).TupleConcat(798)).TupleConcat(798)).TupleConcat(798)).TupleConcat(
                798)).TupleConcat(798)).TupleConcat(797)).TupleConcat(797)).TupleConcat(797)).TupleConcat(
                797)).TupleConcat(797)).TupleConcat(797)).TupleConcat(797)).TupleConcat(797)).TupleConcat(
                797)).TupleConcat(797)).TupleConcat(797)).TupleConcat(797)).TupleConcat(797)).TupleConcat(
                797)).TupleConcat(797)).TupleConcat(797)).TupleConcat(797)).TupleConcat(797)).TupleConcat(
                797)).TupleConcat(797)).TupleConcat(797)).TupleConcat(797)).TupleConcat(797)).TupleConcat(
                797)).TupleConcat(797)).TupleConcat(797)).TupleConcat(797)).TupleConcat(797)).TupleConcat(
                797)).TupleConcat(797)).TupleConcat(797)).TupleConcat(797)).TupleConcat(797)).TupleConcat(
                797)).TupleConcat(797)).TupleConcat(797)).TupleConcat(797)).TupleConcat(797)).TupleConcat(
                797)).TupleConcat(797)).TupleConcat(796)).TupleConcat(796)).TupleConcat(796)).TupleConcat(
                796)).TupleConcat(796)).TupleConcat(796)).TupleConcat(796)).TupleConcat(796)).TupleConcat(
                796)).TupleConcat(796)).TupleConcat(796)).TupleConcat(796)).TupleConcat(796)).TupleConcat(
                796)).TupleConcat(796)).TupleConcat(796)).TupleConcat(796)).TupleConcat(796)).TupleConcat(
                796)).TupleConcat(796)).TupleConcat(796)).TupleConcat(796)).TupleConcat(796)).TupleConcat(
                796)).TupleConcat(796)).TupleConcat(796)).TupleConcat(796)).TupleConcat(796)).TupleConcat(
                796)).TupleConcat(796)).TupleConcat(796)).TupleConcat(796)).TupleConcat(796)).TupleConcat(
                796)).TupleConcat(796)).TupleConcat(796)).TupleConcat(796)).TupleConcat(796)).TupleConcat(
                796)).TupleConcat(796)).TupleConcat(795)).TupleConcat(795)).TupleConcat(795)).TupleConcat(
                795)).TupleConcat(795)).TupleConcat(795)).TupleConcat(795)).TupleConcat(795)).TupleConcat(
                795)).TupleConcat(795)).TupleConcat(795)).TupleConcat(795)).TupleConcat(795)).TupleConcat(
                795)).TupleConcat(795)).TupleConcat(795)).TupleConcat(795)).TupleConcat(795)).TupleConcat(
                795)).TupleConcat(795)).TupleConcat(795)).TupleConcat(795)).TupleConcat(795)).TupleConcat(
                795)).TupleConcat(795)).TupleConcat(795)).TupleConcat(795)).TupleConcat(795)).TupleConcat(
                795)).TupleConcat(795)).TupleConcat(795)).TupleConcat(795)).TupleConcat(795)).TupleConcat(
                795)).TupleConcat(795)).TupleConcat(795)).TupleConcat(795)).TupleConcat(795)).TupleConcat(
                795)).TupleConcat(794)).TupleConcat(794)).TupleConcat(794)).TupleConcat(794)).TupleConcat(
                794)).TupleConcat(794)).TupleConcat(794)).TupleConcat(794)).TupleConcat(794)).TupleConcat(
                794)).TupleConcat(794)).TupleConcat(794)).TupleConcat(794)).TupleConcat(794)).TupleConcat(
                794)).TupleConcat(794)).TupleConcat(794)).TupleConcat(794)).TupleConcat(794)).TupleConcat(
                794)).TupleConcat(794)).TupleConcat(794)).TupleConcat(794)).TupleConcat(794)).TupleConcat(
                794)).TupleConcat(794)).TupleConcat(794)).TupleConcat(794)).TupleConcat(794)).TupleConcat(
                794)).TupleConcat(794)).TupleConcat(794)).TupleConcat(794)).TupleConcat(794)).TupleConcat(
                794)).TupleConcat(794)).TupleConcat(794)).TupleConcat(794)).TupleConcat(794)).TupleConcat(
                794)).TupleConcat(793)).TupleConcat(793)).TupleConcat(793)).TupleConcat(793)).TupleConcat(
                793)).TupleConcat(793)).TupleConcat(793)).TupleConcat(793)).TupleConcat(793)).TupleConcat(
                793)).TupleConcat(793)).TupleConcat(793)).TupleConcat(793)).TupleConcat(793)).TupleConcat(
                793)).TupleConcat(793)).TupleConcat(793)).TupleConcat(793)).TupleConcat(793)).TupleConcat(
                793)).TupleConcat(793)).TupleConcat(793)).TupleConcat(793)).TupleConcat(793)).TupleConcat(
                793)).TupleConcat(793)).TupleConcat(793)).TupleConcat(793)).TupleConcat(793)).TupleConcat(
                793)).TupleConcat(793)).TupleConcat(793)).TupleConcat(793)).TupleConcat(793)).TupleConcat(
                793)).TupleConcat(793)).TupleConcat(793)).TupleConcat(793)).TupleConcat(793)).TupleConcat(
                793)).TupleConcat(792)).TupleConcat(792)).TupleConcat(792)).TupleConcat(792)).TupleConcat(
                792)).TupleConcat(792)).TupleConcat(792)).TupleConcat(792)).TupleConcat(792)).TupleConcat(
                792)).TupleConcat(792)).TupleConcat(792)).TupleConcat(792)).TupleConcat(792)).TupleConcat(
                792)).TupleConcat(792)).TupleConcat(792)).TupleConcat(792)).TupleConcat(792)).TupleConcat(
                792)).TupleConcat(792)).TupleConcat(792)).TupleConcat(792)).TupleConcat(792)).TupleConcat(
                792)).TupleConcat(792)).TupleConcat(792)).TupleConcat(792)).TupleConcat(792)).TupleConcat(
                792)).TupleConcat(792)).TupleConcat(792)).TupleConcat(792)).TupleConcat(792)).TupleConcat(
                792)).TupleConcat(792)).TupleConcat(792)).TupleConcat(792)).TupleConcat(792)).TupleConcat(
                791)).TupleConcat(791)).TupleConcat(791)).TupleConcat(791)).TupleConcat(791)).TupleConcat(
                791)).TupleConcat(791)).TupleConcat(791)).TupleConcat(791)).TupleConcat(791)).TupleConcat(
                791)).TupleConcat(791)).TupleConcat(791)).TupleConcat(791)).TupleConcat(791)).TupleConcat(
                791)).TupleConcat(791)).TupleConcat(791)).TupleConcat(791)).TupleConcat(791)).TupleConcat(
                791)).TupleConcat(791)).TupleConcat(791)).TupleConcat(791)).TupleConcat(791)).TupleConcat(
                791)).TupleConcat(791)).TupleConcat(791)).TupleConcat(791)).TupleConcat(791)).TupleConcat(
                791)).TupleConcat(791)).TupleConcat(791)).TupleConcat(791)).TupleConcat(791)).TupleConcat(
                791)).TupleConcat(791)).TupleConcat(791)).TupleConcat(791)).TupleConcat(791)).TupleConcat(
                790)).TupleConcat(790)).TupleConcat(790)).TupleConcat(790)).TupleConcat(790)).TupleConcat(
                790)).TupleConcat(790)).TupleConcat(790)).TupleConcat(790)).TupleConcat(790)).TupleConcat(
                790)).TupleConcat(790)).TupleConcat(790)).TupleConcat(790)).TupleConcat(790)).TupleConcat(
                790)).TupleConcat(790)).TupleConcat(790)).TupleConcat(790)).TupleConcat(790)).TupleConcat(
                790)).TupleConcat(790)).TupleConcat(790)).TupleConcat(790)).TupleConcat(790)).TupleConcat(
                790)).TupleConcat(790)).TupleConcat(790)).TupleConcat(790)).TupleConcat(790)).TupleConcat(
                790)).TupleConcat(790)).TupleConcat(790)).TupleConcat(790)).TupleConcat(790)).TupleConcat(
                790)).TupleConcat(790)).TupleConcat(790)).TupleConcat(790)).TupleConcat(790)).TupleConcat(
                789)).TupleConcat(789)).TupleConcat(789)).TupleConcat(789)).TupleConcat(789)).TupleConcat(
                789)).TupleConcat(789)).TupleConcat(789)).TupleConcat(789)).TupleConcat(789)).TupleConcat(
                789)).TupleConcat(789)).TupleConcat(789)).TupleConcat(789)).TupleConcat(789)).TupleConcat(
                789)).TupleConcat(789)).TupleConcat(789)).TupleConcat(789)).TupleConcat(789)).TupleConcat(
                944)).TupleConcat(944)).TupleConcat(945)).TupleConcat(945)).TupleConcat(946)).TupleConcat(
                946)).TupleConcat(947)).TupleConcat(947)).TupleConcat(948)).TupleConcat(948)).TupleConcat(
                949)).TupleConcat(949)).TupleConcat(950)).TupleConcat(950)).TupleConcat(951)).TupleConcat(
                951)).TupleConcat(952)).TupleConcat(952)).TupleConcat(953)).TupleConcat(953)).TupleConcat(
                954)).TupleConcat(954)).TupleConcat(955)).TupleConcat(955)).TupleConcat(956)).TupleConcat(
                956)).TupleConcat(957)).TupleConcat(957)).TupleConcat(958)).TupleConcat(958)).TupleConcat(
                959)).TupleConcat(959)).TupleConcat(960)).TupleConcat(960)).TupleConcat(961)).TupleConcat(
                961)).TupleConcat(962)).TupleConcat(962)).TupleConcat(963)).TupleConcat(963)).TupleConcat(
                964)).TupleConcat(964)).TupleConcat(965)).TupleConcat(965)).TupleConcat(966)).TupleConcat(
                966)).TupleConcat(967)).TupleConcat(967)).TupleConcat(968)).TupleConcat(968)).TupleConcat(
                969)).TupleConcat(969)).TupleConcat(970)).TupleConcat(970)).TupleConcat(971)).TupleConcat(
                971)).TupleConcat(972)).TupleConcat(972)).TupleConcat(973)).TupleConcat(973)).TupleConcat(
                974)).TupleConcat(974)).TupleConcat(975)).TupleConcat(975)).TupleConcat(976)).TupleConcat(
                976)).TupleConcat(977)).TupleConcat(977)).TupleConcat(978)).TupleConcat(978)).TupleConcat(
                979)).TupleConcat(979)).TupleConcat(980)).TupleConcat(980)).TupleConcat(981)).TupleConcat(
                981)).TupleConcat(982)).TupleConcat(982)).TupleConcat(982)).TupleConcat(982)).TupleConcat(
                982)).TupleConcat(982)).TupleConcat(982)).TupleConcat(982)).TupleConcat(982)).TupleConcat(
                982)).TupleConcat(982)).TupleConcat(982)).TupleConcat(981)).TupleConcat(981)).TupleConcat(
                981)).TupleConcat(981)).TupleConcat(981)).TupleConcat(981)).TupleConcat(981)).TupleConcat(
                981)).TupleConcat(981)).TupleConcat(981)).TupleConcat(981)).TupleConcat(981)).TupleConcat(
                981)).TupleConcat(981)).TupleConcat(981)).TupleConcat(981)).TupleConcat(981)).TupleConcat(
                981)).TupleConcat(981)).TupleConcat(981)).TupleConcat(981)).TupleConcat(981)).TupleConcat(
                980)).TupleConcat(980)).TupleConcat(980)).TupleConcat(980)).TupleConcat(980)).TupleConcat(
                980)).TupleConcat(980)).TupleConcat(980)).TupleConcat(980)).TupleConcat(980)).TupleConcat(
                980)).TupleConcat(980)).TupleConcat(980)).TupleConcat(980)).TupleConcat(980)).TupleConcat(
                980)).TupleConcat(980)).TupleConcat(980)).TupleConcat(980)).TupleConcat(980)).TupleConcat(
                980)).TupleConcat(979)).TupleConcat(979)).TupleConcat(979)).TupleConcat(979)).TupleConcat(
                979)).TupleConcat(979)).TupleConcat(979)).TupleConcat(979)).TupleConcat(979)).TupleConcat(
                979)).TupleConcat(979)).TupleConcat(979)).TupleConcat(979)).TupleConcat(979)).TupleConcat(
                979)).TupleConcat(979)).TupleConcat(979)).TupleConcat(979)).TupleConcat(979)).TupleConcat(
                979)).TupleConcat(979)).TupleConcat(979)).TupleConcat(978)).TupleConcat(978)).TupleConcat(
                978)).TupleConcat(978)).TupleConcat(978)).TupleConcat(978)).TupleConcat(978)).TupleConcat(
                978)).TupleConcat(978)).TupleConcat(978)).TupleConcat(978)).TupleConcat(978)).TupleConcat(
                978)).TupleConcat(978)).TupleConcat(978)).TupleConcat(978)).TupleConcat(978)).TupleConcat(
                978)).TupleConcat(978)).TupleConcat(978)).TupleConcat(978)).TupleConcat(977)).TupleConcat(
                977)).TupleConcat(977)).TupleConcat(977)).TupleConcat(977)).TupleConcat(977)).TupleConcat(
                977)).TupleConcat(977)).TupleConcat(977)).TupleConcat(977)).TupleConcat(977)).TupleConcat(
                977)).TupleConcat(977)).TupleConcat(977)).TupleConcat(977)).TupleConcat(977)).TupleConcat(
                977)).TupleConcat(977)).TupleConcat(977)).TupleConcat(977)).TupleConcat(977)).TupleConcat(
                977)).TupleConcat(976)).TupleConcat(976)).TupleConcat(976)).TupleConcat(976)).TupleConcat(
                976)).TupleConcat(976)).TupleConcat(976)).TupleConcat(976)).TupleConcat(976)).TupleConcat(
                976)).TupleConcat(976)).TupleConcat(1015)).TupleConcat(1092)).TupleConcat(
                1169)).TupleConcat(1246)).TupleConcat(1323), ((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((
                (new HTuple(1230)).TupleConcat(1236)).TupleConcat(1242)).TupleConcat(1247)).TupleConcat(
                1253)).TupleConcat(1259)).TupleConcat(1265)).TupleConcat(1270)).TupleConcat(
                1276)).TupleConcat(1282)).TupleConcat(1287)).TupleConcat(1293)).TupleConcat(
                1299)).TupleConcat(1305)).TupleConcat(1310)).TupleConcat(1316)).TupleConcat(
                1322)).TupleConcat(1325)).TupleConcat(1326)).TupleConcat(1326)).TupleConcat(
                1327)).TupleConcat(1327)).TupleConcat(1328)).TupleConcat(1328)).TupleConcat(
                1329)).TupleConcat(1329)).TupleConcat(1330)).TupleConcat(1330)).TupleConcat(
                1331)).TupleConcat(1331)).TupleConcat(1332)).TupleConcat(1332)).TupleConcat(
                1333)).TupleConcat(1333)).TupleConcat(1334)).TupleConcat(1334)).TupleConcat(
                1335)).TupleConcat(1335)).TupleConcat(1336)).TupleConcat(1336)).TupleConcat(
                1337)).TupleConcat(1337)).TupleConcat(1338)).TupleConcat(1338)).TupleConcat(
                1339)).TupleConcat(1339)).TupleConcat(1340)).TupleConcat(1340)).TupleConcat(
                1341)).TupleConcat(1341)).TupleConcat(1342)).TupleConcat(1343)).TupleConcat(
                1343)).TupleConcat(1344)).TupleConcat(1344)).TupleConcat(1345)).TupleConcat(
                1345)).TupleConcat(1346)).TupleConcat(1346)).TupleConcat(1347)).TupleConcat(
                1347)).TupleConcat(1348)).TupleConcat(1348)).TupleConcat(1349)).TupleConcat(
                1349)).TupleConcat(1350)).TupleConcat(1350)).TupleConcat(1351)).TupleConcat(
                1351)).TupleConcat(1352)).TupleConcat(1352)).TupleConcat(1353)).TupleConcat(
                1353)).TupleConcat(1354)).TupleConcat(1354)).TupleConcat(1355)).TupleConcat(
                1355)).TupleConcat(1356)).TupleConcat(1356)).TupleConcat(1357)).TupleConcat(
                1357)).TupleConcat(1358)).TupleConcat(1358)).TupleConcat(1359)).TupleConcat(
                1360)).TupleConcat(1360)).TupleConcat(1361)).TupleConcat(1361)).TupleConcat(
                1362)).TupleConcat(1362)).TupleConcat(1363)).TupleConcat(1363)).TupleConcat(
                1364)).TupleConcat(1364)).TupleConcat(1365)).TupleConcat(1365)).TupleConcat(
                1366)).TupleConcat(1366)).TupleConcat(1367)).TupleConcat(1367)).TupleConcat(
                1368)).TupleConcat(1368)).TupleConcat(1369)).TupleConcat(1369)).TupleConcat(
                1370)).TupleConcat(1370)).TupleConcat(1371)).TupleConcat(1371)).TupleConcat(
                1372)).TupleConcat(1372)).TupleConcat(1373)).TupleConcat(1373)).TupleConcat(
                1374)).TupleConcat(1374)).TupleConcat(1375)).TupleConcat(1375)).TupleConcat(
                1376)).TupleConcat(1376)).TupleConcat(1377)).TupleConcat(1378)).TupleConcat(
                1378)).TupleConcat(1379)).TupleConcat(1379)).TupleConcat(1380)).TupleConcat(
                1380)).TupleConcat(1381)).TupleConcat(1381)).TupleConcat(1382)).TupleConcat(
                1382)).TupleConcat(1383)).TupleConcat(1383)).TupleConcat(1384)).TupleConcat(
                1384)).TupleConcat(1385)).TupleConcat(1385)).TupleConcat(1386)).TupleConcat(
                1386)).TupleConcat(1387)).TupleConcat(1387)).TupleConcat(1388)).TupleConcat(
                1388)).TupleConcat(1389)).TupleConcat(1389)).TupleConcat(1390)).TupleConcat(
                1390)).TupleConcat(1391)).TupleConcat(1391)).TupleConcat(1392)).TupleConcat(
                1392)).TupleConcat(1393)).TupleConcat(1393)).TupleConcat(1395)).TupleConcat(
                1397)).TupleConcat(1400)).TupleConcat(1402)).TupleConcat(1405)).TupleConcat(
                1407)).TupleConcat(1409)).TupleConcat(1412)).TupleConcat(1414)).TupleConcat(
                1417)).TupleConcat(1419)).TupleConcat(1422)).TupleConcat(1424)).TupleConcat(
                1427)).TupleConcat(1429)).TupleConcat(1431)).TupleConcat(1434)).TupleConcat(
                1436)).TupleConcat(1439)).TupleConcat(1441)).TupleConcat(1444)).TupleConcat(
                1446)).TupleConcat(1449)).TupleConcat(1451)).TupleConcat(1454)).TupleConcat(
                1456)).TupleConcat(1458)).TupleConcat(1461)).TupleConcat(1463)).TupleConcat(
                1466)).TupleConcat(1468)).TupleConcat(1471)).TupleConcat(1473)).TupleConcat(
                1476)).TupleConcat(1478)).TupleConcat(1480)).TupleConcat(1483)).TupleConcat(
                1485)).TupleConcat(1488)).TupleConcat(1490)).TupleConcat(1503)).TupleConcat(
                1527)).TupleConcat(1540)).TupleConcat(1540)).TupleConcat(1540)).TupleConcat(
                1540)).TupleConcat(1540)).TupleConcat(1540)).TupleConcat(1540)).TupleConcat(
                1540)).TupleConcat(1540)).TupleConcat(1540)).TupleConcat(1540)).TupleConcat(
                1540)).TupleConcat(1540)).TupleConcat(1540)).TupleConcat(1540)).TupleConcat(
                1540)).TupleConcat(1540)).TupleConcat(1540)).TupleConcat(1540)).TupleConcat(
                1540)).TupleConcat(1540)).TupleConcat(1540)).TupleConcat(1540)).TupleConcat(
                1540)).TupleConcat(1540)).TupleConcat(1540)).TupleConcat(1540)).TupleConcat(
                1540)).TupleConcat(1540)).TupleConcat(1539)).TupleConcat(1539)).TupleConcat(
                1539)).TupleConcat(1539)).TupleConcat(1539)).TupleConcat(1539)).TupleConcat(
                1539)).TupleConcat(1539)).TupleConcat(1539)).TupleConcat(1539)).TupleConcat(
                1539)).TupleConcat(1539)).TupleConcat(1539)).TupleConcat(1539)).TupleConcat(
                1539)).TupleConcat(1539)).TupleConcat(1539)).TupleConcat(1539)).TupleConcat(
                1539)).TupleConcat(1539)).TupleConcat(1539)).TupleConcat(1539)).TupleConcat(
                1539)).TupleConcat(1539)).TupleConcat(1539)).TupleConcat(1539)).TupleConcat(
                1539)).TupleConcat(1539)).TupleConcat(1539)).TupleConcat(1539)).TupleConcat(
                1539)).TupleConcat(1539)).TupleConcat(1539)).TupleConcat(1539)).TupleConcat(
                1539)).TupleConcat(1539)).TupleConcat(1539)).TupleConcat(1539)).TupleConcat(
                1539)).TupleConcat(1539)).TupleConcat(1539)).TupleConcat(1539)).TupleConcat(
                1539)).TupleConcat(1539)).TupleConcat(1539)).TupleConcat(1539)).TupleConcat(
                1539)).TupleConcat(1539)).TupleConcat(1539)).TupleConcat(1539)).TupleConcat(
                1539)).TupleConcat(1539)).TupleConcat(1539)).TupleConcat(1539)).TupleConcat(
                1539)).TupleConcat(1539)).TupleConcat(1539)).TupleConcat(1539)).TupleConcat(
                1538)).TupleConcat(1538)).TupleConcat(1538)).TupleConcat(1538)).TupleConcat(
                1538)).TupleConcat(1538)).TupleConcat(1538)).TupleConcat(1538)).TupleConcat(
                1538)).TupleConcat(1538)).TupleConcat(1538)).TupleConcat(1538)).TupleConcat(
                1538)).TupleConcat(1538)).TupleConcat(1538)).TupleConcat(1538)).TupleConcat(
                1538)).TupleConcat(1538)).TupleConcat(1538)).TupleConcat(1538)).TupleConcat(
                1538)).TupleConcat(1538)).TupleConcat(1538)).TupleConcat(1538)).TupleConcat(
                1538)).TupleConcat(1538)).TupleConcat(1538)).TupleConcat(1538)).TupleConcat(
                1538)).TupleConcat(1538)).TupleConcat(1538)).TupleConcat(1538)).TupleConcat(
                1538)).TupleConcat(1538)).TupleConcat(1538)).TupleConcat(1538)).TupleConcat(
                1538)).TupleConcat(1538)).TupleConcat(1538)).TupleConcat(1538)).TupleConcat(
                1538)).TupleConcat(1538)).TupleConcat(1538)).TupleConcat(1538)).TupleConcat(
                1538)).TupleConcat(1538)).TupleConcat(1538)).TupleConcat(1538)).TupleConcat(
                1538)).TupleConcat(1538)).TupleConcat(1538)).TupleConcat(1538)).TupleConcat(
                1538)).TupleConcat(1538)).TupleConcat(1538)).TupleConcat(1538)).TupleConcat(
                1538)).TupleConcat(1537)).TupleConcat(1537)).TupleConcat(1537)).TupleConcat(
                1537)).TupleConcat(1537)).TupleConcat(1537)).TupleConcat(1537)).TupleConcat(
                1537)).TupleConcat(1537)).TupleConcat(1537)).TupleConcat(1537)).TupleConcat(
                1537)).TupleConcat(1537)).TupleConcat(1537)).TupleConcat(1537)).TupleConcat(
                1537)).TupleConcat(1537)).TupleConcat(1537)).TupleConcat(1537)).TupleConcat(
                1537)).TupleConcat(1537)).TupleConcat(1537)).TupleConcat(1537)).TupleConcat(
                1537)).TupleConcat(1537)).TupleConcat(1537)).TupleConcat(1537)).TupleConcat(
                1537)).TupleConcat(1537)).TupleConcat(1537)).TupleConcat(1537)).TupleConcat(
                1537)).TupleConcat(1537)).TupleConcat(1537)).TupleConcat(1537)).TupleConcat(
                1537)).TupleConcat(1537)).TupleConcat(1537)).TupleConcat(1537)).TupleConcat(
                1537)).TupleConcat(1537)).TupleConcat(1537)).TupleConcat(1537)).TupleConcat(
                1537)).TupleConcat(1537)).TupleConcat(1537)).TupleConcat(1537)).TupleConcat(
                1537)).TupleConcat(1537)).TupleConcat(1537)).TupleConcat(1537)).TupleConcat(
                1537)).TupleConcat(1537)).TupleConcat(1537)).TupleConcat(1537)).TupleConcat(
                1537)).TupleConcat(1537)).TupleConcat(1537)).TupleConcat(1536)).TupleConcat(
                1536)).TupleConcat(1536)).TupleConcat(1536)).TupleConcat(1536)).TupleConcat(
                1536)).TupleConcat(1536)).TupleConcat(1536)).TupleConcat(1536)).TupleConcat(
                1536)).TupleConcat(1536)).TupleConcat(1536)).TupleConcat(1536)).TupleConcat(
                1536)).TupleConcat(1536)).TupleConcat(1536)).TupleConcat(1536)).TupleConcat(
                1536)).TupleConcat(1536)).TupleConcat(1536)).TupleConcat(1536)).TupleConcat(
                1536)).TupleConcat(1536)).TupleConcat(1536)).TupleConcat(1536)).TupleConcat(
                1536)).TupleConcat(1536)).TupleConcat(1536)).TupleConcat(1536)).TupleConcat(
                1536)).TupleConcat(1536)).TupleConcat(1536)).TupleConcat(1536)).TupleConcat(
                1536)).TupleConcat(1536)).TupleConcat(1536)).TupleConcat(1536)).TupleConcat(
                1536)).TupleConcat(1536)).TupleConcat(1536)).TupleConcat(1536)).TupleConcat(
                1536)).TupleConcat(1536)).TupleConcat(1536)).TupleConcat(1536)).TupleConcat(
                1536)).TupleConcat(1536)).TupleConcat(1536)).TupleConcat(1536)).TupleConcat(
                1536)).TupleConcat(1536)).TupleConcat(1536)).TupleConcat(1536)).TupleConcat(
                1536)).TupleConcat(1536)).TupleConcat(1536)).TupleConcat(1536)).TupleConcat(
                1536)).TupleConcat(1535)).TupleConcat(1535)).TupleConcat(1535)).TupleConcat(
                1535)).TupleConcat(1535)).TupleConcat(1535)).TupleConcat(1535)).TupleConcat(
                1535)).TupleConcat(1535)).TupleConcat(1535)).TupleConcat(1535)).TupleConcat(
                1535)).TupleConcat(1535)).TupleConcat(1535)).TupleConcat(1535)).TupleConcat(
                1535)).TupleConcat(1535)).TupleConcat(1535)).TupleConcat(1535)).TupleConcat(
                1535)).TupleConcat(1535)).TupleConcat(1535)).TupleConcat(1535)).TupleConcat(
                1535)).TupleConcat(1535)).TupleConcat(1535)).TupleConcat(1535)).TupleConcat(
                1535)).TupleConcat(1535)).TupleConcat(1535)).TupleConcat(1535)).TupleConcat(
                1535)).TupleConcat(1535)).TupleConcat(1535)).TupleConcat(1535)).TupleConcat(
                1535)).TupleConcat(1535)).TupleConcat(1535)).TupleConcat(1535)).TupleConcat(
                1535)).TupleConcat(1535)).TupleConcat(1535)).TupleConcat(1535)).TupleConcat(
                1535)).TupleConcat(1535)).TupleConcat(1535)).TupleConcat(1535)).TupleConcat(
                1535)).TupleConcat(1535)).TupleConcat(1535)).TupleConcat(1535)).TupleConcat(
                1535)).TupleConcat(1535)).TupleConcat(1535)).TupleConcat(1535)).TupleConcat(
                1535)).TupleConcat(1535)).TupleConcat(1534)).TupleConcat(1534)).TupleConcat(
                1534)).TupleConcat(1534)).TupleConcat(1534)).TupleConcat(1534)).TupleConcat(
                1534)).TupleConcat(1534)).TupleConcat(1534)).TupleConcat(1534)).TupleConcat(
                1534)).TupleConcat(1534)).TupleConcat(1534)).TupleConcat(1534)).TupleConcat(
                1534)).TupleConcat(1534)).TupleConcat(1534)).TupleConcat(1534)).TupleConcat(
                1534)).TupleConcat(1534)).TupleConcat(1534)).TupleConcat(1534)).TupleConcat(
                1534)).TupleConcat(1534)).TupleConcat(1534)).TupleConcat(1534)).TupleConcat(
                1534)).TupleConcat(1534)).TupleConcat(1534)).TupleConcat(1534)).TupleConcat(
                1534)).TupleConcat(1534)).TupleConcat(1534)).TupleConcat(1534)).TupleConcat(
                1534)).TupleConcat(1534)).TupleConcat(1534)).TupleConcat(1534)).TupleConcat(
                1534)).TupleConcat(1534)).TupleConcat(1534)).TupleConcat(1534)).TupleConcat(
                1534)).TupleConcat(1534)).TupleConcat(1534)).TupleConcat(1534)).TupleConcat(
                1534)).TupleConcat(1534)).TupleConcat(1534)).TupleConcat(1534)).TupleConcat(
                1534)).TupleConcat(1534)).TupleConcat(1534)).TupleConcat(1534)).TupleConcat(
                1534)).TupleConcat(1534)).TupleConcat(1534)).TupleConcat(1534)).TupleConcat(
                1533)).TupleConcat(1533)).TupleConcat(1533)).TupleConcat(1533)).TupleConcat(
                1533)).TupleConcat(1533)).TupleConcat(1533)).TupleConcat(1533)).TupleConcat(
                1533)).TupleConcat(1533)).TupleConcat(1533)).TupleConcat(1533)).TupleConcat(
                1533)).TupleConcat(1533)).TupleConcat(1533)).TupleConcat(1533)).TupleConcat(
                1533)).TupleConcat(1533)).TupleConcat(1533)).TupleConcat(1533)).TupleConcat(
                1533)).TupleConcat(1533)).TupleConcat(1533)).TupleConcat(1533)).TupleConcat(
                1533)).TupleConcat(1533)).TupleConcat(1533)).TupleConcat(1533)).TupleConcat(
                1533)).TupleConcat(1528)).TupleConcat(1519)).TupleConcat(1510)).TupleConcat(
                1501)).TupleConcat(1492)).TupleConcat(1483)).TupleConcat(1474)).TupleConcat(
                1465)).TupleConcat(1455)).TupleConcat(1446)).TupleConcat(1437)).TupleConcat(
                1428)).TupleConcat(1419)).TupleConcat(1410)).TupleConcat(1401)).TupleConcat(
                1397)).TupleConcat(1397)).TupleConcat(1396)).TupleConcat(1396)).TupleConcat(
                1396)).TupleConcat(1396)).TupleConcat(1396)).TupleConcat(1395)).TupleConcat(
                1395)).TupleConcat(1395)).TupleConcat(1395)).TupleConcat(1395)).TupleConcat(
                1394)).TupleConcat(1394)).TupleConcat(1394)).TupleConcat(1394)).TupleConcat(
                1394)).TupleConcat(1393)).TupleConcat(1393)).TupleConcat(1393)).TupleConcat(
                1393)).TupleConcat(1393)).TupleConcat(1392)).TupleConcat(1392)).TupleConcat(
                1392)).TupleConcat(1392)).TupleConcat(1392)).TupleConcat(1391)).TupleConcat(
                1391)).TupleConcat(1391)).TupleConcat(1391)).TupleConcat(1391)).TupleConcat(
                1390)).TupleConcat(1390)).TupleConcat(1390)).TupleConcat(1390)).TupleConcat(
                1390)).TupleConcat(1389)).TupleConcat(1389)).TupleConcat(1389)).TupleConcat(
                1389)).TupleConcat(1389)).TupleConcat(1388)).TupleConcat(1388)).TupleConcat(
                1388)).TupleConcat(1388)).TupleConcat(1388)).TupleConcat(1387)).TupleConcat(
                1387)).TupleConcat(1387)).TupleConcat(1387)).TupleConcat(1387)).TupleConcat(
                1386)).TupleConcat(1386)).TupleConcat(1386)).TupleConcat(1386)).TupleConcat(
                1386)).TupleConcat(1385)).TupleConcat(1385)).TupleConcat(1385)).TupleConcat(
                1385)).TupleConcat(1385)).TupleConcat(1384)).TupleConcat(1384)).TupleConcat(
                1384)).TupleConcat(1384)).TupleConcat(1384)).TupleConcat(1383)).TupleConcat(
                1383)).TupleConcat(1383)).TupleConcat(1383)).TupleConcat(1383)).TupleConcat(
                1382)).TupleConcat(1382)).TupleConcat(1382)).TupleConcat(1382)).TupleConcat(
                1382)).TupleConcat(1381)).TupleConcat(1381)).TupleConcat(1381)).TupleConcat(
                1381)).TupleConcat(1381)).TupleConcat(1380)).TupleConcat(1380)).TupleConcat(
                1380)).TupleConcat(1380)).TupleConcat(1380)).TupleConcat(1379)).TupleConcat(
                1379)).TupleConcat(1379)).TupleConcat(1379)).TupleConcat(1379)).TupleConcat(
                1378)).TupleConcat(1378)).TupleConcat(1378)).TupleConcat(1378)).TupleConcat(
                1378)).TupleConcat(1377)).TupleConcat(1377)).TupleConcat(1377)).TupleConcat(
                1377)).TupleConcat(1377)).TupleConcat(1376)).TupleConcat(1376)).TupleConcat(
                1376)).TupleConcat(1375)).TupleConcat(1374)).TupleConcat(1374)).TupleConcat(
                1373)).TupleConcat(1372)).TupleConcat(1371)).TupleConcat(1370)).TupleConcat(
                1370)).TupleConcat(1369)).TupleConcat(1368)).TupleConcat(1367)).TupleConcat(
                1367)).TupleConcat(1366)).TupleConcat(1365)).TupleConcat(1364)).TupleConcat(
                1363)).TupleConcat(1363)).TupleConcat(1362)).TupleConcat(1361));
            ho_ImageReduced2.Dispose();
            HOperatorSet.ReduceDomain(ho_ImageAffineTrans1, ho_RegionCheck, out ho_ImageReduced2
                );
            ho_Regions1.Dispose();
            HOperatorSet.Threshold(ho_ImageReduced2, out ho_Regions1, 0, 60);
            ho_RegionIntersection.Dispose();
            HOperatorSet.Intersection(ho_RegionCheck, ho_Regions1, out ho_RegionIntersection
                );
            ho_ConnectedRegions.Dispose();
            HOperatorSet.Connection(ho_RegionIntersection, out ho_ConnectedRegions);
            ho_SelectedRegions.Dispose();
            HOperatorSet.SelectShapeStd(ho_ConnectedRegions, out ho_SelectedRegions, "max_area",
                70);
            HOperatorSet.SmallestCircle(ho_SelectedRegions, out hv_Row2, out hv_Column2,
                out hv_Radius);
            ho_ContCircle.Dispose();
            
            ho_RegionCheck.Dispose();
            ho_ImageReduced2.Dispose();
            ho_Regions1.Dispose();
            ho_RegionIntersection.Dispose();
            ho_ConnectedRegions.Dispose();
            ho_ContCircle.Dispose();

            return;
        }

        public void CheckHeight(HTuple hv_testH, HTuple hv_HeightMin, HTuple hv_HeightMax,
            out HTuple hv_flag)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_num = null, hv_Index1 = null;
            // Initialize local and output iconic variables 
            hv_flag = 0;
            hv_num = new HTuple(hv_testH.TupleLength());
            HTuple end_val2 = hv_num - 1;
            HTuple step_val2 = 1;
            for (hv_Index1 = 0; hv_Index1.Continue(end_val2, step_val2); hv_Index1 = hv_Index1.TupleAdd(step_val2))
            {
                if ((int)(new HTuple(((hv_testH.TupleSelect(hv_Index1))).TupleLess(hv_HeightMin))) != 0)
                {
                    hv_flag = 1;
                }
                if ((int)(new HTuple(((hv_testH.TupleSelect(hv_Index1))).TupleGreater(hv_HeightMax))) != 0)
                {
                    hv_flag = 1;
                }
            }

            return;
        }

        public void testWidth(HTuple hv_testW, HTuple hv_WidthMax, out HTuple hv_flag)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_num = null, hv_Index1 = null;
            // Initialize local and output iconic variables 
            hv_flag = 0;
            hv_num = new HTuple(hv_testW.TupleLength());
            HTuple end_val2 = hv_num - 1;
            HTuple step_val2 = 1;
            for (hv_Index1 = 0; hv_Index1.Continue(end_val2, step_val2); hv_Index1 = hv_Index1.TupleAdd(step_val2))
            {
                if ((int)(new HTuple(((hv_testW.TupleSelect(hv_Index1))).TupleGreater(hv_WidthMax))) != 0)
                {
                    hv_flag = 1;
                }
            }

            return;
        }



        public void testMiss(HObject ho_ImageAffineTrans2, out HObject ho_RegionUnion,
      out HTuple hv_Number)
        {



            // Local iconic variables 

            HObject ho_regionCK, ho_ImageReduced, ho_Regions1;
            HObject ho_ConnectedRegions, ho_SelectedRegions1;
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_RegionUnion);
            HOperatorSet.GenEmptyObj(out ho_regionCK);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced);
            HOperatorSet.GenEmptyObj(out ho_Regions1);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions1);
            ho_regionCK.Dispose();
            HOperatorSet.GenRectangle1(out ho_regionCK, 697.238, 778.491, 1487.97, 1567.96);
            ho_ImageReduced.Dispose();
            HOperatorSet.ReduceDomain(ho_ImageAffineTrans2, ho_regionCK, out ho_ImageReduced
                );
            ho_Regions1.Dispose();
            HOperatorSet.Threshold(ho_ImageReduced, out ho_Regions1, 120, 255);
            ho_ConnectedRegions.Dispose();
            HOperatorSet.Connection(ho_Regions1, out ho_ConnectedRegions);
            ho_SelectedRegions1.Dispose();
            HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_SelectedRegions1, "area",
                "and", 500, 50000);
            HOperatorSet.CountObj(ho_SelectedRegions1, out hv_Number);
            ho_RegionUnion.Dispose();
            HOperatorSet.Union1(ho_SelectedRegions1, out ho_RegionUnion);
            ho_regionCK.Dispose();
            ho_ImageReduced.Dispose();
            ho_Regions1.Dispose();
            ho_ConnectedRegions.Dispose();
            ho_SelectedRegions1.Dispose();

            return;
        }

    }
}
