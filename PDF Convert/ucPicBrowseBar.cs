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
    public partial class ucPicBrowseBar : UserControl
    {
        public bool IsEnable { get; set; }

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
        public Font ButtonTextFont
        {
            get { return lblText.Font; }
            set { lblText.Font = value; }
        }
        public Color ButtonForeColor
        {
            get { return lblText.ForeColor; }
            set { lblText.ForeColor = value; }
        }


        public ucPicBrowseBar()
        {

            InitializeComponent();
            IsEnable = true;

        }




        private void lblText_MouseEnter(object sender, EventArgs e)
        {
            if (IsEnable)
                this.OnMouseEnter(e);
        }

        private void lblText_MouseLeave(object sender, EventArgs e)
        {
            if (IsEnable)
                this.OnMouseLeave(e);
        }

        private void lblText_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left && IsEnable)
            {
                this.OnMouseClick(e);
            }
        }



    }
}
