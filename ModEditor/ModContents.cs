using Microsoft.Xna.Framework.Graphics;
using ModEditor.Controls;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Xml;
using System.Xml.Serialization;
using WeifenLuo.WinFormsUI.Docking;

namespace ModEditor
{    
    /// <summary>
    /// Wraps item, i.e Tech, Device, or generic data. 
    /// Stores active ui information concerned to selected node
    /// </summary>
    public class Item : IDockContent
    {
        public Controller controller;
        public Object target;          // actual gamedata object
        public Item prev, next;

        public bool isBase = false;
        public bool isNameEditable = true;

        public string name = "";
        protected string sourcePath = "";

        public TreeNode node;           // Assigned tree node
        public TabPage page;            // Assigned tab page

        public DateTime fileTime;       // Cached time to check if file was modified

        protected bool isDirty = false;
        
        public event EventHandler dataChanged;
               
        public Item(Object item, Controller controller, string path)
        {
            target = item;
            this.controller = controller;
            this.sourcePath = path;
            fileTime = File.GetLastWriteTime(Path);

            dataChanged += Item_dataChanged;
        }

        public void RaiseDataChanged()
        {
            dataChanged(null, null);
        }

        void Item_dataChanged(object sender, EventArgs e)
        {
            if (HasVariants() && node != null)
                node.BackColor = System.Drawing.Color.YellowGreen;
            if (page != null)
                page.Name = Name;
        }

        public string Path
        {
            get
            {
                return controller.Mod.RootPath + "/" + sourcePath;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
        }

        public Item Next
        {
            get
            {
                return next;
            }
        }

        public Item Prev
        {
            get
            {
                return prev;
            }
        }

        public bool IsBase()
        {
            return controller.IsBase();
        }

        public bool OnTabClosed()
        {
            page = null;
            return true;
        }

        public bool Dirty
        {
            set
            {
                if (value)
                {
                    if (!IsBase())
                        controller.Mod.MarkDirty();
                }

                isDirty = value;
            }
        }

        // Set source path, relative to mod root folder
        public void SetSourcePath(string path)
        {
            sourcePath = path;
            fileTime = File.GetLastAccessTime(Path);           
        }
                
        public bool FileExists()
        {
            FileInfo info = new FileInfo(Path);
            return info.Exists;
        }

        public bool ModifiedOutside()
        {
            var newTime = File.GetLastWriteTime(Path);
            int result = newTime.CompareTo(fileTime);
            if (result > 0)
            {
                return true;
            }
            return false;
        }

        public void Refresh()
        {
            controller.ReloadItem(this);
        }

        public bool IsOverrided()
        {
            return next != null;
        }

        public virtual Object getTarget()
        {
            return target;
        }

        public ContextMenuStrip GenerateContextMenu()
        {
            return controller.GenerateContextMenu(this);
        }

        public Control GenerateControl()
        {
            return controller.GenerateControl(this);
        }

        // remove links with other items
        public void UnLink()
        {
            if (prev != null)
            {
                prev.next = next;
                prev.RaiseDataChanged();
            }
            if (next != null)
            {
                next.prev = prev;
                next.RaiseDataChanged();
            }
        }

        public bool HasVariants()
        {
            return Prev != null || Next != null;
        }
        
        public TabPage GetTabPage()
        {
            return page;
        }
    }

    /// <summary>
    /// Defines methods to interact with specific mod objects
    /// </summary>
    public abstract class Controller
    {
        public Item rootItem;
        protected string groupName = "unnamed";

        protected System.Type targetType;

        public bool isBase;

        /*
        public Dictionary<string, List<ModEditorAttribute>> customAttributes = new Dictionary<string, List<ModEditorAttribute>>();
         */
        // Local cache for items created by this controller/mod
        protected Dictionary<string, Item> localCache = new Dictionary<string, Item>();

        public List<string> fieldStringToken = new List<string>();

        protected ModContents mod;

        

        public Controller(ModContents mod)
        {
            this.mod = mod;
            rootItem = new Item(null, this, "");
            rootItem.controller = this;
            rootItem.isNameEditable = false;
        }

        public ModContents Mod
        {
            get
            {
                return mod;
            }
        }

        public System.Type TargetType
        {
            get
            {
                return targetType;
            }
        }

        public Controller GetFrontController()
        {
            return ModContents.GetMod().GetController(this.groupName);
        }

        public bool IsBase()
        {
            return isBase;
        }

        // try to set new item name
        public virtual bool RenameItem(Item item, string newName)
        {
            return false;
        }

        /*
        // Add custom field attribute to modify ItemExplorer sheet generation
        public void AddCustomAttribute(string fieldName, ModEditorAttribute attrib)
        {
            if (!customAttributes.ContainsKey(fieldName))
                customAttributes.Add(fieldName, new List<ModEditorAttribute>());
            customAttributes[fieldName].Add(attrib);
        }
        // Mark field as "string token"            
        public void OverrideFieldLocString(string fieldName)
        {
            PropertyGridExplorer.AddOverridedAttribute(targetType, fieldName, new LocStringToken());
        }
        // Mark field as "object reference"            
        public void OverrideFieldObjectReference(string fieldName, string group)
        {
            PropertyGridExplorer.AddOverridedAttribute(targetType, fieldName, new ObjectReference(group));
        }
        // Ignore field
        public void IgnoreField(string fieldName)
        {
            PropertyGridExplorer.AddOverridedAttribute(targetType, fieldName, new IgnoreByEditor());
        }*/

