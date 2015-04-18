namespace ModEditor
{
    partial class FormSimpleEditContainer
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
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.contents = new System.Windows.Forms.TableLayoutPanel();
            this.SuspendLayout();
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(239, 451);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 19);
            this.cancelButton.TabIndex = 36;
            this.cancelButton.Text = "&Cancel";
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(108, 451);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 19);
            this.okButton.TabIndex = 35;
            this.okButton.Text = "&OK";
            // 
            // contents
            // 
            this.contents.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.contents.AutoScroll = true;
            this.contents.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.contents.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.contents.ColumnCount = 2;
            this.contents.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.contents.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.contents.Location = new System.Drawing.Point(6, 12);
            this.contents.Name = "contents";
            this.contents.RowCount = 2;
            this.contents.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.contents.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.contents.Size = new System.Drawing.Size(547, 433);
            this.contents.TabIndex = 43;
            // 
            // FormSimpleEditContainer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(565, 472);
            this.Controls.Add(this.contents);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Name = "FormSimpleEditContainer";
            this.Text = "FormSimpleEditContainer";
            this.Load += new System.EventHandler(this.FormSimpleEditContainer_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.TableLayoutPanel contents;
    }
}