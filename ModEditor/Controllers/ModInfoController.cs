using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ModEditor.Controllers
{
    public class ModInfoController : ModEditor.Controller
    {
        public ModInfoController(ModContents mod)
            : base(mod)
        {
            rootItem.name = "ModInfo";
            groupName = "ModInfo";
            targetType = typeof(Ship_Game.ModInformation);
            /*
            OverrideFieldObjectReference("ModImagePath_1920x1280", "Textures");
            OverrideFieldObjectReference("PortraitPath", "Textures");*/
        }

        public override void ClearCache()
        {

        }

        private void OnNewItem(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        public override Control GenerateControl(ModEditor.Item item)
        {
            if (item.target == null)
                return null;
            ItemView explorer = new ItemView();
            Ship_Game.ModInformation targetObj = (Ship_Game.ModInformation)item.target;
            explorer.Init(typeof(Ship_Game.ModInformation), item);
            return explorer;
        }

        static string GetDirShortName(string path)
        {
            var separators = new char[] {
              Path.DirectorySeparatorChar,  
              Path.AltDirectorySeparatorChar  
            };

            string[] entries = null;
            entries = path.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            /*
            if (Directory.Exists(path))
            {
                
            }
            else if (File.Exists(path))
            {
                entries = Path.GetDirectoryName(path).Split(separators, StringSplitOptions.RemoveEmptyEntries);
            }
            else
            {
                return "";
            }*/
            return entries[entries.Length - 1];
        }
        public override void ObtainModData(string basePath, bool isBase)
        {
            this.isBase = isBase;
            string entryPath = GetDirShortName(basePath);
            rootItem = new Item(mod.modInfo, this, isBase ? "" : "../" + entryPath + ".xml");
            rootItem.name = "Mod Info";
        }

        public override void PopulateModOverview(TreeNodeCollection root)
        {
            if (rootItem == null)
                return;
            TreeNode groupNode = null;
            string folder = "ModInfo";
            if (root.ContainsKey(folder))
                groupNode = root[folder];
            else
            {
                groupNode = new TreeNode(folder);
                root.Add(groupNode);
            }
            groupNode.Tag = rootItem;
            rootItem.node = groupNode;

        }

        public override void CheckDataIntegrity(Action<ItemReport> reporter)
        {
            CheckDataImpl(reporter, rootItem, "", rootItem.target, targetType, null);
        }

        public override void SaveAll(string dir)
        {
        }

        public override void UpdateItems()
        {
        }
    }
}
