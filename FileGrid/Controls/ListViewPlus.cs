using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;

namespace Controls
{
    public partial class ListViewPlus : ListView
    {
        #region API Const

        private const int LVM_FIRST = 0x1000;
        private const int LVM_GETHEADER = (LVM_FIRST + 31);

        private const int HDI_ORDER = 0x0080;
        private const int HDI_WIDTH = 0x0001;
        private const int HDI_HEIGHT = HDI_WIDTH;

        private const int HDM_FIRST = 0x1200;
        private const int HDM_GETITEMCOUNT = (HDM_FIRST + 0);
        private const int HDM_GETITEMA = (HDM_FIRST + 3);
        private const int HDM_GETITEMRECT = (HDM_FIRST + 7);

        private const int WM_HSCROLL = 0x0114;    // WM_HSCROLL消息  
        private const int WM_VSCROLL = 0x0115;    // WM_VSCROLL消息 

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr SendMessage(IntPtr hwnd, Int32 wMsg, IntPtr wParam, IntPtr lParam);

        #endregion

        #region 长度常量定义

        private const int COL_INDEX_WIDTH = 55;
        private const int COL_FILENAME_WIDTH = 275;
        private const int COL_SELECTPAGE_WIDTH = 130;
        private const int COL_STATUS_WIDTH = 140;
        private const int COL_OPERATE_WIDTH = 130;
        private const int COL_OPEN_WIDTH = 45;
        private const int COL_FOLDER_WIDTH = 45;
        private const int COL_DEL_WIDTH = 45;
        private const byte STATUSBUTTONXPOS = 45;
        private const byte STATUSBUTTONYPOS = 13;
        private const byte COL_INDEX = 0;
        private const byte COL_FILENAME = 1;
        private const byte COL_SELECTPAGES = 2;
        private const byte COL_STATUS = 3;
        private const byte COL_OPERATE = 4;
        private const byte COL_OPEN = 5;
        private const byte COL_FLODER = 6;
        private const byte COL_DEL = 7;

        #endregion

        #region 颜色定义

        private Color m_BaseColor = Color.FromArgb(229, 229, 229);
        private Color m_BorderColor = Color.FromArgb(229, 229, 229);
        private Color m_InnerBorderColor = Color.FromArgb(150, 255, 255, 255);
        private Color m_RowSelectColor = Color.FromArgb(234, 249, 255);
        private Color m_ItemSelectColor = Color.FromArgb(234, 249, 255);
        private Pen m_BroderPen = new Pen(Color.FromArgb(240, 240, 240));
        private Pen m_StatusPen = new Pen(Color.FromArgb(204, 204, 204));
        private Brush m_OpenTextBrush = new SolidBrush(Color.FromArgb(52, 52, 52));
        private Brush m_StatusBrush = new SolidBrush(Color.FromArgb(204, 204, 204));
        private Brush m_StatusPersentValueBrush = new SolidBrush(Color.FromArgb(41, 173, 255));

        #endregion

        #region 事件定义

        private HeaderNativeWindow _headerNativeWindow;

        public delegate void StatusChangedDelegate(int index, StatusType status);
        /// <summary>
        /// 状态按钮点击事件
        /// </summary>
        public event StatusChangedDelegate OnStatusButtonClicked;

        public delegate void OpenFileDelegate(int index);
        /// <summary>
        /// 打开文件按钮点击事件
        /// </summary>
        public event OpenFileDelegate OnOpenFileButtonClicked;

        public delegate void OpenDirectoryDelegate(int index);
        /// <summary>
        /// 打开文件路径按钮点击事件
        /// </summary>
        public event OpenDirectoryDelegate OnOpenDirectoryButtonClicked;

        public delegate void DeleteButtonDelegate(int index);
        /// <summary>
        /// 删除按钮点击事件
        /// </summary>
        public event DeleteButtonDelegate OnDeleteButtonClicked;

        #endregion

        #region 属性

        [Category("ColumnWidth")]
        public int IndexWidth
        {
            get { return this.Columns[COL_INDEX].Width; }
            set { this.Columns[COL_INDEX].Width = value; }
        }

        [Category("ColumnWidth")]
        public int FileNameWidth
        {
            get { return this.Columns[COL_FILENAME].Width; }
            set { this.Columns[COL_FILENAME].Width = value; }
        }

        [Category("ColumnWidth")]
        public int SelectPageWidth
        {
            get { return this.Columns[COL_SELECTPAGES].Width; }
            set { this.Columns[COL_SELECTPAGES].Width = value; }
        }

        [Category("ColumnWidth")]
        public int StatusWidth
        {
            get { return this.Columns[COL_STATUS].Width; }
            set { this.Columns[COL_STATUS].Width = value; }
        }

        [Category("ColumnWidth")]
        public int OperateWidth
        {
            get { return this.Columns[COL_OPERATE].Width; }
            set { this.Columns[COL_OPERATE].Width = value; }
        }

        [Category("ColumnWidth")]
        public int OpenWidth
        {
            get { return this.Columns[COL_OPEN].Width; }
            set { this.Columns[COL_OPEN].Width = value; }
        }

        [Category("ColumnWidth")]
        public int FloderWidth
        {
            get { return this.Columns[COL_FLODER].Width; }
            set { this.Columns[COL_FLODER].Width = value; }
        }

