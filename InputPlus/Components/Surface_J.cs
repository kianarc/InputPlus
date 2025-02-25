using System;
using System.Text.Json;
using System.Collections.Generic;
using System.IO;
using GH_IO.Serialization;
using Grasshopper.Kernel;
using Rhino.Geometry;
using Grasshopper.Kernel.Types;
using System.Windows.Forms;

namespace InputPlus.Components
{
    public class Surface_J : GH_Component
    {
        // Fields to store the Surface data
        private List<Surface> _storedSurfaces;

        /// <summary>
        /// Initializes a new instance of the Surface_J class.
        /// </summary>
        public Surface_J()
          : base("Surface_J", "SJ",
              "Surface From JSON",
              "Input Plus", "Json")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Path", "P", "Path to JSON file", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddSurfaceParameter("Surface", "S", "The stored Surface", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Clear any previous runtime messages
            this.ClearRuntimeMessages();

            // Get the input path
            string filePath = string.Empty;
            if (!DA.GetData(0, ref filePath))
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "No file path provided.");
                return;
            }

            // Check if the file exists
            if (!File.Exists(filePath))
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "File does not exist.");
                return;
            }

            try
            {
                // Read the JSON file
                string json = File.ReadAllText(filePath);
                var surfaceDatas = JsonSerializer.Deserialize<List<string>>(json);

                // Deserialize the Surfaces
                _storedSurfaces = new List<Surface>();
                bool hasError = false;

                foreach (var surfaceData in surfaceDatas)
                {
                    try
                    {
                        Surface surface = Surface.FromJSON(surfaceData) as Surface;
                        if (surface == null)
                        {
                            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Failed to deserialize a surface from JSON.");
                            hasError = true;
                            break;
                        }
                        _storedSurfaces.Add(surface);
                    }
                    catch (Exception ex)
                    {
                        this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, $"Failed to deserialize a surface: {ex.Message}");
                        hasError = true;
                        break;
                    }
                }

                // If there was an error, stop further processing
                if (hasError)
                {
                    _storedSurfaces = null; // Clear stored surfaces
                    DA.SetDataList(0, null); // Output null
                    return;
                }

                // Output the Surfaces
                DA.SetDataList(0, _storedSurfaces);
            }
            catch (Exception ex)
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, $"Failed to read JSON: {ex.Message}");
                _storedSurfaces = null; // Clear stored surfaces
                DA.SetDataList(0, null); // Output null
            }
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                // Return a 24x24 pixel bitmap for the component's icon
                return Properties.Resources.Surface_J; // Replace with your icon
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("5987FCE4-76D8-4F7B-8231-0CB380B0ED12"); }
        }
    }
}