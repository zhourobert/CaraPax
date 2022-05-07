using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using Car_Functions;


// In order to load the result of this wizard, you will also need to
// add the output bin/ folder of this project to the list of loaded
// folder in Grasshopper.
// You can use the _GrasshopperDeveloperSettings Rhino command for that.

namespace Carapax
{
    public class SurfaceUVLine : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public SurfaceUVLine()
          : base("SurfaceUVLine", "UVline",
              "get the uv line of a surface",
              "Carapax", "Surface basis")
        {

        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddSurfaceParameter("surace", "S", "choose a surface", GH_ParamAccess.item);
            pManager.AddIntegerParameter("u number", "U", "Enter the number of U segments", GH_ParamAccess.item,10);
            pManager.AddIntegerParameter("v number", "V", "Enter the number of U segments", GH_ParamAccess.item,10);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            
            pManager.AddCurveParameter("u curve", "U", "", GH_ParamAccess.list);
            pManager.AddCurveParameter("v curve", "V", "", GH_ParamAccess.list);
            
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //实例化子类从而实例化父类，算不上好办法，日后记得改成反射
            Surface S = null;
            int u = 0;
            int v = 0;

            //传入对象并判断是否为空
            if (!DA.GetData(0, ref S)) return;
            if (!DA.GetData(1, ref u)) return;
            if (!DA.GetData(2, ref v)) return;

            List<Curve> u_list = geometry_base.IsoUVCurves(S, u, 0);
            List<Curve> v_list = geometry_base.IsoUVCurves(S, v, 1);

            DA.SetDataList(0, u_list);
            DA.SetDataList(1, v_list);
            //DA.SetData(2, U);
        }

        
   

        

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                // You can add image files to your project resources and access them like this:
                //return Resources.IconForThisComponent;
                return null;
            }
        }


        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("7b95b87d-c22d-43dc-941f-87e8856c5d9d"); }
        }
    }
}
