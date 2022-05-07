using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using Car_Functions;

namespace Workshop_v6
{
    public class Triangle_bar : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the Triangle_bar class.
        /// </summary>
        public Triangle_bar()
          : base("Triangle_bar", "Nickname",
              "Description",
              "Carapax", "Facade")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddSurfaceParameter("Surface", "S", "Choose a surface", GH_ParamAccess.item);
            pManager.AddNumberParameter("Step Lenth", "SL", "input the interval of triangle surface", GH_ParamAccess.item,1200);
            pManager.AddNumberParameter("Width", "W", "input the width of the facade", GH_ParamAccess.item, 200);
            pManager.AddNumberParameter("Height", "H", "input the height of the facade", GH_ParamAccess.item, 100);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddBrepParameter("Brep", "B", "", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Surface S = null;
            double SL = 0; 
            double W = 0;
            double H = 0;

            if (!DA.GetData(0, ref S)) return;
            if (!DA.GetData(1, ref SL)) return;
            if (!DA.GetData(2, ref W)) return;
            if (!DA.GetData(3, ref H)) return;

            Interval Dom = S.Domain(0);
            int num = calculate.approximate_divisible(Dom, SL);

            List<Curve> Curve_list = geometry_base.IsoUVCurves(S, num, 0);//获取线

            List<Brep> Breps = new List<Brep>();

            for (int i = 0; i < Curve_list.Count; i++)
            {
                Curve C = Curve_list[i];
                Point3d Pt_mid = geometry_base.Curve_middle(C);
                Plane P = geometry_base.PointOnSurface(Pt_mid, S,out Vector3d normal);

                Curve C1 = C.DuplicateCurve();
                Vector3d V1 = P.YAxis * W / 2;
                geometry_base.move(C1, V1);

                Curve C2=geometry_base.OffsetFromSurface(S, C, H);

                Curve C3 = C.DuplicateCurve();
                Vector3d V3 = -P.YAxis * W / 2;
                geometry_base.move(C3, V3);

                Curve[] C_combine = new Curve[3] { C1, C2, C3 };

                Brep[] B = Brep.CreateFromLoft(C_combine, Point3d.Unset, Point3d.Unset, LoftType.Straight, false);

                Breps.Add(B[0]);
            }

            DA.SetDataList(0, Breps);
        }

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
            get { return new Guid("dcb3aab2-e22d-43e1-8c7a-a2e93a23b16a"); }
        }
    }
}