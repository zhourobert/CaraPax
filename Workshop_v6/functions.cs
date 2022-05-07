using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;
using Grasshopper.Kernel;

namespace Car_Functions
{
    static class calculate
    {
        ///<summary>
        ///输入一个域然后得到他分成多份的数值的等差数列
        ///<summary>

        public static List<double> Arithmetic_list(Interval domin, int interval_num)
        {
            List<double> alist = new List<double>();
            for (int i = 0; i < interval_num; i++)
            {
                alist.Add(domin.Min + domin.Length / interval_num * i);
            }
            alist.Add(domin.Max);
            return alist;
        }

        public static int approximate_divisible(Interval interval,double step_lenth)
        {
            GH_Convert.ToDouble(interval,out double D_interval,GH_Conversion.Both);
            int num =(int)Math.Round(D_interval / step_lenth);
            return num;
        }


    }

    static class geometry_base
    {
        ///<summary>
        ///将目标平面法线方向转到一个指定方向，并将原点换到一个指定的点上（仅仅支持2D旋转）
        ///<summary>

        //莫名其妙不可以，想了10来分钟想不明白算了
        public static bool Adjust_plane(ref Plane P, Vector3d V, Point3d Pt)
        {
            double angle = Vector3d.VectorAngle(V, P.Normal);
            if (!P.Rotate(angle, P.ZAxis, P.Origin))
            {
                return false;
            }
            else
            {
                P.Origin = Pt;
                return true;
            }


        }

        public static bool move(GeometryBase geometry, Vector3d vector)
        {
            Transform T = Transform.Translation(vector);
            bool B= geometry.Transform(T);
            return B;
        }

        public static Point3d move(Point3d geometry, Vector3d vector)
        {
            //Transform T = Transform.Translation(vector);
            //geometry.Transform(T);
            Vector3d V = -vector;
            Point3d P = Point3d.Subtract(geometry, V);
            return P;
        }

        public static void move(Rectangle3d geometry, Vector3d vector)
        {
            Transform T = Transform.Translation(vector);
            geometry.Transform(T);

        }

        public static void move(Plane geometry, Vector3d vector)
        {
            Transform T = Transform.Translation(vector);
            geometry.Transform(T);

        }

        public static List<Curve> IsoUVCurves(Surface S, int IntervalNum, int dir)
        {
            List<double> interval_list = new List<double>();

            List<Curve> C_list = new List<Curve>();

            //Interval Dom1 = S.Domain(dir);
            Interval Dom2 = S.Domain(Math.Abs(dir - 1));//得到另一个方向的域值

            interval_list = calculate.Arithmetic_list(Dom2, IntervalNum);

            for (int i = 0; i < IntervalNum+1; i++)
            {
                Curve C = S.IsoCurve(dir, interval_list[i]);
                C_list.Add(C);
            }

            return C_list;
        }

        public static Point3d Curve_middle(Curve C)
        {
            C.LengthParameter(C.GetLength() / 2, out double t);
            Point3d P = C.PointAt(t);
            return P;
        }

        public static Plane PointOnSurface(Point3d Pt,Surface S,out Vector3d normal)
        {
            
            S.ClosestPoint(Pt, out double u, out double v);
            normal = S.NormalAt(u, v);
            S.FrameAt(u, v, out Plane P);
            return P;
        }

        public static Curve Center_rec(Plane plane,double height,double width)
        {
            Curve C_rec = null;
            Rectangle3d rec = new Rectangle3d(plane, width, height);
            Vector3d Transfrom = Point3d.Subtract(plane.Origin, rec.Center);
            GH_Convert.ToCurve(rec, ref C_rec, GH_Conversion.Both);
            move(C_rec, Transfrom);
            return C_rec;
        }

        public static Curve OffsetFromSurface(Surface surface,Curve curve,int accuracy,double distance)
        {
            List<Point3d> p = new List<Point3d>();
            curve.DivideByCount(accuracy, true,out Point3d[] points);
            for (int i = 0; i < points.Length; i++)
            {
                PointOnSurface(points[i], surface, out Vector3d normal);
                Vector3d n = normal * distance;

                p.Add( move(points[i], n));
            }
            Curve C = Curve.CreateInterpolatedCurve(p, curve.Degree);
            return C;
        }

        public static Curve OffsetFromSurface(Surface surface, Curve curve, double distance)
        {
            Point3d Pt_mid = Curve_middle(curve);
            PointOnSurface(Pt_mid, surface, out Vector3d normal);
            normal = distance * normal;
            Curve C = curve.DuplicateCurve();
            move(C, normal);
            return C;
        }

        public static Brep Simple_loft(Curve curve0,Curve curve1)
        {
            Curve curve3 = null;
            Curve curve4 = null;

            Line L_start = new Line(curve0.PointAtStart, curve1.PointAtStart);
            GH_Convert.ToCurve(L_start, ref curve3, GH_Conversion.Both);

            Line L_end = new Line(curve0.PointAtEnd, curve1.PointAtEnd);
            GH_Convert.ToCurve(L_end, ref curve4, GH_Conversion.Both);

            Curve[] C = new Curve[4] { curve0, curve3, curve1, curve4 };
            Brep B = Brep.CreateEdgeSurface(C);
            return B;
        }
    }

