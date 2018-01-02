using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CarZero.Classes;

namespace CarZero
{
    public partial class RobotData
    {
        public static List<FrameType> FrameTypes = new List<FrameType>();

        
        public static string StationCode = null;
        public static string StationName = null;
        private static ArrayList SystemCodes = new ArrayList();
        private static List<AppToolName> ToolNames = new List<AppToolName>();
        public static ArrayList RobotFrames = new ArrayList();
        public static ArrayList RobotFramesChecked = new ArrayList();
        private static ArrayList FrameDescriptions = new ArrayList();
        public static List<int> FrameNumbers = new List<int>();
        private static bool Initializing = false;
        private static bool ChecksChanged = false;
        public static string RobotMechanismName = null;
        public static string RobotName = null;
        public static string RobotStationName = null;
        private static bool ShowingRobots = false;
    }
}
