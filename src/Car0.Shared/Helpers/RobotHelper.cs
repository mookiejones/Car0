using System;
using System.Collections.Generic;
using System.Text;

namespace CarZero.Helpers
{
    public static class RobotHelper
    {

        public const string ABB = "ABB";
        public const string FANUC = "Fanuc";
        public const string FANUC_RJ = "FanucRJ";
        public const string KUKA = "KUKA";
        public const string NACHI = "Nachi";
        public static string[] GetRobotTypes() => new[] {ABB, FANUC, FANUC_RJ, KUKA, NACHI};

    }
}
