using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ModEditor.Controls;
using ModEditor.XNATool;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WinFormsContentLoading;

namespace ModEditor
{
    public partial class MainForm : Form
    {        
        public Ship_Game.Game1 game;
        public XNAWrap baseGame;
        
        /// <summary>
        /// Mod item, i.e Tech or Device
        /// </summary>
        public class ItemBase
        {
            public virtual Object getTarget()
            {
                return null;
            }
            public virtual string getType()
            {
                return "unknown";
            }
            public virtual void Serialize()
            {
            }

            public virtual void PopulateExplorer(ItemExplorer explorer) { }
            public string name;
            public TreeNode node;       /// Assigned tree node
            public TabPage page;        /// Assigned tab page
        }

        public class ItemSpec<Type> : ItemBase
        {
            public Type target;

            public override Object getTarget()
            {
                return target;
            }

            public override string getType()
            {
                return "Game." + typeof(Type).Name;
            }

            public override void PopulateExplorer(ItemExplorer explorer)
            {
                explorer.Init<Type>(target);
            }
        }

        public class WeaponSpec : ItemSpec<WeaponSpec>
        {
            static void FillWorkspaceView(WorkspaceView view, TreeNode root)
            {
            }
            void Serialize()
            {
            }
        }

        public class WorkspaceView
        {
            public TreeView treeView;
            public TreeNode rootBase;
            public TreeNode rootMod;

            public Ship_Game.ModInformation modInfo;

            public WorkspaceView(TreeView treeView)
            {
                this.treeView = treeView;
                this.treeView.Nodes.Clear();
                /*
                rootBase = new TreeNode("Base");
                rootMod = new TreeNode("Mod");

                treeView.Nodes.Clear();
                treeView.Nodes.Add(rootBase);
                treeView.Nodes.Add(rootMod);*/
                Reset();
            }

            public void Reset()
            {
                rootBase = null;
                rootMod = null;
                modInfo = null;
                treeView.Nodes.Clear();
                treeView.Nodes.Add("Load mod to observe data");
            }

            public void PopulateData(TreeNodeCollection root)
            {
                PopulateModOverview(Ship_Game.ResourceManager.ArtifactsDict, "Artifacts", root);
                PopulateModOverview(Ship_Game.ResourceManager.ShipModulesDict, "Modules", root);
                PopulateModOverview(Ship_Game.ResourceManager.WeaponsDict, "Weapons", root);
                PopulateModOverview(Ship_Game.ResourceManager.BuildingsDict, "Buildings", root);
                PopulateModOverview(Ship_Game.ResourceManager.ModelDict, "Models", root);
                PopulateModOverview(Ship_Game.ResourceManager.TroopsDict, "Troops", root);                
                PopulateModOverview(Ship_Game.ResourceManager.TextureDict, "Textures", root);
                PopulateModOverview(Ship_Game.ResourceManager.TechTree, "Tech", root);
                PopulateModOverview(Ship_Game.ResourceManager.ModelDict, "Models", root);
            }

            /// <summary>
            /// Iterate through all items in dictionary and add them to overview
            /// </summary>
            /// <typeparam name="Type"></typeparam>
            /// <param name="data"></param>
            /// <param name="name"></param>
            /// <param name="root"></param>
            public void PopulateModOverview<Type>(Dictionary<string, Type> data, string name, TreeNodeCollection root)
            {
                TreeNode[] nodes = root.Find(name, false);
                TreeNode groupRoot = null;
                if (nodes != null && nodes.Length > 0)
                    groupRoot = nodes[0];
                if (groupRoot == null)
                {
                    groupRoot = new TreeNode(name);
                    root.Add(groupRoot);
                }
                groupRoot.Nodes.Clear();
                foreach (var entry in data)
                {
                    TreeNode node = new TreeNode(entry.Key);
                    ItemSpec<Type> item = new ItemSpec<Type>();
                    item.name = entry.Key;
                    item.node = node;
                    item.target = entry.Value;
                    
                    node.Tag = item;
                    groupRoot.Nodes.Add(node);
                }
            }
        }

        WorkspaceView workspaceView;

        Boolean modReady = false;
        public MainForm()
        {            
            InitializeComponent();
            EditorManager.Init();
        }

        public enum EditorStatus
        {
            Empty,
            Loading,
            Ready,
        }

        public EditorStatus Status
        {
            set
            {
                statusGeneral.Text = value.ToString();
            }
        }
        
