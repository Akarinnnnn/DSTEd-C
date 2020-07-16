using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DSTEd.Core.Contents.Editors.PropertyBags
{
	internal class ModBasicInfo : Properties
	{
		Document document;
		public ModBasicInfo(string title, string icon, string description, Document document, Klei.Data.ModInfo info) : base(title, icon, description)
		{
			this.document = document;

            // Build Properties-Editor
            AddCategory(I18N.__("Informations"));
            AddEntry("name", I18N.__("Name"), Type.STRING, info.GetName());
            AddEntry("version", I18N.__("Version"), Type.STRING, info.GetVersion());
            AddEntry("description", I18N.__("Description"), Type.TEXT, info.GetDescription());
            AddEntry("author", I18N.__("Author"), Type.STRING, info.GetAuthor());
            AddEntry("forumthread", I18N.__("Forum-Thread"), Type.URL, info.GetForumThread());
            AddEntry("server_filter_tags", I18N.__("Tags"), Type.STRINGLIST, info.GetTags());

            AddCategory(I18N.__("Assets"));
            AddEntry("icon_atlas", I18N.__("Icon (Atlas)"), Type.ATLAS, info.GetIconAtlas());
            AddEntry("icon", I18N.__("Icon (Texture)"), Type.KTEX, info.GetIcon());

            AddCategory(I18N.__("System"));
            AddEntry("priority", I18N.__("Priority"), Type.NUMBER, info.GetPriority());
            AddEntry("api_version", I18N.__("API Version"), Type.SELECTION, new Selection(new Dictionary<object, string> {
                   { 6, "Don't Starve" },
                   { 10, "Don't Starve Together" }
                }, info.GetAPIVersion()));

            // Only add outdated dst_api_version if it's exists on the modinfo.
            if (info.GetDSTAPIVersion() > 0)
            {
                AddEntry("dst_api_version", I18N.__("API Version (DST)"), Type.SELECTION, new Selection(new Dictionary<object, string> {
                       { 6, "6" },
                       { 10, "10" }
                    }, info.GetDSTAPIVersion()));
            }

            AddCategory(I18N.__("Compatiblity"));
            AddEntry("dont_starve_compatible", I18N.__("Don't Starve"), Type.YESNO, info.IsDS());
            AddEntry("dst_compatible", I18N.__("Don't Starve Together"), Type.YESNO, info.IsDST());
            AddEntry("reign_of_giants_compatible", I18N.__("Reign of Giants"), Type.YESNO, info.IsROG());
            AddEntry("shipwrecked_compatible", I18N.__("Shipwrecked"), Type.YESNO, info.IsSW());

            AddCategory(I18N.__("Requirements"));
            AddEntry("standalone", I18N.__("Standalone"), Type.YESNO, info.ModsAllowed());
            AddEntry("client_only_mod", I18N.__("Only Client"), Type.YESNO, info.IsOnlyClient());
            AddEntry("all_clients_require_mod", I18N.__("All Clients Required"), Type.YESNO, info.IsRequired());
            AddEntry("restart_require", I18N.__("Restart Required"), Type.YESNO, info.MustRestart());

        }

        public override void Save()
		{
			CheckSaveable();
			using (StreamWriter fileStream = new StreamWriter(new FileStream(document.GetFilename(), FileMode.Append)))
			{

			}
		}
	}
}
