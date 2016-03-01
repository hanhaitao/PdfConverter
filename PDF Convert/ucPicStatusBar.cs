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
    public partial class ucPicStatusBar : UserControl
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
        public Font ButtonTextFont
        {
            get { return lblText.Font; }
            set { lblText.Font = value; }
        }
        public int FromType
        {
            get;
            set;
        }

        public ucPicStatusBar()
        {
            InitializeComponent();

        }

        private void ucPicStatusBar_MouseEnter(object sender, EventArgs e)
        {
            switch (FromType)
            {
                case 0:
                    this.picImg.Image = Properties.Resources.onlinehover;
                    break;
                case 1:
                    this.picImg.Image = Properties.Resources.moneyhover;
                    break;
                case 2:
                    this.picImg.Image = Properties.Resources.qqhover;
                    break;
                case 3:
                    this.picImg.Image = Properties.Resources.phonehover;
                    break;
                case 4:
                    this.picImg.Image = Properties.Resources.qqcnhover;
                    break;
                case 5:
                    this.picImg.Image = Properties.Resources.phonecomhover;
                    break;
                case 6:
                    this.picImg.Image = Properties.Resources.emailenhover;
                    break;
                case 7:
                    this.picImg.Image = Properties.Resources.emailnew_hover;
                    break;
                default:
                    break;
            }
            this.lblText.ForeColor = Color.FromArgb(102, 102, 102);
        }

        private void ucPicStatusBar_MouseLeave(object sender, EventArgs e)
        {
            switch (FromType)
            {
                case 0:
                    this.picImg.Image = Properties.Resources.online;
                    break;
                case 1:
                    this.picImg.Image = Properties.Resources.money;
                    break;
                case 2:
                    this.picImg.Image = Properties.Resources.qq;
                    break;
                case 3:
                    this.picImg.Image = Properties.Resources.phone;
                    break;
                case 4:
                    this.picImg.Image = Properties.Resources.qqcn;
                    break;
                case 5:
                    this.picImg.Image = Properties.Resources.phonecom;
                    break;
                case 6:
                    this.picImg.Image = Properties.Resources.emailen;
                    break;
                case 7:
                    this.picImg.Image = Properties.Resources.emailnew;
                    break;
                default:
                    break;
            }
            this.lblText.ForeColor = Color.FromArgb(171, 171, 171);
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
