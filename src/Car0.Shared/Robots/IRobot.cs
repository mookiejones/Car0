using System;
using System.Collections.Generic;
using System.Text;

namespace CarZero.Robots
{
    public interface IRobot
    {
        void ReadTargets(string rootPath, List<Vector> j4, List<Vector> j5, List<Vector> j6, List<string> problems);
    }
}
