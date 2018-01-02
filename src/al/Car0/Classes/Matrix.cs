using System;
using System.Collections.Generic;
using System.Collections;

using System.Text;
using System.Windows.Forms;

namespace Car0
{
    public class Matrix
    {
        #region Public Variables
        public event NotifyMessageEventHandler NotifyMessage;
        public void raiseNotify(string title, string message)
        {
            if (NotifyMessage != null)
                NotifyMessage(this,new NotifyMessageEventArgs(){Message = message, Title = title});
        }
        public int Rows
        {
            get { return _rows; }
            set { _rows = value; }
        }

        public int Cols
        {
            get { return this._cols; }
            set { this._cols = value; }
        }

        public List<double> Value
        {
            get { return _value; }
            set { _value = value; }
        }

        #endregion
        #region Private Variables
         private int _rows = 0;
         private int _cols = 0;
         private List<double> _value = new List<double>();
        #endregion

        #region Public Methods

        public Matrix(int NumRows, int NumCols)
        {
            this.Rows = NumRows;
            this.Cols = NumCols;
            _value = new List<double>(Rows * Cols);

            for (int i = 0; i < Rows * Cols; ++i)
                _value.Add(0.0);
        }

        public Matrix(Matrix CloneMe)
        {
            this.Rows = CloneMe.Rows;
            this.Cols = CloneMe.Cols;
            this._value = new List<double>(Rows * Cols);

            for (int i = 0; i < Rows * Cols; ++i)
                _value.Add(CloneMe._value[i]);
        }
        public Matrix(Vector3 v)
        {
            Rows = 3;
            Cols = 1;
            _value = new List<double>(3);
            _value.Add(v.X);
            _value.Add(v.Y);
            _value.Add(v.Z);
        }


        public void assign(int AtRow, int AtCol, double x)
        {
            _value[AtRow * Cols + AtCol] = x;
        }


        public double getvalue(int FromRow, int FromCol)
        {
            return _value[FromRow * Cols + FromCol];
        }

        public Matrix madd(Matrix b)
        {
            Matrix matrix = new Matrix(Rows, Cols);

            for (int i = 0; i < Rows * Cols; ++i)
                matrix.Value[i] = Value[i] + b.Value[i];

            return matrix;
        }

        public Matrix msub(Matrix b)
        {
            Matrix matrix = new Matrix(Rows, Cols);

            for (int i = 0; i < Rows * Cols; ++i)
                matrix.Value[i] = Value[i] - b.Value[i];

            return matrix;
        }

        public Matrix trans()
        {
            Matrix matrix = new Matrix(Cols, Rows);
            int i,j;

            for (i = 0; i < Rows; ++i)
            {
                for (j = 0; j < Cols; ++j)
                    matrix.Value[j * matrix.Cols + i] = Value[i * Cols + j];
            }

            return matrix;
        }

        public void equate(Matrix CloneMe)
        {
            this.Rows = CloneMe.Rows;
            this.Cols = CloneMe.Cols;
            this._value = new List<double>(this.Rows * this.Cols);

            for (int i = 0; i < Rows * Cols; ++i)
                this._value.Add(CloneMe.Value[i]);
        }
        public void equate(Vector3 vector)
        {
            if (!(Rows.Equals(3) && Cols.Equals(1)))
            {
                raiseNotify("Vector3 equate matrix not 3x1", "equate");
                return;
            }

            assign(0, 0, vector.X);
            assign(1, 0, vector.Y);
            assign(2, 0, vector.Z);
        }

        public void scale(double factor)
        {

            for (int i = 0; i < Rows * Cols; ++i)
                _value[i] *= factor;
        }

        public Matrix mmult(Matrix matrix)
        {
            int  k, indx;
            Matrix c = new Matrix(Rows, matrix.Cols);

            //Sanity check
            if (Cols.Equals(matrix.Rows))
            {
                for (int i = 0; i < c.Rows; ++i)
                {
                    for (int j = 0; j < c.Cols; ++j)
                    {
                        indx = i * c.Cols + j;
                        c.Value[indx] = 0.0;        //Just cuz it was that way in lidmat

                        for (k = 0; k < Cols; ++k)
                            c.Value[indx] += (this._value[i * Cols + k] * matrix.Value[k * matrix.Cols + j]);
                    }
                }
            }
            else
            {
                raiseNotify("Matrix dimension error", "mmult");
            }

            return c;
        }

        public  Matrix mident(int dimension)
        {
            Matrix matrix = new Matrix(dimension, dimension);
          
            for (int i = 0; i < dimension; ++i)
            {
                for (int j = 0; j < dimension; ++j)
                {
                    if (i.Equals(j))
                        matrix.assign(i, j, 1.0);
                }
            }

            return matrix;
        }

        public Matrix inverse()
        {
            if (!Rows.Equals(Cols))
            {
                raiseNotify("Matrix not square", "inverse");
                return null;
            }

            Matrix imatrix = mident(Rows);

            return MatrixSolver.msolve(this, imatrix);
        }

        public Matrix CrossProduct(Matrix matrix)
        {
            if (!(this.Rows.Equals(3) && this.Cols.Equals(1) && matrix.Rows.Equals(3) && matrix.Cols.Equals(1)))
            {
                raiseNotify("Matrix dimensions wrong", "CrossProduct");
                
                return null;
            }

            double ax = getvalue(0, 0);
            double ay = getvalue(1, 0);
            double az = getvalue(2, 0);
            double bx = matrix.getvalue(0, 0);
            double by = matrix.getvalue(1, 0);
            double bz = matrix.getvalue(2, 0);
            Matrix result = new Matrix(Rows, Cols);

            result.assign(0, 0, ay * bz - az * by);
            result.assign(1, 0, az * bx - ax * bz);
            result.assign(2, 0, ax * by - ay * bx);

            return result;
        }

        public double magof()
        {
            double sum_of_squares = 0.0;

            for (int i = 0; i < Rows * Cols; ++i)
                sum_of_squares += Math.Pow(this._value[i], 2);

            return Math.Sqrt(sum_of_squares);
        }

        public void Normalize()
        {
            double m = magof();

            if (m < 1e-10)
                raiseNotify("Matrix too small", "Normalize");
            else
                scale(1 / m);
        }

        public void Normalize(double Scaler)
        {
            double m = magof();

            if (m < 1e-10)
                raiseNotify("Matrix too small", "Normalize");
            else
                scale(Scaler / m);
        }


        public double DotProduct(Matrix b)
        {
            if (!(this.Rows.Equals(3) && this.Cols.Equals(1) && b.Rows.Equals(3) && b.Cols.Equals(1)))
            {
                raiseNotify("Matrix dimensions wrong", "DotProduct");
                return 0.0;
            }

            double x = 0.0;

            for (int i = 0; i < 3; ++i)
                x += getvalue(i, 0) * b.getvalue(i, 0);

            return x;
        }

        #endregion
        #region Private Methods

        #endregion
    }
}
