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
    public partial class ucPicNavigationBar : UserControl
    {
        public Image ButtonImage
        {
            get { return picImg.Image; }
            set { picImg.Image = value; }
        }

        public string ButtonText
        {
            get { return lblText.Text; }
            set { lblText.Text = value; }
        }

        public int FromType
        {
            get;
            set;
        }

        public ucPicNavigationBar()
        {
            InitializeComponent();

        }
        private void ucPicNavigationBar_MouseEnter(object sender, EventArgs e)
        {

            this.picImg.Image = FromType == 1 ? Properties.Resources.registerhover : Properties.Resources.helphover;
        }

        private void ucPicNavigationBar_MouseLeave(object sender, EventArgs e)
        {
            this.picImg.Image = FromType == 1 ? Properties.Resources.register : Properties.Resources.help;
        }

        private void picImg_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                this.OnMouseClick(e);
            }
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
