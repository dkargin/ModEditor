using ModEditor.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Forms;

namespace ModEditor
{
    public partial class FormEditContainer : Form
    {
        public System.Type targetType;
        public List<object> data = new List<object>();
        // Attributes used to override targetType. Mostly for overriding primitive types
        public List<ModEditorAttribute> attributes;

        public FormEditContainer(System.Type targetType, System.Collections.IList source, List<ModEditorAttribute> attributes)
        {
            InitializeComponent();
            this.targetType = targetType;
            this.attributes = attributes;

            foreach(var obj in source)
            {
                data.Add(obj);
            }

            UpdateList();
        }

        // This editor does not bother undo/redo, so we can use generic accessors
        Control GenerateItemControl(Object obj, int index)
        {
            string name = String.Format("Item {0}", index+1);
            if(FieldEditorManager.IsPrimitiveType(targetType))
            {
                ListObjectAccessor accessor = new ListObjectAccessor(data, index, targetType, name, false, attributes);
                TypeEditor editor = FieldEditorManager.GenerateEditor(accessor);
                editor.UpdateValue();
                return editor.GetControl();
            }
            else
            {
                GenericFieldAccessor callback = new GenericFieldAccessor(obj, targetType, false);
                PropertyGridExplorer explorer = new PropertyGridExplorer();
                explorer.AutoSize = true;
                explorer.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
                //explorer.Width = CustomList.Width;
                
                explorer.GrowStyle = TableLayoutPanelGrowStyle.AddRows;
                //explorer.Dock = DockStyle.Fill;
                explorer.Anchor = AnchorStyles.Left | AnchorStyles.Right;
                explorer.SetSource(this.targetType, callback);
                return explorer;
            }
        }

        void UpdateList()
        {
            this.SuspendLayout();
            listView.Items.Clear();
            int i = 0;
            foreach (var obj in data)
            {
                listView.Items.Add(obj);
                CustomList.Controls.Add(GenerateItemControl(obj, i++));
            }
            this.ResumeLayout();
        }       
        

        void ExploreItem(object item)
        {
            SuspendLayout();
            if (item == null)
            {
                itemView.SetSource(targetType, null);
            }
            else
            {
                itemView.SetSource(targetType, new GenericFieldAccessor(item, targetType, false));
            }
            ResumeLayout();
        }

        private void FormEditContainer_Load(object sender, EventArgs e)
        {

        }

        private void listView_SelectedIndexChanged(object sender, EventArgs e)
        {
            ExploreItem(listView.SelectedItem);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                object item = FieldEditorManager.CreateObject(targetType);

                if (item != null)
                {
                    data.Add(item);
                    UpdateList();
                }
            }
            catch(Exception ex)
            {
                PanelErrors.LogErrorString(ex.Message);
            }                     
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            if (listView.SelectedItem != null)
            {
                object selected = listView.SelectedItem;
                object item = Tools.Clone(selected, targetType);
                if (item != null)
                {
                    int index = listView.Items.IndexOf(selected);
                    listView.Items.Insert(index, item);
                    data.Insert(index, item);
                    UpdateList();
                }
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (listView.SelectedItem != null)
            {
                itemView.SetSource(targetType, null);
                data.Remove(listView.SelectedItem);
                listView.Items.Remove(listView.SelectedItem);
                UpdateList();                
            }
        }
    }
}
