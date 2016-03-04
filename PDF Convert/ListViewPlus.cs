using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices; //运行Windows的API,引用DLL专用
using System.Drawing.Drawing2D;
using System.Threading;

using Aspose.Slides;
//using Aspose.Words;

namespace PDF_Convert
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

        ////KongMengyuan增加,2015-11-19,得到文件的总页数
        //private Aspose.Pdf.Document pdf_doc;
        //private Aspose.Words.Document word_doc;
        //private Aspose.Cells.Workbook excel_doc;
        //private Aspose.Slides.Presentation ppt_doc;
        //private Aspose.Pdf.Facades.PdfExtractor txt_doc;
        //private FileStream fileStream;
        //private int pages;

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr SendMessage(IntPtr hwnd, Int32 wMsg, IntPtr wParam, IntPtr lParam);

        #endregion

        #region 长度常量定义
        private const int COL_INDEX_WIDTH = 55;
        private const int COL_FILENAME_WIDTH = 250; //215;
        private const int COL_PAGECOUNT_WIDTH = 60;
        private const int COL_SELECTPAGE_WIDTH = 130;
        private const int COL_STATUS_WIDTH = 140;
        private const int COL_OPERATE_WIDTH = 130;
        private const int COL_OPEN_WIDTH = 45;
        private const int COL_FOLDER_WIDTH = 45;
        private const int COL_DEL_WIDTH = 45;
        private const byte STATUSBUTTONXPOS = 34;
        private const byte STATUSBUTTONYPOS = 13;
        private const byte COL_INDEX = 0;
        private const byte COL_FILENAME = 1;
        private const byte COL_PAGECOUNT = 2;
        private const byte COL_SELECTPAGES = 3;
        private const byte COL_STATUS = 4;
        //private const byte COL_OPERATE = 4;
        private const byte COL_OPEN = 5;
        private const byte COL_FLODER = 6;
        private const byte COL_DEL = 7;
        #endregion

        #region 颜色定义
        private Color m_BaseColor = Color.FromArgb(229, 229, 229);
        private Color m_BorderColor = Color.FromArgb(229, 229, 229);
        private Color m_InnerBorderColor = Color.FromArgb(150, 255, 255, 255);
        private Color m_RowSelectColor = Color.FromArgb(234, 249, 255);
        private Color m_ItemSelectColor = Color.FromArgb(134, 249, 255);//234, 249, 255   KongMengyuan,2015-10-30,修改鼠标选中行颜色
        private Brush m_SelectBrush = new SolidBrush(Color.FromArgb(234, 249, 255));
        private Color progressTextColor = Color.FromArgb(87, 99, 115); //153, 153, 153, 进度条的文本字体颜色
        private Pen m_BroderPen = new Pen(Color.FromArgb(240, 240, 240));
        private Pen m_StatusPen = new Pen(Color.FromArgb(221, 221, 221));
        private Brush m_OpenTextBrush = new SolidBrush(Color.FromArgb(52, 52, 52));
        private Brush m_StatusBrush = new SolidBrush(Color.FromArgb(221, 221, 221));
        private Brush m_StatusInnerBrush = new SolidBrush(Color.White); //进度条的百分比显示框-转换中,原来是白色底框,V6.2把它取消了,不用这个白框了 new SolidBrush(Color.FromArgb(255, 255, 255));
        private Brush m_StatusInnerBrushStart = new SolidBrush(Color.FromArgb(205, 231, 255));
        private Brush m_StatusInnerBrushFinish = new SolidBrush(Color.FromArgb(187, 240, 188)); //进度条的百分比显示框-完成,原来是白色底框,V6.2把它取消了,不用这个白框了 new SolidBrush(Color.FromArgb(255, 255, 255));
        private Brush m_StatusPersentValueBrush = new SolidBrush(Color.FromArgb(205, 231, 255));   //41, 173, 255,进度条-转换中
        private Brush m_StatusPersentValueBrushFail = new SolidBrush(Color.FromArgb(254, 204, 203));   //41, 173, 255,进度条-出错,目前不使用,文件在转换过程中不存在出错(因为在转换时被转换文件处于锁死状态)
        private Brush m_StatusPersentValueBrushPause = new SolidBrush(Color.FromArgb(204, 230, 255));   //41, 173, 255,进度条-暂停,目前不使用,和正在转换时状态颜色相同即可
        private Brush m_StatusPersentValueBrushFinish = new SolidBrush(Color.FromArgb(187, 240, 188));   //41, 173, 255,进度条-完成
        #endregion

        #region 事件定义
        private HeaderNativeWindow _headerNativeWindow;
        public delegate void StatusChangedDelegate(int index, StatusType status);
        // 状态按钮点击事件
        public event StatusChangedDelegate OnStatusButtonClicked;
        public delegate void OpenFileDelegate(int index);
        // 打开文件按钮点击事件
        public event OpenFileDelegate OnOpenFileButtonClicked;
        public delegate void OpenDirectoryDelegate(int index);
        // 打开文件路径按钮点击事件
        public event OpenDirectoryDelegate OnOpenDirectoryButtonClicked;
        public delegate void DeleteButtonDelegate(int index);
        // 删除按钮点击事件
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
        public int PageCountWidth
        {
            get { return this.Columns[COL_PAGECOUNT].Width; }
            set { this.Columns[COL_PAGECOUNT].Width = value; }
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

        //[Category("ColumnWidth")]
        //public int OperateWidth
        //{
        //    get { return this.Columns[COL_OPERATE].Width; }
        //    set { this.Columns[COL_OPERATE].Width = value; }
        //}

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

        //列名,列标题头
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
        public string PageCountText
        {
            get { return this.Columns[COL_PAGECOUNT].Text; }
            set { this.Columns[COL_PAGECOUNT].Text = value; }
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

        //[Category("ColumnName")]
        //public string OperateText
        //{
        //    get { return this.Columns[COL_OPERATE].Text; }
        //    set { this.Columns[COL_OPERATE].Text = value; }
        //}

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
                    this.Invalidate(Items[i].SubItems[COL_OPEN].Bounds);
                }
            }
        }private string m_OpenButtonText;

        //public string OpenProgressText
        //{
        //    get { return m_OpenProgressText; }
        //    set
        //    {
        //        m_OpenProgressText = value;
        //        for (int i = 0; i < Items.Count; i++)
        //        {
        //            this.Invalidate(Items[i].SubItems[COL_OPERATE].Bounds);
        //        }
        //    }
        //}
        //private string m_OpenProgressText;

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
                    //ItemInfomation info = (ItemInfomation)this.Items[i].Tag;
                    //if (info.Status != StatusType.Done && info.Status != StatusType.Ready)

                    //KongMengyuan,2015-11-10,ItemInfomation info = (ItemInfomation)this.Items[i].Tag 有时会弹出System.NullReferenception: 未将对象引用设置到对象的实例中
                    try
                    {
                        ItemInfomation info = (ItemInfomation)this.Items[i].Tag;
                        if (info.Status != StatusType.Done && info.Status != StatusType.Ready)
                        { return false; }
                    }
                    catch
                    {
                        return false;
                    }

                }
                return true;
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
            ImageList lst = new ImageList();
            lst.ImageSize = new Size(1, 45); //控制ListView的行宽和行高的(不是标题头,不是列头),KongMengyuan注释,2015-11-19
            this.Font = new Font("微软雅黑", 14);
            this.View = System.Windows.Forms.View.Details;
            this.SmallImageList = lst;
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

        //加标题头,列头
        private void InitColumns()
        {
            ColumnHeader recNo = new ColumnHeader();
            recNo.Name = "RecNo";
            recNo.Text = "编号";
            recNo.TextAlign = HorizontalAlignment.Center;
            recNo.Width = COL_INDEX_WIDTH;

            ColumnHeader fileName = new ColumnHeader();
            fileName.Name = "FileName";
            fileName.Text = "文件名称";
            fileName.TextAlign = HorizontalAlignment.Center;
            fileName.Width = COL_FILENAME_WIDTH;

            //注意：此处如果增加了一个字段,对应的MainInfoOld.cs里面要注意中英文的对应,也就是MainInfoOld.zh-CN.resx要手工加一条,同时MainInfoOld.en.resx也要手工加一条,否则不显示表头
            //然后再在MainInfoOld.cs才可以在SetZhCn和SetEn里面增加,KongMengyuan增加,2015-11-26,这个表头找了好久才发现是这个规律
            //this.lstFile.PageCountText = rm.GetString("PageCount"); //此处影响传过来的"总页数"
            ColumnHeader pageCount = new ColumnHeader();
            pageCount.Name = "PageCount";
            pageCount.Text = "总页数";
            pageCount.TextAlign = HorizontalAlignment.Center;
            pageCount.Width = COL_PAGECOUNT_WIDTH;

            ColumnHeader select = new ColumnHeader();
            select.Name = "SelectPages";
            select.Text = "选择页码";
            select.TextAlign = HorizontalAlignment.Center;
            select.Width = COL_SELECTPAGE_WIDTH;

            ColumnHeader status = new ColumnHeader();
            status.Name = "Status";
            status.Text = "状态";
            status.TextAlign = HorizontalAlignment.Center;
            status.Width = COL_STATUS_WIDTH;

            //ColumnHeader operate = new ColumnHeader();
            //operate.Name = "Operate";
            //operate.Text = "操作";
            //operate.TextAlign = HorizontalAlignment.Center;
            //operate.Width = COL_OPERATE_WIDTH;

            ColumnHeader open = new ColumnHeader();
            open.Name = "Open";
            open.Text = "打开";
            open.TextAlign = HorizontalAlignment.Center;
            open.Width = COL_OPEN_WIDTH;

            ColumnHeader folder = new ColumnHeader();
            folder.Name = "folder";
            folder.Text = "输出";
            folder.TextAlign = HorizontalAlignment.Center;
            folder.Width = COL_FOLDER_WIDTH;

            ColumnHeader del = new ColumnHeader();
            del.Name = "del";
            del.Text = "删除";
            del.TextAlign = HorizontalAlignment.Center;
            del.Width = COL_DEL_WIDTH;

            this.Columns.AddRange(new ColumnHeader[] { recNo, fileName, pageCount, select, status, open, folder, del });//, open, folder, del operate
        }

        #endregion

        #region 填充空白列颜色,空白行颜色

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
                if (m.Msg == 0xF || m.Msg == 0x47) //0xf WM_PAINT    0x47 WM_WINDOWPOSCHANGED
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
            //画列标题头的方框,KongMengyuan注释,2015-11-19
            RenderBackgroundInternal(g,
                                     e.Bounds,
                                     m_BaseColor,
                                     m_BorderColor,
                                     m_InnerBorderColor,
                                     true);
            //绘画文字
            TextRenderer.DrawText(g,
                                  e.Header.Text,//Listview的列标题头,列头
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
        //加载每行数据,KongMengyuan注释,2015-11-21
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
                    //总页数
                    case COL_PAGECOUNT:
                        //得到文件的总页数,KongMengyuan增加,2015-11-19
                        string filePageCount = "1234567890"; //如果取不到文件的总页数就默认为1
                        //如果此处取出的页面是错误的(比如在桌面右键鼠标建立一个记事本文件 aaa.txt,然后直接将它改为 aaa.pdf,则此处取不到页数),则会产生拖入ListView之后发现这条数据只有一个1,其它的没有
                        try
                        {
                            filePageCount = filePageCountGet(info);//目前不加在这里,这里是隐藏使用的,但是总页数目前隐藏用不到
                        }
                        catch (Exception e2)
                        {
                            Console.WriteLine(e2.StackTrace);
                            filePageCount = "1";
                        }
                        TextRenderer.DrawText(e.Graphics,
                                              filePageCount,
                                              ItemFont,
                                              e.Bounds,
                                              this.ForeColor,
                                              TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
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
                        //Rectangle rTxt = e.Bounds;
                        Graphics g = e.Graphics;
                        //g.FillRectangle(Brushes.Transparent, new Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width - 45, 45));
                        //rTxt.X += 10;
                        //rTxt.Y -= 8;
                        //Brush sh = m_StatusInnerBrush;
                        //if (e.Item.Selected)
                        //{
                        //    sh = m_SelectBrush;
                        //    e.Graphics.FillRectangle(sh, e.Bounds.X + 3, e.Bounds.Y + 2, 50, 25);
                        //}
                        //else
                        //{
                        //    e.Graphics.FillRectangle(sh, e.Bounds.X + 3, e.Bounds.Y + 2, 50, 25);
                        //}
                        //TextRenderer.DrawText(e.Graphics,
                        //                      info.PersentValue + "%",
                        //                      ItemFont,
                        //                      rTxt,
                        //                      progressTextColor,
                        //                      TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
                        //进度条
                        //FillRoundRectangle(g, m_StatusPersentValueBrush, new Rectangle(e.Bounds.X + 10, e.Bounds.Y + 20 + 6, e.Bounds.Width - 55, 8), 1);
                        //蓝色

                        ////2015-12-14,KongMengyuan增加,先擦除进度条后面的文字(以白色代替,在选中状态下会显示白色,所以再转成透明的,但发现这种方法也解决不了总页数大于2位数的重影问题)
                        FillRoundRectangle2(g, Brushes.White //Brushes.Transparent
                                , new Rectangle(e.Bounds.X + (int)((e.Bounds.Width) * (info.PersentValue / 100f))
                                                , e.Bounds.Y + 2
                                                , (int)(e.Bounds.Width) - (int)((e.Bounds.Width) * (info.PersentValue / 100f)) - 35 //为了把最后的图标不擦除,如果擦除了也会重写但屏幕会闪
                                                , 43)
                                , 4);

                        if (info.PersentValue == 0)
                        {
                            //KongMengyuanw修改,2015-11-05,转换成功后,显示进度条最后有一点点没有走完,界面有瑕疵
                            //FillRoundRectangle1(g, Brushes.White, new Rectangle(e.Bounds.X + 10, e.Bounds.Y + 20 + 6, (int)((e.Bounds.Width - 55) * (info.PersentValue / 100f)), 8), 4);
                            //KongMengyuanw修改,2015-11-05,进度条的瑕疵已经修改过
                            //FillRoundRectangle1(g, Brushes.White, new Rectangle(e.Bounds.X + 10, e.Bounds.Y + 20 + 6, (int)((e.Bounds.Width - 53) * (info.PersentValue / 100f)), 8), 4);
                            FillRoundRectangle2(g, Brushes.White, new Rectangle(e.Bounds.X, e.Bounds.Y + 2, (int)((e.Bounds.Width) * (info.PersentValue / 100f)), 45), 4);
                        }
                        else
                        {
                            //KongMengyuanw修改,2015-11-05,转换成功后,显示进度条最后有一点点没有走完,界面有瑕疵
                            //FillRoundRectangle1(g, m_StatusPersentValueBrush, new Rectangle(e.Bounds.X + 10 - 1, e.Bounds.Y + 20 + 6, (int)((e.Bounds.Width - 55) * (info.PersentValue / 100f)), 8), 4);
                            //KongMengyuanw修改,2015-11-05,进度条的瑕疵已经修改过
                            //FillRoundRectangle1(g, m_StatusPersentValueBrush, new Rectangle(e.Bounds.X + 10 - 1, e.Bounds.Y + 20 + 6, (int)((e.Bounds.Width - 53) * (info.PersentValue / 100f)), 8), 4);

                            if (info.PersentValue == 100)
                            {
                                FillRoundRectangle2(g, m_StatusPersentValueBrushFinish, new Rectangle(e.Bounds.X, e.Bounds.Y, (int)((e.Bounds.Width) * (info.PersentValue / 100f)), 45), 4);
                            }
                            else
                            {
                                //显示进度条的当前长度
                                FillRoundRectangle2(g, m_StatusPersentValueBrush, new Rectangle(e.Bounds.X, e.Bounds.Y, (int)((e.Bounds.Width) * (info.PersentValue / 100f)), 45), 4);
                            }
                        }

                        //DrawRoundRectangle1(g, m_StatusPen, new Rectangle(e.Bounds.X + 10 - 1, e.Bounds.Y + 20 + 6, e.Bounds.Width - 55 + 2, 8), 4);
                        //DrawRoundRectangle2(g, m_StatusPen, new Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width, 45), 4); //2015-12-15,KongMengyuan注释,底部的框

                        //文本(百分比的文本)
                        Rectangle rTxt = e.Bounds;
                        //Graphics g = e.Graphics;
                        g.FillRectangle(Brushes.Transparent, new Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width - 45, 45));
                        rTxt.X += 10; //百分比文本的左边起始位置
                        rTxt.Y -= 0; //8;  //百分比文本的顶部起始位置,进度条文本的顶部位置,顶端位置
                        //Brush sh = m_StatusInnerBrush; //进度条的白框
                        //数字上面有一个白色的矩形方框,这样方便查看转换的百分比,KongMengyuan注释,2015-11-18,目的是擦除原来的百分比(否则会显示重影)
                        //if (e.Item.Selected)
                        //{
                        //    sh = m_SelectBrush;
                        //    e.Graphics.FillRectangle(sh, e.Bounds.X + 3, e.Bounds.Y + 2, 50, 25);
                        //}
                        //KongMengyuan修改,2015-11-18,把原来的百分比擦除,否则会引起文本重复显示(上下文本叠在一起)
                        //if (info.PersentValue == 20)//如果进度条已经超过百分比的文本显示了,目前只考虑20%的百分比,因为进度条是按照20,20这样走的
                        //{
                        //    //在转换过程中按了"停止转换"后解决字符串的重叠问题(但是仍有后半个白色底部的空格,这个问题目前是解决不了,因为不是整个进度条擦除重画,为了更好的用户体验不擦除原来的进度条)
                        //    Brush sh1 = m_StatusInnerBrush; //进度条的白框
                        //    e.Graphics.FillRectangle(sh1, e.Bounds.X + 25, e.Bounds.Y + 2, 25, 25); //原有百分比的后半部分,前半部分自动被新的进度条覆盖了
                        //}

                        string persentValue = string.Empty;
                        if (info.PersentValue == 100)
                        {
                            persentValue = "100% 完成";
                        }
                        else if (info.PersentValue == 0)
                        {
                            //前面的两个空格不可缺少,它的作用是为了在转换过程中按了"停止转换"后解决字符串的重叠问题(但是仍有半个白色底部的空格,这个问题目前是解决不了,因为不是整个进度条擦除重画,为了更好的用户体验不擦除原来的进度条)
                            persentValue = "  0%";
                        }
                        else
                        {
                            persentValue = info.PersentValue + "% 转换中...";
                        }
                        TextRenderer.DrawText(e.Graphics,
                                              persentValue,
                                              ItemFont,
                                              rTxt,
                                              progressTextColor,
                                              TextFormatFlags.Left | TextFormatFlags.VerticalCenter);

                        //e.Graphics.FillRectangle(m_StatusBrush, e.Bounds.X + 10 - 1, e.Bounds.Y + 20 + 6 - 1 , e.Bounds.Width - 55 + 2, 8 + 2);
                        //e.Graphics.FillRectangle(m_StatusInnerBrush, e.Bounds.X + 10, e.Bounds.Y + 20 + 6, e.Bounds.Width - 55, 8);
                        //e.Graphics.FillRectangle(m_StatusPersentValueBrush, e.Bounds.X + 10, e.Bounds.Y + 20 + 6, (e.Bounds.Width - 55) * (info.PersentValue / 100f), 8);
                        //按钮(画滚动条的右侧状态按钮)
                        if (e.ItemState == ListViewItemStates.Grayed) return;
                        Image imgstatus = GetStatusImage(info.Status);
                        if (imgstatus == null) return;
                        e.Graphics.DrawImage(imgstatus, e.Bounds.X + e.Bounds.Width - STATUSBUTTONXPOS, e.Bounds.Top + STATUSBUTTONYPOS - 4 + 5, 20, 20); //KongMengyuan注释,2015-12-15,后面的小图标
                        if (info.StatusChanged) //此句是把图标放在了进度条显示的下面,去掉它图标就一直显示在上面了,KongMengyuan修改了写法,2015-11-18
                        {
                            info.StatusChanged = false;
                            this.Invalidate(new Rectangle(e.Bounds.X + e.Bounds.Width - STATUSBUTTONXPOS, e.Bounds.Top + STATUSBUTTONYPOS - 4 + 5, 20, 20));
                        }
                        else
                        {
                            info.StatusChanged = true;
                        }

                        DrawRoundRectangle2(g, m_StatusPen, new Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width, 45), 4); //2015-12-15,KongMengyuan注释,底部的框
                        break;

                    case COL_OPEN:
                        Image imgopenFile = Properties.Resources.open_2;
                        e.Graphics.DrawImage(imgopenFile, e.Bounds.Left + 12, e.Bounds.Top + 15);
                        if (e.ItemState == ListViewItemStates.Grayed)
                            this.Invalidate(e.Item.SubItems[COL_OPEN].Bounds);
                        break;
                    case COL_FLODER:
                        Image imgopenFolder = info.OpenPathButtonHover && e.Item.Bounds.Contains(this.PointToClient(Cursor.Position)) ? Properties.Resources.path : Properties.Resources.file;
                        e.Graphics.DrawImage(imgopenFolder, e.Bounds.Left + 12, e.Bounds.Top + 15);
                        if (e.ItemState == ListViewItemStates.Grayed)
                            this.Invalidate(e.Item.SubItems[COL_FLODER].Bounds);
                        break;
                    case COL_DEL:
                        Image imgdel = info.DeleteButtonHover && e.Item.Bounds.Contains(this.PointToClient(Cursor.Position)) ? Properties.Resources.deletehover : Properties.Resources.delete;
                        e.Graphics.DrawImage(imgdel, e.Bounds.Left + 12, e.Bounds.Top + 15, imgdel.Width, imgdel.Height);
                        if (e.ItemState == ListViewItemStates.Grayed)
                            this.Invalidate(e.Item.SubItems[COL_DEL].Bounds);
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }

        public static void DrawRoundRectangle1(Graphics g, Pen pen, Rectangle rect, int cornerRadius)
        {
            using (GraphicsPath path = CreateRoundedRectanglePath1(rect, cornerRadius))
            {
                lock (path)
                {
                    g.DrawPath(pen, path);
                }
            }
        }

        public static void DrawRoundRectangle2(Graphics g, Pen pen, Rectangle rect, int cornerRadius)
        {
            using (GraphicsPath path = CreateRoundedRectanglePath2(rect, cornerRadius))
            {
                lock (path)
                {
                    g.DrawPath(pen, path);
                }
            }
        }

        public static void FillRoundRectangle1(Graphics g, Brush brush, Rectangle rect, int cornerRadius)
        {
            using (GraphicsPath path = CreateRoundedRectanglePath1(rect, cornerRadius))
            {
                lock (path)
                {
                    g.FillPath(brush, path);
                }
            }
        }

        public static void FillRoundRectangle2(Graphics g, Brush brush, Rectangle rect, int cornerRadius)
        {
            using (GraphicsPath path = CreateRoundedRectanglePath2(rect, cornerRadius))
            {
                lock (path)
                {
                    g.FillPath(brush, path);
                }
            }
        }

        //矩形(四个角为圆角)
        internal static GraphicsPath CreateRoundedRectanglePath1(Rectangle rect, int cornerRadius)
        {
            GraphicsPath roundedRect = new GraphicsPath();
            roundedRect.AddArc(rect.X, rect.Y, cornerRadius * 2, cornerRadius * 2, 180, 90);
            roundedRect.AddLine(rect.X + cornerRadius, rect.Y, rect.Right - cornerRadius * 2, rect.Y);
            roundedRect.AddArc(rect.X + rect.Width - cornerRadius * 2, rect.Y, cornerRadius * 2, cornerRadius * 2, 270, 90);
            roundedRect.AddLine(rect.Right, rect.Y + cornerRadius * 2, rect.Right, rect.Y + rect.Height - cornerRadius * 2);
            roundedRect.AddArc(rect.X + rect.Width - cornerRadius * 2, rect.Y + rect.Height - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 0, 90);
            roundedRect.AddLine(rect.Right - cornerRadius * 2, rect.Bottom, rect.X + cornerRadius * 2, rect.Bottom);
            roundedRect.AddArc(rect.X, rect.Bottom - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 90, 90);
            roundedRect.AddLine(rect.X, rect.Bottom - cornerRadius * 2, rect.X, rect.Y + cornerRadius * 2);
            roundedRect.CloseFigure();
            return roundedRect;
        }

        //矩形(四个角为直角)
        internal static GraphicsPath CreateRoundedRectanglePath2(Rectangle rect, int cornerRadius)
        {
            GraphicsPath roundedRect = new GraphicsPath();
            roundedRect.AddRectangle(rect);
            roundedRect.CloseFigure();
            return roundedRect;
        }

        private string CheckDispalyFileName(string strName)
        {
            SizeF sizeF = this.CreateGraphics().MeasureString(strName, ItemFont);
            //KongMengyuan注释,2015-11-21,原来只
            //if (sizeF.Width > COL_FILENAME_WIDTH - 40)
            if (sizeF.Width > COL_FILENAME_WIDTH)
            {
                //strName = strName.Substring(0, 15) + "..." + Path.GetExtension(strName);//显示文件名,多余的文件名使用"..."
                //KongMengyuan修改,2015-11-21,上面的写法不科学,比如"壹贰叁肆伍陆柒捌玖拾壹贰叁肆伍陆柒捌玖拾.pdf"

                strName = GetSubString(strName, 32) + ".." + Path.GetExtension(strName);//显示文件名,多余的文件名使用"..."
            }
            return strName;
        }
        #region
        /// <summary>
        /// 获取指定字节长度的中英文混合字符串,KongMengyuan增加,2015-11-21
        /// </summary>
        /// <summary> 
        /// <summary>
        /// 获取指定字节长度的中英文混合字符串
        /// </summary>
        private string GetSubString(string str, int len)
        {
            string result = string.Empty;// 最终返回的结果
            int byteLen = System.Text.Encoding.Default.GetByteCount(str);// 单字节字符长度
            int charLen = str.Length;// 把字符平等对待时的字符串长度
            int byteCount = 0;// 记录读取进度
            int pos = 0;// 记录截取位置
            if (byteLen > len)
            {
                for (int i = 0; i < charLen; i++)
                {
                    if (System.Convert.ToInt32(str.ToCharArray()[i]) > 255)// 按中文字符计算加2
                        byteCount += 2;
                    else// 按英文字符计算加1
                        byteCount += 1;
                    if (byteCount > len)// 超出时只记下上一个有效位置
                    {
                        pos = i;
                        break;
                    }
                    else if (byteCount == len)// 记下当前位置
                    {
                        pos = i + 1;
                        break;
                    }
                }

                if (pos >= 0)
                    result = str.Substring(0, pos);
            }
            else
                result = str;

            return result;
        }

        #endregion
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
            try
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
            catch
            { }
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
        int sel = 0;
        private void ListViewPlus_MouseClick(object sender, MouseEventArgs e)
        {
            sel = 0;
            if (e.Button != System.Windows.Forms.MouseButtons.Left) return;
            for (int i = 0; i < this.Items.Count; i++)
            {
                if (this.Items[i].Tag is ItemInfomation)
                {
                    if (this.Items[i].Selected)
                    {
                        sel = i;
                        //e.Graphics.FillRectangle(sh, e.Bounds.X + 3, e.Bounds.Y - 3, 50, 40);

                    }
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
                    r = new Rectangle(this.Items[i].SubItems[COL_OPEN].Bounds.X + 5,
                                      this.Items[i].SubItems[COL_OPEN].Bounds.Top + 15,
                                      (int)sizeF.Width, (int)sizeF.Height);
                    if (r.Contains(e.X, e.Y))
                    {
                        if (OnOpenFileButtonClicked != null)
                            OnOpenFileButtonClicked(i);
                        return;
                    }
                    //打开目录按钮
                    r = new Rectangle(this.Items[i].SubItems[COL_FLODER].Bounds.X + 5,
                                      this.Items[i].SubItems[COL_FLODER].Bounds.Top + 15,
                                      24, 24);
                    if (r.Contains(e.X, e.Y))
                    {
                        if (OnOpenDirectoryButtonClicked != null)
                            OnOpenDirectoryButtonClicked(i);
                        return;
                    }

                    //删除按钮
                    r = new Rectangle(this.Items[i].SubItems[COL_DEL].Bounds.X + 5,
                                      this.Items[i].SubItems[COL_DEL].Bounds.Top + 15,
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
                    r = new Rectangle(this.Items[i].SubItems[COL_OPEN].Bounds.X + 5,
                                      this.Items[i].SubItems[COL_OPEN].Bounds.Top + 15,
                                      (int)sizeF.Width, (int)sizeF.Height);
                    if (r.Contains(e.X, e.Y))
                    {
                        this.Cursor = Cursors.Hand;
                        return;
                    }
                    //打开目录按钮
                    r = new Rectangle(this.Items[i].SubItems[COL_FLODER].Bounds.X + 5,
                                      this.Items[i].SubItems[COL_FLODER].Bounds.Top + 15,
                                      24, 24);
                    if (r.Contains(e.X, e.Y))
                    {
                        this.Cursor = Cursors.Hand;
                        info.OpenPathButtonHover = true;
                        //RefreshItems(i);
                        return;
                    }
                    //删除按钮
                    r = new Rectangle(this.Items[i].SubItems[COL_DEL].Bounds.X + 5,
                                      this.Items[i].SubItems[COL_DEL].Bounds.Top + 15,
                                      24, 24);
                    if (r.Contains(e.X, e.Y))
                    {
                        this.Cursor = Cursors.Hand;
                        info.DeleteButtonHover = true;
                        //RefreshItems(i);
                        return;
                    }
                    if (info.DeleteButtonHover || info.OpenPathButtonHover)
                    {
                        info.DeleteButtonHover = false;
                        info.OpenPathButtonHover = false;
                        //RefreshItems(i);
                    }
                }
            }
            this.Cursor = Cursors.Default;
        }

        private void RefreshItems(int index)
        {
            //if (index < 0) return;
            //this.OnDrawSubItem(new DrawListViewSubItemEventArgs(this.CreateGraphics(),
            //                                                    this.Items[index].SubItems[COL_OPERATE].Bounds,
            //                                                    this.Items[index],
            //                                                    this.Items[index].SubItems[COL_OPERATE],
            //                                                    index,
            //                                                    COL_OPERATE,
            //                                                    this.Columns[COL_OPERATE],
            //                                                    ListViewItemStates.Grayed));
        }

        private void ListViewPlus_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
        {
            //KongMengyuan,2015-11-01,可以修改列宽
            //if (Columns[COL_INDEX].Width != COL_INDEX_WIDTH)
            //    this.Columns[COL_INDEX].Width = COL_INDEX_WIDTH;
            //if (this.Columns[COL_FILENAME].Width != COL_FILENAME_WIDTH)
            //    this.Columns[COL_FILENAME].Width = COL_FILENAME_WIDTH;
            //if (Columns[COL_SELECTPAGES].Width != COL_SELECTPAGE_WIDTH)
            //    this.Columns[COL_SELECTPAGES].Width = COL_SELECTPAGE_WIDTH;
            //if (Columns[COL_STATUS].Width != COL_STATUS_WIDTH)
            //    this.Columns[COL_STATUS].Width = COL_STATUS_WIDTH;
            ////if (this.Columns[COL_OPERATE].Width != COL_OPERATE_WIDTH)
            ////    this.Columns[COL_OPERATE].Width = COL_OPERATE_WIDTH;
        }

        private void ListViewPlus_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            e.Cancel = true;
        }

        public void AddFile(ItemInfomation info)
        {
            ListViewItem item = this.Items.Add((this.Items.Count + 1).ToString());
            //item.SubItems.AddRange(new string[] { info.DisplayFileName, ConversionPageDefaultText, string.Empty, string.Empty, string.Empty, string.Empty });

            //得到文件的总页数,KongMengyuan增加,2015-11-19
            string filePageCount = "1234567890"; //如果取不到文件的总页数就默认为1
            //如果此处取出的页面是错误的(比如在桌面右键鼠标建立一个记事本文件 aaa.txt,然后直接将它改为 aaa.pdf,则此处取不到页数),则会产生拖入ListView之后发现这条数据只有一个1,其它的没有
            try
            {
                filePageCount = filePageCountGet(info);//目前不加在这里,这里是隐藏使用的,但是总页数目前隐藏用不到
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                filePageCount = "1";
            }
            item.SubItems.AddRange(new string[] { info.DisplayFileName, filePageCount, ConversionPageDefaultText, string.Empty, string.Empty, string.Empty, string.Empty }); //增加总页数,KongMengyuan修改,2015-11-19
            //MessageBox.Show(info.DisplayFileName +" 总页数 "+ filePageCount+"  "+ConversionPageDefaultText);
            item.Tag = info;
        }

        private string filePageCountGet(ItemInfomation info)
        {
            //KongMengyuan增加,2015-11-19,得到文件的总页数
            Aspose.Pdf.Document pdf_doc;
            Aspose.Words.Document word_doc;
            Aspose.Cells.Workbook excel_doc;
            Aspose.Slides.Presentation ppt_doc;
            Aspose.Pdf.Facades.PdfExtractor txt_doc;
            FileStream fileStream;
            int pages = 1;//如果取不到,则默认总页数为1

            try
            {
                fileStream = new FileStream(info.FileFullPath, FileMode.Open, FileAccess.Read);
            }
            catch (Exception ex)
            {
                return "1";
            }

            try
            {
                //pdf_doc = new Aspose.Pdf.Document(fileStream, global_config.password);
                string fileType = Path.GetExtension(info.FileFullPath).ToLower();

                if (fileType.ToLower() == ".pdf")
                {
                    pdf_doc = new Aspose.Pdf.Document(fileStream); //如果此处取出的页面是错误的(比如在桌面右键鼠标建立一个记事本文件 aaa.txt,然后直接将它改为 aaa.pdf,则此处取不到页数),则此处不会产生错误,如果想解决这个问题,需要期待第三方控件的dll版本更新
                    pages = pdf_doc.Pages.Count;
                }
                else if (fileType.ToLower() == ".doc" || fileType.ToLower() == ".docx")
                {
                    word_doc = new Aspose.Words.Document(fileStream);
                    pages = word_doc.PageCount;
                }
                else if (fileType.ToLower() == ".ppt" || fileType.ToLower() == ".pptx")
                {
                    try
                    {
                        ppt_doc = new Presentation(fileStream);
                        pages = ppt_doc.Slides.Count;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.StackTrace);
                    }
                }
                else if (fileType.ToLower() == ".xls" || fileType.ToLower() == ".xlsx")
                {
                    excel_doc = new Aspose.Cells.Workbook(fileStream);
                    pages = excel_doc.Worksheets.Count;
                }
            }
            catch (Aspose.Pdf.Exceptions.InvalidPasswordException ex)
            {
                pages = 1; //如果取不到,则默认总页数为1
            }
            fileStream.Close();//2015-12-21,KongMengyuan增加,释放内存,否则会出现"当文件转换完成后，在原始文件夹执行“删除”操作，发现原始PDF不能删除。转换完成后，仍然没释放对文件的控制。"
            return pages.ToString();
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
            //text = info.PersentValue + "%";
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
            //ListView的顶部标题头方框定义,KongMengyuan注释,2015-11-02
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
            set { m_PresentStr = value; }
        }

        private string m_PresentStr = "";

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
