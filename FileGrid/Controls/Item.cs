using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Controls.Defines;

namespace Controls
{
    [Serializable]
    public partial class Item : UserControl
    {
        Pen m_BroderPen = new Pen(Color.FromArgb(240, 240, 240));
        Pen m_StatusPen = new Pen(Color.FromArgb(204, 204, 204));
        Rectangle m_StatusRect = new Rectangle(5, 15, 125, 15);
        Brush m_StatusBrush = new SolidBrush(Color.FromArgb(204, 204, 204));

        public delegate void OnStatusChangeDelegate(StatusType status);
        public event OnStatusChangeDelegate OnStatusChange;

        public int StatusPV
        {
            get { return m_StatusPV; }
            set
            {
                m_StatusPV = value;
                if (m_StatusPV >= 100)
                {
                    m_StatusPV = 100;
                    Finished = true;
                    picStatus.Image = Properties.Resources.satdone;
                    Status = StatusType.Done;
                    lblFileName.Text = Path.GetFileName(ConvertPath);
                    FileType = ConvertType;
                    if (OnStatusChange != null)
                        OnStatusChange(Status);
                }
            }
        }private int m_StatusPV;

        public StatusType Status
        {
            get;
            private set;
        }

        public bool Finished
        {
            get;
            private set;
        }

        /// <summary>
        /// 文件路径
        /// </summary>
        public string FilePath
        {
            get;
            set;
        }

        /// <summary>
        /// 转换路径
        /// </summary>
        public string ConvertPath
        {
            get;
            set;
        }

        public FileTypeDefine FileType
        {
            get { return m_FileType; }
            set
            {
                switch (value)
                {
                    case FileTypeDefine.Excel:
                        picType.Image = Properties.Resources.picexcel;
                        break;
                    case FileTypeDefine.Image:
                        picType.Image = Properties.Resources.picimage;
                        break;
                    case FileTypeDefine.PDF:
                        picType.Image = Properties.Resources.picpdf;
                        break;
                    case FileTypeDefine.PPT:
                        picType.Image = Properties.Resources.picppt;
                        break;
                    case FileTypeDefine.TXT:
                        picType.Image = Properties.Resources.pictxt;
                        break;
                    case FileTypeDefine.WORD:
                        picType.Image = Properties.Resources.picword;
                        break;
                    case FileTypeDefine.UnKnow:
                        picType.Image = Properties.Resources.stop;
                        break;
                    default:
                        picType.Image = Properties.Resources.stop;
                        break;

                }
                m_FileType = value;
            }
        }private FileTypeDefine m_FileType;

        public FileTypeDefine ConvertType
        {
            get;
            set;
        }

        public Item()
        {
            InitializeComponent();
            StatusPV = 0;
            Finished = false;
            picType.Image = null;
            lblFileName.Text = string.Empty;
            comboBox1.Visible = false;
            picStatus.Visible = false;
            lblOpen.Text = string.Empty;
            picOpenPath.Visible = false;
            picDelete.Visible = false;
            this.Dock = DockStyle.Top;
            picType.Visible = false;
        }

        public Item(string filePath, string convertPath)
        {
            InitializeComponent();
            FilePath = filePath;
            ConvertPath = convertPath;
            lblFileName.Text = Path.GetFileName(FilePath);
            string ext = Path.GetExtension(FilePath);
            switch (ext.ToLower())
            {
                case "xls":
                case "xlsx":
                    FileType = FileTypeDefine.Excel;
                    break;
                case "bmp":
                case "jpg":
                case "png":
                    FileType = FileTypeDefine.Image;
                    break;
                case "pdf":
                    FileType = FileTypeDefine.PDF;
                    break;
                case "ppt":
                case "pptx":
                    FileType = FileTypeDefine.PPT;
                    break;
                case "txt":
                    FileType = FileTypeDefine.TXT;
                    break;
                case "doc":
                case "docx":
                    FileType = FileTypeDefine.WORD;
                    break;
                default:
                    FileType = FileTypeDefine.UnKnow;
                    break;
            }
        }

        private void lblNo_Paint(object sender, PaintEventArgs e)
        {
            Control c = (Control)sender;
            e.Graphics.DrawLine(m_BroderPen, c.Width - 1, 0, c.Width - 1, this.Height);
            e.Graphics.DrawLine(m_BroderPen, 0, c.Height - 1, c.Width - 1, c.Height - 1);

            if (c.Name == "panelStatus" && picStatus.Visible)
            {
                e.Graphics.DrawRectangle(m_StatusPen, m_StatusRect);
                e.Graphics.FillRectangle(m_StatusBrush, new Rectangle(5, 15, (int)(m_StatusRect.Width * (StatusPV / 100f)), 15));
            }
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left && Status != StatusType.Done)
            {
                Status = Status == StatusType.Start ? StatusType.Pause : StatusType.Start;
                if (OnStatusChange != null)
                    OnStatusChange(Status);
            }
        }

        private void lblOpen_MouseClick(object sender, MouseEventArgs e)
        {
            if (string.IsNullOrEmpty(FilePath) || string.IsNullOrEmpty(ConvertPath)) return;
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                System.Diagnostics.Process.Start(Finished ? ConvertPath : FilePath);
            }
        }

        private void picOpenPath_MouseClick(object sender, MouseEventArgs e)
        {
            if (string.IsNullOrEmpty(FilePath) || string.IsNullOrEmpty(ConvertPath)) return;
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                System.Diagnostics.Process.Start("explorer.exe", Finished ? Path.GetDirectoryName(ConvertPath) : Path.GetDirectoryName(FilePath));
            }
        }

        private void picDelete_MouseClick(object sender, MouseEventArgs e)
        {
            if (string.IsNullOrEmpty(FilePath) || string.IsNullOrEmpty(ConvertPath)) return;
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {

            }
        }
    }

    namespace Defines
    {
        public enum StatusType
        {
            Start,
            Pause,
            Done,
        }

        public enum FileTypeDefine
        {
            UnKnow,
            Excel,
            Image,
            PDF,
            PPT,
            TXT,
            WORD,
        }
    }
}
