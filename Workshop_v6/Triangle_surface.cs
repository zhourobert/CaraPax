using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using Car_Functions;

namespace Workshop_v6
{
    public class Triangle_surface : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the Triangle_surface class.
        /// </summary>
        public Triangle_surface()
          : base("Triangle_surface", "TS",
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
            pManager.AddNumberParameter("Step Lenth", "SL", "input the interval of triangle surface", GH_ParamAccess.item,1000);
            pManager.AddIntegerParameter("v count", "V", "make sure the accurary of curve", GH_ParamAccess.item,10);
            pManager.AddNumberParameter("Height", "H", "input the height of the facade", GH_ParamAccess.item,100);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddSurfaceParameter("A surface", "A", "", GH_ParamAccess.list);
            pManager.AddSurfaceParameter("B surface", "B", "", GH_ParamAccess.list);
            //pManager.AddCurveParameter("C1", "C1", "", GH_ParamAccess.list);
            //pManager.AddCurveParameter("C2", "C2", "", GH_ParamAccess.list);
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
            double H = 0;

            if (!DA.GetData(0, ref S)) return;
            if (!DA.GetData(1, ref SL)) return;
            if (!DA.GetData(2, ref V)) return;
            if (!DA.GetData(3, ref H)) return;

            Interval Dom = S.Domain(0);
            int num = calculate.approximate_divisible(Dom, SL);

            List<Curve> Curve_list = geometry_base.IsoUVCurves(S, num, 0);//获取线

            
            List<Curve> Curve_middle = new List<Curve>();
            List<Brep> brepA = new List<Brep>();
            List<Brep> brepB = new List<Brep>();

            for (int i = 0; i < Curve_list.Count-1; i++)
            {
                Curve[] C_stage = Curve.CreateTweenCurves(Curve_list[i], Curve_list[i + 1], 1, 0.01);
                Curve C_mid = geometry_base.OffsetFromSurface(S, C_stage[0], V, H);
                Curve_middle.Add(C_mid);

                Brep A = geometry_base.Simple_loft(Curve_list[i], C_mid);
                Brep B = geometry_base.Simple_loft(C_mid, Curve_list[i + 1]);

                brepA.Add(A);
                brepB.Add(B);
            }

            DA.SetDataList(0, brepA);
            DA.SetDataList(1, brepB);
            //DA.SetDataList(2, Curve_middle);
            //DA.SetDataList(3, Curve_list);
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
            get { return new Guid("5a660204-0959-40ef-9d06-2bf9d034e562"); }
        }
    }
}