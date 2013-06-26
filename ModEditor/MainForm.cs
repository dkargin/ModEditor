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
using ModEditor;
using WeifenLuo.WinFormsUI.Docking;

namespace ModEditor
{
    public partial class MainForm : Form, ModContents.Explorer
    {
        public class Preferences
        {
            [Description("Interval for external checking. Set 0 to disable")]
            public int externalCheck = 5000;
        }

        //public Ship_Game.Game1 game;
        //public XNAWrap baseGame;

        Game baseGame;

        static MainForm mainForm;
        
        // TODO: more wide mod creation
        ModContents contentsBase;
        ModContents contentsMod;

        private bool m_bSaveLayout = true;
        private DeserializeDockContent m_deserializeDockContent;
        ToolSolutionExplorer solutionExplorer = new ToolSolutionExplorer();
         
        public MainForm()
        {
            mainForm = this;
            InitializeComponent();

            FieldEditorManager.InitBasicTypes();
            FieldEditorManager.InitGameTypes();

            solutionExplorer.RightToLeftLayout = RightToLeftLayout;

            solutionExplorer.Show(workArea);
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

        public void InitXNA(Game game, GraphicsDeviceManager graphics)
        {
            try
            {
                baseGame = game;
                // need this to load textures and models
                graphics.ApplyChanges();
                LogInfoString("XNA attached");
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
                LogInfoString("Saving complete");
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
                    LogInfoString("Base data has been loaded");
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
                LogInfoString("Mod data has been loaded");
                contentsBase.UpdateUI();
                LogInfoString("Done");
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

        public interface EditAction
        {
            void Apply();
            void Rollback();
            string Name();
        }

        public class ActionEditValue : EditAction
        {
            public Item item;

            public object oldValue;

            public void Apply()
            {
            }
            public void Rollback()
            {
            }
            public string Name()
            {
                return "EditValue";
            }
        }

        static int maxActions = 10;
        List<EditAction> actionsList = new List<EditAction>();
        int executedActions = 0;

        public void AddAction(EditAction action, bool apply = true)
        {
            actionsList.Insert(executedActions, action);
            if (apply)
            {
                action.Apply();
                executedActions++;
            }
            // remove 
            if (executedActions < actionsList.Count)
            {
                actionsList.RemoveRange(executedActions, actionsList.Count);
            }
            // remove old position
            if (actionsList.Count > maxActions)
                actionsList.RemoveAt(0);
        }

        public void Undo()
        {            
            if (executedActions > 0)
            {
                executedActions--;
                actionsList[executedActions].Rollback();
            }
        }
        public void Redo()
        {
            // TODO: implement
            if (actionsList.Count > 0 && executedActions < actionsList.Count)
            {
                actionsList[executedActions].Apply();
            }
        } 

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            try
            {

                contentsBase = new ModContents(ModContentsTree, null);
                contentsBase.SetName("Base");
                LogInfoString("Startup is complete");
            }
            catch (Exception ex)
            {
                LogErrorString(ex.Message);
            }
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


        static int maxLogLines = 10;

        List<string> logMessages = new List<string>();

        static public void SelectItem(Item item)
        {
            if(mainForm != null)
            {
                mainForm.ExploreItem(item);
            }
        }

        static public void LogNewString(string message)
        {
            if (mainForm != null)
            {
                mainForm.logMessages.Add(message);               
                if(mainForm.logMessages.Count > maxLogLines)
                    mainForm.logMessages.RemoveAt(0);
                mainForm.logView.Lines = mainForm.logMessages.ToArray();
            }
        }

        static public void LogInfoString(string message)
        {           
            LogNewString(String.Format("Info: {0}", message));
        }

        static public void LogErrorString(string message)
        {
            LogNewString(String.Format("Error: {0}", message));
        }

        public void ExploreItem(ModEditor.Item item)
        {
            if (item.page == null)
            {
                SuspendLayout();
                try
                {
                    Control control = item.GenerateControl();
                    if (control != null)
                    {
                        item.page = new TabPage(item.name);
                        item.page.Tag = item;
                        control.Dock = DockStyle.Fill;
                        item.page.Controls.Add(control);

                        this.EditorTabs.TabPages.Add(item.page);
                        this.EditorTabs.SelectedTab = item.page;
                    }
                }
                catch (Exception)
                {
                }
                ResumeLayout();
            }
            else
            {
                this.EditorTabs.SelectedTab = item.page;
            }

            if (item.node != null)
            {
                item.node.TreeView.SelectedNode = item.node;
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

        private void CheckExternalModifications()
        {
            List<Item> modifiedItems = new List<Item>();
            if (contentsMod == null)
                return;
            //contentsBase.CheckExternalModifications(modifiedItems);
            contentsMod.CheckExternalModifications(modifiedItems);

            if (modifiedItems.Count() > 0)
            {
                string list = "";
                int maxItems = 20;
                foreach (var item in modifiedItems)
                {
                    list = list + item.Path + "\n";
                    if (maxItems-- == 0)
                    {
                        list = list + "\n ... more files";
                        break;
                    }
                }
                if (MessageBox.Show(list, "Found external changes", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    foreach (var item in modifiedItems)
                    {
                        item.Refresh();
                    }
                }
            }
        }

        private void CheckExternalModifications_Tick(object sender, EventArgs e)
        {
            TimerCheckExternalModifications.Enabled = false;
            CheckExternalModifications();
            TimerCheckExternalModifications.Enabled = true;
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            (new FormAbout()).ShowDialog();
        }

        private void runToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("StarDrive.exe");
        }

        private void ModContentsTree_BeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            Item item = e.Node.Tag as Item;
            if (item == null || !item.isNameEditable)
            {
                e.CancelEdit = true;
            }            
        }

        private void ModContentsTree_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            Item item = e.Node.Tag as Item;
            if (item == null || !item.isNameEditable)
            {
                e.CancelEdit = true;
            }
            else if (!item.controller.RenameItem(item, e.Label))
            {
                e.CancelEdit = true;
            }
        }

        private void checkDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CheckExternalModifications();
        }
    }
}