        [Category("ColumnWidth")]
        public int DelWidth
        {
            get { return this.Columns[COL_DEL].Width; }
            set { this.Columns[COL_DEL].Width = value; }
        }

        [Category("ColumnName")]
        public string IndexText
        {
            get { return this.Columns[COL_INDEX].Text; }
            set { this.Columns[COL_INDEX].Text = value; }
        }

        [Category("ColumnName")]
        public string FileNameText
        {
            get { return this.Columns[COL_FILENAME].Text; }
            set { this.Columns[COL_FILENAME].Text = value; }
        }

        [Category("ColumnName")]
        public string SelectPageText
        {
            get { return this.Columns[COL_SELECTPAGES].Text; }
            set { this.Columns[COL_SELECTPAGES].Text = value; }
        }

        [Category("ColumnName")]
        public string StatusText
        {
            get { return this.Columns[COL_STATUS].Text; }
            set { this.Columns[COL_STATUS].Text = value; }
        }


        [Category("ColumnName")]
        public string OperateText
        {
            get { return this.Columns[COL_OPERATE].Text; }
            set { this.Columns[COL_OPERATE].Text = value; }
        }

        [Category("ColumnName")]
        public string OpenText
        {
            get { return this.Columns[COL_OPEN].Text; }
            set { this.Columns[COL_OPEN].Text = value; }
        }

        [Category("ColumnName")]
        public string FolderText
        {
            get { return this.Columns[COL_FLODER].Text; }
            set { this.Columns[COL_FLODER].Text = value; }
        }

        [Category("ColumnName")]
        public string DelText
        {
            get { return this.Columns[COL_DEL].Text; }
            set { this.Columns[COL_DEL].Text = value; }
        }

        public Font ItemFont { get; set; }

        public string OpenButtonText
        {
            get { return m_OpenButtonText; }
            set
            {
                m_OpenButtonText = value;
                for (int i = 0; i < Items.Count; i++)
                {
                    this.Invalidate(Items[i].SubItems[COL_OPERATE].Bounds);
                }
            }
        }private string m_OpenButtonText;