    static class Tubes
    {
        public static Brep Square_Tube(Curve C,Plane P,double H,double W,out int Type)
        {
            
            if (C.IsPlanar(0.01) && !C.IsClosed)//平面不闭合曲线
            {
                
                Brep B = RectangularPath_P_nC(C, H, W);

                Type = 1;
                return B;
                

            }
            else if (C.IsPlanar(0.01) && C.IsClosed)//平面闭合曲线
            {
                Brep B = RectangularPath_P_C(C, H, W);

                Type = 2;
                return B;
            }
            else if (!C.IsPlanar(0.01) && C.IsClosed)//非平面闭合曲线
            {
                Brep B = RectangularPath_nP_C(C, P, H, W);

                Type = 3;
                return B;
            }
            else if (!C.IsPlanar(0.01) && !C.IsClosed)//非平面非闭合曲线
            {
                Brep B = RectangularPath_nP_nC(C, P, H, W);

                Type = 4;
                return B;
            }
            else
            {
                Type = 0;
                return null;
            }
        }


        public static Brep RectangularPath_P_nC(Curve C, double H, double W)
        {
            Curve C_rec_start = null;
            Curve C_rec_end = null;
            //先得到curve起始点切线向量，然后得P2的xz平面法线向量，得到两向量角度差之后旋转并替换原点
            C.TryGetPlane(out Plane P2);

            Plane P3_start = new Plane(P2.Origin, P2.XAxis, P2.ZAxis);
            Plane P3_end = new Plane(P2.Origin, P2.XAxis, P2.ZAxis);//P3是P2平面的XZ平面

            //if(!calculate.Adjust_plane(ref P3_start, C.TangentAtStart, P3_start.Origin)) return;
            //if(!calculate.Adjust_plane(ref P3_end, C.TangentAtEnd, P3_end.Origin)) return;

            double angle1 = Vector3d.VectorAngle(C.TangentAtStart, P3_start.Normal);
            double angle2 = Vector3d.VectorAngle(C.TangentAtEnd, P3_end.Normal);

            P3_start.Rotate(angle1, P2.ZAxis, P3_start.Origin);
            P3_end.Rotate(angle2, P2.ZAxis, P3_end.Origin);

            P3_start.Origin = C.PointAtStart;

            P3_end.Origin = C.PointAtEnd;

            Rectangle3d rec_start = new Rectangle3d(P3_start, W, H);
            GH_Convert.ToCurve(rec_start, ref C_rec_start, GH_Conversion.Both);
            Vector3d start_transform = Point3d.Subtract(C.PointAtStart, rec_start.Center);
            geometry_base.move(C_rec_start, start_transform);

            Rectangle3d rec_end = new Rectangle3d(P3_end, W, H);
            GH_Convert.ToCurve(rec_end, ref C_rec_end, GH_Conversion.Both);
            Vector3d end_transform = Point3d.Subtract(C.PointAtEnd, rec_end.Center);
            geometry_base.move(C_rec_end, end_transform);

            Curve[] C_sweep = new Curve[2] { C_rec_start, C_rec_end };

            Brep[] B_start = Brep.CreatePlanarBreps(C_rec_start, 0.01);
            Brep[] B_end = Brep.CreatePlanarBreps(C_rec_end, 0.01);

            Brep[] B = Brep.CreateFromSweep(C, C_sweep, true, 0.01);
            B = Brep.JoinBreps(B, 0.01);
            Brep[] B_combine = new Brep[3] { B_start[0], B_end[0], B[0] };
            B_combine = Brep.JoinBreps(B_combine, 0.01);

            return B_combine[0];
        }


        public static Brep RectangularPath_P_C(Curve C, double H, double W)
        {
            C.TryGetPlane(out Plane P2);
            Curve[] C_outside = C.Offset(P2, W / 2, 0.01, CurveOffsetCornerStyle.Sharp);
            if (!C_outside[0].IsClosed) return null;
            Curve[] C_inside = C.Offset(P2, -W / 2, 0.01, CurveOffsetCornerStyle.Sharp);
            if (!C_inside[0].IsClosed) return null;

            Curve C1 = C_outside[0].DuplicateCurve();
            Curve C2 = C_outside[0].DuplicateCurve();
            Curve C3 = C_inside[0].DuplicateCurve();
            Curve C4 = C_inside[0].DuplicateCurve();

            Vector3d V_up = P2.Normal * H / 2;
            Vector3d V_down = -P2.Normal * H / 2;

            bool B1 = geometry_base.move(C1, V_up);
            bool B2 = geometry_base.move(C2, V_down);
            bool B3 = geometry_base.move(C3, V_up);
            bool B4 = geometry_base.move(C4, V_down);

            bool[] Boolen = new bool[4] { B1, B2, B3, B4 };
            Curve[] C_loft = new Curve[4] { C1, C2, C4, C3 };

            Brep[] B = Brep.CreateFromLoft(C_loft, Point3d.Unset, Point3d.Unset, LoftType.Straight, true);
            B = Brep.JoinBreps(B, 0.01);

            return B[0];
        }

