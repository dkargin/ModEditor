using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace ModEditor.Controls
{
    class TabControlEx : TabControl
    {        
        int hoverIndex = -1;
       
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
                    SolidBrush br = new SolidBrush(nIndex == hoverIndex ? Color.Blue : SystemColors.Control);
                    e.Graphics.FillRectangle(br, tabTextArea);
                    /*
                    if (renderElementClose != null)
                    //using(Bitmap bmp = new Bitmap(GetContentFromResource("closeinactive.bmp")))
                    {
                        renderElementClose.DrawBackground(e.Graphics, new Rectangle((int)(tabTextArea.X + tabTextArea.Width) - 16, 5, 13, 13));
                        //e.Graphics.DrawImage(iconClose, tabTextArea.X + tabTextArea.Width - 16, 5, 13, 13);
                    }*/
                }
                else
                {
                    tabTextArea = (RectangleF)this.GetTabRect(nIndex);
                    SolidBrush br = new SolidBrush(nIndex == hoverIndex ? Color.Blue : SystemColors.Control);
                    /*
                    LinearGradientBrush br = new LinearGradientBrush(tabTextArea,
                        SystemColors.ControlLightLight,SystemColors.Control,
                        LinearGradientMode.Vertical);*/
                    e.Graphics.FillRectangle(br,tabTextArea);

                    /*if active draw ,inactive close button*/
                    /*
                    if (iconClose != null)
                    //using(Bitmap bmp = new Bitmap( GetContentFromResource("close.bmp")))
                    {
                        renderElementClose.DrawBackground(e.Graphics, new Rectangle((int)(tabTextArea.X + tabTextArea.Width) - 16, 5, 13, 13));
                        //e.Graphics.DrawImage(iconClose, tabTextArea.X+tabTextArea.Width -16, 5, 13, 13);
                    }*/
                    Rectangle buttonRect = (Rectangle)this.GetTabCloseArea(nIndex);
                    if (nIndex == this.hoverIndex)
                    {                        
                        ControlPaint.DrawCaptionButton(e.Graphics, buttonRect, CaptionButton.Close, ButtonState.Normal);                             
                    }
                    else
                    {
                        
                    }
                    
                    br.Dispose();
                }
                string str = this.TabPages[nIndex].Text;
                StringFormat stringFormat = new StringFormat();
                stringFormat.Alignment = StringAlignment.Center;
                stringFormat.Trimming = StringTrimming.EllipsisCharacter;
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

        protected Rectangle GetTabCloseArea(int index)
        {
            Rectangle tabTextArea = this.GetTabRect(SelectedIndex);
            tabTextArea = new Rectangle(tabTextArea.X + tabTextArea.Width - 16, 5, 13, 13);
            return tabTextArea;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            RectangleF tabArea = (RectangleF)this.GetTabRect(SelectedIndex);               
            Point pt = new Point(e.X, e.Y);
            TabPage page = this.GetTabPage(pt);
            if (page != null && (e.Button == System.Windows.Forms.MouseButtons.Right || e.Button == System.Windows.Forms.MouseButtons.Middle))
            {
                ModEditor.Item item = (ModEditor.Item)page.Tag;

                if (confirmOnClose)
                {
                    if (MessageBox.Show("You are about to close " +
                        this.TabPages[SelectedIndex].Text.TrimEnd() +
                        " tab. Are you sure you want to continue?", "Confirm close",
                        MessageBoxButtons.YesNo) == DialogResult.No)
                        return;
                }
                if(item.OnTabClosed())
                    TabPages.Remove(page);                
            }
            base.OnMouseDown(e);
        }
        /*
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            Point pt = new Point(e.X, e.Y);
            hoverIndex = -1;
            for (int i = 0; i < this.TabCount; i++)
            {
                Rectangle rect = GetTabRect(SelectedIndex);
                if (rect.Contains(pt))
                    hoverIndex = i;
            }
        }    */ 
        protected override void OnCreateControl()
        {
            base.OnCreateControl();     
        }

    }
}
