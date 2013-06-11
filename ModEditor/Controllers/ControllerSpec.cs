using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
namespace ModEditor.Controllers
{
    public abstract class ControllerSpec<Target> : ModEditor.Controller
    {
        protected string fileExtension = ".xml";
        protected int lastNewIndex = 0;
        // Global items cache            
        public static Dictionary<string, Item> globalCache = new Dictionary<string, Item>();
        
        public XmlSerializer serializer;

        XmlWriterSettings settings = new XmlWriterSettings() { OmitXmlDeclaration = true, Indent = true, IndentChars = "\t" };

        public ControllerSpec(ModContents mod)
            : base(mod)
        {
            targetType = typeof(Target);
            XmlAttributeOverrides overrides = new XmlAttributeOverrides();
            overrides.Add(typeof(Target), new XmlAttributes() { XmlIgnore = true });
            serializer = new XmlSerializer(typeof(Target));
        }

        public override Dictionary<string, Item> GetItems()
        {
            return globalCache;
        }

        public override void UpdateItems()
        {
            foreach (var record in localCache)
            {
                record.Value.UpdateUI();
            }
        }

        public override void ClearCache()
        {
            localCache.Clear();
        }

        // Generate new item name
        public String GetNewName(string baseName)
        {
            string result = "";
            while (true)
            {
                lastNewIndex++;
                result = baseName + lastNewIndex.ToString();
                if (!globalCache.ContainsKey(result))
                    return result;
            }
        }

        public override ContextMenuStrip GenerateContextMenu(Item item)
        {
            ContextMenuStrip result = new ContextMenuStrip();
            if (item.Equals(rootItem))
            {
                if (rootItem.isBase)
                    return null;
                result.Items.Add("New", null, OnNewItem);
            }
            else
            {
                result.Items.Add("Edit", null, (object sender, EventArgs e) => { this.OpenItemFile(item); });
                result.Items.Add("Copy", null, (object sender, EventArgs e) => { OnCopyItem(item); });
                if (!item.isBase)
                    result.Items.Add("Delete", null, (object sender, EventArgs e) => { OnDeleteItem(item); });
            }
            return result;
        }

        private void OnDeleteItem(Item item)
        {
            try
            {
                localCache.Remove(item.name);
                item.UnLink();
                if (item.HasVariants())
                    globalCache.Remove(item.name);
                // remove it from treeview
                if (item.node != null)
                    item.node.Remove();
                // remove it from open tabs
                if (item.page != null)
                    (item.page.Parent as TabControl).TabPages.Remove(item.page);
            }
            catch (Exception)
            {
            }
        }

        private void OnCopyItem(Item item)
        {
            if (item.isBase)
            {
                // TODO: create item for mod
            }
        }
        // Create context menu for treeview item selection
        /*
        public override Control GenerateControl(Item item)
        {
            if (item.Equals(rootItem))
                return null;
            ItemExplorer explorer = new ItemExplorer();
            Target targetObj = (Target)item.target;
            explorer.Init(item, targetObj);
            return explorer;
        }*/
        protected virtual Target CreateObject()
        {
            return default(Target);
        }

        private void OnNewItem(object sender, EventArgs e)
        {

        }

        public abstract Dictionary<string, Target> GetStorage();

        public virtual XmlSerializer GetSerializer()
        {
            return new XmlSerializer(typeof(Target));
        }

        public void SaveItem(Item item, string rootDir)
        {
            string path = rootDir + "/" + GetGroupFolder() + "/" + item.name + ".xml";
            // Ensure directory exists
            System.IO.Directory.CreateDirectory(Path.GetDirectoryName(path));
            FileInfo FI = new FileInfo(path);
            FI.Delete();
            var writer = XmlWriter.Create(FI.OpenWrite(), settings);
            Target value = (Target)item.target;
            serializer.Serialize(writer, value);
            writer.Close();
        }

