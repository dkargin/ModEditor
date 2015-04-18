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
    public partial class PanelReport : PanelWindow
    {
        public PanelReport()
        {
            InitializeComponent();
        }

        void AddReport(ItemReport report)
        {
            ListViewItem item = new ListViewItem(new string[] { report.item.controller.GetGroupFolder(), report.item.name, report.path, report.Message() });
            item.Tag = report;
            reportView.Items.Add(item); 
        }

        void CheckReports()
        {
            List<ListViewItem> toDelete = new List<ListViewItem>();
            foreach (ListViewItem item in reportView.Items)
            {
                ItemReport report = item.Tag as ItemReport;
                if (report.Check())
                    toDelete.Add(item);
            }
            foreach (var item in toDelete)
            {
                reportView.Items.Remove(item);
            }
        }
        public void ClearReport()
        {
            reportView.Items.Clear();
        }
        public void OnErrorFound(ItemReport report)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)(() => 
                {
                    AddReport(report); 
                }
                ));
            }
            else
                AddReport(report); 
        }

        private void reportView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            try
            {
                ItemReport report = e.Item.Tag as ItemReport;
                if (report.item != null && e.IsSelected)
                    MainForm.SelectItem(report.item);
            }
            catch (Exception)
            {
            }
        }
    }
}
