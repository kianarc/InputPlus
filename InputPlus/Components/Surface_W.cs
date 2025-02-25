using System;
using System.Text.Json;
using System.Collections.Generic;
using System.Windows.Forms;
using GH_IO.Serialization;
using Grasshopper.Kernel;
using Rhino.DocObjects;
using Rhino.Geometry;
using Grasshopper.Getters;
using Grasshopper.Kernel.Types;
using System.IO;

namespace InputPlus.Components
{
    public class Surface_W : GH_Component
    {
        // Fields to store the Surface data
        private List<Surface> _storedSurfaces;

        public Surface_W()
          : base("Surface_W", "SW",
              "Surface Wallet",
              "Input Plus", "Wallet")
        {
        }

        public override void AppendAdditionalMenuItems(ToolStripDropDown menu)
        {
            base.AppendAdditionalMenuItems(menu);

            Menu_AppendItem(menu, "Set one Surface", SetOneSurfaceClicked);
            Menu_AppendItem(menu, "Set multiple Surfaces", SetMultipleSurfacesClicked);

            // Add a menu item for saving to JSON
            Menu_AppendItem(menu, "Save to JSON", SaveToJsonClicked);


            Menu_AppendItem(menu, "Clear Data", ClearSurfaceMenuItemClicked);
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddSurfaceParameter("Surface", "S", "The stored Surface", GH_ParamAccess.item);
        }

        private void SetOneSurfaceClicked(object sender, EventArgs e)
        {
            GetSurfaces(false);
        }

        private void SetMultipleSurfacesClicked(object sender, EventArgs e)
        {
            GetSurfaces(true);
        }

        private void GetSurfaces(bool multiple)
        {
            _storedSurfaces = null;
            Rhino.Input.Custom.GetObject getObject = new Rhino.Input.Custom.GetObject()
            {
                GeometryFilter = ObjectType.Surface,
                SubObjectSelect = false,
            };
            getObject.SetCommandPrompt("Select Surface(s)");
            getObject.EnablePreSelect(true, true);

            Rhino.Input.GetResult get = Rhino.Input.GetResult.NoResult;

            if (multiple)
                get = getObject.GetMultiple(0, 0);
            else
                get = getObject.Get();

            if (get == Rhino.Input.GetResult.Object)
            {
                if (_storedSurfaces == null)
                    _storedSurfaces = new List<Surface>();
                for (int i = 0; i < getObject?.ObjectCount; i++)
                {
                    _storedSurfaces.Add(getObject?.Object(i)?.Surface().ToNurbsSurface());
                }
                ExpireSolution(true);
            }
        }

        private void ClearSurfaceMenuItemClicked(object sender, EventArgs e)
        {
            _storedSurfaces = null;
            ExpireSolution(true);
        }

        /// <summary>
        /// Event handler for the "Save to JSON" menu item.
        /// </summary>
        private void SaveToJsonClicked(object sender, EventArgs e)
        {
            if (_storedSurfaces == null || _storedSurfaces.Count == 0)
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "No Surfaces to save.");
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                Filter = "JSON files (*.json)|*.json",
                Title = "Save Surfaces to JSON"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = saveFileDialog.FileName;
                try
                {
                    var surfaceDatas = new List<string>();
                    foreach (var surface in _storedSurfaces)
                    {
                        surfaceDatas.Add(surface.ToJSON(new Rhino.FileIO.SerializationOptions()));
                    }
                    string json = JsonSerializer.Serialize(surfaceDatas);
                    File.WriteAllText(filePath, json);
                }
                catch (Exception ex)
                {
                    this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, $"Failed to save JSON: {ex.Message}");
                }
            }
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            this.ClearRuntimeMessages();

            if (_storedSurfaces == null || _storedSurfaces.Count == 0)
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "No Surfaces stored. Please set one or more Surfaces.");
                DA.SetData(0, null);
                return;
            }

            if (_storedSurfaces.Count == 1)
            {
                DA.SetData(0, _storedSurfaces[0]);
            }
            else
            {
                DA.SetDataList(0, _storedSurfaces);
            }
        }

        public override bool Write(GH_IWriter writer)
        {
            if (_storedSurfaces != null)
            {
                var surfaceDatas = new List<string>();
                foreach (var surface in _storedSurfaces)
                {
                    surfaceDatas.Add(surface.ToJSON(new Rhino.FileIO.SerializationOptions()));
                }
                writer.SetString("StoredSurfaces", JsonSerializer.Serialize(surfaceDatas));
            }
            return base.Write(writer);
        }

        public override bool Read(GH_IReader reader)
        {
            if (reader.ItemExists("StoredSurfaces"))
            {
                var surfaceDatas_string = reader.GetString("StoredSurfaces");
                var surfaceDatas = JsonSerializer.Deserialize<List<string>>(surfaceDatas_string);
                if (_storedSurfaces == null)
                    _storedSurfaces = new List<Surface>();
                foreach (var surface in surfaceDatas)
                {
                    _storedSurfaces.Add(Surface.FromJSON(surface) as Surface);
                }
            }
            return base.Read(reader);
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.Surface_W; // Replace with your icon
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("6ED95E18-9CBA-4BAD-BE44-BE972461D123"); }
        }
    }
}