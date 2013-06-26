using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace ModEditor.Controls
{
    public partial class PropertyGridExplorer : TableLayoutPanel
    {
        static int rowHeight = 21;
        static int firstColumnWidth = 160;

        public bool RowFiller = true;

        public PropertyGridExplorer()
        {
            AutoScroll = true;
            AutoSize = false;
            CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
        }
        
        public class Column
        {
            public Dictionary<string, TypeEditor> editors = new Dictionary<string, TypeEditor>();
            object source;
        }
        
        public Dictionary<string, TypeEditor> editors = new Dictionary<string, TypeEditor>();

        void AddRow(string name, TypeEditor editor, Control control)
        {
            RowStyle rowStyle = new RowStyle()
            {
                Height = rowHeight,
                SizeType = SizeType.AutoSize
            };

            int row = RowStyles.Add(rowStyle);

            var label = new Label()
            {
                Text = name,
                Anchor = (AnchorStyles.Left | AnchorStyles.Right), /*| AnchorStyles.Top | AnchorStyles.Bottom),*/
                TextAlign = ContentAlignment.MiddleLeft,
                //Dock= DockStyle.Right, 
                AutoSize = false,
                BackColor = SystemColors.ControlLightLight,
                Padding = new Padding(0),
                Margin = new Padding(1),
                //Height = rowHeight,
                Width = firstColumnWidth,
            };

            control.Anchor = (AnchorStyles.Left | AnchorStyles.Right);//DockStyle.Fill;
            
            this.Controls.Add(control, 1, row);
            this.Controls.Add(label, 0, row);
            /*
            dataTable.RowStyles[rows].Height = rowHeight;
            dataTable.RowStyles[rows].SizeType = SizeType.Absolute;*/
            editors.Add(name, editor);
        }

        

        public void UpdateData()
        {
            foreach (var editor in editors)
            {
                editor.Value.UpdateValue();
            }
        }

        public void Clear()
        {
            editors.Clear();
            this.RowStyles.Clear();
            this.Controls.Clear();
            this.RowCount = 0;
        }

        // Set complex object as source
        public void SetSource(System.Type type, FieldAccessor callback)
        {
            SuspendLayout();
            Clear();

            if (callback != null)
            {
                foreach (var fieldInfo in type.GetFields())
                {
                    try
                    {
                        TypeEditor editor = FieldEditorManager.GenerateEditor(new ObjectFieldAccessor(callback, fieldInfo));

                        if (editor == null)
                            continue;
                        Control control = editor.GetControl();

                        //Control control = item.controller.GenerateControl(member, data);

                        if (control != null)
                            AddRow(fieldInfo.Name, editor, control);
                    }
                    catch (Exception e)
                    {
                        MainForm.LogErrorString(e.Message);
                    }
                }
            }
            UpdateData();
            // Setup row and column styles
            foreach (RowStyle row in this.RowStyles)
            {
                row.Height = rowHeight;
                row.SizeType = SizeType.AutoSize;
            }

            if (RowFiller)
            {
                RowStyles.Add(new RowStyle()
                {
                    Height = rowHeight,
                    SizeType = SizeType.AutoSize
                });
            }

            this.RowCount = RowStyles.Count;

            if (this.ColumnCount > 0)
            {
                this.ColumnStyles[0].Width = firstColumnWidth;
                this.ColumnStyles[0].SizeType = SizeType.Absolute;
                this.ColumnStyles[1].SizeType = SizeType.AutoSize;
            }
            ResumeLayout();
        } 
        
        private void PropertyGridExplorer_MouseHover(object sender, EventArgs e)
        {
            //int width = this.ColumnStyles[0].Width
        } 
    }
}
