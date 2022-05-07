using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using Car_Functions;

namespace Workshop_v6
{
    public class PointOnSurface : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the PointOnSurface class.
        /// </summary>
        public PointOnSurface()
          : base("PointOnSurface", "PtOnSf",
              "get the uv of a point on surface",
              "Carapax", "Surface basis")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("Point", "Pt", "The point on the surface", GH_ParamAccess.item);
            pManager.AddSurfaceParameter("Surface", "S", "target surface", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPlaneParameter("Plane", "P", "The plane of a point on surface", GH_ParamAccess.item);
            pManager.AddNumberParameter("u parameter", "U", "the u parameter of the point on surface", GH_ParamAccess.item);
            pManager.AddNumberParameter("v parameter", "V", "the v parameter of the point on surface", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Point3d Pt = Point3d.Unset;
            Surface S = null;

            if (!DA.GetData(0, ref Pt)) return;
            if (!DA.GetData(1, ref S)) return;

            S.ClosestPoint(Pt, out double u, out double v);
            Plane P = geometry_base.PointOnSurface(Pt, S, out Vector3d normal);

            DA.SetData(0, P);
            DA.SetData(1, u);
            DA.SetData(2, v);
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
            get { return new Guid("b0bda4f6-1316-44d8-bce3-37f1a646f7aa"); }
        }
    }
}