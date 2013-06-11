using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ModEditor
{
    public partial class FormSelect : Form
    {
        public class Item
        {
            public string name;
            public bool fromBase;
            public bool fromCurrentMod;
            public bool fromMods;

            public bool selected;

            public override string ToString()
            {
                return name;
            }
        }

        List<Item> items = new List<Item>();
        public Item selected = null;

        bool multiple = false;

        ListBox listBox;
        
        public FormSelect(bool multiple)
        {
            SuspendLayout();
            this.multiple = multiple;
            if (multiple)
            {
                var control = new CheckedListBox();
                control.ItemCheck += itemsList_ItemCheck;
                listBox = control;                
            }
            else 
            {
                listBox = new ListBox();
                listBox.SelectionMode = SelectionMode.One;
                listBox.SelectedValueChanged += listBox_SelectedValueChanged;
            }
            listBox.Dock = DockStyle.Fill;
            
            ResumeLayout();
            InitializeComponent();
        }

        void listBox_SelectedValueChanged(object sender, EventArgs e)
        {
            this.selected = listBox.SelectedItem as Item;            
        }

        public void AddItem(Item item)
        {
            items.Add(item);
        }
        public bool ItemFiltered(Item item)
        {
            if (item.fromBase && filterBase.Checked)
                return true;
            if (item.fromCurrentMod && filterCurrent.Checked)
                return true;
            if (item.fromMods && filterAllMods.Checked)
                return true;
            return false;
        }

        private void AddItemToView(Item item)
        {
            if (multiple)
            {
                (listBox as CheckedListBox).Items.Add(item, item.selected);                   
            }
            else
            {
                listBox.Items.Add(item);
                if (item.selected)
                {
                    listBox.SelectedItem = item;
                    selected = item;
                }
            }
        }
        public void UpdateList()
        {
            listBox.Items.Clear();
            foreach (var item in items)
            {
                if (!ItemFiltered(item))
                    AddItemToView(item);
            }
            listBox.Sorted = true;
        }

        private void itemsList_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            var item = ((listBox as CheckedListBox).Items[e.Index] as Item);
            item.selected = (e.NewValue == CheckState.Checked);
        }

        private void filter_CheckedChanged(object sender, EventArgs e)
        {
            UpdateList();
        }

        private void FormSelect_Load(object sender, EventArgs e)
        {
            areaContents.Controls.Add(listBox);
            UpdateList();
        }        
    }
}
