using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.IO;
using System.Windows.Forms;


namespace Car0
{
   public class FrameRoutines
    {

        public event NotifyMessageEventHandler NotifyMessage;
        private void raiseNotify(string message, string title)
        {
            if (NotifyMessage!=null)
                NotifyMessage(this,new NotifyMessageEventArgs(){Message = message,Title = title};
        }

    #region Constants
        public const int X_Coord = 0;
        public const int Y_Coord = 1;
        public const int Z_Coord = 2;
        public const int A_Coord = 3;
        public const int B_Coord = 4;
        public const int C_Coord = 5;
        public const double CoincidenceLimit = 100.0;
        public const double MinATAN2Arg = 0.001;
    #endregion

        #region Private Methods
        /* get_dp(rx,tx,A,numpts,dp,t3x1)
         * 
         * Finds the change in the position of the origin.
         *
         *      Inputs:         rx      Array of points in the reference CS.
         *                      tx      Array of points in initial CS.
         *                      A       Transform from initial to reference CS.
         *
         *      Returns:        dp      Point vector representing the change.
         *
         *      Destroys:       t3x1
         */
        private  Vector get_dp(List<Vector> rx, List<Vector> tx, Transformation A, ref Vector t3x1)
        {
            int i;
            Vector rxi = new Vector(3);
            Vector dp = new Vector(3);

            for (i = 0; i < rx.Count; ++i)
            {
                t3x1 = t3x1.ListRowEquate(tx, i);

                cs_change(ref t3x1, A);

                rxi = rxi.ListRowEquate(rx, i);
                //Vector.VrEquate(rx, ref rxi, i, 3);

                t3x1 = rxi.Sub(t3x1);
                dp = t3x1.Add(dp);
            }

            dp = dp.Scale(1 / Convert.ToDouble(rx.Count));

            return dp;
        }
        #endregion

        public  int ComputeFrameFrom321Pts(double[] Origin, double[] Xplus, double[] Yplus, ref double[] wAcar0)
        {
            int i;
            double Length = 0.0;
            double[] Delta = new double[3];
            double[] NVector = new double[3];
            double[] SVector = new double[3];
            double[] AVector = new double[3];

            wAcar0[X_Coord] = Origin[X_Coord];
            wAcar0[Y_Coord] = Origin[Y_Coord];
            wAcar0[Z_Coord] = Origin[Z_Coord];

            //Compute positive X direction
            Delta[X_Coord] = Xplus[X_Coord] - Origin[X_Coord];
            Delta[Y_Coord] = Xplus[Y_Coord] - Origin[Y_Coord];
            Delta[Z_Coord] = Xplus[Z_Coord] - Origin[Z_Coord];

            //Calculate length for normalization
            for (i = 0; i < 3; ++i)
                Length += Delta[i] * Delta[i];

            Length = Math.Sqrt(Length);

            if (Length < CoincidenceLimit)
                return 1;

           //Normalize
            for (i = 0; i < 3; ++i)
                NVector[i] = Delta[i] / Length;

            //Compute the positive Y direction
            Delta[X_Coord] = Yplus[X_Coord] - Origin[X_Coord];
            Delta[Y_Coord] = Yplus[Y_Coord] - Origin[Y_Coord];
            Delta[Z_Coord] = Yplus[Z_Coord] - Origin[Z_Coord];

            Length = 0.0;
            //Calculate length for normalization
            for (i = 0; i < 3; ++i)
                Length += Delta[i] * Delta[i];

            Length = Math.Sqrt(Length);

            if (Length < CoincidenceLimit)
                return 1;

            //Compute the length of PoriginPyplus that lies in the direction of originPxplus
            Length = NVector[X_Coord] * Delta[X_Coord] + NVector[Y_Coord] * Delta[Y_Coord] + NVector[Z_Coord] * Delta[Z_Coord];

            //Subtract the component of PoriginPyplus that lies in the direction of originPxplus
            for (i = 0; i < 3; ++i)
                Delta[i] -= NVector[i] * Length;

            Length = 0.0;
            //Calculate length for normalization
            for (i = 0; i < 3; ++i)
                Length += Delta[i] * Delta[i];

            Length = Math.Sqrt(Length);

            if (Length < CoincidenceLimit)
                return 2;

            //Normalize
            for (i = 0; i < 3; ++i)
                SVector[i] = Delta[i] / Length;

            //Approach vector is computed by cross product.
            AVector[X_Coord] = NVector[Y_Coord]*SVector[Z_Coord] - NVector[Z_Coord]*SVector[Y_Coord];
            AVector[Y_Coord] = NVector[Z_Coord]*SVector[X_Coord] - NVector[X_Coord]*SVector[Z_Coord];
            AVector[Z_Coord] = NVector[X_Coord] * SVector[X_Coord] - NVector[Y_Coord] * SVector[X_Coord];

            //Now compute the a, b, and c angles corresponding to N, S, and A

            return ComputeABCFromNSA(NVector, SVector, AVector, ref wAcar0);
        }

        /* ComputeABCFromNSA(NVector, SVector, AVector, ref ResultFrame)
         * 
         * Computes the B and C angles of a frame from the A vector.  Works even if A is not a unit vector.
         * 
         * Inputs:      NVector     Normal vector.
         *              SVector     Orientation vector.
         *              AVector     Attack vector.
         *              
         * Outputs:     ResultFrame A, B, and C angle elements are set if successful.
         * 
         * Returns:     0           Success
         *              1           Singularity warning; cannot determine B, so we force it to
         *                          zero.  C is either +90 or -90.
         *              2           AVector is too short; cannot determine angles.
         */
        public  int ComputeABCFromNSA(double[] NVector, double[] SVector, double[] AVector, ref double[] ResultFrame)
        {
            double CosC, SinC;

            if ((Math.Abs(AVector[X_Coord]) < MinATAN2Arg) && (Math.Abs(AVector[Z_Coord]) < MinATAN2Arg))
            {
                ResultFrame[A_Coord] = 0.0;

                if (NVector[Z_Coord] < 0.0)
                {
                    ResultFrame[B_Coord] = 90.0;
                    ResultFrame[C_Coord] = Utils.RadToDegrees(Math.Atan2(SVector[X_Coord], AVector[X_Coord]));
                }
                else
                {
                    ResultFrame[B_Coord] = -90.0;
                    ResultFrame[C_Coord] = Utils.RadToDegrees(Math.Atan2(-SVector[X_Coord], -AVector[X_Coord]));
                }

                return 1;
            }

            ResultFrame[A_Coord] = Utils.RadToDegrees(Math.Atan2(NVector[Y_Coord], NVector[X_Coord]));

            if ((Math.Abs(SVector[Z_Coord]) < MinATAN2Arg) && (Math.Abs(AVector[Z_Coord]) < MinATAN2Arg))
            {
                //This case really shouldn't happen if the preceding test didn't catch it
                ResultFrame[A_Coord] = 0.0;

                if (NVector[Z_Coord] < 0.0)
                {
                      ResultFrame[B_Coord] = 90.0;
                      ResultFrame[C_Coord] = Utils.RadToDegrees(Math.Atan2(SVector[X_Coord], AVector[X_Coord]));
                }
                else
                {
                      ResultFrame[B_Coord] = -90.0;
                      ResultFrame[C_Coord] = Utils.RadToDegrees(Math.Atan2(-SVector[X_Coord], -AVector[X_Coord]));
                }

                return 1;
               }

            ResultFrame[C_Coord] = Utils.RadToDegrees(Math.Atan2(SVector[Z_Coord], AVector[Z_Coord]));

            CosC = Math.Cos(ResultFrame[C_Coord]);
            SinC = Math.Sin(ResultFrame[C_Coord]);
            ResultFrame[B_Coord] = Utils.RadToDegrees(Math.Atan2(-NVector[Z_Coord], SinC * SVector[Z_Coord] + CosC * AVector[Z_Coord]));

            return 0;  //Quaplah
        }


        public  void WriteDefiner(string RobotRootPath, string FrameName, double[] FrameDef, ref int LinesWritten, Boolean WriteFrame, Boolean WriteEndStatement, Brand Kind, int ind, ref ArrayList Problems)
        {
            switch (Kind.Company)
            {
                case Brand.RobotBrands.ABB:

                    ABB.WriteDefiner(RobotRootPath, FrameDef, ref LinesWritten, WriteFrame, Kind, ind, ref Problems);
                    break;

                case Brand.RobotBrands.Nachi:

                    Nachi.WriteDefiner(RobotRootPath, FrameName, FrameDef, ref LinesWritten, WriteFrame, ref Problems);
                    break;

                case Brand.RobotBrands.Fanuc:

                    Fanuc.WriteDefiner(RobotRootPath, FrameName, FrameDef, ref LinesWritten, WriteFrame, false, ref Problems);
                    break;

                case Brand.RobotBrands.FanucRJ:

                    Fanuc.WriteDefiner(RobotRootPath, FrameName, FrameDef, ref LinesWritten, WriteFrame, true, ref Problems);
                    break;

                case Brand.RobotBrands.KUKA:

                    Kuka.WriteDefiner(RobotRootPath, FrameName, FrameDef, ref LinesWritten, WriteFrame, WriteEndStatement, ref Problems);
                    break;

                default:

                    Problems.Add("Robot brand not supported detected in: WriteDefiner");
                    break;
            }
        }

        public  Boolean ReadDefinerToolData(string RobotRootPath, int ToolNum, ref double[] FrameDef, Brand.RobotBrands Kind)
        {
            Boolean found = false;

            switch (Kind)
            {
                case Brand.RobotBrands.KUKA:

                    found = Kuka.ReadDefinerToolData(RobotRootPath, ToolNum, ref FrameDef);
                    break;

                case Brand.RobotBrands.Fanuc:
                case Brand.RobotBrands.FanucRJ:

                    found = Fanuc.ReadDefinerToolData(RobotRootPath, ToolNum, ref FrameDef);
                    break;

                case Brand.RobotBrands.Nachi:
                case Brand.RobotBrands.ABB:         //Handled differently

                    found = true;
                    break;

                default:

                    raiseNotify("Robot brand not supported.", "ReadDefinerToolData");
                    break;
            }

            return found;
        }
        public  Boolean ReadDefinerBaseData(string RobotRootPath, int ToolNum, ref double[] FrameDef, Brand.RobotBrands Kind)
        {
            Boolean found = false;

            switch (Kind)
            {
                case Brand.RobotBrands.KUKA:


                    found = Kuka.ReadDefinerBaseData(RobotRootPath, ToolNum, ref FrameDef);
                    break;

                default:

                    raiseNotify("Robot brand not supported.", "ReadDefinerBaseData");
                    break;
            }

            return found;
        }
        public  Boolean ReadDefinerPedData(string RobotRootPath, int ToolNum, ref double[] FrameDef, Brand.RobotBrands Kind)
        {
            Boolean found = false;

            //Note: This will go between reading tool (Nachi) and base data dependant on brand
            switch (Kind)
            {
                case Brand.RobotBrands.KUKA:


                    found = Kuka.ReadDefinerBaseData(RobotRootPath, ToolNum, ref FrameDef);
                    break;

                case Brand.RobotBrands.ABB:
                case Brand.RobotBrands.Nachi:

                    found = true;  //Don't complain
                    break;

                default:

                    raiseNotify("Robot brand not supported.", "ReadDefinerPedData");
                    break;
            }

            return found;
        }

        public  void rfgp(List<Vector> tdpts, ref Vector N)
        {
            Vector tdp2 = new Vector(3);
            Vector tdp1 = new Vector(3);
            Vector tdp0 = new Vector(3);
            int i;

            for (i = 0; i < 3; ++i)
            {
                tdp2.Vec[i] = tdpts[2].Vec[i];
                tdp1.Vec[i] = tdpts[1].Vec[i];
                tdp0.Vec[i] = tdpts[0].Vec[i];
            }

            Vector cnd1 = tdp1.Sub(tdp0);
            Vector cnd2 = tdp2.Sub(tdp1);

            N = cnd1.CrossProduct(cnd2).normalize();
        }

        public  Transformation get_model(List<Vector> tdpts, ref Vector t3x1)
        {
            Vector r0err = new Vector(3);
            Vector t0 = new Vector(3);
            Vector t1 = new Vector(3);
            int i;
            Transformation cAp = new Transformation(Transformation.SpecialTransformType.Identity);

            rfgp(tdpts, ref r0err);

            for (i = 0; i < 3; ++i)
            {
                t0.Vec[i] = tdpts[0].Vec[i];
                t1.Vec[i] = tdpts[1].Vec[i];
            }

            //Assign first point as origin of Cap
            cAp.point_trans(t0);

            //Assign normal to plane as angle of attack
            cAp.attack_trans(r0err);

            //Calculate the normal vector in plane
            t3x1 = t1.Sub(t0).normalize();

            //Assign normal vector to cAp
            cAp.norm_trans(t3x1);

            //Calculate Y axis direction.
            t3x1 = r0err.CrossProduct(t3x1).normalize();

            //Assign orientation vector to cAp
            cAp.orient_trans(t3x1);

            return cAp;
        }

        /* get_bac(tdata,t3x1,Cap,Bac,bap)
         * 
         * Calculates transformation from R0 to point (bap), and then
         * uses this information along with transfromation from model
         * to point to calculate the transformation from R0 to the model.
         */
        public  void get_bac(List<Vector> tdata, ref Vector t3x1, Transformation Cap, ref Transformation Bac, ref Transformation bap)
        {
            Vector r0err = new Vector(3);
            Vector t0 = new Vector(3);
            Vector t1 = new Vector(3);
            int i;

            for (i = 0; i < 3; ++i)
            {
                t0.Vec[i] = tdata[0].Vec[i];
                t1.Vec[i] = tdata[1].Vec[i];
            }

            rfgp(tdata, ref r0err);

            //Assign first point as origin of bap
            bap.point_trans(t0);

            //Assign normal to plane as angle of attack
            bap.attack_trans(r0err);

            //Calculate the normal vector in plane
            t3x1 = t1.Sub(t0).normalize();

            //Assign normal vector to bap
            bap.norm_trans(t3x1);

            //Calculate Y axis direction
            t3x1 = r0err.CrossProduct(t3x1).normalize();

            //Assign orientation vector to bap
            bap.orient_trans(t3x1);

            //Get the inverse
            Bac = Cap.inv_trans();

            //Transformation from R0 to model calculated
            Bac = bap.mult_trans(Bac);
        }

        /* cs_change(point, xform)
         * 
         *      Changes the coordinates of a (3x1) matrix point representing
         *      a location in the R0 coordinate system into the coordinate
         *      system thats location with respect to R0 is defined by
         *      the homogeneous transformation xform.
         *
         *  Inputs:     point   Pointer to a (3x1) matrix containing a
         *                      location in coordinate system R0.
         *              xform   Pointer to a homogeneous transformation
         *                      that goes from R0 to the coordinate
         *                      system of interest.
         *
         *  Outputs:    point   Elements of this matrix are changed to
         *                      the coordinates of the point in the new
         *                      coordinate system.
         */
        public  void cs_change(ref Vector point, Transformation xform)
        {
            Transformation cscwrk = new Transformation(Transformation.SpecialTransformType.Identity);   //Set Rotational part of transformation cscwrk to identity

            //Set Displacement Part of cscwrk to point
            cscwrk.point_trans(point);

            //Transform into coordinate system of interest
            cscwrk = xform.mult_trans(cscwrk);

            //Get the point vector
            point = cscwrk.trans_point();
        }
        public  void cs_change(ref Vector3 point, Transformation xform)
        {
            Vector vpoint = new Vector(point);
            cs_change(ref vpoint, xform);
            point = new Vector3(vpoint);
        }
        

        /* get_dk(rx,tx,A,numpts,dk,sm)
         *
         *                              Finds the axis and angle of rotation.
         *
         *      Inputs:         rx      Array of points in the reference CS.
         *                      tx      Array of points in initial CS.
         *                      A       Transform from initial to reference CS.
         *
         *      Outputs:        dk      Point vector representing the change.
         *                              Returns the angle of rotation.
         *
         *      Destroys:       t3x1
         *                      cap
         */
        public  Vector get_dk(List<Vector> rx, List<Vector> tx, ref Vector t3x1, Transformation A, ref double sm)
        {
            int i;
            Vector rxi = new Vector(3);
            Vector dk = new Vector(3);
            sm = 0.0;

            for (i = 0; i < rx.Count; ++i)
            {
                t3x1 = t3x1.ListRowEquate(tx,i);
                cs_change(ref t3x1, A);

                rxi = rxi.ListRowEquate(rx, i);

                sm += rxi.magof() * t3x1.magof();

                t3x1 = rxi.CrossProduct(t3x1);
                dk = t3x1.Add(dk);
            }

           t3x1 = dk.Scale(1 / sm);

            sm = Math.Asin(t3x1.magof());

            if (sm > 1e-6)
                dk = dk.normalize();

            return dk;
        }

        /* okyet(rx,tx,t3x1,A,numpts,worst,Ew,se)
         *
         *              Finds the average error between predicted and measured reference
         *              points.
         *
         *      Inputs:         rx      Array of points in the reference CS.
         *                      tx      Array of points in initial CS.
         *                      A       Transform from initial to reference CS.
         *
         *      Outputs:        worst   Set equal to number of worst point in file
         *                      Ew      Error at worst point in the file.
         *                      se      The average error.
         *
         *      Destroys:       t3x1
         */
        public  void okyet(List<Vector> rx, List<Vector> tx, ref Vector t3x1, Transformation A, ref int worst, ref double Ew, ref double se)
        {
            int i;
            double er = 0.0;
            Vector rxi = new Vector(3);

            Ew = se = 0.0;

            for (i = 0; i < rx.Count; ++i)
            {
                t3x1 = t3x1.ListRowEquate(tx,i);
                cs_change(ref t3x1, A);

                rxi = rxi.ListRowEquate(rx, i);
                t3x1 = rxi.Sub(t3x1);

                er = t3x1.magof();
                se += er;

                if (er >= Ew)
                {
                    Ew = er;
                    worst = i + 1;
                }
            }

            se /= Convert.ToDouble(rx.Count);
        }


        /* where(rfpts,tdpts,NumPts,rA0)
         */
        public  Boolean where(List<Vector> rfpts, List<Vector> tdpts, ref Transformation rA0, ref double AveError, ref double Ew, ref int worst, ref ArrayList Problems)
        {
            int j = 0, tries = 0;
            Boolean done = false;
            double theta = 0.0, lastime = 999999.0;
            Vector t3x1 = new Vector(3);
            Vector r0err = new Vector(3);
            Transformation bAp = new Transformation();
            double[] rpy = new double[6];

            Ew = AveError = 0.0;

            worst = 0;

            //Obtain initial estimation of rA0
            Transformation cAp = get_model(rfpts, ref t3x1);

            get_bac(tdpts, ref t3x1, cAp, ref rA0, ref bAp);
            rA0 = rA0.inv_trans();

            //Iterate until the error is acceptable
            do
            {
                //Save last transform produced
                bAp.tequate(rA0);
                ++j;
                
                r0err = get_dp(rfpts, tdpts, rA0, ref t3x1);

                //Create displacement of &r0err amount
                cAp = new Transformation(Transformation.SpecialTransformType.Identity);
                cAp.point_trans(r0err);

                //Update rAm by displacement
                rA0 = cAp.mult_trans(rA0);

                //Find axis of rotation, &r0err, and angle of rotation, theta
                r0err = get_dk(rfpts, tdpts, ref t3x1, rA0, ref theta);

                cAp = new Transformation(r0err, -theta);

                //Force general rotation to be orthonormal set
                t3x1 = cAp.trans_norm();
                r0err = cAp.trans_orient();
                t3x1 = t3x1.normalize();
                r0err = r0err.normalize();
                cAp.norm_trans(t3x1);
                cAp.orient_trans(r0err);
                r0err = t3x1.CrossProduct(r0err).normalize();
                cAp.attack_trans(r0err);

                //Update rA0 by the rotation
                rA0 = cAp.mult_trans(rA0);

                //Check how well the transformation worked
                Ew = 0.0;

                okyet(rfpts, tdpts, ref t3x1, rA0, ref worst, ref Ew, ref AveError);

                //Check if we are done
                done = (Math.Abs(lastime - AveError) < 1e-6);

                if (done || (Math.Abs(AveError) > Math.Abs(lastime)))
                {
                    //Did things get worse, if so restore the previous value
                    if (!done)
                    {
                        rA0.tequate(bAp);
                        AveError = lastime;
                    }

                    done = true;
                }

                lastime = AveError;

            } while (!done || (++tries > 100));

            if (tries > 100)
            {
                Problems.Add("Transformation fit failed to converge");
                return false;
            }

            return true;
        }

        public  Transformation j4Atf(double[] WristPars, double[] jas)
        {
            Transformation j4Aj5 = new Transformation(WristPars[0], WristPars[1], WristPars[2], WristPars[3], jas[0]);
            Transformation j5Aj6 = new Transformation(WristPars[4], WristPars[5], WristPars[6], WristPars[7], jas[1]);
            Transformation j6Atf = new Transformation(WristPars[8], WristPars[9], WristPars[10], WristPars[11], jas[2]);
            Transformation j4Aj6 = j4Aj5.mult_trans(j5Aj6);
            return j4Aj6.mult_trans(j6Atf);
        }
        public  Transformation j4Atf(double[] WristPars, Vector3 vjas)
        {
            double[] jas = new double[3] { vjas.x, vjas.y, vjas.z };

            return j4Atf(WristPars, jas);
        }
        public  Transformation j4Atf(double[] WristPars, Vector3 vjas,bool dummy)
        {
            double[] jas = new double[3] { Utils.DegreesToRad(vjas.x), Utils.DegreesToRad(vjas.y), Utils.DegreesToRad(vjas.z) };

            return j4Atf(WristPars, jas);
        }

        public  List<Vector3> PointSetCsChange(Transformation toAfrom, List<Vector3> fromVecSet, Boolean SkipZeroVectors)
        {
            List<Vector3> toVecSet = new List<Vector3>();
            int i;
            Vector3 temp;

            for (i = 0; i < fromVecSet.Count; ++i)
            {
                if (SkipZeroVectors && fromVecSet[i].x.Equals(0.0) && fromVecSet[i].y.Equals(0.0) && fromVecSet[i].z.Equals(0.0))
                    temp = new Vector3();
                else
                {
                    temp = new Vector3(fromVecSet[i]);
                    cs_change(ref temp, toAfrom);
                }

                toVecSet.Add(temp);
            }

            return toVecSet;
        }
        public  List<Vector3> PointSetCsChange(Transformation toAfrom, List<Vector3> fromVecSet)
        {
            return PointSetCsChange(toAfrom, fromVecSet, false);
        }


        public  List<Vector3> PointSetJointTransform(List<Vector3> j4Measurements, List<Vector3> jaVecs, double[] WristPars, Boolean SkipZeroVectors)
        {
            List<Vector3> tfVecSet = new List<Vector3>();
            int i;
            Transformation tfAj4;
            Vector3 temp;

            for (i = 0; i < jaVecs.Count; ++i)
            {
                if (SkipZeroVectors && j4Measurements[i].x.Equals(0.0) && j4Measurements[i].y.Equals(0.0) && j4Measurements[i].z.Equals(0.0))
                    temp = new Vector3();
                else
                {
                    tfAj4 = j4Atf(WristPars, jaVecs[i], true).inv_trans();
                    temp = new Vector3(j4Measurements[i]);
                    cs_change(ref temp, tfAj4);
                }

                tfVecSet.Add(temp);
            }

            return tfVecSet;
        }
        public  List<Vector3> PointSetJointTransform(List<Vector3> j4Measurements, List<Vector3> jaVecs, double[] WristPars)
        {
            return PointSetJointTransform(j4Measurements, jaVecs, WristPars, false);
        }
    }
}
