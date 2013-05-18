using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace ModEditor.Controls
{
    class TabControlEx : TabControl
    {

        Bitmap iconClose = null;
        //Bitmap iconOptions = null;
        /*
        public Bitmap IconClose
        {
            get
            {
                return this.IconClose;
            }
            set
            {
                this.IconClose = value;
            }
        }

        public Bitmap IconOptions
        {
            get
            {
                return this.IconOptions;
            }
            set
            {
                this.IconOptions = value;
            }
        }*/

        public TabControlEx()
        {
            
        }
        /// <summary>
        /// override to draw the close button
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            RectangleF tabTextArea = RectangleF.Empty;
            for(int nIndex = 0 ; nIndex < this.TabCount ; nIndex++)
            {
                if( nIndex != this.SelectedIndex )
                {
                    /*if not active draw ,inactive close button*/
                    tabTextArea = (RectangleF)this.GetTabRect(nIndex);
                    if(iconClose != null)
                    //using(Bitmap bmp = new Bitmap(GetContentFromResource("closeinactive.bmp")))
                    {
                        e.Graphics.DrawImage(iconClose, tabTextArea.X + tabTextArea.Width - 16, 5, 13, 13);
                    }
                }
                else
                {
                    tabTextArea = (RectangleF)this.GetTabRect(nIndex);
                  //  SolidBrush br = new SolidBrush(SystemColors.Control);
                    
                    LinearGradientBrush br = new LinearGradientBrush(tabTextArea,
                        SystemColors.ControlLightLight,SystemColors.Control,
                        LinearGradientMode.Vertical);
                    e.Graphics.FillRectangle(br,tabTextArea);

                    /*if active draw ,inactive close button*/
                    if (iconClose != null)
                    //using(Bitmap bmp = new Bitmap( GetContentFromResource("close.bmp")))
                    {
                        e.Graphics.DrawImage(iconClose,
                            tabTextArea.X+tabTextArea.Width -16, 5, 13, 13);
                    }
                    br.Dispose();
                }
                string str = this.TabPages[nIndex].Text;
                StringFormat stringFormat = new StringFormat();
                stringFormat.Alignment = StringAlignment.Center; 
                using(SolidBrush brush = new SolidBrush(this.TabPages[nIndex].ForeColor))
                {
                    /*Draw the tab header text;*/
                    e.Graphics.DrawString(str, this.Font, brush,
                    tabTextArea, stringFormat);
                }
            }
        }
        /// <summary>
        /// Get the stream of the embedded bitmap image
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        private Stream GetContentFromResource(string filename)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            Stream stream = asm.GetManifestResourceStream("MyControlLibrary." + filename);
            return stream;
        }
        private bool confirmOnClose = false;
        public bool ConfirmOnClose
        {
            get
            {
                return this.confirmOnClose;
            }
            set
            {
                this.confirmOnClose = value;
            }
        }
        TabPage GetTabPage(Point pt)
        {
            for (int i = 0; i < this.TabCount; i++)
            {
                var rect = this.GetTabRect(i);
                if (rect.Contains(pt))
                    return this.TabPages[i];
            }
            return null;
        }
        protected RectangleF GetTabTextArea(int index)
        {
            RectangleF tabTextArea = (RectangleF)this.GetTabRect(SelectedIndex);
            tabTextArea = new RectangleF(tabTextArea.X + tabTextArea.Width - 16, 5, 13, 13);
            return tabTextArea;
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            RectangleF tabTextArea = GetTabTextArea(SelectedIndex);                
            Point pt = new Point(e.X, e.Y);
            if (tabTextArea.Contains(pt) && (e.Button == System.Windows.Forms.MouseButtons.Right || e.Button == System.Windows.Forms.MouseButtons.Middle))
            {
                if (confirmOnClose)
                {
                    if (MessageBox.Show("You are about to close " +
                        this.TabPages[SelectedIndex].Text.TrimEnd() +
                        " tab. Are you sure you want to continue?", "Confirm close",
                        MessageBoxButtons.YesNo) == DialogResult.No)
                        return;
                }
                
                TabPage page = GetTabPage(pt);
                if (page != null)
                {
                    this.TabPages.Remove(page);
                }
                
                /*
                //Fire Event to Client
                if (this.OnClose != null)
                {
                    OnClose(this, new CloseEventArgs(SelectedIndex));
                }*/
            }
            base.OnMouseDown(e);
        }
        protected override void OnMouseHover(EventArgs e)
        {
            base.OnMouseHover(e);
        }
        private void RepaintControls(object sender, DrawItemEventArgs e)
        {
            try
            {
                Font _Fnt;
                Brush _BackBrush;
                Brush _ForeBrush;
                Rectangle _Rec = e.Bounds;

                if (e.Index == this.SelectedIndex)
                {
                    // Remove the comment below if you want the font style 
                    // of selected tab page as normal.
                    // _Fnt = new Font(e.Font, e.Font.Style & ~FontStyle.Bold);

                    // Remove the comment below if you want the font style of 
                    // selected tab page as bold.
                    _Fnt = new Font(e.Font, e.Font.Style);

                    _BackBrush = new SolidBrush(this.SelectedTab.BackColor);
                    _ForeBrush = new SolidBrush(this.SelectedTab.ForeColor);
                    _Rec = new Rectangle(_Rec.X + (this.Padding.X / 2),
                            _Rec.Y + this.Padding.Y, _Rec.Width - this.Padding.X,
                            _Rec.Height - (this.Padding.Y * 2));
                }
                else
                {
                    // Remove the comment below if you want the font style 
                    // of inactive tab page as normal.
                    _Fnt = new Font(e.Font, e.Font.Style & ~FontStyle.Bold);

                    // Remove the comment below if you want the font style 
                    // of inactive tab page as bold.
                    //_Fnt = new Font(e.Font, e.Font.Style);
                    /*
                    if (this.InactiveColorOn == true)
                    {
                        _BackBrush = new SolidBrush(this.InactiveBGColor);
                        _ForeBrush = new SolidBrush(this.InactiveFGColor);
                    }
                    else*/
                    {
                        _BackBrush = new SolidBrush(this.TabPages[e.Index].BackColor);
                        _ForeBrush = new SolidBrush(this.TabPages[e.Index].ForeColor);
                    }
                    _Rec = new Rectangle(_Rec.X + (this.Padding.X / 2),
                            _Rec.Y + this.Padding.Y, _Rec.Width - this.Padding.X,
                            _Rec.Height - this.Padding.Y);
                }

                var _TabName = this.TabPages[e.Index].Text;
                StringFormat _SF = new StringFormat();
                _SF.Alignment = StringAlignment.Center;

                e.Graphics.FillRectangle(_BackBrush, _Rec);
                e.Graphics.DrawString(_TabName, _Fnt, _ForeBrush, _Rec, _SF);

                _SF.Dispose();
                _BackBrush.Dispose();
                _ForeBrush.Dispose();
                _Fnt.Dispose();
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message, "Error Occured",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

    }
}
