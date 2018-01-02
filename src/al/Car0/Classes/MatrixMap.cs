using System;
using System.Collections.Generic;

using System.Text;

namespace Car0
{
    class MatrixMap
    {
        #region Public Variables
        public int dim;
        public List<int> mvalue;
        #endregion
        #region Public Methods
        public MatrixMap()
        {
            dim = 0;
            mvalue = new List<int>();
        }
        public MatrixMap(int n)     //Replaces gen_map(n, map)
        {
            int i;
            dim = n;
            mvalue = new List<int>(n);

            for (i = 0; i < dim; ++i)
                mvalue.Add(i);
        }
        #endregion
    }
}
