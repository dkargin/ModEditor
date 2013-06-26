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
    public partial class PanelErrors : PanelWindow
    {
        static PanelErrors panelErrors;

        static int maxLogLines = 10;

        List<string> logMessages = new List<string>();

        public PanelErrors()
        {
            InitializeComponent();
            panelErrors = this;
        }

        static public void LogNewString(string message)
        {
            if (panelErrors != null)
            {
                panelErrors.logMessages.Add(message);
                if (panelErrors.logMessages.Count > maxLogLines)
                    panelErrors.logMessages.RemoveAt(0);

                panelErrors.logView.Lines = panelErrors.logMessages.ToArray();
            }
        }

        static public void LogInfoString(string message)
        {
            LogNewString(String.Format("Info: {0}", message));
        }

        static public void LogErrorString(string message)
        {
            LogNewString(String.Format("Error: {0}", message));
        }
    }
}
