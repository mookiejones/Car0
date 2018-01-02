namespace CarZero
{
    using System;
    using System.Collections.Generic;

    internal class MatrixMap
    {
        public int dim;
        public List<int> mvalue;

        public MatrixMap()
        {
            dim = 0;
            mvalue = new List<int>();
        }

        public MatrixMap(int n)
        {
            dim = n;
            mvalue = new List<int>(n);
            for (var i = 0; i < dim; i++)
            {
                mvalue.Add(i);
            }
        }
    }
}