        public string GetGroupFolder()
        {
            return groupName;
        }

        // Do not sure what does it do
        public abstract void ClearCache();

        public virtual Dictionary<string, Item> GetItems()
        {
            return null;
        }

        public Dictionary<string, Item> GetLocalItems()
        {
            return localCache;
        }

        /*
        // Obtain all the attributes within spedified field
        public List<ModEditorAttribute> GetFieldAttribute(System.Reflection.FieldInfo fieldInfo)
        {
            List<ModEditorAttribute> result = new List<ModEditorAttribute>();

            // Get attributes from local override list
            if (customAttributes.ContainsKey(fieldInfo.Name))
            {
                var list = customAttributes[fieldInfo.Name];
                result.AddRange(list);
            }

            // Get attributes from class definition                
            foreach (ModEditorAttribute attribute in fieldInfo.GetCustomAttributes(true))
            {
                result.Add(attribute);
            }

            return result;
        }*/

        // Generate control to edit item contents. Most times it is ItemExplorer property grid
        public virtual Control GenerateControl(Item item)
        {
            if (item.Equals(rootItem))
                return null;
            if (targetType == null)
                return null;
            ItemView explorer = new ItemView();
            explorer.Init(targetType, item);
            return explorer;
        }

        public abstract void ObtainModData(string basePath, bool isBase);
        public abstract void PopulateModOverview(TreeNodeCollection root);
        public abstract void SaveAll(string dir);

        public abstract void UpdateItems();

        public virtual ContextMenuStrip GenerateContextMenu(Item item)
        {
            return null;
        }

        // Reloads item from file
        public virtual void ReloadItem(Item item)
        {            
        }

        // Opens system editor
        public virtual bool OpenItemFile(Item item)
        {
            if (item.Path != "")
            {
                try
                {                    
                    System.Diagnostics.Process.Start(item.Path);
                    return true;
                }
                catch (Exception e)
                {
                    MessageBox.Show("Cannot open editor for file \"" + item.Path + "\" : \n\n" + e.ToString());
                }
            }
            return false;
        }

        public virtual void CheckRootModifications(List<Item> modifiedItems)
        {
        }

        public void CheckCacheModifications(List<Item> modifiedItems)
        {            
            foreach (var record in GetLocalItems())
            {
                if (record.Value.ModifiedOutside())
                    modifiedItems.Add(record.Value);
            }
        }

        // Check group for modifications
        public virtual void CheckExternalModifications(List<Item> modifiedItems)
        {
            CheckRootModifications(modifiedItems);
            CheckCacheModifications(modifiedItems);
        }
    }

    public class ModContents
    {        
        // Editor references to other mods
        public List<ModContents> references;

        public interface Explorer
        {
            void ExploreItem(Item item);
        }

        ModContents next, prev;

        protected static ModContents lastMod;

        public bool mergeReferences;        

        public Explorer activeExplorer;

        public TreeView treeView;
        public TreeNode rootTreeNode;

        public Ship_Game.ModInformation modInfo;

        public Dictionary<string, Controller> controllers = new Dictionary<string, Controller>();

        public enum State
        {
            Empty,
            Unloaded,
            Loaded 
        }

        public State state = State.Empty;

        ModEditor.Controllers.ModInfoController modInfoController;
        ModEditor.Controllers.StringsController stringsController;

        // Directory with mod data
        protected string modRootPath = "Contents";

        public ModContents(TreeView treeView, ModContents baseMod)
        {
            this.treeView = treeView;
            prev = baseMod;

            if (baseMod != null)
            {
                if (baseMod.next != null)
                    throw new Exception("base mod is already linked");
                baseMod.next = this;
            }

            rootTreeNode = new TreeNode("Root Node");
            treeView.Nodes.Add(rootTreeNode);

            lastMod = this;

            InitControllers();
            /*             
             	Races
	            Encounters 
             * ToolTips
             */
        }

        public static ModContents CreateNewMod(TreeView treeView, string modPath, ModContents baseMod)
        {
            ModContents result = new ModContents(treeView, baseMod);
            result.modRootPath = modPath;
            result.modInfo = new Ship_Game.ModInformation()
            {
                ModName = Path.GetFileNameWithoutExtension(modPath)
            };
            result.UpdateModInfo();
            result.PopulateData(result.modRootPath, false);
            return result;
        }
        
        static public ModContents GetMod()
        {
            return lastMod;
        }

