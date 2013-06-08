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

namespace ModEditor
{    
    public class ModContents
    {        
        // Editor references to other mods
        public List<ModContents> references;

        public interface Explorer
        {
            void ExploreItem(ModEditor.ModContents.Item item);
        }

        public bool mergeReferences;        

        public Explorer activeExplorer;

        public TreeView treeView;
        public TreeNode rootTreeNode;

        public Ship_Game.ModInformation modInfo;

        public Dictionary<string, ModContents.Controller> controllers = new Dictionary<string, ModContents.Controller>();

        public enum State
        {
            Empty,
            Unloaded,
            Loaded 
        }
        // Writes data to temporary file
        public class SafeWriter
        {
            FileStream stream;

            public FileStream Stream
            {
                get
                {
                    return stream;
                }
            }

            //public FI.OpenRead();
            FileInfo tmpFileInfo;
            FileInfo fileInfo;

            public SafeWriter(FileInfo info)
            {
                this.fileInfo = info;
                tmpFileInfo = new FileInfo(info.FullName + ".tmp");
                try
                {
                    tmpFileInfo.Delete();
                }catch(Exception)
                {
                }
                stream = tmpFileInfo.OpenWrite();
            }
            // close stream and copy data from temporary file, deleting temporary file
            public void Finish()
            {
                if (stream != null)
                {
                    stream.Close();
                    stream.Dispose();                    
                    tmpFileInfo.CopyTo(fileInfo.FullName, true);
                    tmpFileInfo.Delete();
                }
            }
        }

        public State state = State.Empty;

        ModEditor.Controllers.ModInfoController modInfoController;
        ModEditor.Controllers.StringsController stringsController;

        // Directory with mod data
        protected string modRootPath = "Contents";

        public ModContents(TreeView treeView)
        {
            this.treeView = treeView;
            
            rootTreeNode = new TreeNode("Root Node");
            treeView.Nodes.Add(rootTreeNode);

            InitControllers();
            /*             
             	Races
	            Encounters 
             * ToolTips
             */
        }

        public static ModContents CreateNewMod(TreeView treeView, string modPath)
        {
            ModContents result = new ModContents(treeView);
            result.modRootPath = modPath;
            result.modInfo = new Ship_Game.ModInformation()
            {
                ModName = Path.GetFileNameWithoutExtension(modPath)
            };
            result.UpdateModInfo();
            result.PopulateData(result.modRootPath, false);
            return result;
        }
        /*
        static ModContents LoadMod(T)
        {
        }*/

