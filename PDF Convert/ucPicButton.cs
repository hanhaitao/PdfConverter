using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace PDF_Convert
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

        public bool ShowHot
        {
            get { return hotPB.Visible; }
            set { hotPB.Visible = value; }
        }

        private Image btnSelected;

        public Image BtnSelected
        {
            get { return btnSelected; }
            set { btnSelected = value; }
        }

        private Image btnNoSelected;

        public Image BtnNoSelected
        {
            get { return btnNoSelected; }
            set { btnNoSelected = value; }
        }

        Pen bottomPen = new Pen(Color.FromArgb(0xe0, 0xe0, 0xe0));

        public bool Selected
        {
            get { return m_Selected; }
            set 
            { 
                m_Selected = value; 
                //BackColor = m_Selected ? Color.FromArgb(0xff, 0xff, 0xff) : Color.FromArgb(0xf0, 0xf0, 0xf0);
                picImg.Image = m_Selected ? BtnSelected : BtnNoSelected;
                this.lblText.ForeColor = m_Selected ? Color.FromArgb(0x00, 0x7b, 0xf2) : Color.FromArgb(0x33, 0x33, 0x33);
                this.lblText.Font = m_Selected ? new Font("微软雅黑", 12) : new Font("微软雅黑", 10);

                //this.BackColor = m_Selected ? Color.FromArgb(0xe0, 0xf1, 0xfe) : Color.FromArgb(0xff, 0xff, 0xff);
                this.BackColor = m_Selected ? Color.FromArgb(0xe0, 0xf1, 0xfe) : Color.Transparent;
            }
            //this.BackgroundImage = m_Selected ? Properties.Resources.BtnHover : Properties.Resources.BtnNormal; }
            //set { m_Selected = value; this.BackColor = m_Selected ? Color.FromArgb(0xe0, 0xe0, 0x00) : Color.FromArgb(0xe0, 0xe0, 0xe0); }
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
            this.BackgroundImage = null;
            //this.BackgroundImage = Properties.Resources.BtnHover;
            if (!Selected)
            {
                this.BackgroundImage = null;
                BackColor = Color.FromArgb(0xe0, 0xf1, 0xfe);//(0xf9, 0xf9, 0xf9);
            }
        }

        private void ControlMouseLeave(object sender, EventArgs e)
        {
            BottomLine = true;
            if (!Selected)
            {
                this.BackgroundImage = null;
                //BackColor = Color.FromArgb(0xec, 0xeb, 0xf1);
                BackColor = Color.Transparent; //KongMengyuan修改,2015-12-07
            }
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
                e.Graphics.DrawLine(bottomPen, 0, this.Height - 1, this.Width, this.Height - 1);
        }

        private void ucPicButton_Load(object sender, EventArgs e)
        {
            this.BackgroundImage = null;
            //BackColor = Color.FromArgb(0xf0, 0xf0, 0xf0);
            BackColor = Color.Transparent;
        }
    }
}
