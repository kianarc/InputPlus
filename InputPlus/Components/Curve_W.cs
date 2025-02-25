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
    public class Curve_W : GH_Component
    {
        // Fields to store the Curve data
        private List<Curve> _storedCurves;

        /// <summary>
        /// Initializes a new instance of the Curve_W class.
        /// </summary>
        public Curve_W()
          : base("Curve_W", "CW",
              "Curve Wallet",
              "Input Plus", "Wallet")
        {
        }

        public override void AppendAdditionalMenuItems(ToolStripDropDown menu)
        {
            base.AppendAdditionalMenuItems(menu);

            // Add menu items for setting and clearing Curves
            Menu_AppendItem(menu, "Set one Curve", SetOneCurveClicked);
            Menu_AppendItem(menu, "Set multiple Curves", SetMultipleCurvesClicked);

            // Add a menu item for saving to JSON
            Menu_AppendItem(menu, "Save to JSON", SaveToJsonClicked);

            Menu_AppendItem(menu, "Clear Data", ClearCurveMenuItemClicked);
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Curve", "C", "The stored Curve", GH_ParamAccess.item);
        }

        private void SetOneCurveClicked(object sender, EventArgs e)
        {
            GetCurves(false);
        }

        private void SetMultipleCurvesClicked(object sender, EventArgs e)
        {
            GetCurves(true);
        }

        private void GetCurves(bool multiple)
        {
            _storedCurves = null;
            Rhino.Input.Custom.GetObject getObject = new Rhino.Input.Custom.GetObject()
            {
                GeometryFilter = ObjectType.Curve,
                SubObjectSelect = false,
            };
            getObject.SetCommandPrompt("Select Curve(s)");
            getObject.EnablePreSelect(true, true);

            Rhino.Input.GetResult get = Rhino.Input.GetResult.NoResult;

            if (multiple)
                get = getObject.GetMultiple(0, 0);
            else
                get = getObject.Get();

            if (get == Rhino.Input.GetResult.Object)
            {
                if (_storedCurves == null)
                    _storedCurves = new List<Curve>();
                for (int i = 0; i < getObject?.ObjectCount; i++)
                {
                    _storedCurves.Add(getObject?.Object(i)?.Curve());
                }
                ExpireSolution(true);
            }
        }

        private void ClearCurveMenuItemClicked(object sender, EventArgs e)
        {
            _storedCurves = null;
            ExpireSolution(true);
        }

        /// <summary>
        /// Event handler for the "Save to JSON" menu item.
        /// </summary>
        private void SaveToJsonClicked(object sender, EventArgs e)
        {
            if (_storedCurves == null || _storedCurves.Count == 0)
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "No Curves to save.");
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                Filter = "JSON files (*.json)|*.json",
                Title = "Save Curves to JSON"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = saveFileDialog.FileName;
                try
                {
                    var curveDatas = new List<string>();
                    foreach (var curve in _storedCurves)
                    {
                        curveDatas.Add(curve.ToJSON(new Rhino.FileIO.SerializationOptions()));
                    }
                    string json = JsonSerializer.Serialize(curveDatas);
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

            if (_storedCurves == null || _storedCurves.Count == 0)
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "No Curves stored. Please set one or more Curves.");
                DA.SetData(0, null);
                return;
            }

            if (_storedCurves.Count == 1)
            {
                DA.SetData(0, _storedCurves[0]);
            }
            else
            {
                DA.SetDataList(0, _storedCurves);
            }
        }

        public override bool Write(GH_IWriter writer)
        {
            if (_storedCurves != null)
            {
                var curveDatas = new List<string>();
                foreach (var curve in _storedCurves)
                {
                    curveDatas.Add(curve.ToJSON(new Rhino.FileIO.SerializationOptions()));
                }
                writer.SetString("StoredCurves", JsonSerializer.Serialize(curveDatas));
            }
            return base.Write(writer);
        }

        public override bool Read(GH_IReader reader)
        {
            if (reader.ItemExists("StoredCurves"))
            {
                var curveDatas_string = reader.GetString("StoredCurves");
                var curveDatas = JsonSerializer.Deserialize<List<string>>(curveDatas_string);
                if (_storedCurves == null)
                    _storedCurves = new List<Curve>();
                foreach (var curve in curveDatas)
                {
                    _storedCurves.Add(Curve.FromJSON(curve) as Curve);
                }
            }
            return base.Read(reader);
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.Curve_W; // Replace with your icon
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("6ED95E18-9CBA-4BAD-BE44-BE972461D121"); }
        }
    }
}