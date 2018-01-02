using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Resources;
using System.Data;
using System.IO;
using System.Windows.Forms;

namespace Car0
{
    class AppToolName
    {
        #region Public Variables
        public string Name;
        #endregion
        #region Private Variables
         private string RecognizePed = "Ped";
         private string RecognizeSpot1 = "W/Gun";
         private string RecognizeSpot2 = "Weld";
         private string RecognizeGripper = "Gripper";
         private string RecognizeSeal1 = "Seal";
         private string RecognizeSeal2 = "ADH";
         private string RecognizeStud = "Stud";
         private string RecognizeNut = "Nut";
         private string RecognizeScribe = "Scribe";
         private string RecognizeVision = "Vision";
         private string RecognizeMig = "Mig";
         private string RecognizeRivet = "Rivet";

         private string Spot = "_tGun";
         private string Grip = "_tGrip";
         private string Stud = "_tStud";
         private string Nut = "_tndrive";
         private string Mig = "_tMig";
         private string Rivet = "_tRiv";
         private string Seal = "_tGlue";
         private string Scribe = "_tScribe";
         private string Vision = "_tVision";

         private string PedSpot = "_ST_tGun";
         private string PedStud = "_ST_tStud";
         private string PedRivet = "_ST_Riv";
         private string PedSeal = "_ST_tGlue";
         private string PedScribe = "_ST_tScribe";
        #endregion
        #region Public Methods
        public AppToolName()
        {
            Name = "_tl";
        }
        public AppToolName(string AppType)
        {
            Name = "_tl";

            if (AppType.Contains(RecognizePed))
            {
                if (AppType.Contains(RecognizeSpot1) || AppType.Contains(RecognizeSpot2))
                    Name = PedSpot;
                else if (AppType.Contains(RecognizeStud))
                    Name = PedStud;
                else if (AppType.Contains(RecognizeRivet))
                    Name = PedRivet;
                else if (AppType.Contains(RecognizeSeal1) || AppType.Contains(RecognizeSeal2))
                    Name = PedSeal;
                else if (AppType.Contains(RecognizeScribe))
                    Name = PedScribe;
            }
            else
            {
                if (AppType.Contains(RecognizeSpot1) || AppType.Contains(RecognizeSpot2))
                    Name = Spot;
                else if (AppType.Contains(RecognizeGripper))
                    Name = Grip;
                else if (AppType.Contains(RecognizeStud))
                    Name = Stud;
                else if (AppType.Contains(RecognizeNut))
                    Name = Nut;
                else if (AppType.Contains(RecognizeMig))
                    Name = Mig;
                else if (AppType.Contains(RecognizeRivet))
                    Name = Rivet;
                else if (AppType.Contains(RecognizeSeal1) || AppType.Contains(RecognizeSeal2))
                    Name = Seal;
                else if (AppType.Contains(RecognizeScribe))
                    Name = Scribe;
                else if (AppType.Contains(RecognizeVision))
                    Name = Vision;
            }
        }
        public AppToolName(RobotData.FrameType Ftype, Brand RobMake)
        {
            switch (Ftype)
            {
                case RobotData.FrameType.DropTool:
                case RobotData.FrameType.PickTool:
                case RobotData.FrameType.FixtureCar0:

                    Name = RobMake.FixtureCar0NameFormat;
                    break;

                case RobotData.FrameType.GripperCar0:

                    Name = RobMake.GripperCar0NameFormat;
                    break;

                case RobotData.FrameType.GripPin:

                    Name = RobMake.GripTcpNameFormat;
                    break;

                case RobotData.FrameType.MigGunTip:

                    Name = RobMake.MigTcpNameFormat;
                    break;

                case RobotData.FrameType.LaserTip:

                    Name = RobMake.MigTcpNameFormat;        //TODO: Eliminate
                    break;

                case RobotData.FrameType.PierceTip:

                    Name = RobMake.MigTcpNameFormat;        //TODO: Eliminate once we are sure not ped
                    break;

                case RobotData.FrameType.NutGunTip:

                    Name = RobMake.NutTcpNameFormat;
                    break;

                case RobotData.FrameType.RivetGunTip:

                    Name = RobMake.RivetTcpNameFormat;
                    break;

                case RobotData.FrameType.ScribeGun:

                    Name = RobMake.ScribeTcpNameFormat;
                    break;

                case RobotData.FrameType.SealGunTip:

                    Name = RobMake.SealTcpNameFormat;
                    break;

                case RobotData.FrameType.SpotGunTip:

                    Name = RobMake.SpotTcpNameFormat;
                    break;

                case RobotData.FrameType.StudGunTip:

                    Name = RobMake.StudTcpNameFormat;
                    break;

                case RobotData.FrameType.PedRivetGunTip:

                    Name = RobMake.PedRivetTcpNameFormat;
                    break;

                case RobotData.FrameType.PedScribeGunTip:

                    Name = RobMake.PedScribeTcpNameFormat;
                    break;

                case RobotData.FrameType.PedSealGunTip:

                    Name = RobMake.PedSealTcpNameFormat;
                    break;

                case RobotData.FrameType.PedSpotGunTip:

                    Name = RobMake.PedSpotTcpNameFormat;
                    break;

                case RobotData.FrameType.PedStudGunTip:

                    Name = RobMake.PedStudTcpNameFormat;
                    break;

                case RobotData.FrameType.VisionTcp:

                    break;

                default:

                    Name = "_tl";
                    break;
            }
        }
        #endregion
        #region Private Methods
        #endregion
    }
}