        private void LoadGameBackend()
        {
            try
            {
                baseGame = new XNAWrap();
                Ship_Game.GlobalStats.Config = new Ship_Game.Config();
               // contentBuilder = new ContentBuilder();
                //contentManager = new ContentManager(modelViewer.Services, "Content");
                /*
                graphics = new GraphicsDeviceManager(baseGame);
                graphics.MinimumPixelShaderProfile = ShaderProfile.PS_2_0;
                graphics.MinimumVertexShaderProfile = ShaderProfile.VS_2_0;
                string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                Directory.CreateDirectory(path + "/StarDrive");
                Directory.CreateDirectory(path + "/StarDrive/Saved Games");
                Directory.CreateDirectory(path + "/StarDrive/Fleet Designs");
                Directory.CreateDirectory(path + "/StarDrive/Saved Designs");
                Directory.CreateDirectory(path + "/StarDrive/WIP");
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/StarDrive/Saved Games/Headers");
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/StarDrive/Saved Games/Fog Maps");

                graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                graphics.IsFullScreen = false;*/
                baseGame.Content.RootDirectory = "Content";
                Ship_Game.ResourceManager.localContentManager = baseGame.Content;
                baseGame.graphics.ApplyChanges();
                //baseGame.Run();                
            }
            catch(Exception e)
			{
				MessageBox.Show("Cannot load StarDrive backend:\n\n" + e.ToString());
				return;
			}
        }

        void UnloadMod()
        {
            modReady = false;
            workspaceView.Reset();
            Ship_Game.ResourceManager.Reset();
        }
        /// <summary>
        /// Loads unmodded game data
        /// </summary>
        void LoadBaseData()
        {
            try
            {
                // 1. Load contents
                Ship_Game.ResourceManager.Initialize(baseGame.Content);
                
                workspaceView.PopulateData(workspaceView.treeView.Nodes);               
                // 2. Analyze contents
                modReady = true;
            }
            catch (Exception e)
            {
                MessageBox.Show("Failed to base data:\n\n" + e.ToString());
                return;
            }
        }

        void LoadMod(string ModEntryPath)        
        {
            string modRootDirectory = "Mods";// System.IO.Path.GetDirectoryName(ModEntryPath);
            Status = EditorStatus.Empty;            
            UnloadMod();
            Status = EditorStatus.Loading;
            this.statusModPath.Text = ModEntryPath;
           // LoadBaseData();
            try
            {

                FileInfo FI = new FileInfo(ModEntryPath);
                Stream file = FI.OpenRead();
                workspaceView.modInfo = (Ship_Game.ModInformation)Ship_Game.ResourceManager.ModSerializer.Deserialize(file);
                string modDirectory = modRootDirectory + "\\" + Path.GetFileNameWithoutExtension(FI.Name);

                // 1. Load contents                
                Ship_Game.ResourceManager.LoadMods(modDirectory);                
                // 2. Populate treeview
                workspaceView.treeView.Nodes.Clear();
                workspaceView.PopulateData(workspaceView.treeView.Nodes);               
                //workspaceView.PopulateData(workspaceView.rootMod);
               
                modReady = true;
                Status = EditorStatus.Ready;
            }
            catch (Exception e)
            {
                MessageBox.Show("Failed to load Mod \"" + ModEntryPath + "\":\n\n" + e.ToString());
                Status = EditorStatus.Empty;
                this.statusModPath.Text = "";
                return;
            }
            
        }

        private void openModToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Mod XML description|*.xml";
            dialog.Title = "Select Mod XML description file";

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // Assign the cursor in the Stream to the Form's Cursor property.
                LoadMod(dialog.FileName);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);           

            LoadGameBackend();

            workspaceView = new WorkspaceView(ModContentsTree);
            Status = EditorStatus.Empty;
        }

        private void OnViewportResize(object sender, EventArgs e)
        {           
        }
        private void OnVieweportPaint(object sender, PaintEventArgs e)
        {
            /*
            mDevice.Clear(Microsoft.Xna.Framework.Graphics.Color.SteelBlue);

            // Do your rendering stuff here...
            mDevice.Present();*/
        }

        private TabPage FindTabByTag(Object tag)
        {
            foreach (TabPage tab in this.EditorTabs.TabPages)
            {
                if (tab.Tag == tag)
                    return tab;
            }
            return null;
        }

        public void ExploreItem(ItemBase item)
        {
            if (item.page == null)
            {
                item.page = new TabPage(item.name);
                ItemExplorer explorer = new ItemExplorer();
                explorer.Dock = DockStyle.Fill;
                item.PopulateExplorer(explorer);   
                item.page.Controls.Add(explorer);
                            
                this.EditorTabs.TabPages.Add(item.page);
                this.EditorTabs.SelectedTab = item.page;
            }
            else
            {
                this.EditorTabs.SelectedTab = item.page;
            }
        }

        private void ModContentsTree_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            TreeNode selected = e.Node;
            ItemBase item = selected.Tag as ItemBase;
            if (selected != null && item != null)
            {
                ExploreItem(item);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
