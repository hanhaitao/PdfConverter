using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace PDF_Convert
{
    public partial class ucTextBoxListView : System.Windows.Forms.TextBox
    {
        public string BackGroundText 
        {
            get { return m_BackGroundText; }
            set { m_BackGroundText = value; this.Invalidate(); DrawString(); }
        }private string m_BackGroundText;
        public Image Img { get; set; }

        private void DrawString()
        {
            if (!this.Focused && string.IsNullOrEmpty(this.Text))
            {
                Graphics g = this.CreateGraphics();
                TextRenderer.DrawText(g,
                                      BackGroundText,
                                      this.Font,
                                      this.ClientRectangle,
                                      Color.Gray,
                                      TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            }
        }

        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == 0x0F)
                DrawString();
        }

    }
}
