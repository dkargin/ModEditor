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

        public FormEditContainer(System.Type targetType, System.Collections.IList source)
        {
            InitializeComponent();
            this.targetType = targetType;

            foreach(var obj in source)
            {
                data.Add(obj);
            }

            UpdateList();
        }

        void UpdateList()
        {
            listView.Items.Clear();
            foreach (var obj in data)
            {
                listView.Items.Add(obj);
            }
        }

        public class ObjectFieldCallback : ModEditor.FieldCallback
        {
            object item;
            bool readOnly;
            System.Type targetType;
            public ObjectFieldCallback(object item, System.Type type, bool readOnly)
            {
                this.item = item;
                this.readOnly = readOnly;
                this.targetType = type;
            }

            public bool ReadOnly()
            {
                return readOnly;
            }            

            public System.Type GetTargetType()
            {
                return targetType;
            }

            public object ReadValue(System.Reflection.FieldInfo fieldInfo)
            {
                return fieldInfo.GetValue(item);
            }

            public void WriteValue(System.Reflection.FieldInfo fieldInfo, object value)
            {
                fieldInfo.SetValue(item, value);
            }
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
                itemView.SetSource(targetType, new ObjectFieldCallback(item, targetType, false));
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
                object item = System.Activator.CreateInstance(targetType, new object[] { });
                if (item != null)
                {
                    data.Add(item);
                    UpdateList();
                }
            }
            catch(Exception ex)
            {
                MainForm.LogErrorString(ex.Message);
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
