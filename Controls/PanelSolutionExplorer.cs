using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ModEditor.Controls
{
    public partial class PanelSolutionExplorer : PanelWindow
    {
        MainForm mainForm;

        public PanelSolutionExplorer(MainForm form)
        {
            mainForm = form;
            InitializeComponent();
        }

        private void ModContentsTree_BeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            Item item = e.Node.Tag as Item;
            if (item == null || !item.isNameEditable)
            {
                e.CancelEdit = true;
            }
        }

        private void ModContentsTree_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            Item item = e.Node.Tag as Item;
            if (item == null || !item.isNameEditable)
            {
                e.CancelEdit = true;
            }
            else if (!item.controller.RenameItem(item, e.Label))
            {
                e.CancelEdit = true;
            }
        }

        private void ModContentsTree_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                TreeNode selected = e.Node;
                ModEditor.Item item = selected.Tag as ModEditor.Item;
                if (selected != null && item != null)
                {
                    mainForm.ExploreItem(item);
                }
            }
        }

        private void ModContentsTree_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                // Select the clicked node
                TreeNode selected = ModContentsTree.GetNodeAt(e.X, e.Y);
                ModContentsTree.SelectedNode = selected;

                if (selected != null && selected.Tag != null)
                {
                    ModEditor.Item item = selected.Tag as ModEditor.Item;
                    if (item != null)
                    {
                        ContextMenuStrip menu = item.GenerateContextMenu();
                        if (menu != null)
                            menu.Show(ModContentsTree, e.Location);
                    }
                }
            }
        }
    }
}