        public override void Save(string rootDir)
        {
            XmlSerializer serializer = GetSerializer();


            if (serializer == null)
                return;
            var data = GetStorage();

            System.IO.Directory.CreateDirectory(rootDir + "/" + GetGroupFolder());

            foreach (var item in localCache)
            {
                SaveItem(item.Value, rootDir);
            }
        }
        /// <summary>
        /// Iterate through cache and show data
        /// </summary>
        /// <param name="root"></param>
        public override void PopulateModOverview(TreeNodeCollection root)
        {
            TreeNode groupNode = null;
            string folder = this.GetGroupFolder();
            if (root.ContainsKey(folder))
                groupNode = root[folder];
            else
            {
                groupNode = new TreeNode(folder);
                root.Add(groupNode);
            }

            groupNode.Tag = rootItem;
            rootItem.node = groupNode;

            foreach (var entry in localCache)
            {
                if (entry.Value.node == null)
                {
                    TreeNode node = new TreeNode(entry.Key);
                    entry.Value.node = node;
                    node.Tag = entry.Value;
                    groupNode.Nodes.Add(node);
                }
            }
        }
        /*
        public virtual string GetSourceExtension()
        {
            return ".xml";
        }*/
        /// <summary>
        /// Iterate through all items in dictionary and add them to overview
        /// </summary> 
        public override void ObtainModData(string basePath, bool isBase)
        {
            this.isBase = isBase;

            var data = GetStorage();

            foreach (var entry in data)
            {
                Item item = null;
                string itemPath = this.GetGroupFolder() + "/" + entry.Key + fileExtension;
                if (globalCache.ContainsKey(entry.Key))
                {
                    Item oldItem = globalCache[entry.Key];
                    // found the same item
                    if (oldItem.target.Equals(entry.Value))
                        continue;

                    item = CreateSpec(entry.Value, itemPath);
                    item.name = entry.Key;

                    localCache.Add(entry.Key, item);
                    item.prev = oldItem;
                    oldItem.next = item;

                    globalCache[entry.Key] = item;
                }
                else
                {
                    /// add item to both global and local cache
                    item = CreateSpec(entry.Value, itemPath);
                    item.name = entry.Key;
                    item.isBase = isBase;
                    globalCache.Add(entry.Key, item);
                    localCache.Add(entry.Key, item);
                }
            }
        }
    }

    public class WeaponGroup : ControllerSpec<Ship_Game.Gameplay.Weapon>
    {
        public WeaponGroup(ModContents mod)
            : base(mod)
        {
            this.groupName = "Weapons";

            //fieldStringToken.Add(
            serializer = Ship_Game.ResourceManager.weapon_serializer;
            OverrideFieldObjectReference("BeamTexture", "Textures");
            OverrideFieldObjectReference("ProjectileTexturePath", "Textures");           
        }

        public override Dictionary<string, Ship_Game.Gameplay.Weapon> GetStorage()
        {
            return Ship_Game.ResourceManager.WeaponsDict;
        }

        override protected Ship_Game.Gameplay.Weapon CreateObject()
        {
            return new Ship_Game.Gameplay.Weapon();
        }
    }

    public class TexturesGroup : ControllerSpec<Texture2D>
    {
        public TexturesGroup(ModContents mod)
            : base(mod)
        {
            fileExtension = ".xnb";
            groupName = "Textures";
            serializer = null;
        }

        public override Dictionary<string, Texture2D> GetStorage()
        {
            return Ship_Game.ResourceManager.TextureDict;
        }

        override protected Texture2D CreateObject()
        {
            return null;
        }

        public override void Save(string rootDir)
        {
            var data = GetStorage();

            System.IO.Directory.CreateDirectory(rootDir + "/" + GetGroupFolder());

            foreach (var record in localCache)
            {
                string path = rootDir + "/" + GetGroupFolder() + "/" + record.Key + this.fileExtension;
                var item = record.Value;
                if (item.FileExists())
                {
                    File.Copy(item.Path, path);
                }
                /*
                FileInfo FI = new FileInfo(path);
                FileStream stream = FI.OpenWrite();
                Target value = (Target)item.Value.target;
                serializer.Serialize(stream, value);
                stream.Close();
                stream.Dispose();*/
            }
        }
    }

    public class EventsGroup : ControllerSpec<Ship_Game.ExplorationEvent>
    {
        public EventsGroup(ModContents mod)
            : base(mod)
        {
            //serializer = Ship_Game.ResourceManager.weapon_serializer;
            groupName = "Exploration Events";
        }

        public override Dictionary<string, Ship_Game.ExplorationEvent> GetStorage()
        {
            return Ship_Game.ResourceManager.EventsDict;
        }
        override protected Ship_Game.ExplorationEvent CreateObject()
        {
            return new Ship_Game.ExplorationEvent();
        }
    }

