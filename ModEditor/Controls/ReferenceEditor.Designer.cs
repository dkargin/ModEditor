namespace ModEditor.Controls
{
    partial class ReferenceEditor
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
            this.refName = new System.Windows.Forms.TextBox();
            this.pick = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // refName
            // 
            this.refName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.refName.Location = new System.Drawing.Point(0, 0);
            this.refName.Name = "refName";
            this.refName.ReadOnly = true;
            this.refName.Size = new System.Drawing.Size(119, 20);
            this.refName.TabIndex = 0;
            // 
            // pick
            // 
            this.pick.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pick.Location = new System.Drawing.Point(125, 0);
            this.pick.Name = "pick";
            this.pick.Size = new System.Drawing.Size(25, 20);
            this.pick.TabIndex = 1;
            this.pick.Text = "...";
            this.pick.UseVisualStyleBackColor = true;
            // 
            // ReferenceEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pick);
            this.Controls.Add(this.refName);
            this.MinimumSize = new System.Drawing.Size(150, 21);
            this.Name = "ReferenceEditor";
            this.Size = new System.Drawing.Size(150, 21);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.TextBox refName;
        public System.Windows.Forms.Button pick;

    }
}
