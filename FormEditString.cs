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
    public partial class FormEditString : Form
    {
        public int token;
        DataTable stringsTable;

        public FormEditString(int token)
        {
            InitializeComponent();
            SetToken(token);
        }

        public void SetToken(int token)
        {
            stringsTable = Controllers.StringsController.LoadDataTable(token);
            stringsTable.ColumnChanged += stringsTable_ColumnChanged;
            this.tokenView.Minimum = int.MinValue;
            this.tokenView.Maximum = int.MaxValue;
            this.tokenView.Value = (Decimal)Convert.ChangeType(token, typeof(Decimal)); ;
            this.stringsView.DataSource = stringsTable;
            this.stringsView.AllowUserToAddRows = false;
        }

        void stringsTable_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            if (e.Column.ColumnName == "Value")
            {
                string value = e.Row[1].ToString();
                string language = e.Row[0].ToString();
                Controllers.StringsController.SetLocString(token, value, language);
            }            
        }

        private void tokenView_ValueChanged(object sender, EventArgs e)
        {
            int token = (int)Convert.ChangeType(tokenView.Value, typeof(int));
            SetToken(token);
        }
    }
}
