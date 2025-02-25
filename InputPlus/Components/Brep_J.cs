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
    public class Brep_J : GH_Component
    {
        // Fields to store the Brep data
        private List<Brep> _storedBreps;

        /// <summary>
        /// Initializes a new instance of the Brep_J class.
        /// </summary>
        public Brep_J()
          : base("Brep_J", "BJ",
              "Brep From JSON",
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
            pManager.AddBrepParameter("Brep", "B", "The stored Brep", GH_ParamAccess.list);
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
                var brepDatas = JsonSerializer.Deserialize<List<string>>(json);

                // Deserialize the Breps
                _storedBreps = new List<Brep>();
                bool hasError = false; // Flag to track errors

                foreach (var brepData in brepDatas)
                {
                    try
                    {
                        Brep brep = Brep.FromJSON(brepData) as Brep;
                        if (brep == null)
                        {
                            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Failed to deserialize a Brep from JSON.");
                            hasError = true;
                            break;
                        }
                        _storedBreps.Add(brep);
                    }
                    catch (Exception ex)
                    {
                        this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, $"Failed to deserialize a Brep: {ex.Message}");
                        hasError = true;
                        break;
                    }
                }

                // If there was an error, stop further processing
                if (hasError)
                {
                    _storedBreps = null; // Clear stored Breps
                    DA.SetDataList(0, null); // Output null
                    return;
                }

                // Output the Breps
                DA.SetDataList(0, _storedBreps);
            }
            catch (Exception ex)
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, $"Failed to read JSON: {ex.Message}");
                _storedBreps = null; // Clear stored Breps
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
                return Properties.Resources.Brep_J; // Replace with your icon
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("5987FCE4-76D8-4F7B-8231-0CB380B0ED10"); }
        }
    }
}