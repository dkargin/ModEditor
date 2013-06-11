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
    public partial class StringEditor : UserControl
    {
        public StringEditor()
        {
            InitializeComponent();
        }

        private void StringEditor_Load(object sender, EventArgs e)
        {

        }

        public TextBox TokenBox
        {
            get
            {
                return valueToken;
            }
        }

        public TextBox ValueBox
        {
            get
            {
                return valueText;
            }
        }

        public Button EditButton
        {
            get
            {
                return pickString;
            }
        }       
    }
}
