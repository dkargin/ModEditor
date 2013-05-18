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
using System.Xml.Serialization;
using WinFormsContentLoading;

namespace ModEditor
{
    public partial class MainForm : Form
    {        
        public Ship_Game.Game1 game;
        public XNAWrap baseGame;        
        
        public class WorkspaceView
        {
            public TreeView treeView;
            public TreeNode rootBase;
            public TreeNode rootMod;

            public Ship_Game.ModInformation modInfo;

            public List<Workspace.Controller> controllers = new List<Workspace.Controller>();

            public WorkspaceView(TreeView treeView)
            {
                this.treeView = treeView;
                this.treeView.Nodes.Clear();
                
                rootBase = new TreeNode("Base");
                rootMod = new TreeNode("Mod");

                treeView.Nodes.Clear();
                treeView.Nodes.Add(rootBase);
                treeView.Nodes.Add(rootMod);
                Reset();

                controllers.Add(new Workspace.ArtifactsSpec());
                controllers.Add(new Workspace.BuildingSpec());
                controllers.Add(new Workspace.ModuleSpec());
                controllers.Add(new Workspace.WeaponGroup());
                controllers.Add(new Workspace.TechSpec());
                controllers.Add(new Workspace.TroopSpec());
                /*
                controllers.Add(new Workspace.TexturesSpec());
                controllers.Add(new Workspace.ModelsSpec());*/

            }

            public void Reset()
            {
                rootBase.Nodes.Clear();// = null;
                rootMod.Nodes.Clear();// = null;
                modInfo = null;
                //treeView.Nodes.Clear();
                //treeView.Nodes.Add("Load mod to observe data");
            }

            public void PopulateData(TreeNodeCollection root)
            {
                /*
                ModEditor.Workspace.ModuleSpec.PopulateGroup(root);
                ModEditor.Workspace.ArtifactsSpec.PopulateGroup(root);
                ModEditor.Workspace.BuildingSpec.PopulateGroup(root);
                ModEditor.Workspace.WeaponGroup.PopulateGroup(root);
                ModEditor.Workspace.TechSpec.PopulateGroup(root);
                ModEditor.Workspace.TroopSpec.PopulateGroup(root);*/
                foreach (var controller in controllers)
                {
                    controller.PopulateModOverview(root);
                }

                //ItemBase.PopulateModOverview(Ship_Game.ResourceManager.ArtifactsDict, "Artifacts", root);                
                //PopulateModOverview(Ship_Game.ResourceManager.ShipModulesDict, "Modules", root);
                
                //PopulateModOverview(Ship_Game.ResourceManager.WeaponsDict, "Weapons", root);
                //PopulateModOverview(Ship_Game.ResourceManager.BuildingsDict, "Buildings", root);
                //PopulateModOverview(Ship_Game.ResourceManager.ModelDict, "Models", root);
                //PopulateModOverview(Ship_Game.ResourceManager.TroopsDict, "Troops", root);                
                //PopulateModOverview(Ship_Game.ResourceManager.TextureDict, "Textures", root);
                //PopulateModOverview(Ship_Game.ResourceManager.TechTree, "Tech", root);               
            }            
        }

        WorkspaceView workspaceView;

       // Boolean modReady = false;
        public MainForm()
        {            
            InitializeComponent();
            EditorManager.Init();
        }

        public enum EditorStatus
        {
            Empty,
            Loading,
            Saving,
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
            //modReady = false;
            workspaceView.Reset();
            Ship_Game.ResourceManager.Reset();
        }

        /// <summary>
        /// Loads base game data
        /// </summary>
        void LoadBaseData()
        {
            try
            {
                this.statusModPath.Text = "base game data";
                // 1. Load contents
                Ship_Game.ResourceManager.Initialize(baseGame.Content);
                
                workspaceView.PopulateData(workspaceView.rootBase.Nodes);               
                // 2. Analyze contents
                //modReady = true;
            }
            catch (Exception e)
            {
                MessageBox.Show("Failed to base data:\n\n" + e.ToString());
                return;
            }
        }

        bool LoadModInfo(string ModEntryPath)
        {

            try
            {
                FileInfo FI = new FileInfo(ModEntryPath);
                Stream file = FI.OpenRead();
                workspaceView.modInfo = (Ship_Game.ModInformation)Ship_Game.ResourceManager.ModSerializer.Deserialize(file);
                file.Close();
                file.Dispose();
                return true;
            }
            catch (Exception e)
            {
            }
            return false;
        }

        bool SaveModInfo(string ModEntryPath)
        {
            try
            {
                FileInfo FI = new FileInfo(ModEntryPath);
                Stream file = FI.OpenWrite();
                Ship_Game.ResourceManager.ModSerializer.Serialize(file, workspaceView.modInfo);
                file.Close();
                file.Dispose();
                return true;
            }
            catch (Exception e)
            {
            }
            return false;
        }

        void SaveMod(string ModEntryPath)
        {
            Status = EditorStatus.Saving;
            SaveModInfo(ModEntryPath);
            Status = EditorStatus.Ready;
        }

        void LoadMod(string ModEntryPath)        
        {
            string modRootDirectory = "Mods";// System.IO.Path.GetDirectoryName(ModEntryPath);
            Status = EditorStatus.Empty;            
            UnloadMod();
            Status = EditorStatus.Loading;
            LoadBaseData();
            this.statusModPath.Text = ModEntryPath;
            try
            {
                string modDirectory = modRootDirectory + "\\" + Path.GetFileNameWithoutExtension(ModEntryPath);
                LoadModInfo(ModEntryPath);
                // 1. Load contents                
                Ship_Game.ResourceManager.LoadMods(modDirectory);                
                // 2. Populate treeview
                //workspaceView.treeView.Nodes.Clear();
                workspaceView.PopulateData(workspaceView.rootMod.Nodes);               
                //workspaceView.PopulateData(workspaceView.rootMod);
                workspaceView.rootMod.Name = workspaceView.modInfo.ModName;
                //modReady = true;
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

        /// <summary>
        /// Order Main UI to show item contents
        /// </summary>
        /// <param name="item"></param>
        public void ExploreItem(ModEditor.Workspace.ItemBase item)
        {
            if (item.page == null)
            {                
                Control control = item.GenerateControl();
                if (control != null)
                {
                    item.page = new TabPage(item.name);
                    //ItemExplorer explorer = new ItemExplorer();
                    //explorer.Dock = DockStyle.Fill;
                    //item.PopulateExplorer(explorer);
                    control.Dock = DockStyle.Fill;
                    item.page.Controls.Add(control);

                    this.EditorTabs.TabPages.Add(item.page);
                    this.EditorTabs.SelectedTab = item.page;
                }
            }
            else
            {
                this.EditorTabs.SelectedTab = item.page;
            }
        }

        private void ModContentsTree_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            TreeNode selected = e.Node;
            ModEditor.Workspace.ItemBase item = selected.Tag as ModEditor.Workspace.ItemBase;
            if (selected != null && item != null)
            {
                ExploreItem(item);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveMod("testExport.xml");
        }
    }
}
