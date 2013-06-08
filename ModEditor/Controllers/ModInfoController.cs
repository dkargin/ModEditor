using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ModEditor.Controllers
{
    public class ModInfoController : ModEditor.ModContents.Controller
    {
        public ModInfoController(ModContents mod)
            : base(mod)
        {
            rootItem.name = "ModInfo";
            groupName = "ModInfo";
        }

        public override void ClearCache()
        {

        }

        private void OnNewItem(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        public override Control GenerateControl(ModEditor.ModContents.Item item)
        {
            if (item.target == null)
                return null;
            ItemExplorer explorer = new ItemExplorer();
            Ship_Game.ModInformation targetObj = (Ship_Game.ModInformation)item.target;
            explorer.Init(item, targetObj);
            return explorer;
        }

        public override void ObtainModData(string basePath, bool isBase)
        {
            this.isBase = isBase;
            rootItem.SetPath(basePath);
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

        public override void Save(string dir)
        {
        }

        public override void UpdateItems()
        {
        }
    }
}
