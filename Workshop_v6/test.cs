using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using Car_Functions;

namespace Workshop_v6
{
    public class test : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the test class.
        /// </summary>
        public test()
          : base("test", "Nickname",
              "Description",
              "Carapax", "test")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("Pt", "pt", "", GH_ParamAccess.item);
            pManager.AddVectorParameter("V", "v", "", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("P", "P", "", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Point3d P = Point3d.Unset;
            Vector3d V = Vector3d.Unset;

            if (!DA.GetData(0, ref P)) return;
            if (!DA.GetData(1, ref V)) return;

            P = geometry_base.move(P, V);

            DA.SetData(0, P);
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
            get { return new Guid("df6cc7d8-3158-4b7e-a497-2645bc614333"); }
        }
    }
}