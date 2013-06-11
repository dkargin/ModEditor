using ModEditor.Controls;
namespace ModEditor
{
    partial class ItemView
    {

        #region Component Designer generated code
        private void InitializeComponent()
        {
            System.Windows.Forms.Panel dataPanel;
            System.Windows.Forms.TableLayoutPanel headerContainer;
            this.dataTable = new PropertyGridExplorer();
            this.labelPath = new System.Windows.Forms.Label();
            this.labelType = new System.Windows.Forms.Label();
            this.valueType = new System.Windows.Forms.TextBox();
            this.valuePath = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            dataPanel = new System.Windows.Forms.Panel();
            headerContainer = new System.Windows.Forms.TableLayoutPanel();
            dataPanel.SuspendLayout();
            headerContainer.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataPanel
            // 
            dataPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            dataPanel.AutoScroll = true;
            dataPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            dataPanel.Controls.Add(this.dataTable);
            dataPanel.Location = new System.Drawing.Point(3, 60);
            dataPanel.Name = "dataPanel";
            dataPanel.Size = new System.Drawing.Size(388, 319);
            dataPanel.TabIndex = 1;
            // 
            // dataTable
            // 
            this.dataTable.AutoScroll = true;
            this.dataTable.AutoSize = true;
            this.dataTable.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.dataTable.ColumnCount = 2;
            this.dataTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            this.dataTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.dataTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataTable.Location = new System.Drawing.Point(0, 0);
            this.dataTable.Name = "dataTable";
            this.dataTable.Padding = new System.Windows.Forms.Padding(1);
            this.dataTable.RowCount = 2;
            this.dataTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            this.dataTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            this.dataTable.Size = new System.Drawing.Size(386, 317);
            this.dataTable.TabIndex = 3;
            // 
            // headerContainer
            // 
            headerContainer.AutoSize = true;
            headerContainer.ColumnCount = 2;
            headerContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.71429F));
            headerContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 66.28571F));
            headerContainer.Controls.Add(this.labelPath, 0, 1);
            headerContainer.Controls.Add(this.labelType, 0, 0);
            headerContainer.Controls.Add(this.valueType, 1, 0);
            headerContainer.Controls.Add(this.valuePath, 1, 1);
            headerContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            headerContainer.Location = new System.Drawing.Point(0, 0);
            headerContainer.Name = "headerContainer";
            headerContainer.RowCount = 2;
            headerContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            headerContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            headerContainer.Size = new System.Drawing.Size(386, 52);
            headerContainer.TabIndex = 2;
            // 
            // labelPath
            // 
            this.labelPath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelPath.Location = new System.Drawing.Point(3, 26);
            this.labelPath.Name = "labelPath";
            this.labelPath.Size = new System.Drawing.Size(124, 26);
            this.labelPath.TabIndex = 4;
            this.labelPath.Text = "XML Path";
            this.labelPath.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelType
            // 
            this.labelType.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelType.Location = new System.Drawing.Point(3, 0);
            this.labelType.Name = "labelType";
            this.labelType.Size = new System.Drawing.Size(124, 26);
            this.labelType.TabIndex = 3;
            this.labelType.Text = "Type";
            this.labelType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // valueType
            // 
            this.valueType.Dock = System.Windows.Forms.DockStyle.Fill;
            this.valueType.Location = new System.Drawing.Point(133, 3);
            this.valueType.Name = "valueType";
            this.valueType.ReadOnly = true;
            this.valueType.Size = new System.Drawing.Size(250, 20);
            this.valueType.TabIndex = 0;
            // 
            // valuePath
            // 
            this.valuePath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.valuePath.Location = new System.Drawing.Point(133, 29);
            this.valuePath.Name = "valuePath";
            this.valuePath.ReadOnly = true;
            this.valuePath.Size = new System.Drawing.Size(250, 20);
            this.valuePath.TabIndex = 1;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.AutoSize = true;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(headerContainer);
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(388, 54);
            this.panel1.TabIndex = 3;
            // 
            // ItemExplorer
            // 
            this.Controls.Add(dataPanel);
            this.Controls.Add(this.panel1);
            this.Name = "ItemExplorer";
            this.Size = new System.Drawing.Size(394, 382);
            dataPanel.ResumeLayout(false);
            dataPanel.PerformLayout();
            headerContainer.ResumeLayout(false);
            headerContainer.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion


        private System.Windows.Forms.TextBox valueType;
        private System.Windows.Forms.TextBox valuePath;
        private System.Windows.Forms.Label labelPath;
        private System.Windows.Forms.Label labelType;
        private PropertyGridExplorer dataTable;
        private System.Windows.Forms.Panel panel1;
    }
}
