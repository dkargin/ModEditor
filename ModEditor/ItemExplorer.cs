using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ModEditor.Controls;

namespace ModEditor
{
    public partial class ItemExplorer : UserControl
    {
        //public static EditorManager manager = new EditorManager();
        public ItemExplorer()
        {
            InitializeComponent();
        }

        private int rows = 0;

        public Dictionary<string, EditorManager.FieldEditor> editors = new Dictionary<string,EditorManager.FieldEditor>();

        public void Init<DataType>(ModContents.Item item, DataType data)
        {
            Type typedef = typeof(DataType);
           // ModContents.Item baseItem = item as 
            this.valueType.Text = typedef.Name;
            this.valuePath.Text = item.GetPath();
            this.SuspendLayout();
            foreach (var member in typedef.GetFields())            
            {
                var value = member.GetValue(data);
                if (value != null)
                {
                    EditorManager.FieldEditor editor = item.controller.GenerateFieldEditor(member, item);

                    if (editor == null)
                        continue;
                    Control control = editor.GetControl();

                    //Control control = item.controller.GenerateControl(member, data);
                    control.Dock = DockStyle.Fill;
                    if (control != null)
                    {
                        this.dataTable.Controls.Add(new Label() { Text = member.Name }, 0, rows);
                        this.dataTable.Controls.Add(control, 1, rows);
                        editors.Add(member.Name, editor);
                        rows++;
                    }
                }
            }
            this.ResumeLayout();
            UpdateData();
        }

        public void UpdateData()
        {
            foreach (var editor in editors)
            {
                editor.Value.UpdateValue();
            }
        }        
    }
}
