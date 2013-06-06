namespace ModEditor
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.SplitContainer splitContainer1;
            this.ModContentsTree = new System.Windows.Forms.TreeView();
            this.EditorTabs = new ModEditor.Controls.TabControlEx();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fILEToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadBaseDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newModToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openModToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hELPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.statusGeneral = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusModPath = new System.Windows.Forms.ToolStripStatusLabel();
            this.GenericObjectsMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.TimerCheckExternalModifications = new System.Windows.Forms.Timer(this.components);
            splitContainer1 = new System.Windows.Forms.SplitContainer();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainer1.Location = new System.Drawing.Point(0, 24);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(this.ModContentsTree);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(this.EditorTabs);
            splitContainer1.Size = new System.Drawing.Size(706, 435);
            splitContainer1.SplitterDistance = 235;
            splitContainer1.TabIndex = 2;
            // 
            // ModContentsTree
            // 
            this.ModContentsTree.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ModContentsTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ModContentsTree.Location = new System.Drawing.Point(0, 0);
            this.ModContentsTree.Name = "ModContentsTree";
            this.ModContentsTree.Size = new System.Drawing.Size(233, 433);
            this.ModContentsTree.TabIndex = 3;
            this.ModContentsTree.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.ModContentsTree_NodeMouseClick);
            this.ModContentsTree.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ModContentsTree_MouseUp);
            // 
            // EditorTabs
            // 
            this.EditorTabs.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.EditorTabs.ConfirmOnClose = false;
            this.EditorTabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.EditorTabs.HotTrack = true;
            this.EditorTabs.Location = new System.Drawing.Point(0, 0);
            this.EditorTabs.Name = "EditorTabs";
            this.EditorTabs.Padding = new System.Drawing.Point(3, 1);
            this.EditorTabs.SelectedIndex = 0;
            this.EditorTabs.Size = new System.Drawing.Size(465, 433);
            this.EditorTabs.TabIndex = 1;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fILEToolStripMenuItem,
            this.hELPToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(706, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            this.menuStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.menuStrip1_ItemClicked);
            // 
            // fILEToolStripMenuItem
            // 
            this.fILEToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadBaseDataToolStripMenuItem,
            this.newModToolStripMenuItem,
            this.openModToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.exportToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.fILEToolStripMenuItem.Name = "fILEToolStripMenuItem";
            this.fILEToolStripMenuItem.Size = new System.Drawing.Size(40, 20);
            this.fILEToolStripMenuItem.Text = "FILE";
            // 
            // loadBaseDataToolStripMenuItem
            // 
            this.loadBaseDataToolStripMenuItem.Name = "loadBaseDataToolStripMenuItem";
            this.loadBaseDataToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.loadBaseDataToolStripMenuItem.Text = "Load base data";
            this.loadBaseDataToolStripMenuItem.Click += new System.EventHandler(this.loadBaseDataToolStripMenuItem_Click);
            // 
            // newModToolStripMenuItem
            // 
            this.newModToolStripMenuItem.Name = "newModToolStripMenuItem";
            this.newModToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.newModToolStripMenuItem.Text = "New Mod";
            this.newModToolStripMenuItem.Click += new System.EventHandler(this.newModToolStripMenuItem_Click);
            // 
            // openModToolStripMenuItem
            // 
            this.openModToolStripMenuItem.Name = "openModToolStripMenuItem";
            this.openModToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.openModToolStripMenuItem.Text = "Open Mod";
            this.openModToolStripMenuItem.Click += new System.EventHandler(this.openModToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.exportToolStripMenuItem.Text = "Export";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(145, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // hELPToolStripMenuItem
            // 
            this.hELPToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.hELPToolStripMenuItem.Name = "hELPToolStripMenuItem";
            this.hELPToolStripMenuItem.Size = new System.Drawing.Size(43, 20);
            this.hELPToolStripMenuItem.Text = "HELP";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusGeneral,
            this.statusModPath});
            this.statusStrip1.Location = new System.Drawing.Point(0, 459);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(706, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // statusGeneral
            // 
            this.statusGeneral.Name = "statusGeneral";
            this.statusGeneral.Size = new System.Drawing.Size(38, 17);
            this.statusGeneral.Text = "Status";
            // 
            // statusModPath
            // 
            this.statusModPath.Name = "statusModPath";
            this.statusModPath.Size = new System.Drawing.Size(44, 17);
            this.statusModPath.Text = "no path";
            // 
            // GenericObjectsMenu
            // 
            this.GenericObjectsMenu.Name = "GenericObjectsMenu";
            this.GenericObjectsMenu.Size = new System.Drawing.Size(61, 4);
            // 
            // TimerCheckExternalModifications
            // 
            this.TimerCheckExternalModifications.Interval = 2000;
            this.TimerCheckExternalModifications.Tick += new System.EventHandler(this.CheckExternalModifications_Tick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(706, 481);
            this.Controls.Add(splitContainer1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "StarDrive Mod Editor";
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            splitContainer1.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fILEToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openModToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripMenuItem hELPToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.TreeView ModContentsTree;
        private Controls.TabControlEx EditorTabs;
        private System.Windows.Forms.ToolStripStatusLabel statusGeneral;
        private System.Windows.Forms.ToolStripStatusLabel statusModPath;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem loadBaseDataToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip GenericObjectsMenu;
        private System.Windows.Forms.ToolStripMenuItem newModToolStripMenuItem;
        private System.Windows.Forms.Timer TimerCheckExternalModifications;
    }
}

