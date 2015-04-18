namespace ModEditor.Controls
{
    partial class ListItemCaption
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cmdDelete = new System.Windows.Forms.Button();
            this.cmdCopy = new System.Windows.Forms.Button();
            this.itemName = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // cmdDelete
            // 
            this.cmdDelete.Location = new System.Drawing.Point(85, 3);
            this.cmdDelete.Name = "cmdDelete";
            this.cmdDelete.Size = new System.Drawing.Size(38, 23);
            this.cmdDelete.TabIndex = 0;
            this.cmdDelete.Text = "Del";
            this.cmdDelete.UseVisualStyleBackColor = true;
            // 
            // cmdCopy
            // 
            this.cmdCopy.Location = new System.Drawing.Point(129, 3);
            this.cmdCopy.Name = "cmdCopy";
            this.cmdCopy.Size = new System.Drawing.Size(42, 23);
            this.cmdCopy.TabIndex = 1;
            this.cmdCopy.Text = "Copy";
            this.cmdCopy.UseVisualStyleBackColor = true;
            // 
            // itemName
            // 
            this.itemName.Location = new System.Drawing.Point(3, 3);
            this.itemName.Name = "itemName";
            this.itemName.Size = new System.Drawing.Size(76, 23);
            this.itemName.TabIndex = 2;
            this.itemName.Text = "itemName";
            this.itemName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ListItemCaption
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.itemName);
            this.Controls.Add(this.cmdCopy);
            this.Controls.Add(this.cmdDelete);
            this.MinimumSize = new System.Drawing.Size(174, 29);
            this.Name = "ListItemCaption";
            this.Size = new System.Drawing.Size(174, 29);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Button cmdDelete;
        public System.Windows.Forms.Button cmdCopy;
        public System.Windows.Forms.Label itemName;

    }
}
