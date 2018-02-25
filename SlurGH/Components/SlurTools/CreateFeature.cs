﻿using System;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

using SpatialSlur.SlurTools;
using SpatialSlur.SlurTools.Features;

/*
 * Notes
 */

namespace SpatialSlur.SlurGH.Components
{
    /// <summary>
    /// 
    /// </summary>
    public class CreateFeature : GH_Component
    {
        /// <summary>
        /// 
        /// </summary>
        public CreateFeature()
          : base("Creates Features", "Feat",
              "Creates a geometric feature used for dynamic remeshing",
              "SpatialSlur", "Mesh")
        {
        }


        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("geometry", "geom", "", GH_ParamAccess.item);
        }


        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("feature", "feat", "", GH_ParamAccess.item);
        }


        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GH_ObjectWrapper goo = null;
            if (!DA.GetData(0, ref goo)) return;

            var obj = goo.Value;
            IFeature feat = null;

            switch (obj)
            {
                case Mesh m:
                    feat = new MeshFeature(m);
                    break;
                case Curve c:
                    feat = new CurveFeature(c);
                    break;
                case Point3d p:
                    feat = new PointFeature(p);
                    break;
                default:
                    throw new ArgumentException();
            }
            
            DA.SetData(0, new GH_ObjectWrapper(feat));
        }


        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return null; }
        }


        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{0FEF2BE4-6432-4352-ADE2-F160108EDA12}"); }
        }
    }
}
