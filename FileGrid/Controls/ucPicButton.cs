using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Controls
{
    [DefaultEvent("Click")]
    public partial class ucPicButton : UserControl
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

        public bool BottomLine
        {
            get;
            set;
        }

        Pen bottomPen = new Pen(Color.FromArgb(0xe0, 0xe0, 0xe0));

        public bool Selected
        {
            get { return m_Selected; }
            //set { m_Selected = value; this.BackgroundImage = m_Selected ? Properties.Resources.BtnHover : Properties.Resources.BtnNormal; }
            set { m_Selected = value; this.BackColor = m_Selected ? Color.FromArgb(0xe0, 0xe0, 0x00) : Color.FromArgb(0xe0, 0xe0, 0xe0); }
        }private bool m_Selected;

        public ucPicButton()
        {
            InitializeComponent();
            Selected = false;
            BottomLine = true;
        }

        private void ControlMouseEnter(object sender, EventArgs e)
        {
            BottomLine = false;
            //this.BackgroundImage = Properties.Resources.BtnHover;
            this.BackColor = Color.FromArgb(0xe0, 0xe0, 0xe0);
        }

        private void ControlMouseLeave(object sender, EventArgs e)
        {
            BottomLine = true;
            if (!Selected)
                //this.BackgroundImage = Properties.Resources.BtnNormal;
                this.BackColor = Color.FromArgb(0xe0, 0xe0, 0xe0);
        }

        private void ControlMouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                Selected = true;
                this.OnClick(new EventArgs());
            }
        }

        private void ucPicButton_Paint(object sender, PaintEventArgs e)
        {
            if (BottomLine && !Selected)
                e.Graphics.DrawLine(bottomPen, 35, this.Height - 1, this.Width - 35, this.Height - 1);
        }
    }
}