        public static Brep RectangularPath_nP_C(Curve C, Plane P, double H, double W)
        {

            Curve[] C_outside = C.Offset(P, W / 2, 0.01, CurveOffsetCornerStyle.Sharp);
            if (!C_outside[0].IsClosed) return null;
            Curve[] C_inside = C.Offset(P, -W / 2, 0.01, CurveOffsetCornerStyle.Sharp);
            if (!C_inside[0].IsClosed) return null;

            Curve C1 = C_outside[0].DuplicateCurve();
            Curve C2 = C_outside[0].DuplicateCurve();
            Curve C3 = C_inside[0].DuplicateCurve();
            Curve C4 = C_inside[0].DuplicateCurve();

            Vector3d V_up = P.Normal * H / 2;
            Vector3d V_down = -P.Normal * H / 2;

            bool B1 = geometry_base.move(C1, V_up);
            bool B2 = geometry_base.move(C2, V_down);
            bool B3 = geometry_base.move(C3, V_up);
            bool B4 = geometry_base.move(C4, V_down);

            bool[] Boolen = new bool[4] { B1, B2, B3, B4 };
            Curve[] C_loft = new Curve[4] { C1, C2, C4, C3 };

            Brep[] B = Brep.CreateFromLoft(C_loft, Point3d.Unset, Point3d.Unset, LoftType.Straight, true);
            B = Brep.JoinBreps(B, 0.01);

            return B[0];
        }


        public static Brep RectangularPath_nP_nC(Curve C, Plane P, double H, double W)
        {
            Curve C_rec_start = null;
            Curve C_rec_end = null;


            Plane P3_start = new Plane(P.Origin, P.XAxis, P.ZAxis);
            Plane P3_end = new Plane(P.Origin, P.XAxis, P.ZAxis);//P3是P2平面的XZ平面

            //if(!calculate.Adjust_plane(ref P3_start, C.TangentAtStart, P3_start.Origin)) return;
            //if(!calculate.Adjust_plane(ref P3_end, C.TangentAtEnd, P3_end.Origin)) return;

            double angle1 = Vector3d.VectorAngle(C.TangentAtStart, P3_start.Normal);
            double angle2 = Vector3d.VectorAngle(C.TangentAtEnd, P3_end.Normal);

            P3_start.Rotate(angle1, P.ZAxis, P3_start.Origin);
            P3_end.Rotate(angle2, P.ZAxis, P3_end.Origin);

            P3_start.Origin = C.PointAtStart;

            P3_end.Origin = C.PointAtEnd;

            Rectangle3d rec_start = new Rectangle3d(P3_start, W, H);
            GH_Convert.ToCurve(rec_start, ref C_rec_start, GH_Conversion.Both);
            Vector3d start_transform = Point3d.Subtract(C.PointAtStart, rec_start.Center);
            geometry_base.move(C_rec_start, start_transform);

            Rectangle3d rec_end = new Rectangle3d(P3_end, W, H);
            GH_Convert.ToCurve(rec_end, ref C_rec_end, GH_Conversion.Both);
            Vector3d end_transform = Point3d.Subtract(C.PointAtEnd, rec_end.Center);
            geometry_base.move(C_rec_end, end_transform);

            Curve[] C_sweep = new Curve[2] { C_rec_start, C_rec_end };

            Brep[] B_start = Brep.CreatePlanarBreps(C_rec_start, 0.01);
            Brep[] B_end = Brep.CreatePlanarBreps(C_rec_end, 0.01);

            Brep[] B = Brep.CreateFromSweep(C, C_sweep, true, 0.01);
            B = Brep.JoinBreps(B, 0.01);
            Brep[] B_combine = new Brep[3] { B_start[0], B_end[0], B[0] };
            B_combine = Brep.JoinBreps(B_combine, 0.01);

            return B_combine[0];
        }

        public static Brep Sweep1(List<Curve> C,Curve rail,bool soild)
        {
            Brep[] B = Brep.CreateFromSweep(rail, C, true, 0.01);
            B = Brep.JoinBreps(B, 0.01);

            if (soild)
            {
                int len = C.Count-1;
                Brep[] B_start= Brep.CreatePlanarBreps(C[0], 0.01);
                Brep[] B_end = Brep.CreatePlanarBreps(C[len], 0.01);
                Brep[] B_combine = new Brep[3] { B[0], B_start[0], B_end[0] };
                B = Brep.JoinBreps(B_combine, 0.01);
            }

            return B[0];
        }


    }
}
