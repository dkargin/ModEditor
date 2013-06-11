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

        public TreeNode node;           // Assigned tree node
        public TabPage page;            // Assigned tab page

        public DateTime fileTime;       // Cached time to check if file was modified

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

        protected System.Type targetType;

        public bool isBase;

        public Dictionary<string, List<ModEditorAttribute>> customAttributes = new Dictionary<string, List<ModEditorAttribute>>();
        // Local cache for items created by this controller/mod
        protected Dictionary<string, Item> localCache = new Dictionary<string, Item>();

        public List<string> fieldStringToken = new List<string>();

        protected ModContents mod;

        public ModContents Mod
        {
            get
            {
                return mod;
            }
        }

        public Controller(ModContents mod)
        {
            this.mod = mod;
            rootItem = new Item(null, this, "");
            rootItem.controller = this;
        }

        public bool IsBase()
        {
            return isBase;
        }

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
            AddCustomAttribute(fieldName, new LocStringToken());
        }
        // Mark field as "object reference"            
        public void OverrideFieldObjectReference(string fieldName, string group)
        {
            AddCustomAttribute(fieldName, new ObjectReference(group));
        }
        // Ignore field
        public void IgnoreField(string fieldName)
        {
            AddCustomAttribute(fieldName, new IgnoreByEditor());
        }


        public virtual Item CreateSpec(Object obj, string path)
        {
            return new Item(obj, this, path);
        }

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
        }

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
        public abstract void Save(string dir);

        public abstract void UpdateItems();

        public virtual ContextMenuStrip GenerateContextMenu(Item item)
        {
            return null;
        }

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

    public class ModContents
    {        
        // Editor references to other mods
        public List<ModContents> references;

        public interface Explorer
        {
            void ExploreItem(Item item);
        }

        ModContents next, prev;

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
        /*
        static ModContents LoadMod(T)
        {
        }*/

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
                MainForm.LogString(e.Message);
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
                MainForm.LogString(e.Message);
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
                MainForm.LogString(e.Message);
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
                }
                catch (Exception)
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

        public Controller GetGroupController(string group)
        {
            if(controllers.ContainsKey(group))                
                return this.controllers[group];
            return null;
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
        #endregion   
    }
}