    /*
    public class RacesGroup : ControllerSpec<Ship_Game.EmpireData>
    {
        Dictionary<string, Ship_Game.EmpireData> data = new Dictionary<string, Ship_Game.EmpireData>();

        public override Dictionary<string, Ship_Game.EmpireData> GetStorage()
        {
            foreach (var empire in Ship_Game.ResourceManager.Empires)
            {
                
            }
            return data;
        }

        public override string GetGroupFolder()
        {
            return "Races";
        }
    }
    */
    public class HullsGroup : ControllerSpec<Ship_Game.ShipData>
    {
        public HullsGroup(ModContents mod)
            : base(mod)
        {
            groupName = "Hulls";
            //OverrideFieldObjectReference("IconPath", "Textures");
        }

        public override Dictionary<string, Ship_Game.ShipData> GetStorage()
        {
            return Ship_Game.ResourceManager.HullsDict;
        }
    }

    public class TechSpec : ControllerSpec<Ship_Game.Technology>
    {
        public TechSpec(ModContents mod)
            : base(mod)
        {
            groupName = "Technology";
            this.OverrideFieldLocString("DescriptionIndex");
            this.OverrideFieldLocString("NameIndex");
            //OverrideFieldObjectReference("IconPath", "Textures");
        }

        override public Dictionary<string, Ship_Game.Technology> GetStorage()
        {
            return Ship_Game.ResourceManager.TechTree;
        }
    }

    public class TroopSpec : ControllerSpec<Ship_Game.Troop>
    {
        public TroopSpec(ModContents mod)
            : base(mod)
        {
            groupName = "Troops";
            //OverrideFieldObjectReference("Icon", "Textures");
            OverrideFieldObjectReference("TexturePath", "Textures");
        }

        override public Dictionary<string, Ship_Game.Troop> GetStorage()
        {
            return Ship_Game.ResourceManager.TroopsDict;
        }
    }

    public class BuildingSpec : ControllerSpec<Ship_Game.Building>
    {
        public BuildingSpec(ModContents mod)
            : base(mod)
        {
            groupName = "Buildings";
            XmlAttributeOverrides overrides = new XmlAttributeOverrides();
            //overrides.Add(targetType, "weapon", new XmlAttributes() { XmlIgnore = true });
            serializer = new XmlSerializer(typeof(Ship_Game.Building), overrides);

            OverrideFieldLocString("NameTranslationIndex");
            OverrideFieldLocString("DescriptionIndex");
            OverrideFieldLocString("ShortDescriptionIndex");
            OverrideFieldObjectReference("Weapon", "Weapons");
            //OverrideFieldObjectReference("Icon", "Textures");           
        }
        override public Dictionary<string, Ship_Game.Building> GetStorage()
        {
            return Ship_Game.ResourceManager.BuildingsDict;
        }
    }

    public class ArtifactsSpec : ControllerSpec<Ship_Game.Artifact>
    {
        public ArtifactsSpec(ModContents mod)
            : base(mod)
        {
            groupName = "Artifacts";
            this.OverrideFieldLocString("DescriptionIndex");
            this.OverrideFieldLocString("NameIndex");
           // OverrideFieldObjectReference("Icon", "Textures");
        }
        override public Dictionary<string, Ship_Game.Artifact> GetStorage()
        {
            return Ship_Game.ResourceManager.ArtifactsDict;
        }
    }

    public class ModuleSpec : ControllerSpec<Ship_Game.Gameplay.ShipModule>
    {
        public ModuleSpec(ModContents mod)
            : base(mod)
        {
            groupName = "ShipModules";

            this.OverrideFieldLocString("DescriptionIndex");
            this.OverrideFieldLocString("NameIndex");
            this.IgnoreField("ModuleCenter");
            this.IgnoreField("installedSlot");
            this.IgnoreField("hangarShip");
            this.IgnoreField("LinkedModulesList");
            this.OverrideFieldObjectReference("WeaponType", "Weapons");

            //OverrideFieldObjectReference("IconTexturePath", "Textures");            
        }
        /*
        public override XmlSerializer GetSerializer()
        {
            return Ship_Game.ResourceManager.ModSerializer;
        } */

        override public Dictionary<string, Ship_Game.Gameplay.ShipModule> GetStorage()
        {
            return Ship_Game.ResourceManager.ShipModulesDict;
        }
    }
}
