﻿using ModEditor.Controls;
namespace ModEditor
{
    partial class ItemView
    {

        #region Component Designer generated code
        private void InitializeComponent()
        {
            System.Windows.Forms.Panel dataPanel;
            System.Windows.Forms.TableLayoutPanel headerContainer;
            this.dataTable = new ModEditor.Controls.PropertyGridExplorer();
            this.panel1 = new System.Windows.Forms.Panel();
            this.gotoPrev = new System.Windows.Forms.Button();
            this.labelPath = new System.Windows.Forms.Label();
            this.labelType = new System.Windows.Forms.Label();
            this.valueType = new System.Windows.Forms.TextBox();
            this.valuePath = new System.Windows.Forms.TextBox();
            this.gotoNext = new System.Windows.Forms.Button();
            dataPanel = new System.Windows.Forms.Panel();
            headerContainer = new System.Windows.Forms.TableLayoutPanel();
            dataPanel.SuspendLayout();
            this.panel1.SuspendLayout();
            headerContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataPanel
            // 
            dataPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            dataPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            dataPanel.Controls.Add(this.dataTable);
            dataPanel.Location = new System.Drawing.Point(0, 61);
            dataPanel.Name = "dataPanel";
            dataPanel.Size = new System.Drawing.Size(394, 318);
            dataPanel.TabIndex = 1;
            // 
            // dataTable
            // 
            this.dataTable.AutoScroll = true;
            this.dataTable.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
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
            this.dataTable.Size = new System.Drawing.Size(392, 316);
            this.dataTable.TabIndex = 3;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(headerContainer);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(394, 56);
            this.panel1.TabIndex = 2;
            // 
            // headerContainer
            // 
            headerContainer.AutoSize = true;
            headerContainer.ColumnCount = 3;
            headerContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 32.96703F));
            headerContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 43.95604F));
            headerContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 23.07692F));
            headerContainer.Controls.Add(this.gotoPrev, 2, 1);
            headerContainer.Controls.Add(this.labelPath, 0, 1);
            headerContainer.Controls.Add(this.labelType, 0, 0);
            headerContainer.Controls.Add(this.valueType, 1, 0);
            headerContainer.Controls.Add(this.valuePath, 1, 1);
            headerContainer.Controls.Add(this.gotoNext, 2, 0);
            headerContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            headerContainer.Location = new System.Drawing.Point(0, 0);
            headerContainer.Name = "headerContainer";
            headerContainer.Padding = new System.Windows.Forms.Padding(1);
            headerContainer.RowCount = 2;
            headerContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            headerContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            headerContainer.Size = new System.Drawing.Size(392, 54);
            headerContainer.TabIndex = 3;
            // 
            // gotoPrev
            // 
            this.gotoPrev.Location = new System.Drawing.Point(303, 30);
            this.gotoPrev.Name = "gotoPrev";
            this.gotoPrev.Size = new System.Drawing.Size(84, 20);
            this.gotoPrev.TabIndex = 6;
            this.gotoPrev.Text = "Prev";
            this.gotoPrev.UseVisualStyleBackColor = true;
            // 
            // labelPath
            // 
            this.labelPath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelPath.Location = new System.Drawing.Point(4, 27);
            this.labelPath.Name = "labelPath";
            this.labelPath.Size = new System.Drawing.Size(122, 26);
            this.labelPath.TabIndex = 4;
            this.labelPath.Text = "XML Path";
            this.labelPath.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelType
            // 
            this.labelType.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelType.Location = new System.Drawing.Point(4, 1);
            this.labelType.Name = "labelType";
            this.labelType.Size = new System.Drawing.Size(122, 26);
            this.labelType.TabIndex = 3;
            this.labelType.Text = "Type";
            this.labelType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // valueType
            // 
            this.valueType.Dock = System.Windows.Forms.DockStyle.Fill;
            this.valueType.Location = new System.Drawing.Point(132, 4);
            this.valueType.Name = "valueType";
            this.valueType.ReadOnly = true;
            this.valueType.Size = new System.Drawing.Size(165, 20);
            this.valueType.TabIndex = 0;
            // 
            // valuePath
            // 
            this.valuePath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.valuePath.Location = new System.Drawing.Point(132, 30);
            this.valuePath.Name = "valuePath";
            this.valuePath.ReadOnly = true;
            this.valuePath.Size = new System.Drawing.Size(165, 20);
            this.valuePath.TabIndex = 1;
            // 
            // gotoNext
            // 
            this.gotoNext.Location = new System.Drawing.Point(303, 4);
            this.gotoNext.Name = "gotoNext";
            this.gotoNext.Size = new System.Drawing.Size(84, 20);
            this.gotoNext.TabIndex = 5;
            this.gotoNext.Text = "Next";
            this.gotoNext.UseVisualStyleBackColor = true;
            // 
            // ItemView
            // 
            this.Controls.Add(this.panel1);
            this.Controls.Add(dataPanel);
            this.Name = "ItemView";
            this.Size = new System.Drawing.Size(394, 382);
            dataPanel.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            headerContainer.ResumeLayout(false);
            headerContainer.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private PropertyGridExplorer dataTable;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button gotoPrev;
        private System.Windows.Forms.Label labelPath;
        private System.Windows.Forms.Label labelType;
        private System.Windows.Forms.TextBox valueType;
        private System.Windows.Forms.TextBox valuePath;
        private System.Windows.Forms.Button gotoNext;
    }
}
