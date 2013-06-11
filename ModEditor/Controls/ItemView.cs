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
        
        public void Init(System.Type typedef, ModEditor.Item item)
        {            
                // ModContents.Item baseItem = item as 
                this.valueType.Text = typedef.Name;
                this.valuePath.Text = item.GetPath();
                this.SuspendLayout();
                dataTable.SetSource(typedef, item);
                this.ResumeLayout();
                UpdateData();            
        }

        public void UpdateData()
        {
            dataTable.UpdateData();
        }        
    }
}
