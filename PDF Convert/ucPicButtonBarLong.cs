using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace PDF_Convert
{
    [DefaultEvent("MouseClick")]
    public partial class ucPicButtonBarLong : UserControl
    {
        public string ButtonText
        {
            get { return lblText.Text; }
            set { lblText.Text = value; }
        }

        public Image ButtonBackIMG
        {
            get { return this.BackgroundImage; }
            set { this.BackgroundImage = value; }
        }


        public ucPicButtonBarLong()
        {
            InitializeComponent();
        }

        public Image PicImg
        {
            get { return picBtn.Image; }
            set { picBtn.Image = value; }
        }


        private void lblText_Click(object sender, EventArgs e)
        {
            this.OnClick(e);
        }


        private void lblText_MouseEnter(object sender, EventArgs e)
        {
            this.OnMouseEnter(e);
        }

        private void lblText_MouseLeave(object sender, EventArgs e)
        {
            this.OnMouseLeave(e);
        }

        private void lblText_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                this.OnMouseClick(e);
            }
        }
    }
}
