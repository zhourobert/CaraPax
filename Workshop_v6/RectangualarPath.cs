using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using Grasshopper.Kernel.Types;
using Car_Functions;


namespace Carapax
{
    public class RectangualarPath : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public RectangualarPath()
          : base("RectangularPath", "RP",
              "make curves turn into square tube",
              "Carapax", "Others")
        {
        }


        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curves", "C", "choose curves to be square tube", GH_ParamAccess.item);
            pManager.AddPlaneParameter("plane", "P", "add a standard plane for the curve", GH_ParamAccess.item, Plane.WorldXY);
            pManager.AddNumberParameter("Height", "H", "the height of the tube", GH_ParamAccess.item, 100);
            pManager.AddNumberParameter("Width", "W", "the width of the tube", GH_ParamAccess.item, 100);
        }




        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddBrepParameter("Square tube", "tube", "", GH_ParamAccess.item);

            pManager.AddBooleanParameter("Solid", "solid", "show the tube is solid", GH_ParamAccess.item);

            pManager.AddIntegerParameter("Type", "T", "the type of the curve 1:Planer but not closed,2:planer and closed,3:closed but not planer,4:not close and not planer", GH_ParamAccess.item);


        }


        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //记得这里最好用反射new对象
            Curve C = null;
            Plane P = new Plane();
            double H = 0;
            double W = 0;

            if (!DA.GetData(0, ref C)) return;
            if (!DA.GetData(1, ref P)) return;
            if (!DA.GetData(2, ref H)) return;
            if (!DA.GetData(3, ref W)) return;

            Brep B = Tubes.Square_Tube(C, P, H, W, out int Type);

            DA.SetData(0, B);
            DA.SetData(1, B.IsSolid);
            DA.SetData(2, Type);

            

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
            get { return new Guid("b36c4454-15da-4e8e-a1b1-013e5eebd13f"); }
        }
    }
}