 
using CarZero.Classes;

namespace CarZero
{
    using System;

    internal class AppToolName
    {
        private static string Grip = "_tGrip";
        private static string Mig = "_tMig";
        public string Name;
        private static string Nut = "_tndrive";
        private static string PedRivet = "_ST_Riv";
        private static string PedScribe = "_ST_tScribe";
        private static string PedSeal = "_ST_tGlue";
        private static string PedSpot = "_ST_tGun";
        private static string PedStud = "_ST_tStud";
        private static string RecognizeGripper = "Gripper";
        private static string RecognizeMig = "Mig";
        private static string RecognizeNut = "Nut";
        private static string RecognizePed = "Ped";
        private static string RecognizeRivet = "Rivet";
        private static string RecognizeScribe = "Scribe";
        private static string RecognizeSeal1 = "Seal";
        private static string RecognizeSeal2 = "ADH";
        private static string RecognizeSpot1 = "W/Gun";
        private static string RecognizeSpot2 = "Weld";
        private static string RecognizeStud = "Stud";
        private static string RecognizeVision = "Vision";
        private static string Rivet = "_tRiv";
        private static string Scribe = "_tScribe";
        private static string Seal = "_tGlue";
        private static string Spot = "_tGun";
        private static string Stud = "_tStud";
        private static string Vision = "_tVision";

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
                {
                    Name = PedSpot;
                }
                else if (AppType.Contains(RecognizeStud))
                {
                    Name = PedStud;
                }
                else if (AppType.Contains(RecognizeRivet))
                {
                    Name = PedRivet;
                }
                else if (AppType.Contains(RecognizeSeal1) || AppType.Contains(RecognizeSeal2))
                {
                    Name = PedSeal;
                }
                else if (AppType.Contains(RecognizeScribe))
                {
                    Name = PedScribe;
                }
            }
            else if (AppType.Contains(RecognizeSpot1) || AppType.Contains(RecognizeSpot2))
            {
                Name = Spot;
            }
            else if (AppType.Contains(RecognizeGripper))
            {
                Name = Grip;
            }
            else if (AppType.Contains(RecognizeStud))
            {
                Name = Stud;
            }
            else if (AppType.Contains(RecognizeNut))
            {
                Name = Nut;
            }
            else if (AppType.Contains(RecognizeMig))
            {
                Name = Mig;
            }
            else if (AppType.Contains(RecognizeRivet))
            {
                Name = Rivet;
            }
            else if (AppType.Contains(RecognizeSeal1) || AppType.Contains(RecognizeSeal2))
            {
                Name = Seal;
            }
            else if (AppType.Contains(RecognizeScribe))
            {
                Name = Scribe;
            }
            else if (AppType.Contains(RecognizeVision))
            {
                Name = Vision;
            }
        }

        public AppToolName(FrameType Ftype, Brand RobMake)
        {
            switch (Ftype)
            {
                case FrameType.MigGunTip:
                    Name = RobMake.MigTcpNameFormat;
                    break;

                case FrameType.NutGunTip:
                    Name = RobMake.NutTcpNameFormat;
                    break;

                case FrameType.RivetGunTip:
                    Name = RobMake.RivetTcpNameFormat;
                    break;

                case FrameType.ScribeGun:
                    Name = RobMake.ScribeTcpNameFormat;
                    break;

                case FrameType.SealGunTip:
                    Name = RobMake.SealTcpNameFormat;
                    break;

                case FrameType.SpotGunTip:
                    Name = RobMake.SpotTcpNameFormat;
                    break;

                case FrameType.StudGunTip:
                    Name = RobMake.StudTcpNameFormat;
                    break;

                case FrameType.GripPin:
                    Name = RobMake.GripTcpNameFormat;
                    break;

                case FrameType.FixtureCar0:
                case FrameType.PickTool:
                case FrameType.DropTool:
                    Name = RobMake.FixtureCar0NameFormat;
                    break;

                case FrameType.GripperCar0:
                    Name = RobMake.GripperCar0NameFormat;
                    break;

                case FrameType.PedRivetGunTip:
                    Name = RobMake.PedRivetTcpNameFormat;
                    break;

                case FrameType.PedSealGunTip:
                    Name = RobMake.PedSealTcpNameFormat;
                    break;

                case FrameType.PedSpotGunTip:
                    Name = RobMake.PedSpotTcpNameFormat;
                    break;

                case FrameType.PedScribeGunTip:
                    Name = RobMake.PedScribeTcpNameFormat;
                    break;

                case FrameType.PedStudGunTip:
                    Name = RobMake.PedStudTcpNameFormat;
                    break;

                case FrameType.LaserTip:
                    Name = RobMake.MigTcpNameFormat;
                    break;

                case FrameType.PierceTip:
                    Name = RobMake.MigTcpNameFormat;
                    break;

                case FrameType.VisionTcp:
                    break;

                default:
                    Name = "_tl";
                    break;
            }
        }
    }
}

