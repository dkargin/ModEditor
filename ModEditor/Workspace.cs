using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace ModEditor
{
    public class Workspace
    {
        /// <summary>
        /// Wraps item, i.e Tech or Device, stores active ui information concerned to selected node
        /// </summary>
        public class ItemBase
        {
            public Controller controller;
            public Object target;

            public ItemBase(Object item, Controller controller)
            {
                target = item;
                this.controller = controller;
            }

            public virtual bool Modded()
            {
                return false;
            }

            public virtual Object getTarget()
            {
                return target;
            }

            public Control GenerateControl()
            {
                return controller.GenerateControl(this);
            }


            public string name;
            public TreeNode node;       /// Assigned tree node
            public TabPage page;        /// Assigned tab page
        }

        /// <summary>
        /// Defines methods to interact with specific mod objects
        /// </summary>
        public abstract class Controller
        {
            public virtual ItemBase CreateSpec(Object obj)
            {
                return new ItemBase(obj, this);
            }

            public virtual string GetGroupFolder()
            {
                return ".";
            }
            public abstract Control GenerateControl(ItemBase item);
            public abstract void PopulateModOverview(TreeNodeCollection root);
        }

        public abstract class ControllerSpec<Target> : Controller
        {
            public abstract Dictionary<string, Target> GetStorage();

            public override Control GenerateControl(ItemBase item)
            {
                ItemExplorer explorer = new ItemExplorer();
                Target targetObj = (Target)item.target;
                explorer.Init(targetObj);
                return explorer;
            }
            /// <summary>
            /// Iterate through all items in dictionary and add them to overview
            /// </summary> 
            public override void PopulateModOverview(TreeNodeCollection root)
            {
                string GroupName = GetGroupFolder();
                var data = GetStorage();
                TreeNode[] nodes = root.Find(GroupName, false);
                TreeNode groupRoot = null;
                if (nodes != null && nodes.Length > 0)
                    groupRoot = nodes[0];
                if (groupRoot == null)
                {
                    groupRoot = new TreeNode(GroupName);
                    root.Add(groupRoot);
                }
                groupRoot.Nodes.Clear();
                foreach (var entry in data)
                {
                    TreeNode node = new TreeNode(entry.Key);
                    ItemBase item = CreateSpec(entry.Value);// new ItemSpec<Type>();
                    item.name = entry.Key;
                    item.node = node;

                    node.Tag = item;
                    groupRoot.Nodes.Add(node);
                }
            }
        }

        public class WeaponGroup : ControllerSpec<Ship_Game.Gameplay.Weapon>
        {
            static XmlSerializer serializer = Ship_Game.ResourceManager.weapon_serializer;
            static string groupFolder = "Weapons";

            public void Serialize(string modPath)
            {
                /*
                string path = modPath + groupFolder + this.name;

                FileInfo FI = new FileInfo(path);
                Stream file = FI.OpenWrite();
                serializer.Serialize(file, target);*/
            }

            public void WriteAll(string modPath)
            {
            }

            #region Static overrides
            public override Dictionary<string, Ship_Game.Gameplay.Weapon> GetStorage()
            {
                return Ship_Game.ResourceManager.WeaponsDict;
            }

            public override string GetGroupFolder()
            {
                return "Weapons";
            }            
            #endregion

        }

        public class TechSpec : ControllerSpec<Ship_Game.Technology>
        {
            #region Static overrides
            override public Dictionary<string, Ship_Game.Technology> GetStorage()
            {
                return Ship_Game.ResourceManager.TechTree;
            }

            public override string GetGroupFolder()
            {
                return "Technology";
            }

            #endregion
        }

        public class TroopSpec : ControllerSpec<Ship_Game.Troop>
        {
            #region Static overrides
            override public Dictionary<string, Ship_Game.Troop> GetStorage()
            {
                return Ship_Game.ResourceManager.TroopsDict;
            }

            override public string GetGroupFolder()
            {
                return "Troops";
            }

            #endregion
        }

        public class BuildingSpec : ControllerSpec<Ship_Game.Building>
        {
            #region Static overrides
            override public Dictionary<string, Ship_Game.Building> GetStorage()
            {
                return Ship_Game.ResourceManager.BuildingsDict;
            }

            override public string GetGroupFolder()
            {
                return "Buildings";
            }
            
            #endregion
        }

        public class ArtifactsSpec : ControllerSpec<Ship_Game.Artifact>
        {
            #region Static overrides
            override public Dictionary<string, Ship_Game.Artifact> GetStorage()
            {
                return Ship_Game.ResourceManager.ArtifactsDict;
            }

            override public string GetGroupFolder()
            {
                return "Artifacts";
            }

            #endregion
        }

        public class ModuleSpec : ControllerSpec<Ship_Game.Gameplay.ShipModule>
        {
            #region Static overrides
            override public Dictionary<string, Ship_Game.Gameplay.ShipModule> GetStorage()
            {
                return Ship_Game.ResourceManager.ShipModulesDict;
            }

            override public string GetGroupFolder()
            {
                return "Artifacts";
            }

            #endregion
        }
    }
}
