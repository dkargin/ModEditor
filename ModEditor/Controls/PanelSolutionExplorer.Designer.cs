namespace ModEditor.Controls
{
    partial class PanelSolutionExplorer
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
            this.ModContentsTree = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // ModContentsTree
            // 
            this.ModContentsTree.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ModContentsTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ModContentsTree.LabelEdit = true;
            this.ModContentsTree.Location = new System.Drawing.Point(0, 0);
            this.ModContentsTree.Name = "ModContentsTree";
            this.ModContentsTree.Size = new System.Drawing.Size(263, 344);
            this.ModContentsTree.TabIndex = 4;
            this.ModContentsTree.BeforeLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.ModContentsTree_BeforeLabelEdit);
            this.ModContentsTree.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.ModContentsTree_AfterLabelEdit);
            this.ModContentsTree.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.ModContentsTree_NodeMouseClick);
            this.ModContentsTree.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ModContentsTree_MouseUp);
            // 
            // PanelSolutionExplorer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(263, 344);
            this.Controls.Add(this.ModContentsTree);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Name = "PanelSolutionExplorer";
            this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockLeft;
            this.Text = "Game Contents";
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.TreeView ModContentsTree;

    }
}