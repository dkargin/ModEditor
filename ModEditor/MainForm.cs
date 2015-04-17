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

        delegate bool AsyncMethod();

        AsyncMethod asyncCheckData = null;
        AsyncMethod asyncLoadData = null;

        Game baseGame;

        static MainForm mainForm;
        
        // TODO: more wide mod creation
        ModContents contentsBase;
        ModContents contentsMod;

        private bool m_bSaveLayout = true;
        private DeserializeDockContent m_deserializeDockContent;

        PanelReport panelReport = new PanelReport();
        PanelSolutionExplorer solutionExplorer;
        PanelErrors panelLog = new PanelErrors();

        //List<PanelItemContainer> itemPanels = new List<PanelItemContainer>();

        TreeView ModContentsTree;
         
        public MainForm()
        {
            mainForm = this;
            InitializeComponent();

            FieldEditorManager.InitBasicTypes();
            FieldEditorManager.InitGameTypes();

            solutionExplorer = new PanelSolutionExplorer(this);
            m_deserializeDockContent = new DeserializeDockContent(GetContentFromPersistString);
            //solutionExplorer.RightToLeftLayout = RightToLeftLayout;
            
            
            solutionExplorer.Show(workArea);
            
            panelLog.Show(workArea);
            
            ModContentsTree = solutionExplorer.ModContentsTree;

            contentsBase = new ModContents(ModContentsTree, null);
            contentsBase.SetName("Base");

            try
            {
                contentsBase.InitControllers();
                
                PanelErrors.LogInfoString("Startup is complete");
            }
            catch (Exception ex)
            {
                PanelErrors.LogErrorString(ex.Message);
            }

            Status = EditorStatus.Empty;
            /// Strings test
            //string source = "Laser weapons factory. Provides production bonus ${Building.SoftAttack} per assigned colonist. Also can defend colony using its production directly from the assembly line, shooting orbital targets at range ${Weapon.Range}. Needs ${Building.Maintenance} maintance per turn.";
            //Ship_Game.Building building = new Ship_Game.Building();
            //Console.WriteLine(ModContents.EmbedToString(source, building));

            UpdateMenus();
        }

        void UpdateMenus()
        {
            errorsToolStripMenuItem.Checked = (panelReport.VisibleState != DockState.Hidden);
            logViewToolStripMenuItem.Checked = (this.panelLog.VisibleState != DockState.Hidden);
            modExplorerToolStripMenuItem.Checked = (this.solutionExplorer.VisibleState != DockState.Hidden);
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

        private IDockContent GetContentFromPersistString(string persistString)
        {
            if (persistString == typeof(PanelErrors).ToString())
                return panelLog;
            else if (persistString == typeof(PanelSolutionExplorer).ToString())
                return solutionExplorer;
                /*
            else if (persistString == typeof(DummyPropertyWindow).ToString())
                return m_propertyWindow;
            else if (persistString == typeof(DummyToolbox).ToString())
                return m_toolbox;
            else if (persistString == typeof(DummyOutputWindow).ToString())
                return m_outputWindow;
            else if (persistString == typeof(DummyTaskList).ToString())
                return m_taskList;*/
            else
            {
                /*
                string[] parsedStrings = persistString.Split(new char[] { ',' });
                if (parsedStrings.Length != 3)
                    return null;

                if (parsedStrings[0] != typeof(DummyDoc).ToString())
                    return null;

                DummyDoc dummyDoc = new DummyDoc();
                if (parsedStrings[1] != string.Empty)
                    dummyDoc.FileName = parsedStrings[1];
                if (parsedStrings[2] != string.Empty)
                    dummyDoc.Text = parsedStrings[2];
                
                return dummyDoc;*/
                return null;
            }
        }

        public void InitXNA(Game game, GraphicsDeviceManager graphics)
        {
            try
            {
                baseGame = game;
                // need this to load textures and models
                graphics.ApplyChanges();
                PanelErrors.LogInfoString("XNA attached");
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
                    LoadBase();
                    contentsMod = ModContents.CreateNewMod(ModContentsTree, dialog.FileName, contentsBase);
                    contentsMod.InitControllers();
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
                PanelErrors.LogInfoString("Saving complete");
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
                    PanelErrors.LogInfoString("Base data has been loaded");
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
            if (this.asyncLoadData != null)
                return;
            
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
                PanelErrors.LogInfoString("Mod data has been loaded");
                contentsBase.UpdateUI();
                PanelErrors.LogInfoString("Done");
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
        /*
        private PanelItemContainer FindTabByTag(Object tag)
        {
            foreach (PanelItemContainer tab in this.itemPanels)
            {
                if (tab.Tag == tag)
                    return tab;
            }
            return null;
        }
        */
        static public void SelectItem(Item item)
        {
            if(mainForm != null)
            {
                mainForm.ExploreItem(item);
            }
        }        

        static public void OnPanelItemClosed(PanelItemContainer page)
        {
            if(mainForm != null)
            {  
                Item item = page.Item;
                item.OnTabClosed();
            }
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
                        PanelItemContainer page = new PanelItemContainer()
                        {
                            Text = item.Name,
                            Tag = item,
                        };

                        page.AttachItem(item, control);
                        /*
                        ItemView page = new ItemView()
                        {
                            Text = item.Name,
                            Tag = item,
                        };
                        page.Init(item.controller.TargetType, item);*/
                        item.page = page;
                        //itemPanels.Add(page);
                        page.Show(workArea);
                        page.Select();
                    }
                }
                catch (Exception ex)
                {
                    PanelErrors.LogErrorString(ex.Message);
                }
                ResumeLayout();
            }
            else
            {                
                item.page.Select();                
            }

            if (item.node != null)
            {
                //item.node.TreeView.Focus();
                item.node.EnsureVisible();
                item.node.TreeView.SelectedNode = item.node;                
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

        private void UpdateProgress(int value)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)(() => { toolStripProgress.Value = value; }));
            }
            else
            {
                toolStripProgress.Value = value;
            }
        }

        private void EnableProgress(bool value)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)(() => { EnableProgress(value); }));
            }
            else
            {
                toolStripProgress.Visible = value;
                if(value)
                {
                    toolStripProgress.Maximum = 100;
                    toolStripProgress.Value = 0;
                    toolStripProgress.Visible = true;
                }
            }
        }

        private void checkDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (contentsMod != null && contentsMod.state == ModContents.State.Loaded && this.asyncCheckData == null)
            {
                panelReport.ClearReport();
                EnableProgress(true);

                asyncCheckData = new AsyncMethod(() =>
                {
                    contentsMod.CheckDataIntegrity(panelReport.OnErrorFound, UpdateProgress);
                    EnableProgress(false);
                    return false;
                });
                asyncCheckData.BeginInvoke((IAsyncResult ar) => { this.asyncCheckData = null; }, null);
                /*
                contentsMod.CheckDataIntegrity(panelReport.OnErrorFound, UpdateProgress);
                toolStripProgress.Visible = false;
                //return false;*/
            }
            else
            {
                PanelErrors.LogErrorString("Mod data is not loaded");
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            string configFile = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "DockPanel.config");
            if (m_bSaveLayout)
                workArea.SaveAsXml(configFile);
            else if (File.Exists(configFile))
                File.Delete(configFile);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            string configFile = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "DockPanel.config");
            /*
            if (File.Exists(configFile))
                workArea.LoadFromXml(configFile, m_deserializeDockContent);*/
        }

        private void errorsToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            if (errorsToolStripMenuItem.Checked)
                panelReport.Show(workArea);
            else
                panelReport.Hide();
        }

        private void logViewToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            if (logViewToolStripMenuItem.Checked)
                panelLog.Show(workArea);
            else
                panelLog.Hide();
        }

        private void modExplorerToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            if (modExplorerToolStripMenuItem.Checked)
                solutionExplorer.Show(workArea);
            else
                solutionExplorer.Hide();
        }

        private void checkExternalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CheckExternalModifications();
        }
    }
}
