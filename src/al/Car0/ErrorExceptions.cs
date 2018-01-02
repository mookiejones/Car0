using System;
using System.Collections.Generic;
using System.Text;

namespace Car0
{
    public class DefinerWriteException:Exception
    {
        public DefinerWriteException(string message)
            : base(message)
        {
        }
    }

    public class ExcelMatrixException : Exception  { public ExcelMatrixException(string message) : base(message) { }}
    public class VectorException : Exception { public VectorException(string message) : base(message) { } }
}