        void InitControllers()
        {
            modInfoController = new ModEditor.Controllers.ModInfoController(this);
            stringsController = new ModEditor.Controllers.StringsController(this);
            AddController(new ModEditor.Controllers.ArtifactsSpec(this));
            AddController(new ModEditor.Controllers.EventsGroup(this));
            AddController(new ModEditor.Controllers.HullsGroup(this));
            AddController(new ModEditor.Controllers.BuildingSpec(this));
            AddController(new ModEditor.Controllers.ModuleSpec(this));
            AddController(new ModEditor.Controllers.TechSpec(this));
            AddController(new ModEditor.Controllers.TexturesGroup(this));
            AddController(new ModEditor.Controllers.TroopSpec(this));
            AddController(new ModEditor.Controllers.WeaponGroup(this));
            AddController(new ModEditor.Controllers.ShipsGroup(this));

            AddController(modInfoController);
            AddController(stringsController);
        }

        public Controller GetController(string group)
        {
            return controllers[group];
        }

        public static string GetLocString(int index)
        {
            return Controllers.StringsController.GetLocString(index);
        }

        public void MarkDirty()
        {

        }

        public bool Loaded()
        {
            return state == State.Loaded;
        }

        void UpdateModInfo()
        {
            modInfoController.rootItem.target = modInfo;
            rootTreeNode.Name = modInfo.ModName;
            rootTreeNode.Text = modInfo.ModName;
        }

        protected bool LoadModInfo(string ModEntryPath)
        {
            try
            {
                FileInfo FI = new FileInfo(ModEntryPath);
                Stream file = FI.OpenRead();
                modInfo = (Ship_Game.ModInformation)Ship_Game.ResourceManager.ModSerializer.Deserialize(file);
                modRootPath = Path.GetDirectoryName(ModEntryPath) + "/" + Path.GetFileNameWithoutExtension(ModEntryPath);

                UpdateModInfo();                
                
                file.Close();
                file.Dispose();
                return true;
            }
            catch (Exception e)
            {
                MainForm.LogErrorString(e.Message);
            }
            return false;
        }

        protected bool SaveModInfo(string ModEntryPath)
        {
            try
            {
                FileInfo FI = new FileInfo(ModEntryPath);
                Stream file = FI.OpenWrite();
                Ship_Game.ResourceManager.ModSerializer.Serialize(file, modInfo);
                file.Close();
                file.Dispose();
                return true;
            }
            catch (Exception e)
            {
                MainForm.LogErrorString(e.Message);
            }
            return false;
        }

        // Change item field
        public static void ChangeItemValue(Item item, System.Reflection.FieldInfo fieldInfo, Object value)
        {
            // TODO: undo/redo handling
            fieldInfo.SetValue(item.target, value);
        }
        // Read value from item
        public static object ReadItemValue(Item item, System.Reflection.FieldInfo fieldInfo)
        {
            return fieldInfo.GetValue(item.target);
        }

        // Check all items for external modificatios
        public void CheckExternalModifications(List<Item> modifiedItems)
        {
            foreach (var record in controllers)
            {                
                record.Value.CheckExternalModifications(modifiedItems);
            }
        }

        public void SaveMod(string ModEntryPath)
        {
            if (RootPath.Equals("Content"))
                return;

            ModEntryPath = RootPath;
            SaveModInfo(ModEntryPath+".xml");
            string outputDir = ModEntryPath;
            System.IO.Directory.CreateDirectory(outputDir);
            foreach (var controller in controllers)
            {
                controller.Value.SaveAll(outputDir);
            }            
        }

        public void SetName(string Name)
        {
            if (modInfo != null)
                modInfo.ModName = Name;
            rootTreeNode.Name = Name;
            rootTreeNode.Text = Name;
        }

        protected void AddController(Controller controller)
        {
            controllers.Add(controller.GetGroupFolder(), controller);
        }

        public string RootPath
        {
            get
            {
                if (modInfo != null)
                    return modRootPath;
                return "Content";
            }
        }

        public void LoadMod(string ModEntryPath)
        {
            try
            {
                string modRootDirectory = "Mods";
                string modDirectory = modRootDirectory + "\\" + Path.GetFileNameWithoutExtension(ModEntryPath);
                LoadModInfo(ModEntryPath);
                // 1. Load contents                
                Ship_Game.ResourceManager.LoadMods(modDirectory);
                // 2. Populate treeview
                PopulateData(modDirectory, false);
                
                state = State.Loaded;
            }
            catch (Exception e)
            {
                MainForm.LogErrorString(e.Message);
            }
            UpdateUI();
        }
        
        public void Reset()
        {
            //rootBase.Nodes.Clear();// = null;
            rootTreeNode.Nodes.Clear();// = null;
            
            //treeView.Nodes.Clear();
            //treeView.Nodes.Add("Load mod to observe data");
            foreach (var controller in controllers)
            {
                controller.Value.ClearCache();
            }

            modInfo = null;
        }

        public void PopulateData(string basePath, bool isBase)
        {
            foreach (var controller in controllers)
            {
                controller.Value.ObtainModData(basePath, isBase);
                controller.Value.PopulateModOverview(rootTreeNode.Nodes);
            }                                  
        }

        public void UpdateUI()
        {
            foreach (var controller in controllers)
            {
                controller.Value.UpdateItems();                
            } 
        }

        #region Tools
        

        public Controller GetGroupController(string group)
        {
            if(controllers.ContainsKey(group))                
                return this.controllers[group];
            return null;
        }
        
        #endregion   
    }
}
