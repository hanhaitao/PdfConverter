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
    public partial class ucPicClearBar : UserControl
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


        public ucPicClearBar()
        {

            InitializeComponent();

        }




        private void ucPicClearBar_MouseEnter(object sender, EventArgs e)
        {
            this.picImg.Image = Properties.Resources.qingkonghover;
        }

        private void ucPicClearBar_MouseLeave(object sender, EventArgs e)
        {
            this.picImg.Image = Properties.Resources.qingkong;
        }



        private void lblText_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                this.OnMouseClick(e);
            }
        }

        private void picImg_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                this.OnMouseClick(e);
            }
        }



    }
}
