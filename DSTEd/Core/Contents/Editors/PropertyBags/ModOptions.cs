using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSTEd.Core.Contents.Editors.PropertyBags
{
	internal class ModOptions : Properties
	{
		public ModOptions(string title, string icon, string description, Klei.Data.ModInfo info) : base(title, icon, description)
		{
			//Add properties
		}

		public override void Save()
		{

		}
	}
}
