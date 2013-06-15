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
    public partial class ItemView : UserControl
    {       
        public ItemView()
        {
            InitializeComponent();
        }
        
        public class ItemFieldCallback : ModEditor.FieldCallback
        {
            public Item item;
            public ItemFieldCallback(Item item)
            {
                this.item = item;
            }

            public bool ReadOnly()
            {
                return item.IsBase();
            }

            public System.Type GetTargetType()
            {
                return item.controller.TargetType;
            }

            public object ReadValue(System.Reflection.FieldInfo fieldInfo)
            {
                return ModContents.ReadItemValue(item, fieldInfo);
            }

            public void WriteValue(System.Reflection.FieldInfo fieldInfo, object value)
            {
                ModContents.ChangeItemValue(item, fieldInfo, value);
            }
        }

        ItemFieldCallback callback;

        public void Init(System.Type typedef, ModEditor.Item item)
        {            
            callback = new ItemFieldCallback(item);
            this.valueType.Text = typedef.Name;

            this.SuspendLayout();
            dataTable.SetSource(typedef, callback);
            this.ResumeLayout();

            this.callback.item.dataChanged += (object sender, EventArgs e) => { UpdateData(); };

            UpdateData();          
        }
        
        public void UpdateData()
        {
            if (callback == null || callback.item == null)
                return;
            Item item = callback.item;

            this.valuePath.Text = item.Path;

            if (item.Next == null)
                gotoNext.Enabled = false;
            else
            {
                gotoNext.Enabled = true;
                gotoNext.Click += (object sender, EventArgs e) => { MainForm.SelectItem(item.Next); };
            }

            if (item.Prev == null)
                gotoPrev.Enabled = false;
            else
            {
                gotoPrev.Enabled = true;
                gotoPrev.Click += (object sender, EventArgs e) => { MainForm.SelectItem(item.Prev); };
            }

            dataTable.UpdateData();
        }        
    }
}
