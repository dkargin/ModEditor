namespace ModEditor
{
    partial class FormSelect
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
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.filterBase = new System.Windows.Forms.CheckBox();
            this.itemsList = new System.Windows.Forms.CheckedListBox();
            this.filterAllMods = new System.Windows.Forms.CheckBox();
            this.filterCurrent = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(53, 338);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 19);
            this.okButton.TabIndex = 25;
            this.okButton.Text = "&OK";
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(175, 338);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 19);
            this.cancelButton.TabIndex = 26;
            this.cancelButton.Text = "&Cancel";
            // 
            // filterBase
            // 
            this.filterBase.AutoSize = true;
            this.filterBase.Location = new System.Drawing.Point(6, 21);
            this.filterBase.Name = "filterBase";
            this.filterBase.Size = new System.Drawing.Size(50, 17);
            this.filterBase.TabIndex = 28;
            this.filterBase.Text = "Base";
            this.filterBase.UseVisualStyleBackColor = true;
            this.filterBase.CheckedChanged += new System.EventHandler(this.filter_CheckedChanged);
            // 
            // itemsList
            // 
            this.itemsList.FormattingEnabled = true;
            this.itemsList.Location = new System.Drawing.Point(1, 57);
            this.itemsList.Name = "itemsList";
            this.itemsList.Size = new System.Drawing.Size(289, 274);
            this.itemsList.TabIndex = 29;
            this.itemsList.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.itemsList_ItemCheck);
            // 
            // filterAllMods
            // 
            this.filterAllMods.AutoSize = true;
            this.filterAllMods.Location = new System.Drawing.Point(62, 21);
            this.filterAllMods.Name = "filterAllMods";
            this.filterAllMods.Size = new System.Drawing.Size(66, 17);
            this.filterAllMods.TabIndex = 30;
            this.filterAllMods.Text = "All Mods";
            this.filterAllMods.UseVisualStyleBackColor = true;
            this.filterAllMods.CheckedChanged += new System.EventHandler(this.filter_CheckedChanged);
            // 
            // filterCurrent
            // 
            this.filterCurrent.AutoSize = true;
            this.filterCurrent.Location = new System.Drawing.Point(134, 21);
            this.filterCurrent.Name = "filterCurrent";
            this.filterCurrent.Size = new System.Drawing.Size(84, 17);
            this.filterCurrent.TabIndex = 31;
            this.filterCurrent.Text = "Current Mod";
            this.filterCurrent.UseVisualStyleBackColor = true;
            this.filterCurrent.CheckedChanged += new System.EventHandler(this.filter_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.AutoSize = true;
            this.groupBox1.Controls.Add(this.filterCurrent);
            this.groupBox1.Controls.Add(this.filterBase);
            this.groupBox1.Controls.Add(this.filterAllMods);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(292, 57);
            this.groupBox1.TabIndex = 32;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Filter";
            // 
            // FormSelect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 361);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.itemsList);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Name = "FormSelect";
            this.Text = "Select";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.CheckBox filterBase;
        private System.Windows.Forms.CheckedListBox itemsList;
        private System.Windows.Forms.CheckBox filterAllMods;
        private System.Windows.Forms.CheckBox filterCurrent;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}