        public string ConversionPageDefaultText
        {
            get { return m_ConversionPageDefaultText; }
            set
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    if (Items[i].SubItems[COL_SELECTPAGES].Text == ConversionPageDefaultText)
                        Items[i].SubItems[COL_SELECTPAGES].Text = value;
                }
                m_ConversionPageDefaultText = value;
            }
        }private string m_ConversionPageDefaultText;

        public bool IsAllFinished
        {
            get
            {
                for (int i = 0; i < this.Items.Count; i++)
                {
                    ItemInfomation info = (ItemInfomation)this.Items[i].Tag;
                    if (info.Status != StatusType.Done && info.Status != StatusType.Ready)
                        return false;
                }
                return true ;
            }
        }

        private IntPtr HeaderWnd
        {
            get
            {
                return new IntPtr(NativeMethods.SendMessage(
                base.Handle, LVM_GETHEADER, 0, 0));
            }
        }

        [DefaultValue(typeof(Color), "75, 188, 254")]
        private Color HeadColor
        {
            get { return m_BorderColor; }
            set
            {
                m_BorderColor = value;
                base.Invalidate(true);
            }
        }

        private int ColumnCount
        {
            get
            {
                return NativeMethods.SendMessage(
                    HeaderWnd, HDM_GETITEMCOUNT, 0, 0);
            }
        }

        private Rectangle AbsoluteClientRectangle
        {
            get
            {
                NativeMethods.RECT absoluteClientRECT = AbsoluteClientRECT;

                Rectangle rect = Rectangle.FromLTRB(
                    absoluteClientRECT.Left,
                    absoluteClientRECT.Top,
                    absoluteClientRECT.Right,
                    absoluteClientRECT.Bottom);
                CreateParams cp = base.CreateParams;
                bool bHscroll = (cp.Style &
                    (int)NativeMethods.WindowStyle.WS_HSCROLL) != 0;
                bool bVscroll = (cp.Style &
                    (int)NativeMethods.WindowStyle.WS_VSCROLL) != 0;

                if (bHscroll)
                {
                    rect.Height += SystemInformation.HorizontalScrollBarHeight;
                }

                if (bVscroll)
                {
                    rect.Width += SystemInformation.VerticalScrollBarWidth;
                }

                return rect;
            }
        }

        private NativeMethods.RECT AbsoluteClientRECT
        {
            get
            {
                NativeMethods.RECT lpRect = new NativeMethods.RECT();
                CreateParams createParams = CreateParams;
                NativeMethods.AdjustWindowRectEx(
                    ref lpRect,
                    createParams.Style,
                    false,
                    createParams.ExStyle);
                int left = -lpRect.Left;
                int right = -lpRect.Top;
                NativeMethods.GetClientRect(
                    base.Handle,
                    ref lpRect);

                lpRect.Left += left;
                lpRect.Right += left;
                lpRect.Top += right;
                lpRect.Bottom += right;
                return lpRect;
            }
        }

        private bool m_OpenPathButtonHover;
        private bool m_DeleteButtonHover;

        #endregion

        #region 初始化
        /// <summary>
        /// 资源图片
        /// </summary>
        ImageList imgs;
        public ListViewPlus()
        {
            ItemFont = this.Font;
            OpenButtonText = "打开";
            //设置行宽使用的ImageList
            ImageList l = new ImageList();
            l.ImageSize = new Size(1, 45);
            this.Font = new Font("宋体", 14);
            this.View = System.Windows.Forms.View.Details;
            this.SmallImageList = l;
            this.GridLines = true;
            this.FullRowSelect = true;
            this.DoubleBuffered = true;
            this.Scrollable = true;

            this.ColumnWidthChanging += ListViewPlus_ColumnWidthChanging;
            this.ColumnWidthChanged += ListViewPlus_ColumnWidthChanged;
            this.MouseClick += ListViewPlus_MouseClick;
            this.MouseMove += ListViewPlus_MouseMove;

            InitColumns();
            InitImages();

            base.OwnerDraw = true;
        }

        private void InitImages()
        {
            imgs = new ImageList();
            imgs.ImageSize = new System.Drawing.Size(20, 20);
            imgs.ColorDepth = ColorDepth.Depth24Bit;
            imgs.Images.Add("word", Properties.Resources.picword);
            imgs.Images.Add("txt", Properties.Resources.pictxt);
            imgs.Images.Add("ppt", Properties.Resources.picppt);
            imgs.Images.Add("pdf", Properties.Resources.picpdf);
            imgs.Images.Add("html", Properties.Resources.pichtml);
            imgs.Images.Add("image", Properties.Resources.picimage);
            imgs.Images.Add("excel", Properties.Resources.picexcel);
            imgs.Images.Add("start", Properties.Resources.satstart);
            imgs.Images.Add("pause", Properties.Resources.satpause);
            imgs.Images.Add("done", Properties.Resources.satdone);
            imgs.Images.Add("dir", Properties.Resources.file);
            imgs.Images.Add("del", Properties.Resources.delete);
            imgs.Images.Add("dirhover", Properties.Resources.path);
            imgs.Images.Add("delhover", Properties.Resources.deletehover);
        }

        private void InitColumns()
        {
            ColumnHeader recNo = new ColumnHeader();
            recNo.Name = "RecNo";
            recNo.Text = "编号";
            recNo.TextAlign = HorizontalAlignment.Left;
            recNo.Width = COL_INDEX_WIDTH;

            ColumnHeader fileName = new ColumnHeader();
            fileName.Name = "FileName";
            fileName.Text = "文件名称";
            fileName.TextAlign = HorizontalAlignment.Left;
            fileName.Width = COL_FILENAME_WIDTH;

            ColumnHeader select = new ColumnHeader();
            select.Name = "SelectPages";
            select.Text = "选择页码";
            select.TextAlign = HorizontalAlignment.Left;
            select.Width = COL_SELECTPAGE_WIDTH;

            ColumnHeader status = new ColumnHeader();
            status.Name = "Status";
            status.Text = "状态";
            status.TextAlign = HorizontalAlignment.Left;
            status.Width = COL_STATUS_WIDTH;

            ColumnHeader operate = new ColumnHeader();
            operate.Name = "Operate";
            operate.Text = "操作";
            operate.TextAlign = HorizontalAlignment.Left;
            operate.Width = COL_OPERATE_WIDTH;

            ColumnHeader open = new ColumnHeader();
            open.Name = "Open";
            open.Text = "打开";
            open.TextAlign = HorizontalAlignment.Left;
            open.Width = COL_OPEN_WIDTH;

            ColumnHeader folder = new ColumnHeader();
            folder.Name = "folder";
            folder.Text = "输出";
            folder.TextAlign = HorizontalAlignment.Left;
            folder.Width = COL_FOLDER_WIDTH;

            ColumnHeader del = new ColumnHeader();
            del.Name = "del";
            del.Text = "删除";
            del.TextAlign = HorizontalAlignment.Left;
            del.Width = COL_DEL_WIDTH;


            this.Columns.AddRange(new ColumnHeader[] { recNo, fileName, select, status, operate, open, folder, del});
        }

        #endregion

        #region 填充空白列颜色

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            if (_headerNativeWindow == null)
            {
                if (HeaderWnd != IntPtr.Zero)
                {
                    _headerNativeWindow = new HeaderNativeWindow(this);
                }
            }
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            base.OnHandleDestroyed(e);
            if (_headerNativeWindow != null)
            {
                _headerNativeWindow.Dispose();
                _headerNativeWindow = null;
            }
        }

        #region HeaderNativeWindow Class

        private class HeaderNativeWindow
            : NativeWindow, IDisposable
        {
            private ListViewPlus _owner;

            public HeaderNativeWindow(ListViewPlus owner)
                : base()
            {
                _owner = owner;
                base.AssignHandle(owner.HeaderWnd);
            }

            protected override void WndProc(ref Message m)
            {
                base.WndProc(ref m);
                if (m.Msg == 0xF || m.Msg == 0x47)
                {
                    IntPtr hdc = NativeMethods.GetDC(m.HWnd);
                    try
                    {
                        using (Graphics g = Graphics.FromHdc(hdc))
                        {
                            Rectangle bounds = _owner.HeaderEndRect();
                            Color baseColor = _owner.HeadColor;
                            Color borderColor = _owner.HeadColor;
                            Color innerBorderColor = _owner.m_InnerBorderColor;
                            if (_owner.ColumnCount > 0)
                            {
                                bounds.X--;
                                bounds.Width++;
                            }
                            _owner.RenderBackgroundInternal(
                                g,
                                bounds,
                                baseColor,
                                borderColor,
                                innerBorderColor,
                                true);
                        }
                    }
                    finally
                    {
                        NativeMethods.ReleaseDC(m.HWnd, hdc);
                    }
                }
            }

            #region IDisposable 成员

            public void Dispose()
            {
                ReleaseHandle();
                _owner = null;
            }

            #endregion
        }

        #endregion

        #endregion

        #region 重绘

        protected override void OnDrawColumnHeader(DrawListViewColumnHeaderEventArgs e)
        {
            base.OnDrawColumnHeader(e);
            Graphics g = e.Graphics;
            Rectangle r = e.Bounds;
            if (e.ColumnIndex != 0)
            {
                r.X--;
                r.Width++;
            }
            RenderBackgroundInternal(g,
                                     e.Bounds,
                                     m_BaseColor,
                                     m_BorderColor,
                                     m_InnerBorderColor,
                                     true);
            //绘画文字
            TextRenderer.DrawText(g,
                                  e.Header.Text,
                                  e.Font,
                                  e.Bounds,
                                  e.ForeColor,
                                  TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }

        protected override void OnDrawItem(DrawListViewItemEventArgs e)
        {
            if (e.ItemIndex < 0) return;
            base.OnDrawItem(e);
            try
            {
                if ((e.State & ListViewItemStates.Selected) != 0)
                {
                    RenderBackgroundInternal(
                        e.Graphics,
                        e.Bounds,
                        m_ItemSelectColor,
                        m_BorderColor,
                        m_InnerBorderColor,
                        true);
                }
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.ToString());
            }
        }

        bool m_refresh;
        protected override void OnDrawSubItem(DrawListViewSubItemEventArgs e)
        {
            if (e.ItemIndex < 0) return;
            base.OnDrawSubItem(e);
            try
            {
                ItemInfomation info = (ItemInfomation)e.Item.Tag;
                switch (e.ColumnIndex)
                {
                    //序号列
                    case COL_INDEX:
                        TextRenderer.DrawText(e.Graphics,
                                              (e.Item.Index + 1).ToString(),
                                              ItemFont,
                                              e.Bounds,
                                              this.ForeColor,
                                              TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                        break;
                    //文件名
                    case COL_FILENAME:
                        Image img = GetTypeImage(info.PersentValue >= 100 ? info.ConvertType : info.FileType);
                        if (img == null) return;
                        e.Graphics.DrawImage(img, e.Bounds.Left + 5, e.Bounds.Top + 8, 25, 25);
                        Rectangle r = e.Bounds;
                        r.X += 40;
                        r.Width -= 40;
                        TextRenderer.DrawText(e.Graphics,
                                              CheckDispalyFileName(info.DisplayFileName),
                                              ItemFont,
                                              r,
                                              this.ForeColor,
                                              TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
                        break;
                    //选择页面
                    case COL_SELECTPAGES:
                        TextRenderer.DrawText(e.Graphics,
                                              this.Items[e.ItemIndex].SubItems[e.ColumnIndex].Text,
                                              ItemFont,
                                              e.Bounds,
                                              this.ForeColor,
                                              TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                        break;
                    //状态
                    case COL_STATUS:
                        Rectangle rTxt = e.Bounds;
                        rTxt.X += 10;
                        rTxt.Y -= 6;
                        TextRenderer.DrawText(e.Graphics,
                                              info.PersentValue + " %",
                                              ItemFont,
                                              rTxt,
                                              this.ForeColor,
                                              TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
                        //进度条
                        e.Graphics.FillRectangle(m_StatusBrush, e.Bounds.X + 10, e.Bounds.Y + 20 + 3 , e.Bounds.Width - 55, 8);
                        e.Graphics.FillRectangle(m_StatusPersentValueBrush, e.Bounds.X + 10, e.Bounds.Y + 20 + 3, (e.Bounds.Width - 55) * (info.PersentValue / 100f), 8);
                        //按钮
                        if (e.ItemState == ListViewItemStates.Grayed) return;
                        Image imgstatus = GetStatusImage(info.Status);
                        if (imgstatus == null) return;
                        e.Graphics.DrawImage(imgstatus, e.Bounds.X + e.Bounds.Width - STATUSBUTTONXPOS, e.Bounds.Top + STATUSBUTTONYPOS - 4, 20, 20);
                        if (info.StatusChanged)
                        {
                            info.StatusChanged = false;
                            this.Invalidate(new Rectangle(e.Bounds.X + e.Bounds.Width - STATUSBUTTONXPOS, e.Bounds.Top + STATUSBUTTONYPOS - 4, 20, 20));
                        }
                        break;
                    //操作按钮
                    case COL_OPERATE:
                        e.Graphics.DrawString(OpenButtonText, ItemFont, m_OpenTextBrush, new Rectangle(e.Bounds.X + e.Bounds.Width / 2 - 50, e.Bounds.Top + 15 - 4, 40, 18));

                        Image imgopen = info.OpenPathButtonHover && e.Item.Bounds.Contains(this.PointToClient(Cursor.Position)) ? Properties.Resources.path : Properties.Resources.file;
                        e.Graphics.DrawImage(imgopen, e.Bounds.X + e.Bounds.Width / 2 - 5 - 4, e.Bounds.Top + 12);

                        Image imgdelete = info.DeleteButtonHover && e.Item.Bounds.Contains(this.PointToClient(Cursor.Position)) ? Properties.Resources.deletehover : Properties.Resources.delete;
                        e.Graphics.DrawImage(imgdelete, e.Bounds.X + e.Bounds.Width / 2 + 35, e.Bounds.Top + 12 - 4, imgdelete.Width, imgdelete.Height);

                        if (e.ItemState == ListViewItemStates.Grayed)
                            this.Invalidate(e.Item.SubItems[COL_OPERATE].Bounds);
                        break;
                    case COL_OPEN:
                        Image imgopenFille =  Properties.Resources.path;
                        e.Graphics.DrawImage(imgopenFille, e.Bounds.X + e.Bounds.Width / 2 - 50, e.Bounds.Top + 15 - 4);
                        break;
                    case COL_FLODER:
                        Image imgopenFolder = info.OpenPathButtonHover && e.Item.Bounds.Contains(this.PointToClient(Cursor.Position)) ? Properties.Resources.path : Properties.Resources.file;
                        e.Graphics.DrawImage(imgopenFolder, e.Bounds.X + e.Bounds.Width / 2 - 5 - 4, e.Bounds.Top + 12);
                        break;
                    case COL_DEL:
                        Image imgdel = info.DeleteButtonHover && e.Item.Bounds.Contains(this.PointToClient(Cursor.Position)) ? Properties.Resources.deletehover : Properties.Resources.delete;
                        e.Graphics.DrawImage(imgdel, e.Bounds.X + e.Bounds.Width / 2 + 35, e.Bounds.Top + 12 - 4, imgdel.Width, imgdel.Height);
                        break;
                }
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.ToString());
            }
        }

        private string CheckDispalyFileName(string strName)
        {
            SizeF sizeF = this.CreateGraphics().MeasureString(strName, ItemFont);
            if (sizeF.Width > COL_FILENAME_WIDTH - 40)
            {
                strName = strName.Substring(0, 15) + "..." + Path.GetExtension(strName);
            }
            return strName;
        }

        internal void RenderBackgroundInternal(
            Graphics g,
            Rectangle rect,
            Color baseColor,
            Color borderColor,
            Color innerBorderColor,
            float basePosition,
            bool drawBorder,
            LinearGradientMode mode)
        {
            if (drawBorder)
            {
                rect.Width--;
                rect.Height--;
            }

            if (rect.Width <= 0 || rect.Height <= 0)
            {
                return;
            }

            using (LinearGradientBrush brush = new LinearGradientBrush(
               rect, Color.Transparent, Color.Transparent, mode))
            {
                Color[] colors = new Color[4];
                colors[0] = GetColor(baseColor, 0, 35, 24, 9);
                colors[1] = GetColor(baseColor, 0, 13, 8, 3);
                colors[2] = baseColor;
                colors[3] = GetColor(baseColor, 0, 68, 69, 54);

                ColorBlend blend = new ColorBlend();
                blend.Positions = new float[] {
                    0.0f, basePosition, basePosition + 0.05f, 1.0f };
                blend.Colors = colors;
                brush.InterpolationColors = blend;
                g.FillRectangle(brush, rect);
            }
            if (baseColor.A > 80)
            {
                Rectangle rectTop = rect;
                if (mode == LinearGradientMode.Vertical)
                {
                    rectTop.Height = (int)(rectTop.Height * basePosition);
                }
                else
                {
                    rectTop.Width = (int)(rect.Width * basePosition);
                }
                using (SolidBrush brushAlpha =
                    new SolidBrush(Color.FromArgb(80, 255, 255, 255)))
                {
                    g.FillRectangle(brushAlpha, rectTop);
                }
            }

            if (drawBorder)
            {
                using (Pen pen = new Pen(borderColor))
                {
                    g.DrawRectangle(pen, rect);
                }

                rect.Inflate(-1, -1);
                using (Pen pen = new Pen(innerBorderColor))
                {
                    g.DrawRectangle(pen, rect);
                }
            }
        }

        internal void RenderBackgroundInternal(
            Graphics g,
            Rectangle rect,
            Color baseColor,
            Color borderColor,
            Color innerBorderColor,
            bool drawBorder)
        {
            if (drawBorder)
            {
                rect.Width--;
                rect.Height--;
            }

            if (rect.Width <= 0 || rect.Height <= 0)
            {
                return;
            }

            using (SolidBrush brush = new SolidBrush(baseColor))
            {
                g.FillRectangle(brush, rect);
            }

            if (drawBorder)
            {
                using (Pen pen = new Pen(borderColor))
                {
                    g.DrawRectangle(pen, rect);
                }

                rect.Inflate(-1, -1);
                using (Pen pen = new Pen(innerBorderColor))
                {
                    g.DrawRectangle(pen, rect);
                }
            }
        }

        private Color GetColor(Color colorBase, int a, int r, int g, int b)
        {
            int a0 = colorBase.A;
            int r0 = colorBase.R;
            int g0 = colorBase.G;
            int b0 = colorBase.B;

            if (a + a0 > 255) { a = 255; } else { a = Math.Max(a + a0, 0); }
            if (r + r0 > 255) { r = 255; } else { r = Math.Max(r + r0, 0); }
            if (g + g0 > 255) { g = 255; } else { g = Math.Max(g + g0, 0); }
            if (b + b0 > 255) { b = 255; } else { b = Math.Max(b + b0, 0); }

            return Color.FromArgb(a, r, g, b);
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case (int)NativeMethods.WindowsMessgae.WM_NCPAINT:
                    WmNcPaint(ref m);
                    break;
                case (int)NativeMethods.WindowsMessgae.WM_WINDOWPOSCHANGED:
                    base.WndProc(ref m);
                    IntPtr result = m.Result;
                    WmNcPaint(ref m);
                    m.Result = result;
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        private void WmNcPaint(ref Message m)
        {
            base.WndProc(ref m);
            if (base.BorderStyle == BorderStyle.None)
            {
                return;
            }

            IntPtr hDC = NativeMethods.GetWindowDC(m.HWnd);
            if (hDC == IntPtr.Zero)
            {
                throw new Win32Exception();
            }
            try
            {
                Color backColor = BackColor;
                Color borderColor = m_BorderColor;

                Rectangle bounds = new Rectangle(0, 0, Width, Height);
                using (Graphics g = Graphics.FromHdc(hDC))
                {
                    using (Region region = new Region(bounds))
                    {
                        region.Exclude(AbsoluteClientRectangle);
                        using (Brush brush = new SolidBrush(backColor))
                        {
                            g.FillRegion(brush, region);
                        }
                    }

                    ControlPaint.DrawBorder(
                        g,
                        bounds,
                        borderColor,
                        ButtonBorderStyle.Solid);
                }
            }
            finally
            {
                NativeMethods.ReleaseDC(m.HWnd, hDC);
            }
            m.Result = IntPtr.Zero;
        }

        #endregion

        private void ListViewPlus_MouseClick(object sender, MouseEventArgs e)
        {

            if (e.Button != System.Windows.Forms.MouseButtons.Left) return;
            for (int i = 0; i < this.Items.Count; i++)
            {
                if (this.Items[i].Tag is ItemInfomation)
                {
                    //状态按钮
                    Rectangle r = new Rectangle(this.Items[i].SubItems[COL_STATUS].Bounds.X + this.Items[i].SubItems[COL_STATUS].Bounds.Width - STATUSBUTTONXPOS,
                                                this.Items[i].SubItems[COL_STATUS].Bounds.Top + STATUSBUTTONYPOS,
                                                20, 20);
                    if (r.Contains(e.X, e.Y))
                    {
                        BtnStatusChangeClicked(i);
                        return;
                    }
                    //打开文件按钮
                    SizeF sizeF = this.CreateGraphics().MeasureString(OpenButtonText, ItemFont);
                    r = new Rectangle(this.Items[i].SubItems[COL_OPERATE].Bounds.X + this.Items[i].SubItems[COL_OPERATE].Bounds.Width / 2 - 50,
                                      this.Items[i].SubItems[COL_OPERATE].Bounds.Top + 15,
                                      (int)sizeF.Width, (int)sizeF.Height);
                    if (r.Contains(e.X, e.Y))
                    {
                        if (OnOpenFileButtonClicked != null)
                            OnOpenFileButtonClicked(i);
                        return;
                    }
                    //打开目录按钮
                    r = new Rectangle(this.Items[i].SubItems[COL_OPERATE].Bounds.X + this.Items[i].SubItems[COL_OPERATE].Bounds.Width / 2 - 5,
                                      this.Items[i].SubItems[COL_OPERATE].Bounds.Top + 12,
                                      24, 24);
                    if (r.Contains(e.X, e.Y))
                    {
                        if (OnOpenDirectoryButtonClicked != null)
                            OnOpenDirectoryButtonClicked(i);
                        return;
                    }

                    //删除按钮
                    r = new Rectangle(this.Items[i].SubItems[COL_OPERATE].Bounds.X + this.Items[i].SubItems[COL_OPERATE].Bounds.Width / 2 + 35,
                                      this.Items[i].SubItems[COL_OPERATE].Bounds.Top + 12,
                                      24, 24);
                    if (r.Contains(e.X, e.Y))
                    {
                        if (OnDeleteButtonClicked != null)
                            OnDeleteButtonClicked(i);
                    }
                }
            }
        }

        private void BtnStatusChangeClicked(int index)
        {
            if (index < 0) return;
            if (this.Items[index].Tag is ItemInfomation)
            {
                ItemInfomation info = (ItemInfomation)this.Items[index].Tag;
                if (info.Status == StatusType.Done) return;
                if (OnStatusButtonClicked != null)
                    OnStatusButtonClicked(index, info.Status);
            }
        }

        private void ListViewPlus_MouseMove(object sender, MouseEventArgs e)
        {
            for (int i = 0; i < this.Items.Count; i++)
            {
                if (this.Items[i].Tag is ItemInfomation)
                {
                    ItemInfomation info = this.Items[i].Tag as ItemInfomation;
                    //状态按钮
                    Rectangle r = new Rectangle(this.Items[i].SubItems[COL_STATUS].Bounds.X + this.Items[i].SubItems[COL_STATUS].Bounds.Width - STATUSBUTTONXPOS,
                                                this.Items[i].SubItems[COL_STATUS].Bounds.Top + STATUSBUTTONYPOS,
                                                24, 24);
                    if (r.Contains(e.X, e.Y))
                    {
                        this.Cursor = Cursors.Hand;
                        return;
                    }
                    //打开文件按钮
                    SizeF sizeF = this.CreateGraphics().MeasureString(OpenButtonText, ItemFont);
                    r = new Rectangle(this.Items[i].SubItems[COL_OPERATE].Bounds.X + this.Items[i].SubItems[COL_OPERATE].Bounds.Width / 2 - 50,
                                      this.Items[i].SubItems[COL_OPERATE].Bounds.Top + 15,
                                      (int)sizeF.Width, (int)sizeF.Height);
                    if (r.Contains(e.X, e.Y))
                    {
                        this.Cursor = Cursors.Hand;
                        return;
                    }
                    //打开目录按钮
                    r = new Rectangle(this.Items[i].SubItems[COL_OPERATE].Bounds.X + this.Items[i].SubItems[COL_OPERATE].Bounds.Width / 2 - 5,
                                      this.Items[i].SubItems[COL_OPERATE].Bounds.Top + 12,
                                      24, 24);
                    if (r.Contains(e.X, e.Y))
                    {
                        this.Cursor = Cursors.Hand;
                        info.OpenPathButtonHover = true;
                        RefreshItems(i);
                        return;
                    }
                    //删除按钮
                    r = new Rectangle(this.Items[i].SubItems[COL_OPERATE].Bounds.X + this.Items[i].SubItems[COL_OPERATE].Bounds.Width / 2 + 35,
                                      this.Items[i].SubItems[COL_OPERATE].Bounds.Top + 12,
                                      24, 24);
                    if (r.Contains(e.X, e.Y))
                    {
                        this.Cursor = Cursors.Hand;
                        info.DeleteButtonHover = true;
                        RefreshItems(i);
                        return;
                    }
                    if (info.DeleteButtonHover || info.OpenPathButtonHover)
                    {
                        info.DeleteButtonHover = false;
                        info.OpenPathButtonHover = false;
                        RefreshItems(i);
                    }
                }
            }
            this.Cursor = Cursors.Default;
        }

        private void RefreshItems(int index)
        {
            if (index < 0) return;
            this.OnDrawSubItem(new DrawListViewSubItemEventArgs(this.CreateGraphics(),
                                                                this.Items[index].SubItems[COL_OPERATE].Bounds,
                                                                this.Items[index],
                                                                this.Items[index].SubItems[COL_OPERATE],
                                                                index,
                                                                COL_OPERATE,
                                                                this.Columns[COL_OPERATE],
                                                                ListViewItemStates.Grayed));
        }

        private void ListViewPlus_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
        {
            if (Columns[COL_INDEX].Width != COL_INDEX_WIDTH)
                this.Columns[COL_INDEX].Width = COL_INDEX_WIDTH;
            if (this.Columns[COL_FILENAME].Width != COL_FILENAME_WIDTH)
                this.Columns[COL_FILENAME].Width = COL_FILENAME_WIDTH;
            if (Columns[COL_SELECTPAGES].Width != COL_SELECTPAGE_WIDTH)
                this.Columns[COL_SELECTPAGES].Width = COL_SELECTPAGE_WIDTH;
            if (Columns[COL_STATUS].Width != COL_STATUS_WIDTH)
                this.Columns[COL_STATUS].Width = COL_STATUS_WIDTH;
            if (this.Columns[COL_OPERATE].Width != COL_OPERATE_WIDTH)
                this.Columns[COL_OPERATE].Width = COL_OPERATE_WIDTH;
        }

        private void ListViewPlus_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            e.Cancel = true;
        }

        public void AddFile(ItemInfomation info)
        {
            ListViewItem item = this.Items.Add((this.Items.Count + 1).ToString());
            item.SubItems.AddRange(new string[] { info.DisplayFileName, ConversionPageDefaultText, string.Empty, string.Empty });
            item.Tag = info;
        }

        public bool RemoveFile(int index)
        {
            if (index < 0) return false;
            this.Items.Remove(this.Items[index]);
            for (int i = 0; i < Items.Count; i++)
            {
                this.Invalidate(this.Items[i].SubItems[COL_INDEX].Bounds);
            }
            return true;
        }

        public bool SetStausPV(int index, int persentValue)
        {
            if (index < 0) return false;
            if (index > this.Items.Count - 1) return false;
            if (persentValue > 100) persentValue = 100;
            ItemInfomation info = (ItemInfomation)this.Items[index].Tag;
            info.PersentValue = persentValue;
            this.Items[index].Tag = info;
            this.OnDrawSubItem(new DrawListViewSubItemEventArgs(this.CreateGraphics(),
                                                                this.Items[index].SubItems[COL_FILENAME].Bounds,
                                                                this.Items[index],
                                                                this.Items[index].SubItems[COL_FILENAME],
                                                                index,
                                                                COL_FILENAME,
                                                                this.Columns[COL_FILENAME],
                                                                ListViewItemStates.Default));
            this.Invalidate(this.Items[index].SubItems[COL_FILENAME].Bounds);
            this.OnDrawSubItem(new DrawListViewSubItemEventArgs(this.CreateGraphics(),
                                                                this.Items[index].SubItems[COL_STATUS].Bounds,
                                                                this.Items[index],
                                                                this.Items[index].SubItems[COL_STATUS],
                                                                index,
                                                                COL_STATUS,
                                                                this.Columns[COL_STATUS],
                                                                ListViewItemStates.Grayed));
            return true;
        }

        private Image GetTypeImage(FileTypeDefine type)
        {
            switch (type)
            {
                case FileTypeDefine.Excel:
                    return Properties.Resources.picexcel;
                case FileTypeDefine.Image:
                    return Properties.Resources.picimage;
                case FileTypeDefine.PDF:
                    return Properties.Resources.picpdf;
                case FileTypeDefine.PPT:
                    return Properties.Resources.picppt;
                case FileTypeDefine.TXT:
                    return Properties.Resources.pictxt;
                case FileTypeDefine.WORD:
                    return Properties.Resources.picword;
                case FileTypeDefine.HTML:
                    return Properties.Resources.pichtml;
                default:
                    return Properties.Resources.picUnknow;
            }
        }

        private Image GetStatusImage(StatusType type)
        {
            switch (type)
            {
                case StatusType.Done:
                    return Properties.Resources.satdone;
                case StatusType.Pause:
                case StatusType.Ready:
                    return Properties.Resources.satstart;
                case StatusType.Start:
                    return Properties.Resources.satpause;
                default:
                    return null;
            }
        }

        private Rectangle HeaderEndRect()
        {
            NativeMethods.RECT rect = new NativeMethods.RECT();
            IntPtr headerWnd = HeaderWnd;
            NativeMethods.SendMessage(
                headerWnd, HDM_GETITEMRECT, ColumnAtIndex(ColumnCount - 1), ref rect);
            int left = rect.Right;
            NativeMethods.GetWindowRect(headerWnd, ref rect);
            NativeMethods.OffsetRect(ref rect, -rect.Left, -rect.Top);
            rect.Left = left;

            return Rectangle.FromLTRB(rect.Left, rect.Top, rect.Right, rect.Bottom);
        }

        private int ColumnAtIndex(int column)
        {
            NativeMethods.HDITEM hd = new NativeMethods.HDITEM();
            hd.mask = HDI_ORDER;
            for (int i = 0; i < ColumnCount; i++)
            {
                if (NativeMethods.SendMessage(
                    HeaderWnd, HDM_GETITEMA, column, ref hd) != IntPtr.Zero)
                {
                    return hd.iOrder;
                }
            }
            return 0;
        }
    }

    public class ItemInfomation
    {
        /// <summary>
        /// 文件原路径
        /// </summary>
        public string FileFullPath { get; set; }
        /// <summary>
        /// 转换生成文件全路径
        /// </summary>
        public string FileFullConvertPath { get; set; }
        /// <summary>
        /// 原文件名称
        /// </summary>
        public string FileName { get { return Path.GetFileName(FileFullPath); } }
        /// <summary>
        /// 转换生成文件名称
        /// </summary>
        public string ConvertFileName { get { return Path.GetFileName(FileFullConvertPath); } }
        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayFileName { get { return Status == StatusType.Done ? ConvertFileName : FileName; } }
        /// <summary>
        /// 转换进度 最小 0 最大 100
        /// </summary>
        public int PersentValue
        {
            get { return m_PersentValue; }
            set { m_PersentValue = value; if (m_PersentValue >= 100) { m_PersentValue = 100; Status = StatusType.Done; } if (m_PersentValue < 0) m_PersentValue = 0; }
        }private int m_PersentValue;

        public string PersentStr
        {
            get { return m_PresentStr; }
            set { m_PresentStr = value;}
        }

        private string m_PresentStr = "0%";

        /// <summary>
        /// 当前状态
        /// </summary>
        public StatusType Status
        {
            get { return m_Status; }
            set
            {
                if (Status != value)
                {
                    m_Status = value;
                    StatusChanged = true;
                }
                if (value == StatusType.Done) m_PersentValue = 100;
            }

        }private StatusType m_Status;
        /// <summary>
        /// 原文件类型
        /// </summary>
        public FileTypeDefine FileType { get { return GetFileExtension(FileFullPath); } }
        /// <summary>
        /// 转换文件类型
        /// </summary>
        public FileTypeDefine ConvertType { get { return GetFileExtension(FileFullConvertPath); } }
        /// <summary>
        /// 代开文件夹按钮焦点
        /// </summary>
        public bool OpenPathButtonHover { get; set; }
        /// <summary>
        /// 删除按钮焦点
        /// </summary>
        public bool DeleteButtonHover { get; set; }
        /// <summary>
        /// 单元格位置
        /// </summary>
        public bool StatusChanged { get; set; }
        public ItemInfomation(string fileFullPath)
        {
            this.FileFullPath = fileFullPath;
            PersentValue = 0;
            Status = StatusType.Ready;
            StatusChanged = true;
        }

        public FileTypeDefine GetFileExtension(string path)
        {
            if (string.IsNullOrEmpty(path)) return FileTypeDefine.UnKnow;
            string ext = Path.GetExtension(path);
            ext = ext.Replace(".", string.Empty);
            switch (ext.ToLower())
            {
                case "xls":
                case "xlsx":
                    return FileTypeDefine.Excel;
                case "bmp":
                case "jpg":
                case "jpeg":
                case "png":
                case "gif":
                case "tif":
                case "tiff":
                    return FileTypeDefine.Image;
                case "pdf":
                    return FileTypeDefine.PDF;
                case "ppt":
                case "pptx":
                    return FileTypeDefine.PPT;
                case "txt":
                    return FileTypeDefine.TXT;
                case "doc":
                case "docx":
                    return FileTypeDefine.WORD;
                case "html":
                    return FileTypeDefine.HTML;
                default:
                    return FileTypeDefine.UnKnow;
            }
        }
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
        HTML,
    }

    public enum StatusType
    {
        Start,
        Pause,
        Done,
        Ready,
    }
}
