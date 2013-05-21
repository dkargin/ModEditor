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
        
        ModContents contentsBase;
        ModContents contentsMod;

       // Boolean modReady = false;
        public MainForm()
        {            
            InitializeComponent();
            EditorManager.Init();

            string source = "Laser weapons factory. Provides production bonus ${Building.SoftAttack} per assigned colonist. Also can defend colony using its production directly from the assembly line, shooting orbital targets at range ${Weapon.Range}. Needs ${Building.Maintenance} maintance per turn.";
            Ship_Game.Building building = new Ship_Game.Building();
            Console.WriteLine(ModContents.EmbedToString(source, building));
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
            contentsBase.Reset();
            Ship_Game.ResourceManager.Reset();
        }
      

        void SaveMod(string ModEntryPath)
        {
            if (contentsMod != null)
            {
                Status = EditorStatus.Saving;
                contentsMod.SaveMod(ModEntryPath);
                Status = EditorStatus.Ready;
            }
        }

        void LoadBase()
        {
            if (!contentsBase.Loaded())
            {
                this.statusModPath.Text = "base game data";
                try
                {
                    // 1. Load contents
                    Ship_Game.ResourceManager.Initialize(baseGame.Content);
                    contentsBase.PopulateData(true);
                    contentsBase.state = ModContents.State.Loaded;
                    // 2. Analyze contents
                    //modReady = true;
                }
                catch (Exception e)
                {
                    MessageBox.Show("Failed to base data:\n\n" + e.ToString());
                    return;
                }
            }
        }

        void LoadMod(string ModEntryPath)        
        {
            Status = EditorStatus.Empty;            
            //UnloadMod();
            Status = EditorStatus.Loading;

            // Load base data
            LoadBase();
            // Load mod data
            this.statusModPath.Text = ModEntryPath;
            try
            {
                contentsMod = new ModContents(ModContentsTree);
                contentsMod.LoadMod(ModEntryPath);

                contentsBase.UpdateUI();
                contentsMod.UpdateUI();

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

            contentsBase = new ModContents(ModContentsTree);
            contentsBase.SetName("Base");
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
        public void ExploreItem(ModEditor.ModContents.ItemBase item)
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
            ModEditor.ModContents.ItemBase item = selected.Tag as ModEditor.ModContents.ItemBase;
            if (selected != null && item != null)
            {
                ExploreItem(item);
            }
        }

        private void ModContentsTree_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                // Select the clicked node
                TreeNode selected = ModContentsTree.GetNodeAt(e.X, e.Y);
                ModContentsTree.SelectedNode = selected;

                if (selected != null && selected.Tag != null )
                {
                    ModContents.ItemBase item = selected.Tag as ModContents.ItemBase;
                    if(item != null)
                    {
                        ContextMenuStrip menu = item.GenerateContextMenu();
                        if(menu != null)
                            menu.Show(ModContentsTree, e.Location);                        
                    }                    
                }
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

        private void loadBaseDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadBase();
        }        
    }
}
