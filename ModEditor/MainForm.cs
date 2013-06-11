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
    public partial class MainForm : Form, ModContents.Explorer
    {   
        public Ship_Game.Game1 game;
        public XNAWrap baseGame;        
        
        ModContents contentsBase;
        ModContents contentsMod;
         
       // Boolean modReady = false;
        public MainForm()
        {            
            InitializeComponent();
            PropertyGridExplorer.Init();
            /// Strings test
            //string source = "Laser weapons factory. Provides production bonus ${Building.SoftAttack} per assigned colonist. Also can defend colony using its production directly from the assembly line, shooting orbital targets at range ${Weapon.Range}. Needs ${Building.Maintenance} maintance per turn.";
            //Ship_Game.Building building = new Ship_Game.Building();
            //Console.WriteLine(ModContents.EmbedToString(source, building));
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
                baseGame.Content.RootDirectory = "Content";
                Ship_Game.ResourceManager.localContentManager = baseGame.Content;
                baseGame.graphics.ApplyChanges();
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

        void CreateMod()
        {
            if (contentsMod == null)
            {
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.Filter = "Mod XML description|*.xml";
                dialog.Title = "Mod XML description file";

                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    contentsMod = ModContents.CreateNewMod(ModContentsTree, dialog.FileName, contentsBase);
                    contentsMod.UpdateUI();
                    contentsBase.UpdateUI();
                }
            }
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

        public string GetGamePath()
        {
            return "";
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
                    contentsBase.PopulateData(GetGamePath()+"Content", true);
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
                contentsMod = new ModContents(ModContentsTree, contentsBase);
                contentsMod.LoadMod(ModEntryPath);

                contentsBase.UpdateUI();

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

            contentsBase = new ModContents(ModContentsTree, null);
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

        static public void LogString(string message)
        {

        }

        public void ExploreItem(ModEditor.Item item)
        {
            if (item.page == null)
            {
                Control control = item.GenerateControl();
                if (control != null)
                {
                    item.page = new TabPage(item.name);
                    item.page.Tag = item;
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
            if (e.Button == MouseButtons.Left)
            {
                TreeNode selected = e.Node;
                ModEditor.Item item = selected.Tag as ModEditor.Item;                
                if (selected != null && item != null)
                {
                    ExploreItem(item);
                }
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
                    ModEditor.Item item = selected.Tag as ModEditor.Item;
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

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void newModToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateMod();
        }

        private void CheckExternalModifications_Tick(object sender, EventArgs e)
        {
            contentsBase.CheckExternalModifications();
            contentsMod.CheckExternalModifications();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            (new FormAbout()).ShowDialog();
        }        
    }
}
