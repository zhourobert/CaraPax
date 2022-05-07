using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using Car_Functions;

namespace Workshop_v6
{
    public class SquareTubeOnSurface : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the SquareTubeOnSurface class.
        /// </summary>
        public SquareTubeOnSurface()
          : base("SquareTubeOnSurface", "TubeOnSurface",
              "Create curtain wall bar on surface",
              "Carapax", "Surface basis")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddSurfaceParameter("Surface", "S", "Add a surface", GH_ParamAccess.item);
            pManager.AddNumberParameter("Step Lenth", "SL", "The interval between two bars", GH_ParamAccess.item,1200);
            pManager.AddIntegerParameter("v_count", "V", "Ensure curve accuracy", GH_ParamAccess.item, 10);
            pManager.AddNumberParameter("Offset", "O", "The offset of bars contrast to surface", GH_ParamAccess.item,0);
            pManager.AddNumberParameter("Height", "H", "The height of the tube", GH_ParamAccess.item,100);
            pManager.AddNumberParameter("Width", "W", "The width of the tube", GH_ParamAccess.item,100);

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddBrepParameter("Brep", "B", "result", GH_ParamAccess.list);
            
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Surface S = null;
            double SL = 0;
            int V = 0;
            double F = 0;
            double H = 0;
            double W = 0;

            if (!DA.GetData(0, ref S)) return;
            if (!DA.GetData(1, ref SL)) return;
            if (!DA.GetData(2, ref V)) return;
            if (!DA.GetData(3, ref F)) return;
            if (!DA.GetData(4, ref H)) return;
            if (!DA.GetData(5, ref W)) return;

            Interval Dom = S.Domain(0);
            int num = calculate.approximate_divisible(Dom, SL);

            List<Curve> Curve_list = geometry_base.IsoUVCurves(S, num, 0);

            List<Brep> Brep_list = new List<Brep>();

            //List<bool> success = new List<bool>();

            for (int i = 0; i < Curve_list.Count; i++)//获得每根线的循环
            {
                Curve C = Curve_list[i];
                
                
                List<Curve> rec = new List<Curve>();
                List<Point3d> pt_transform = new List<Point3d>();

                double[] Pt_list = C.DivideByCount(V, true);
                for (int j = 0; j < Pt_list.Length; j++)//获取每个点的循环
                {
                    Point3d pt = C.PointAt(Pt_list[j]);//得到点
                    Plane P_sf = geometry_base.PointOnSurface(pt, S, out Vector3d normal);//得法向量和平面
                    Plane P = new Plane(P_sf.Origin, P_sf.ZAxis, P_sf.YAxis);

                    if (F!=0)//位移平面
                    {
                        geometry_base.move(P, normal * F);//移动中心点
                    }
                    
                    rec.Add(geometry_base.Center_rec(P, H, W));
                    pt_transform.Add(pt);
                }
                Brep B = Tubes.Sweep1(rec, C, true);

                

                Brep_list.Add(B);


                


            }
            DA.SetDataList(0, Brep_list);
            

        }

        //internal List<Brep> SquareTubeOnSurface(Surface S,double SL,int V,double F,double H,double W)
        //{
        //    Interval Dom = S.Domain(0);
        //    int num = calculate.approximate_divisible(Dom, SL);

        //    List<Curve> Curve_list = geometry_base.IsoUVCurves(S, num, 0);

        //    List<Brep> Brep_list = new List<Brep>();

        //    List<bool> success = new List<bool>();

        //    for (int i = 0; i < Curve_list.Count; i++)
        //    {
        //        Curve C = Curve_list[i];
        //        if (Curve_list[i].IsLinear())
        //        {
        //            if (F != 0)
        //            {
        //                Point3d Pt = geometry_base.Curve_middle(C);
        //                geometry_base.PointOnSurface(Pt, S, out Vector3d normal);
        //                Vector3d offset = normal * F;
        //                geometry_base.move(C, offset);
        //            }

        //            Brep B = Tubes.RectangularPath_P_nC(C, H, W);

        //            if (B != null)
        //            {
        //                success.Add(true);
        //            }
        //            else
        //            {
        //                success.Add(false);
        //            }

        //            Brep_list.Add(B);
        //        }
        //        else if (Curve_list[i].IsClosed && Curve_list[i].IsPlanar())
        //        {

        //        }
        //        else
        //        {
        //            if (F != 0)
        //            {
        //                List<Curve> rec = new List<Curve>();
        //                List<Point3d> pt_transform = new List<Point3d>();
        //                double[] Pt_list = C.DivideByCount(V, true);
        //                for (int j = 0; j < Pt_list.Length; j++)
        //                {
        //                    Point3d pt = C.PointAt(Pt_list[j]);//得到点
        //                    Plane P = geometry_base.PointOnSurface(pt, S, out Vector3d normal);//得法向量和平面

        //                    geometry_base.move(pt, normal * F);//移动中心点
        //                    rec.Add(geometry_base.Center_rec(P, H, W));
        //                    pt_transform.Add(pt);
        //                }
        //                Brep B = Tubes.Sweep1(rec, C, true);

        //                if (B != null)
        //                {
        //                    success.Add(true);
        //                }
        //                else
        //                {
        //                    success.Add(false);
        //                }

        //                Brep_list.Add(B);
        //            }

        //        }

        //    }
        //}

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("29ef7f41-85e6-4507-800d-044b0da7f9cb"); }
        }
    }
}