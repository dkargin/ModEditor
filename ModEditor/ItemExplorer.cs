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

        public void Init<ItemType>(ItemType item)
        {
            Type typedef = typeof(ItemType);
            this.valueType.Text = typedef.Name;
            this.SuspendLayout();
            foreach (var member in typedef.GetFields())            
            {
                var value = member.GetValue(item);
                if (value != null)
                {
                    EditorManager.FieldEditor editor = EditorManager.generateControl(member, item);
                    Control control = editor.GetControl();
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
