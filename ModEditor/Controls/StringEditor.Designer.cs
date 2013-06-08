namespace ModEditor.Controls
{
    partial class StringEditor
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
            this.valueToken = new System.Windows.Forms.TextBox();
            this.valueText = new System.Windows.Forms.TextBox();
            this.pickString = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // valueToken
            // 
            this.valueToken.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.valueToken.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.valueToken.Location = new System.Drawing.Point(3, 4);
            this.valueToken.Name = "valueToken";
            this.valueToken.Size = new System.Drawing.Size(56, 13);
            this.valueToken.TabIndex = 1;
            // 
            // valueText
            // 
            this.valueText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.valueText.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.valueText.Location = new System.Drawing.Point(94, 4);
            this.valueText.Name = "valueText";
            this.valueText.Size = new System.Drawing.Size(88, 13);
            this.valueText.TabIndex = 2;
            // 
            // pickString
            // 
            this.pickString.Location = new System.Drawing.Point(62, 0);
            this.pickString.Name = "pickString";
            this.pickString.Size = new System.Drawing.Size(26, 19);
            this.pickString.TabIndex = 3;
            this.pickString.Text = "E";
            this.pickString.UseVisualStyleBackColor = true;
            // 
            // StringEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pickString);
            this.Controls.Add(this.valueText);
            this.Controls.Add(this.valueToken);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "StringEditor";
            this.Size = new System.Drawing.Size(185, 21);
            this.Load += new System.EventHandler(this.StringEditor_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox valueToken;
        private System.Windows.Forms.TextBox valueText;
        private System.Windows.Forms.Button pickString;
    }
}
