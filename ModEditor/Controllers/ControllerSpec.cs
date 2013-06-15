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

        // Settings for XML writer. Mostly cosmetic
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
                record.Value.RaiseDataChanged();
            }
        }

        // Reloads item from file
        public override void ReloadItem(Item item)
        {
            var storage = GetStorage();
            
            FileInfo info = new FileInfo(item.Path);

            try
            {
                using (FileStream stream = info.OpenRead())
                {
                    Target data = (Target)serializer.Deserialize(stream);
                    if (data != null)
                    {
                        item.target = data;
                    }
                }
            }
            catch (Exception ex)
            {
            }
            var newTime = info.LastWriteTime;
            item.fileTime = newTime;
            item.RaiseDataChanged();
        }

        public override bool RenameItem(Item item, string newName)
        {
            if(!localCache.ContainsKey(item.name) || !globalCache.ContainsKey(item.name))
            {
                return false;
                //throw (new ArgumentException("Item does not exists in current group"));
            }
            else if (localCache.ContainsKey(newName) || globalCache.ContainsKey(newName))
            {
                return false;
                //throw (new ArgumentException("Name already exists within group"));
            }
            else if (!item.name.Equals(newName))
            {
                string oldName = item.name;
                item.name = newName;
                // 1. Update local cache
                localCache.Remove(oldName);
                localCache.Add(newName, item);

                if (item.Next == null)                
                {
                    // 2. Update global cache
                    globalCache.Remove(oldName);
                    globalCache.Add(newName, item);
                    // 3. Update game cache
                    var data = GetStorage();
                    data.Remove(oldName);
                    data.Add(newName, (Target)item.target);                   
                }

                item.RaiseDataChanged();
                return true;
            }
            return false;
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
                result.Items.Add("New", null, OnMenuNewItem);
            }
            else
            {                
                result.Items.Add("External Editor", null, (object sender, EventArgs e) => { this.OpenItemFile(item); });
                result.Items.Add("Refresh", null, (object sender, EventArgs e) => { item.Refresh(); });
                result.Items.Add(new ToolStripSeparator());
                result.Items.Add("Rename", null, (object sender, EventArgs e) => { this.OpenItemFile(item); });
                result.Items.Add("New", null, (object sender, EventArgs e) => { OnMenuNewBased(item); });
                if (!item.isBase)
                    result.Items.Add("Delete", null, (object sender, EventArgs e) => { OnMenuDeleteItem(item); });
                
                if (item.Next != null || item.Prev != null)
                {
                    result.Items.Add(new ToolStripSeparator());
                    if (item.Next != null)
                        result.Items.Add("Next", null, (object sender, EventArgs e) => { OnMenuGotoItem(item, true); });
                    if (item.Prev != null)
                        result.Items.Add("Prev", null, (object sender, EventArgs e) => { OnMenuGotoItem(item, false); });
                }
            }
            return result;
        }

        private void OnMenuGotoItem(Item item, bool next)
        {
            if (item.node == null)
                return;
            TreeView treeView = item.node.TreeView;
            Item gotoItem = next ? item.Next : item.Prev;
            if (gotoItem != null && gotoItem.node != null)
                MainForm.SelectItem(gotoItem);
        }

        private void OnMenuRenameItem(Item item)
        {
            if (item.node == null)
                return;
            TreeView treeView = item.node.TreeView;
            treeView.SelectedNode = item.node;
            item.node.BeginEdit();
        }

        private void OnMenuDeleteItem(Item item)
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

        private void OnMenuNewItem(object sender, EventArgs e)
        {
            Target result = CreateObject();

            if (result != null)
            {
                Item item = CreateItem(GetNewName(targetType.FullName), result);
                item.SetSourcePath(item.name + this.fileExtension);

                if (item != null && rootItem.node != null)
                {
                    TreeNode node = new TreeNode(item.name);
                    item.node = node;
                    node.Tag = item;
                    rootItem.node.Nodes.Add(node);
                }
            }
        }

        private void OnMenuNewBased(Item item)
        {
            Object result = Tools.Clone(item.target, targetType);

            if (result == null)
                return;            
            
            ControllerSpec<Target> frontController = this.GetFrontController() as ControllerSpec<Target>;
            Item newItem = frontController.CreateItem(item.name, result);
            if (newItem != null && frontController.rootItem.node != null)
            {
                TreeNode node = new TreeNode(newItem.name);
                newItem.node = node;
                node.Tag = newItem;
                frontController.rootItem.node.Nodes.Add(node);
            }
            newItem.RaiseDataChanged();
            item.RaiseDataChanged();
        }

        // Create Target object
        protected virtual Target CreateObject()
        {
            System.Reflection.ConstructorInfo constructor = targetType.GetConstructor(new Type[] { });
            if (constructor != null)
            {
                return (Target)constructor.Invoke(new Object[] { });
            }
            return default(Target);
        }

        // Access to game data storage
        public abstract Dictionary<string, Target> GetStorage();

        // Access to serializer. Not viable for textures and models
        public virtual XmlSerializer GetSerializer()
        {
            return new XmlSerializer(targetType);
        }

        // Save single (XML) item. Not viable for texture or model
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

        // Save all (XML) items. Not viable for texture or model
        public override void SaveAll(string rootDir)
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
        /// Iterate through local cache and create mod overview node for each item
        /// </summary>
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

        // Create item and store it in both local and global cache
        public Item CreateItem(string name, object data)
        {
            string itemPath = this.GetGroupFolder() + "/" + name + fileExtension;
            Item item = null;
            if (globalCache.ContainsKey(name))
            {
                Item oldItem = globalCache[name];
                // found the same item
                if (oldItem.target.Equals(data))
                    return null;

                item = new Item(data, this, itemPath);
                item.name = name;

                localCache.Add(name, item);
                item.prev = oldItem;
                oldItem.next = item;

                globalCache[name] = item;
            }
            else
            {
                /// add item to both global and local cache
                item = new Item(data, this, itemPath);
                item.name = name;
                item.isBase = isBase;
                globalCache.Add(name, item);
                localCache.Add(name, item);
            }
            return item;
        }

        /// <summary>
        /// Iterate through all items in dictionary and add them to overview
        /// </summary> 
        public override void ObtainModData(string basePath, bool isBase)
        {
            this.isBase = isBase;

            var data = GetStorage();

            foreach (var entry in data)
                CreateItem(entry.Key, entry.Value);
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
            /*
            OverrideFieldObjectReference("BeamTexture", "Textures");
            OverrideFieldObjectReference("ProjectileTexturePath", "Textures");           */
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

        public override void ReloadItem(Item item)
        {

        }

        public override void SaveAll(string rootDir)
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
            /*
            this.OverrideFieldLocString("DescriptionIndex");
            this.OverrideFieldLocString("NameIndex");*/
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
            /*
            //OverrideFieldObjectReference("Icon", "Textures");
            OverrideFieldObjectReference("TexturePath", "Textures");*/
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
            /*
            OverrideFieldLocString("NameTranslationIndex");
            OverrideFieldLocString("DescriptionIndex");
            OverrideFieldLocString("ShortDescriptionIndex");
            OverrideFieldObjectReference("Weapon", "Weapons");
            //OverrideFieldObjectReference("Icon", "Textures");   */        
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
            /*
            this.OverrideFieldLocString("DescriptionIndex");
            this.OverrideFieldLocString("NameIndex");*/
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
            /*
            this.OverrideFieldLocString("DescriptionIndex");
            this.OverrideFieldLocString("NameIndex");
            this.IgnoreField("ModuleCenter");
            this.IgnoreField("installedSlot");
            this.IgnoreField("hangarShip");
            this.IgnoreField("LinkedModulesList");
            this.IgnoreField("Center");
            this.IgnoreField("moduleCenter");
            this.OverrideFieldObjectReference("WeaponType", "Weapons");
*/
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
