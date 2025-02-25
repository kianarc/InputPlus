using Grasshopper;
using Grasshopper.Kernel;
using System;
using System.Drawing;

namespace InputPlus
{
    public class InputPlusInfo : GH_AssemblyInfo
    {
        public override string Name => "Kian";

        //Return a 24x24 pixel bitmap to represent this GHA library.
        public override Bitmap Icon => Properties.Resources.Logo ;

        //Return a short string describing the purpose of this GHA library.
        public override string Description => "";

        public override Guid Id => new Guid("cdee944a-dda2-4d77-a1eb-65e86b5c2d59");

        //Return a string identifying you or your company.
        public override string AuthorName => "";

        //Return a string representing your preferred contact details.
        public override string AuthorContact => "";

        //Return a string representing the version.  This returns the same version as the assembly.
        public override string AssemblyVersion => GetType().Assembly.GetName().Version.ToString();
    }
}