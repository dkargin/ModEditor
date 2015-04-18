using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ModEditor.Controls
{
    public partial class PanelItemContainer : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        Item attachedItem;

        public PanelItemContainer()
        {
            InitializeComponent();
        }

        public void AttachItem(Item item, Control control)
        {
            container.Controls.Clear();
            attachedItem = item;
            control.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            container.Controls.Add(control);
            control.ClientSize = container.ClientSize;
        }

        public Item Item
        {
            get
            {
                return attachedItem;
            }
        }

        public void Unlink()
        {
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            MainForm.OnPanelItemClosed(this); ;
        }
    }
}
