using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using System.Windows.Forms;
using GH_IO.Serialization;
using Grasshopper.Kernel;
using Rhino.DocObjects;
using Rhino.Geometry;
using Grasshopper.Getters;
using Grasshopper.Kernel.Types;

namespace InputPlus.Components
{
    public class Brep_W : GH_Component
    {

        // Fields to store the Brep and Curve data
        private List<Brep> _storedBreps;
       


        /// <summary>
        /// Initializes a new instance of the Brep_W class.
        /// </summary>
        public Brep_W()
          : base("Brep_W", "BW",
              "Brep Wallet",
              "Input Plus", "Wallet")
        {
        }
        
        public override void AppendAdditionalMenuItems(ToolStripDropDown menu)
        {
            base.AppendAdditionalMenuItems(menu);

            // Add a menu item for setting a Brep or Curve
            Menu_AppendItem(menu, "Set one Brep", SetOneBrepClicked);
            Menu_AppendItem(menu, "Set multiple Breps", SetMultipleBrepsClicked);

            // Add a menu item for saving to JSON
            Menu_AppendItem(menu, "Save to JSON", SaveToJsonClicked);

            // Add a menu item for clearing the Brep or Curve
            Menu_AppendItem(menu, "Clear Data", ClearBrepMenuItemClicked);
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddBrepParameter("Brep", "B", "The stored Brep or Curve", GH_ParamAccess.item);
        }

        /// <summary>
        /// Event handler for the "Set Brep" menu item.
        /// </summary>
        private void SetOneBrepClicked(object sender, EventArgs e)
        {
            GetBreps(false);
        }
        private void SetMultipleBrepsClicked(object sender, EventArgs e)
        {
            GetBreps(true);
        }
        private void GetBreps(bool multiple)
        {
            // Clear the stored Brep and Curve
            _storedBreps = null;
            // Prompt the user to select a Brep or Curve from the Rhino document
            Rhino.Input.Custom.GetObject getObject = new Rhino.Input.Custom.GetObject()
            {
                GeometryFilter = ObjectType.Brep,
                SubObjectSelect = false,

            };
            getObject.SetCommandPrompt("Select Brep(s)");
            getObject.EnablePreSelect(true, true);

            Rhino.Input.GetResult get = Rhino.Input.GetResult.NoResult;


            if (multiple)
                get = getObject.GetMultiple(0, 0);
            else
                get = getObject.Get();

                
            if (get == Rhino.Input.GetResult.Object)
            {
                if (_storedBreps == null)
                    _storedBreps = new List<Brep>();
                // Get the selected object
                for (int i = 0; i < getObject?.ObjectCount; i++)
                {
                    _storedBreps.Add(getObject?.Object(i)?.Brep());

                }

                // Expire the component to refresh its output
                ExpireSolution(true);
            }

        }

        /// <summary>
        /// Event handler for the "Clear Brep" menu item.
        /// </summary>
        private void ClearBrepMenuItemClicked(object sender, EventArgs e)
        {
            // Clear the stored Brep and Curve
            _storedBreps = null;

            // Expire the component to refresh its output
            ExpireSolution(true);
        }

        /// <summary>
        /// Event handler for the "Save to JSON" menu item.
        /// </summary>
        private void SaveToJsonClicked(object sender, EventArgs e)
        {
            if (_storedBreps == null || _storedBreps.Count == 0)
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "No Breps to save.");
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                Filter = "JSON files (*.json)|*.json",
                Title = "Save Breps to JSON"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = saveFileDialog.FileName;
                try
                {
                    var brepDatas = new List<string>();
                    foreach (var brep in _storedBreps)
                    {
                        brepDatas.Add(brep.ToJSON(new Rhino.FileIO.SerializationOptions()));
                    }
                    string json = JsonSerializer.Serialize(brepDatas);
                    File.WriteAllText(filePath, json);
                }
                catch (Exception ex)
                {
                    this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, $"Failed to save JSON: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Clear any previous runtime messages
            this.ClearRuntimeMessages();

            if (_storedBreps == null || _storedBreps.Count == 0)
            {
                // Add a warning message if no Breps are stored
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "No Breps stored. Please set one or more Breps.");
                // Optionally, set the output to null
                DA.SetData(0, null);
                return;
            }

            // If there is only one Brep, output it directly
            if (_storedBreps.Count == 1)
            {
                DA.SetData(0, _storedBreps[0]);
            }
            // If there are multiple Breps, output them as a list
            else
            {
                DA.SetDataList(0, _storedBreps);
            }
        }

        /// <summary>
        /// Serialize the component's data.
        /// </summary>
        public override bool Write(GH_IWriter writer)
        {
            // Serialize the Brep
            if (_storedBreps != null)
            {
                var brepDatas = new List<string>();
                foreach (var brep in _storedBreps)
                {
                    brepDatas.Add(brep.ToJSON(new Rhino.FileIO.SerializationOptions()));
                }
                writer.SetString("StoredBreps", JsonSerializer.Serialize( brepDatas));
            }

            return base.Write(writer);
        }

        /// <summary>
        /// Deserialize the component's data.
        /// </summary>
        public override bool Read(GH_IReader reader)
        {
            // Deserialize the Brep
            if (reader.ItemExists("StoredBreps"))
            {
                var brepDatas_string = reader.GetString("StoredBreps");
                var brepDatas = JsonSerializer.Deserialize<List<string>>(brepDatas_string);
                if(_storedBreps == null)
                    _storedBreps = new List<Brep>();
                foreach (var brep in brepDatas)
                {
                    _storedBreps.Add(Brep.FromJSON(brep) as Brep);
                }
            }

            return base.Read(reader);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                // Return a 24x24 pixel bitmap for the component's icon
                return Properties.Resources.Brep_W; // Replace with your icon
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("6ED95E18-9CBA-4BAD-BE44-BE972461D120"); }
        }
    }
}