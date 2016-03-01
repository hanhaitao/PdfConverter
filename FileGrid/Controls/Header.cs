using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Controls
{
    public partial class Header: UserControl
    {
        Pen m_BroderPen = new Pen(Color.FromArgb(227, 227, 227));
        public Header()
        {
            InitializeComponent();
            ReSizeLabels();
        }

        private void ReSizeLabels()
        {
            lblNo.Width = Configs.COL_NO_WIDTH;
            lblFileName.Width = Configs.COL_FILENAME_WIDTH;
            lblSelectPage.Width = Configs.COL_SELECTPAGE_WIDTH;
            lblStatus.Width = Configs.COL_STATUS_WIDTH;
            lblOperate.Width = Configs.COL_OPERATE_WIDTH;
        }

        private void lbl_Paint(object sender, PaintEventArgs e)
        {
            Control c = (Control)sender;
            e.Graphics.DrawLine(m_BroderPen, c.Width -1 , 0, c.Width -1 , this.Height);
            e.Graphics.DrawLine(m_BroderPen, 0, c.Height -1, c.Width -1, c.Height -1);
        }
    }
}