        void InitControllers()
        {
            modInfoController = new ModEditor.Controllers.ModInfoController(this);
            stringsController = new ModEditor.Controllers.StringsController(this);
            AddController(new ModContents.ArtifactsSpec(this));
            AddController(new ModContents.EventsGroup(this));
            AddController(new ModContents.HullsGroup(this));
            AddController(new ModContents.BuildingSpec(this));
            AddController(new ModContents.ModuleSpec(this));
            AddController(new ModContents.TechSpec(this));
            AddController(new ModContents.TexturesGroup(this));
            AddController(new ModContents.TroopSpec(this));
            AddController(new ModContents.WeaponGroup(this));

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
        public void CheckExternalModifications()
        { 
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
                controller.Value.Save(outputDir);
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

        public static FileInfo[] GetFilesFromDirectory(string DirPath)
        {
            DirectoryInfo Dir = new DirectoryInfo(DirPath);
            FileInfo[] result;
            try
            {
                FileInfo[] FileList = Dir.GetFiles("*.*", SearchOption.AllDirectories);

                result = FileList;
            }
            catch
            {
                result = new FileInfo[0];
            }
            return result;
        }

        public static DirectoryInfo[] GetDirectoriesFromDirectory(string DirPath)
        {
            DirectoryInfo Dir = new DirectoryInfo(DirPath);
            DirectoryInfo[] result;
            try
            {
                DirectoryInfo[] DirList = Dir.GetDirectories();//("*.*", SearchOption.AllDirectories);

                result = DirList;
            }
            catch
            {
                result = new DirectoryInfo[0];
            }
            return result;
        }

        private const int TVIF_STATE = 0x8;
        private const int TVIS_STATEIMAGEMASK = 0xF000;
        private const int TV_FIRST = 0x1100;
        private const int TVM_SETITEM = TV_FIRST + 63;

        [StructLayout(LayoutKind.Sequential, Pack = 8, CharSet = CharSet.Auto)]
        private struct TVITEM
        {
            public int mask;
            public IntPtr hItem;
            public int state;
            public int stateMask;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpszText;
            public int cchTextMax;
            public int iImage;
            public int iSelectedImage;
            public int cChildren;
            public IntPtr lParam;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam,
                                                 ref TVITEM lParam);

        /// <summary>
        /// Hides the checkbox for the specified node on a TreeView control.
        /// </summary>
        public static void HideCheckBox(TreeNode node)
        {
            TreeView tvw = node.TreeView;
            if (tvw == null)
                return;
            TVITEM tvi = new TVITEM();
            tvi.hItem = node.Handle;
            tvi.mask = TVIF_STATE;
            tvi.stateMask = TVIS_STATEIMAGEMASK;
            tvi.state = 0;
            SendMessage(tvw.Handle, TVM_SETITEM, IntPtr.Zero, ref tvi);            
        }
       
        static public string EmbedToString<Type>(string source, Type obj)
        {
            Regex rx = new Regex("\\$\\{(\\w+)\\.(\\w+)\\}"); 
           
            var TypeInfo = typeof(Type);

            /// for each match calls lambda to evaluate replacement string
            string result = rx.Replace(source, (Match match)=>
            {
                string className = match.Groups[1].Value;
                string fieldName = match.Groups[2].Value;
                if (className.Equals(TypeInfo.Name))
                {
                    try
                    {
                        /// Obtain string value from specified field
                        return TypeInfo.GetField(fieldName).GetValue(obj).ToString();
                    }
                    catch (Exception)
                    {
                        /// Cannot access <fieldName>
                    }
                }
                else
                {
                /// Do not do anything with data from other class
                }
                return match.ToString();
            });           
            
            return result;        
        }
        #endregion
        /// <summary>
        /// Wraps item, i.e Tech, Device, or generic data. 
        /// Stores active ui information concerned to selected node
        /// </summary>
        public class Item
        {
            public Controller controller;
            public Object target;            
            public Item prev, next;
            
            public bool isBase;
            public string name;
            protected string sourcePath;

            public TreeNode node;       // Assigned tree node
            public TabPage page;        // Assigned tab page

            public DateTime fileTime;   // Cached time to check if file was modified

            public string Path
            {
                get
                {
                    return sourcePath;
                }
            }            

            public Item(Object item, Controller controller, string path)
            {
                target = item;
                this.controller = controller;
                this.sourcePath = path;
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

            // Set source path. TODO: replace this function
            public void SetPath(string path)
            {
                sourcePath = path;
                try
                {
                    fileTime = new FileInfo(path).LastWriteTime;
                }
                catch (Exception)
                {
                }
            }

            public bool FileExists()
            {
                FileInfo info = new FileInfo(sourcePath);
                return info.Exists;
            }

            public bool ModifiedOutside()
            {
                FileInfo info = new FileInfo(sourcePath);
                var newTime = info.LastWriteTime;
                if (newTime.CompareTo(fileTime) > 0)
                {
                    return true;
                }
                return false;
            }

            public void Refresh()
            {
                controller.ReloadItem(this);
            }

            public bool Modded()
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
                    prev.next = next;
                if (next != null)
                    next.prev = prev;
            }

            public bool HasVariants()
            {
                return prev != null || next != null;
            }

            public void UpdateUI()
            {
                if (Modded() && node != null)
                    node.BackColor = System.Drawing.Color.YellowGreen;
            }
             
            public string GetPath()
            {
                return sourcePath;
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

            public bool isBase;

            public Dictionary<string, List<ModEditor.Controls.EditorManager.ModEditorAttribute>> customAttributes = new Dictionary<string, List<ModEditor.Controls.EditorManager.ModEditorAttribute>>();

            public List<string> fieldStringToken = new List<string>();

            public bool IsBase()
            {
                return isBase;
            }

            protected ModContents mod;

            public Controller(ModContents mod)
            {
                this.mod = mod;
                rootItem = new Item(null, this, "");
                rootItem.controller = this;
            }

            public void AddCustomAttribute(string fieldName, EditorManager.ModEditorAttribute attrib)
            {
                if (!customAttributes.ContainsKey(fieldName))
                    customAttributes.Add(fieldName, new List<EditorManager.ModEditorAttribute>());
                customAttributes[fieldName].Add(attrib);
            }

            
            // Mark field as "string token"            
            public void AddLocString(string fieldName)
            {
                AddCustomAttribute(fieldName, new EditorManager.LocStringToken());
            }

            // Mark field as "object reference"            
            public void AddLocString(string fieldName, string group)
            {
                AddCustomAttribute(fieldName, new EditorManager.ObjectReference(group));
            }

            public virtual Control GenerateControl(Item item)
            {
                return null;
            }

            public virtual Item CreateSpec(Object obj, string path)
            {
                return new Item(obj, this, path);
            }
            
            public string GetGroupFolder()
            {
                return groupName;
            }

            public abstract void ClearCache();

            protected virtual bool SkipField(System.Reflection.FieldInfo fieldInfo)
            {
                return false;
            }

            protected List<EditorManager.ModEditorAttribute> GetFieldAttribute(System.Reflection.FieldInfo fieldInfo)
            {
                List<EditorManager.ModEditorAttribute> result = new List<EditorManager.ModEditorAttribute>();

                if (customAttributes.ContainsKey(fieldInfo.Name))
                {
                    var list = customAttributes[fieldInfo.Name];
                    result.AddRange(list);
                }

                try
                {
                    foreach (EditorManager.ModEditorAttribute attribute in fieldInfo.GetCustomAttributes(true))
                    {
                        result.Add(attribute);
                    }
                }
                catch(Exception e)
                {
                }
                return result;
            }

            protected virtual ModEditor.Controls.EditorManager.FieldEditor GenerateOverridedFieldEditor(System.Reflection.FieldInfo fieldInfo, ModContents.Item item)
            {
                foreach (EditorManager.ModEditorAttribute attrib in GetFieldAttribute(fieldInfo))
                {
                    var editor = attrib.GenerateEditor(fieldInfo, item);
                    if (editor != null)
                        return editor;
                }                
                return null;
            }

            public virtual ModEditor.Controls.EditorManager.FieldEditor GenerateFieldEditor(System.Reflection.FieldInfo fieldInfo, ModContents.Item item)
            {
                if (SkipField(fieldInfo))
                    return null;
                EditorManager.FieldEditor editor = GenerateOverridedFieldEditor(fieldInfo, item);
                if (editor != null)
                    return editor;

                return EditorManager.generateControl(fieldInfo, item);
            }
            // Create context menu for treeview item selection
            public virtual ContextMenuStrip GenerateContextMenu(Item item)
            {
                return null;
            }

            public abstract void ObtainModData(string basePath, bool isBase);
            public abstract void PopulateModOverview(TreeNodeCollection root);
            public abstract void Save(string dir);

            public abstract void UpdateItems();

            // Reloads item from file
            public virtual void ReloadItem(Item item)
            {
                FileInfo info = new FileInfo(item.Path);
                var newTime = info.LastWriteTime;
                item.fileTime = newTime;                
            }

            // Opens system editor
            public virtual bool OpenItemFile(Item item)
            {
                if (item.Path != "")
                {
                    try
                    {
                        string path = mod.RootPath + "/" + item.Path;
                        System.Diagnostics.Process.Start(path);
                        return true;
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("Cannot open editor for file \"" + item.Path + "\" : \n\n" + e.ToString());
                    }
                }
                return false;
            }
            // Check group for modifications
            public virtual void CheckExternalModifications()
            {
            }
        }

        public interface EditAction
        {
            void Apply();
            void Undo();
            string Name();
        }

        public class ActionEditValue : EditAction
        {
            public Item item;
            public void Apply()
            { }            
            public void Undo()
            { }
            public string Name()
            {
                return "EditValue";
            }
        }

        Queue<EditAction> actionsList;

        public void Undo() { }
        public void Redo() { }

        
        
        public abstract class ControllerSpec<Target> : Controller
        {
            protected Type targetType = typeof(Target);
            protected string fileExtension = ".xml";
            protected int lastNewIndex = 0;
            // Global items cache            
            public static Dictionary<string, Item> globalCache = new Dictionary<string, Item>();
            // Local cache for items created by this mod
            public Dictionary<string, Item> localCache = new Dictionary<string, Item>();

            public XmlSerializer serializer;

            XmlWriterSettings settings = new XmlWriterSettings() { OmitXmlDeclaration = true, Indent = true, IndentChars = "\t"};
            
            public ControllerSpec(ModContents mod)
                :base(mod)
            {                
                XmlAttributeOverrides overrides = new XmlAttributeOverrides();
                overrides.Add(typeof(Target), new XmlAttributes(){XmlIgnore = true});
                serializer = new XmlSerializer(typeof(Target));
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
                if (item.Equals(rootItem) )
                {
                    if (rootItem.isBase)
                        return null;
                    result.Items.Add("New", null, OnNewItem);
                }
                else
                {
                    result.Items.Add("Edit", null, (object sender, EventArgs e) => { this.OpenItemFile(item); });
                    result.Items.Add("Copy", null, (object sender, EventArgs e) => { OnCopyItem(item); });
                    if(!item.isBase)
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
                    if(item.node != null)
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
                
                
                if(serializer == null)
                    return;
                var data = GetStorage();

                System.IO.Directory.CreateDirectory(rootDir + "/" + GetGroupFolder());

                foreach (var item in localCache)
                {
                    SaveItem(item.Value, rootDir);
                }
            }

            public override Control GenerateControl(Item item)
            {
                if (item.Equals(rootItem))
                    return null;
                ItemExplorer explorer = new ItemExplorer();
                Target targetObj = (Target)item.target;
                explorer.Init(item, targetObj);
                return explorer;
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
        public class RacesGroup : ControllerSpec<Ship_Game.ShipData>
        {
            public override XmlSerializer GetSerializer()
            {
                return Ship_Game.ResourceManager.weapon_serializer;
            }

            public override Dictionary<string, Ship_Game.ShipData> GetStorage()
            {
                return Ship_Game.ResourceManager.HullsDict;
            }

            public override string GetGroupFolder()
            {
                return "Races";
            }
        }*/

        public class HullsGroup : ControllerSpec<Ship_Game.ShipData>
        {
            public HullsGroup(ModContents mod)
                : base(mod)
            {
                groupName = "Hulls";
                //serializer = Ship_Game.ResourceManager.weapon_serializer;
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
                this.AddLocString("DescriptionIndex");
                this.AddLocString("NameIndex");
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
                this.AddLocString("NameTranslationIndex");
                this.AddLocString("DescriptionIndex");
                this.AddLocString("ShortDescriptionIndex");
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
                this.AddLocString("DescriptionIndex");
                this.AddLocString("NameIndex");
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
                this.AddLocString("DescriptionIndex");
                this.AddLocString("NameIndex");
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
}
