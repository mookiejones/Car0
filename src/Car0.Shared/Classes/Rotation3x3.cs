namespace CarZero
{
    using System;

    internal class Rotation3x3
    {
        public double[] rot;

        public Rotation3x3()
        {
            rot = new double[9];
        }

        public Rotation3x3(Transformation a)
        {
            rot = new double[9];
            for (var i = 0; i < 3; i++)
            {
                for (var j = 0; j < 3; j++)
                {
                    rot[(3 * i) + j] = a.mat[(4 * i) + j];
                }
            }
        }

        public Rotation3x3(RotAxis axis, double theta)
        {
            var num = Math.Sin(theta);
            var num2 = Math.Cos(theta);
            rot = new double[9];
            switch (axis)
            {
                case RotAxis.RotX:
                    rot[0] = 1.0;
                    rot[4] = num2;
                    rot[5] = -num;
                    rot[7] = num;
                    rot[8] = num2;
                    break;

                case RotAxis.RotY:
                    rot[0] = num2;
                    rot[2] = num;
                    rot[4] = 1.0;
                    rot[6] = -num;
                    rot[8] = num2;
                    break;

                case RotAxis.RotZ:
                    rot[0] = num2;
                    rot[1] = -num;
                    rot[3] = num;
                    rot[4] = num2;
                    rot[8] = 1.0;
                    break;
            }
        }

        public Rotation3x3 rmult(Rotation3x3 b)
        {
            var rotationx = new Rotation3x3();
            for (var i = 0; i < 3; i++)
            {
                for (var j = 0; j < 3; j++)
                {
                    var index = (3 * i) + j;
                    rotationx.rot[index] = 0.0;
                    for (var k = 0; k < 3; k++)
                    {
                        rotationx.rot[index] += rot[(3 * i) + k] * b.rot[(3 * k) + j];
                    }
                }
            }
            return rotationx;
        }

        public Rotation3x3 Scale(double factor)
        {
            var rotationx = new Rotation3x3();
            for (var i = 0; i < 9; i++)
            {
                rotationx.rot[i] = rot[i] * factor;
            }
            return rotationx;
        }

        public Rotation3x3 transp()
        {
            var rotationx = new Rotation3x3();
            for (var i = 0; i < 3; i++)
            {
                for (var j = 0; j < 3; j++)
                {
                    rotationx.rot[(3 * j) + i] = rot[(3 * i) + j];
                }
            }
            return rotationx;
        }

        public enum RotAxis
        {
            RotX,
            RotY,
            RotZ
        }
    }
}

