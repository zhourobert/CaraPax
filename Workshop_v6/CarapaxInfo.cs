using Grasshopper.Kernel;
using System;
using System.Drawing;

namespace Carapax
{
    public class CarapaxInfo : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "Carapax";
            }
        }
        public override Bitmap Icon
        {
            get
            {
                //Return a 24x24 pixel bitmap to represent this GHA library.
                return null;
            }
        }
        public override string Description
        {
            get
            {
                //Return a short string describing the purpose of this GHA library.
                return "";
            }
        }
        public override Guid Id
        {
            get
            {
                return new Guid("56b4d4c7-7aff-41bf-9b38-81dcff70d937");
            }
        }

        public override string AuthorName
        {
            get
            {
                //Return a string identifying you or your company.
                return "";
            }
        }
        public override string AuthorContact
        {
            get
            {
                //Return a string representing your preferred contact details.
                return "";
            }
        }
    }
}
