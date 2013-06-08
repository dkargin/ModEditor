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
        
        public FormSelect()
        {
            InitializeComponent();
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

        public void UpdateList()
        {
            itemsList.Items.Clear();
            foreach (var item in items)
            {
                if (!ItemFiltered(item))
                    itemsList.Items.Add(item, item.selected);                   
            }
        }

        public CheckedListBox GetList()
        {
            return itemsList;
        }

        private void itemsList_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            var item = (itemsList.Items[e.Index] as Item);
            item.selected = (e.NewValue == CheckState.Checked);
        }

        private void filter_CheckedChanged(object sender, EventArgs e)
        {
            UpdateList();
        }        
    }
}
