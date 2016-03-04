/*
pbFile2Word     文件转Word
pbFile2Excel    文件转Excel
pbFile2PPT      文件转PPT
pbFile2HTML     文件转HTML
pbFile2TXT      文件转TXT
pbFile2IMG      文件转图片
pbIMG2PDF       图片转PDF
pbDoc2PDF       Word转PDF
pbExcel2PDF     Excel转PDF
pbPPT2PDF       PPT转PDF
pbPdfSplit      PDF分割
pbPDFMerge      PDF合并
pbPDFCompress   PDF压缩
pbPDFDecode     PDF密码解除
pbPDFGetImg     PDF图片获取
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using System.Threading;
using System.IO;
using System.Runtime.InteropServices; //运行Windows的API,引用DLL专用
using System.Diagnostics;
using System.Management;//项目下边的 引用--添加引用--框架--System.Management
using System.Text.RegularExpressions; //Regex调用
using System.Globalization;//CultureInfo调用

using System.Net.NetworkInformation; //C# winform点击按钮获取指定ip的mac地址
using System.Net; //KongMengyuan增加,2015-11-09,statisticsPost向服务器POST使用
using System.Collections;//ArrayList使用

//using System.Resources; //调用每个窗体对应的resx使用,ResourceManager调用它
//using Microsoft.Win32; //关于注册表的命名空间

namespace PDF_Convert
{
    public partial class MainInfo01 : Form
    {
        public string spath = string.Empty;
        private Thread[] thread;
        //导航栏目
        private Convert01.FORMAT format = new Convert01.FORMAT();
        private SynchronizationContext syncContext = null;
        public static bool isReg = false; //是否已注册
        private ini_config ini = new ini_config("config.ini");
        public Dictionary<string, bool> diclst = new Dictionary<string, bool>();//列表集合
        public bool isClose = false;
        public Queue<ListViewItem> fileQueue = new Queue<ListViewItem>();//文件队列

        private int m_EditIndex;
        private string outFolderSelect = ""; //当前选择的是哪个RadioButton,rdoOldPath-原文件夹, rdoNewPath-自定义文件夹
        private string outFolder = ""; //保留当前输出文件夹(在文件转换过程中不允许更改它)

        private int cpuNumber = 1; //物理CPU核数
        private int cpuLogicalNumber = 1; //逻辑CPU核数
        //线程管理当前文件集合
        public Dictionary<string, int> dicThreadManagement = new Dictionary<string, int>();

        private bool isStart;
        private bool NeedToPop;
        private bool Poped = false;

        [DllImport("user32")]
        public static extern int ReleaseCapture();//用鼠标拖动窗体,移动窗体
        [DllImport("user32")]
        public static extern int SendMessage(IntPtr hwnd, int msg, int wp, int lp);//用鼠标拖动窗体,移动窗体

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr ChangeWindowMessageFilter(uint message, uint dwFlag);
        private uint WM_DROPFILES = 0x0233;
        private uint WM_COPYDATA = 0x4A;
        private uint MSGFLT_ADD = 1;

        private int selectLineChange = 0;//记录当前点击行的行号

        public MainInfo01()
        {
            InitializeComponent();

            #region  安装之前要注释,测试自动更新界面,便于方便程序员调试程序,KongMengyuan(全文搜索这句话,在发布之前全部去掉注释)
            //string testRegCode_Tmp_1 = new reg().get_cpu_code().ToString();
            //string testRegCode_Tmp_2 = new reg().get_MACaddress_code().ToString();
            //string testRegCode_Tmp_3 = reg.get_SetupDisk_code("C:");
            //string testRegCode_Tmp_4 = new reg().get_machine_code();
            //MessageBox.Show("cpu " + testRegCode_Tmp_1 + "\r\n" + "MACaddress " + testRegCode_Tmp_2 + "\r\n" + "SetupDisk " + testRegCode_Tmp_3 + "\r\n" + "MachineCode " + testRegCode_Tmp_4);
            #endregion

            //tableLayoutPanel8的Columns和Rows的属性可以自己在属性里面修改,可以调整控件的长宽和高低了
            this.Size = new Size(980, 698);//窗体界面固定大小(防止程序员不小心改变了大小,因为是多语言界面,窗体大小是固定的)

            syncContext = SynchronizationContext.Current; //利用SynchronizationContext.Current在线程间同步上下文,允许一个线程和另外一个线程进行通讯，SynchronizationContext在通讯中充当传输者的角色。另外这里有个地方需要清楚的，不是每个线程都附加SynchronizationContext这个对象，只有UI线程是一直拥有的。
            Control.CheckForIllegalCrossThreadCalls = false;//容许子线程随时更新ui.这也是它的死穴：在同一个test函数体内,不能保证自身事务的一致性.给label1付了值,一回头,就已经被别人改了,这和超市的踩踏事件的后果一样严重.

            switch (Program.iniLanguage.ToLower())
            {
                case "zh-cn"://简体中文,中文简体
                    spath = Application.StartupPath + "\\zh-CN\\MainInfo01\\";
                    this.tbBrw.Location = new Point(840, 57);
                    this.tbBrw.Size = new System.Drawing.Size(129, 21);
                    this.pbQQ.Width = 222;

                    this.btnAddFiles.Location = new Point(11, 53); //添加文件
                    this.btnAddFiles.Size = new System.Drawing.Size(103, 29);
                    this.btnFolder.Location = new Point(123, 53); //添加文件夹
                    this.btnFolder.Size = new System.Drawing.Size(103, 29);
                    this.btnStart.Location = new Point(235, 53); //开始转换
                    this.btnStart.Size = new System.Drawing.Size(103, 29);
                    this.btnStop.Location = new Point(347, 53); //停止转换
                    this.btnStop.Size = new System.Drawing.Size(103, 29);
                    this.btnClear.Location = new Point(459, 53); //清空列表
                    this.btnClear.Size = new System.Drawing.Size(103, 29);
                    this.pb_Folder.Location = new Point(571, 53); //文件保存位置
                    this.pb_Folder.Size = new System.Drawing.Size(405, 29);
                    break;
                case "en"://英文,English
                    spath = Application.StartupPath + "\\en\\MainInfo01\\";
                    this.tbBrw.Location = new Point(878, 57);
                    this.tbBrw.Size = new System.Drawing.Size(85, 21);
                    this.pbQQ.Width = 245;
                    break;
                case "ja"://日本语,日文
                    spath = Application.StartupPath + "\\Ja\\MainInfo01\\";
                    this.tbBrw.Location = new Point(895, 53);
                    this.tbBrw.Size = new System.Drawing.Size(70, 21);
                    this.pbQQ.Width = 252;

                    this.btnAddFiles.Location = new Point(11, 45); //添加文件
                    this.btnAddFiles.Size = new System.Drawing.Size(103, 40);
                    this.btnFolder.Location = new Point(123, 45); //添加文件夹
                    this.btnFolder.Size = new System.Drawing.Size(103, 40);
                    this.btnStart.Location = new Point(235, 45); //开始转换
                    this.btnStart.Size = new System.Drawing.Size(103, 40);
                    this.btnStop.Location = new Point(347, 45); //停止转换
                    this.btnStop.Size = new System.Drawing.Size(103, 40);
                    this.btnClear.Location = new Point(459, 45); //清空列表
                    this.btnClear.Size = new System.Drawing.Size(103, 40);
                    this.pb_Folder.Location = new Point(571, 45); //文件保存位置
                    this.pb_Folder.Size = new System.Drawing.Size(405, 40);
                    break;
                case "zh-tw"://繁体中文,中文繁体
                    spath = Application.StartupPath + "\\zh-TW\\MainInfo01\\";
                    break;
                default:
                    spath = Application.StartupPath + "\\zh-CN\\MainInfo01\\";
                    break;
            }
        }

        private void MainInfo01_Load(object sender, EventArgs e)
        {
            this.plTop.BackgroundImage = Image.FromFile(spath + "header_01.png");
            this.plTop.Tag = "Free Trial"; //默认是免费试用版本
            this.btnAddFiles.BackgroundImage = Image.FromFile(spath + "btn_tjwj01.png");
            this.btnFolder.BackgroundImage = Image.FromFile(spath + "btn_tjwjj01.png");
            this.btnStart.BackgroundImage = Image.FromFile(spath + "btn_kszh01.png");
            this.btnStop.BackgroundImage = Image.FromFile(spath + "btn_tzzh01.png");
            this.btnClear.BackgroundImage = Image.FromFile(spath + "btn_qklb01.png");
            this.pb_Folder.BackgroundImage = Image.FromFile(spath + "pb_Folder01.png");
            this.pbReg.BackgroundImage = Image.FromFile(spath + "btn_reg_01.png");
            this.pbBuy1.BackgroundImage = Image.FromFile(spath + "btn_buy_01.png");
            this.plBottom.BackgroundImage = Image.FromFile(spath + "pb_Bottom.png");
            this.pbIndexBackGround.BackgroundImage = Image.FromFile(spath + "index_bg.png");

            this.pb_01.BackgroundImage = Image.FromFile(spath + "pb_01.png");
            this.pbFile2Word.BackgroundImage = Image.FromFile(spath + "pbFile2Word01.png");
            this.pbFile2Word.Top = this.pb_01.Top + this.pb_01.Height - 1;
            this.pbFile2Word.Width = this.pb_01.Width;
            this.pbFile2Word.Left = 0;
            this.pbFile2Excel.BackgroundImage = Image.FromFile(spath + "pbFile2Excel01.png");
            this.pbFile2Excel.Top = this.pbFile2Word.Top + this.pbFile2Word.Height;
            this.pbFile2Excel.Width = this.pb_01.Width;
            this.pbFile2Excel.Left = 0;
            this.pbFile2PPT.BackgroundImage = Image.FromFile(spath + "pbFile2PPT01.png");
            this.pbFile2PPT.Top = this.pbFile2Excel.Top + this.pbFile2Excel.Height;
            this.pbFile2PPT.Width = this.pb_01.Width;
            this.pbFile2PPT.Left = 0;
            this.pbFile2HTML.BackgroundImage = Image.FromFile(spath + "pbFile2HTML01.png");
            this.pbFile2HTML.Top = this.pbFile2PPT.Top + this.pbFile2PPT.Height;
            this.pbFile2HTML.Width = this.pb_01.Width;
            this.pbFile2HTML.Left = 0;
            this.pbFile2TXT.BackgroundImage = Image.FromFile(spath + "pbFile2TXT01.png");
            this.pbFile2TXT.Top = this.pbFile2HTML.Top + this.pbFile2HTML.Height;
            this.pbFile2TXT.Width = this.pb_01.Width;
            this.pbFile2TXT.Left = 0;
            this.pbFile2IMG.BackgroundImage = Image.FromFile(spath + "pbFile2IMG01.png");
            this.pbFile2IMG.Top = this.pbFile2TXT.Top + this.pbFile2TXT.Height;
            this.pbFile2IMG.Width = this.pb_01.Width;
            this.pbFile2IMG.Left = 0;

            this.pb_02.BackgroundImage = Image.FromFile(spath + "pb_02.png");
            this.pb_02.Top = this.pbFile2IMG.Top + this.pbFile2IMG.Height;
            this.pb_02.Width = this.pb_01.Width;
            this.pb_02.Left = 0;
            this.pbIMG2PDF.BackgroundImage = Image.FromFile(spath + "pbIMG2PDF01.png");
            this.pbIMG2PDF.Top = this.pb_02.Top + this.pb_02.Height;
            this.pbIMG2PDF.Width = this.pb_01.Width;
            this.pbIMG2PDF.Left = 0;
            this.pbDoc2PDF.BackgroundImage = Image.FromFile(spath + "pbDoc2PDF01.png");
            this.pbDoc2PDF.Top = this.pbIMG2PDF.Top + this.pbIMG2PDF.Height;
            this.pbDoc2PDF.Width = this.pb_01.Width;
            this.pbDoc2PDF.Left = 0;
            this.pbExcel2PDF.BackgroundImage = Image.FromFile(spath + "pbExcel2PDF01.png");
            this.pbExcel2PDF.Top = this.pbDoc2PDF.Top + this.pbDoc2PDF.Height;
            this.pbExcel2PDF.Width = this.pb_01.Width;
            this.pbExcel2PDF.Left = 0;
            this.pbPPT2PDF.BackgroundImage = Image.FromFile(spath + "pbPPT2PDF01.png");
            this.pbPPT2PDF.Top = this.pbExcel2PDF.Top + this.pbExcel2PDF.Height;
            this.pbPPT2PDF.Width = this.pb_01.Width;
            this.pbPPT2PDF.Left = 0;

            this.pb_03.BackgroundImage = Image.FromFile(spath + "pb_03.png");
            this.pb_03.Top = this.pbPPT2PDF.Top + this.pbPPT2PDF.Height;
            this.pb_03.Width = this.pb_01.Width;
            this.pb_03.Left = 0;
            this.pbPdfSplit.BackgroundImage = Image.FromFile(spath + "pbPdfSplit01.png");
            this.pbPdfSplit.Top = this.pb_03.Top + this.pb_03.Height;
            this.pbPdfSplit.Width = this.pb_01.Width;
            this.pbPdfSplit.Left = 0;
            this.pbPDFMerge.BackgroundImage = Image.FromFile(spath + "pbPDFMerge01.png");
            this.pbPDFMerge.Top = this.pbPdfSplit.Top + this.pbPdfSplit.Height;
            this.pbPDFMerge.Width = this.pb_01.Width;
            this.pbPDFMerge.Left = 0;
            this.pbPDFCompress.BackgroundImage = Image.FromFile(spath + "pbPDFCompress01.png");
            this.pbPDFCompress.Top = this.pbPDFMerge.Top + this.pbPDFMerge.Height;
            this.pbPDFCompress.Width = this.pb_01.Width;
            this.pbPDFCompress.Left = 0;
            this.pbPDFDecode.BackgroundImage = Image.FromFile(spath + "pbPDFDecode01.png");
            this.pbPDFDecode.Top = this.pbPDFCompress.Top + this.pbPDFCompress.Height;
            this.pbPDFDecode.Width = this.pb_01.Width;
            this.pbPDFDecode.Left = 0;
            this.pbPDFGetImg.BackgroundImage = Image.FromFile(spath + "pbPDFGetImg01.png");
            this.pbPDFGetImg.Top = this.pbPDFDecode.Top + this.pbPDFDecode.Height;
            this.pbPDFGetImg.Width = this.pb_01.Width;
            this.pbPDFGetImg.Left = 0;
            this.pbLeft_Bottom.BackgroundImage = Image.FromFile(spath + "pbLeft_Bottom.png");
            this.pbLeft_Bottom.Top = this.pbPDFGetImg.Top + this.pbPDFGetImg.Height;
            this.pbLeft_Bottom.Width = this.pb_01.Width;
            this.pbLeft_Bottom.Left = 0;
            this.pbLeft_Bottom.Height = this.plLeft.Height - this.pbFile2Word.Height * 15 - this.pb_01.Height * 3 + 3; //最后一个的高要注意,同时BackgroundImageLayout也不是Stretch,而是None

            pbIndexBackGround.BringToFront();//BringToFront()置于顶层,SendToBack()置于底层
            backGroundShowHide();//隐藏或显示主界面提示图

            string iniSection = this.Name;
            lstFile.IndexText = Program.Language_Load(Program.iniLanguage, iniSection, "lstFile.RecNo");
            lstFile.FileNameText = Program.Language_Load(Program.iniLanguage, iniSection, "lstFile.FileName");
            lstFile.PageCountText = Program.Language_Load(Program.iniLanguage, iniSection, "lstFile.PageCount");
            lstFile.SelectPageText = Program.Language_Load(Program.iniLanguage, iniSection, "lstFile.SelectPages");
            lstFile.StatusText = Program.Language_Load(Program.iniLanguage, iniSection, "lstFile.Status");
            lstFile.OpenText = Program.Language_Load(Program.iniLanguage, iniSection, "lstFile.Open");
            lstFile.FolderText = Program.Language_Load(Program.iniLanguage, iniSection, "lstFile.folder");
            lstFile.DelText = Program.Language_Load(Program.iniLanguage, iniSection, "lstFile.del");

            lbIsMerger.Text = Program.Language_Load(Program.iniLanguage, iniSection, "lbIsMerger.Text");//将所有图片合并成一个PDF文件
            rbIsMerger1.Text = Program.Language_Load(Program.iniLanguage, iniSection, "rbIsMerger1.Text");//是
            rbIsMerger2.Text = Program.Language_Load(Program.iniLanguage, iniSection, "rbIsMerger2.Text");//否
            lblPPTSize.Text = Program.Language_Load(Program.iniLanguage, iniSection, "lblPPTSize.Text");//定制你的PPT大小
            lblWidth.Text = Program.Language_Load(Program.iniLanguage, iniSection, "lblWidth.Text");//宽：
            lblHeight.Text = Program.Language_Load(Program.iniLanguage, iniSection, "lblHeight.Text");//

            plMerger_box.Width = lbIsMerger.Width + rbIsMerger1.Width + rbIsMerger2.Width + 20 + 10;
            lbIsMerger.Left = 0;
            rbIsMerger1.Left = lbIsMerger.Left + lbIsMerger.Width + 10;
            rbIsMerger2.Left = rbIsMerger1.Left + rbIsMerger1.Width + 10;

            plPPT_box.Width = lblPPTSize.Width + lblWidth.Width + txtWidth.Width + lblHeight.Width + txtHeight.Width + 20 + 10;
            lblPPTSize.Left = 0;
            lblWidth.Left = lblPPTSize.Left + lblPPTSize.Width + 10;
            txtWidth.Left = lblWidth.Left + lblWidth.Width;
            lblHeight.Left = txtWidth.Left + txtWidth.Width + 10;
            txtHeight.Left = lblHeight.Left + lblHeight.Width;

            this.Text = Program.appProgName; //窗体的快捷方式显示,任务栏的显示内容

            //放置“文件转PPT”的"PPT大小设置",“图片转PDF”的"将所有图片合并成一个PDF文件",“PDF合并”百分比,都放在Panel4上(原设计人员定义的,最麻烦的是原设计人员将其Dock定义为Fill,这样即使找到lblPageSize也看不到,已经修改这种作法了)
            this.panel4.Visible = false;
            this.panel4.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.plMerger.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.plPPT.Dock = System.Windows.Forms.DockStyle.Bottom;
            pltext.Height = 29; //45为填写好高度

            this.tsmiStart.Image = Image.FromFile(Application.StartupPath + "\\images\\menu\\menu_play_0.png"); //开始转换,右击鼠标显示菜单
            this.tsmiStop.Image = Image.FromFile(Application.StartupPath + "\\images\\menu\\menu_stop_0.png"); //停止转换,右击鼠标显示菜单
            this.tsmiClear.Image = Image.FromFile(Application.StartupPath + "\\images\\menu\\menu_clear_0.png"); //清空列表,右击鼠标显示菜单
            tsmiStop.Enabled = false;


            this.tsmiStart.Text = Program.Language_Load(Program.iniLanguage, iniSection, "tsmiStart.Text");// 开始转换
            this.tsmiStop.Text = Program.Language_Load(Program.iniLanguage, iniSection, "tsmiStop.Text"); // 停止转换 
            this.tsmiClear.Text = Program.Language_Load(Program.iniLanguage, iniSection, "tsmiClear.Text");// 清空列表
            this.tsmiPageSet.Text = Program.Language_Load(Program.iniLanguage, iniSection, "tsmiPageSet.Text1");// tsmiPageSet.Text1-页数提取方案,tsmiPageSet.Text2-输出设置方案
            this.tsmiPicFormat.Text = Program.Language_Load(Program.iniLanguage, iniSection, "tsmiPicFormat.Text");// 设置图片输出格式

            this.tsmiPicBMP.Text = Program.Language_Load(Program.iniLanguage, iniSection, "tsmiPicBMP.Text");// 输出为BMP格式
            this.tsmiPicEMF.Text = Program.Language_Load(Program.iniLanguage, iniSection, "tsmiPicEMF.Text");// 输出为EMF格式
            this.tsmiPicGIF.Text = Program.Language_Load(Program.iniLanguage, iniSection, "tsmiPicGIF.Text");// 输出为GIF格式
            this.tsmiPicJPG.Text = Program.Language_Load(Program.iniLanguage, iniSection, "tsmiPicJPG.Text");// 输出为JPG格式            
            this.tsmiPicPNG.Text = Program.Language_Load(Program.iniLanguage, iniSection, "tsmiPicPNG.Text");// 输出为PNG格式
            this.tsmiPicTIF.Text = Program.Language_Load(Program.iniLanguage, iniSection, "tsmiPicTIF.Text");// 输出为TIF格式
            this.tsmiPicWMF.Text = Program.Language_Load(Program.iniLanguage, iniSection, "tsmiPicWMF.Text");// 输出为WMF格式

            Convert01.g_splitsettings.split_page_mode = Convert01.tsplitpagemode.spEveryPage; // 默认"文件分割"鼠标右键的RadioButton默认为第1个
            Convert01.pictureFormat = Convert01.picFormat.picFormatJPG; //默认图片提取后转换成jpg图片格式
        }

        private void pbClose_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                this.pbClose.BackgroundImage = Image.FromFile(spath + "btn_tzzh03.png");
                //当有文件正在转换时要给用户一个退出提示
                if (string.Equals(this.pbFile2Word.Tag, "EnabledFalse") && lstFile.Items.Count > 0)
                {
                    //有文件正在转换，是否关闭？
                    string sTip = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_01"); //提示
                    string sOld = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_05"); //有文件正在转换，是否关闭？
                    DialogResult dr = MessageBox.Show(sOld, sTip, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dr.ToString() != "Yes") //DialogResult.Cancel
                    {
                        return;
                    }
                }

                WinformClose();
            }
        }

        private void WinformClose()
        {
            ini_config ini = new ini_config("config.ini");
            ini.write_ini("TargetDir", outFolder);
            ini.write_ini("PicX", this.txtWidth.Text);
            ini.write_ini("PicY", this.txtHeight.Text);
            ini.write_ini("Type", System.Convert.ToInt32(format).ToString());
            if (outFolderSelect == "rdoOldPath") //rdoOldPath-原文件夹, rdoNewPath-自定义文件夹
            {
                ini.write_ini("Out", "1");
            }
            else
            {
                ini.write_ini("Out", "0");
            }
            ini.write_ini("isMerger", this.rbIsMerger1.Checked == true ? "1" : "0");

            isClose = true;
            ////this.Dispose();
            //System.Environment.Exit(-1);
            //this.Close();

            //KongMengyuan修改,2015-11-11,原语句从注册页面返回会错误,而且那种写法也不是C#的WinForm最好的关闭Application方法,应该使用下面语句关闭系统
            Application.Exit();
            System.Diagnostics.Process.GetCurrentProcess().Kill(); //退出系统
        }

        // C# 判断窗体是否已经打开, 避免重复打开同一个窗体
        //判断窗口是否已经打开(这个窗口有时会打开两个,非常奇怪)
        //KongMengyuan增加,2015-11-11,判断多个当前窗口就关闭它(但是多个主窗口只在安装包安装完成后首次运行会有多个主界面出现,但并不是所有机器都有这个问题,只在郑侃炜那里不停的出现,非常奇怪,我们的机器没有这个问题)
        //可能是splash这个页面,因为是多线程写的,可能是郑总的计算机环境哪个变量让它变化了,导致了重新加载frmSplash.或许重新启动的线程在郑总的计算机环境下面认为是另外一个，而我们的计算机环境认为是同一个
        private void CheckFormIsOpen()
        {
            foreach (Form frm in Application.OpenForms)
            {
                if (frm is MainInfo01)
                {
                    if (this.OwnedForms.Length > 0)
                    {
                        try
                        {
                            int i = 0;
                            while (i <= this.OwnedForms.Length)
                            {
                                this.OwnedForms[i].Close();
                                i++;
                            }
                        }
                        catch
                        { }
                    }
                }
            }
        }

        //设置背景图片, False-鼠标离开, True-鼠标进入
        private void pbNavigatorMouseEnterLeave(object sender, Boolean IsLeave)
        {
            if (FuncName == ((Control)sender).Name)
            {
                return;
            }
            // IsLeave: False-Mouse Leave, True-Mouse Enter
            string name = ((Control)sender).Name;
            switch (name)
            {
                case "pbFile2Word": //文件转Word
                    if (IsLeave)
                    {
                        this.pbFile2Word.BackgroundImage = Image.FromFile(spath + "pbFile2Word02.png");
                    }
                    else
                    {
                        this.pbFile2Word.BackgroundImage = Image.FromFile(spath + "pbFile2Word01.png");
                    }
                    break;
                case "pbFile2Excel": //文件转Excel
                    if (IsLeave)
                    {
                        this.pbFile2Excel.BackgroundImage = Image.FromFile(spath + "pbFile2Excel02.png");
                    }
                    else
                    {
                        this.pbFile2Excel.BackgroundImage = Image.FromFile(spath + "pbFile2Excel01.png");
                    }
                    break;
                case "pbFile2PPT": //文件转PPT
                    if (IsLeave)
                    {
                        this.pbFile2PPT.BackgroundImage = Image.FromFile(spath + "pbFile2PPT02.png");
                    }
                    else
                    {
                        this.pbFile2PPT.BackgroundImage = Image.FromFile(spath + "pbFile2PPT01.png");
                    }
                    break;
                case "pbFile2HTML": //文件转HTML
                    if (IsLeave)
                    {
                        this.pbFile2HTML.BackgroundImage = Image.FromFile(spath + "pbFile2HTML02.png");
                    }
                    else
                    {
                        this.pbFile2HTML.BackgroundImage = Image.FromFile(spath + "pbFile2HTML01.png");
                    }
                    break;
                case "pbFile2TXT": //文件转TXT
                    if (IsLeave)
                    {
                        this.pbFile2TXT.BackgroundImage = Image.FromFile(spath + "pbFile2TXT02.png");
                    }
                    else
                    {
                        this.pbFile2TXT.BackgroundImage = Image.FromFile(spath + "pbFile2TXT01.png");
                    }
                    break;
                case "pbFile2IMG": //文件转图片
                    if (IsLeave)
                    {
                        this.pbFile2IMG.BackgroundImage = Image.FromFile(spath + "pbFile2IMG02.png");
                    }
                    else
                    {
                        this.pbFile2IMG.BackgroundImage = Image.FromFile(spath + "pbFile2IMG01.png");
                    }
                    break;
                case "pbIMG2PDF": //图片转PDF
                    if (IsLeave)
                    {
                        this.pbIMG2PDF.BackgroundImage = Image.FromFile(spath + "pbIMG2PDF02.png");
                    }
                    else
                    {
                        this.pbIMG2PDF.BackgroundImage = Image.FromFile(spath + "pbIMG2PDF01.png");
                    }
                    break;
                case "pbDoc2PDF": //Word转PDF
                    if (IsLeave)
                    {
                        this.pbDoc2PDF.BackgroundImage = Image.FromFile(spath + "pbDoc2PDF02.png");
                    }
                    else
                    {
                        this.pbDoc2PDF.BackgroundImage = Image.FromFile(spath + "pbDoc2PDF01.png");
                    }
                    break;
                case "pbExcel2PDF": //Excel转PDF
                    if (IsLeave)
                    {
                        this.pbExcel2PDF.BackgroundImage = Image.FromFile(spath + "pbExcel2PDF02.png");
                    }
                    else
                    {
                        this.pbExcel2PDF.BackgroundImage = Image.FromFile(spath + "pbExcel2PDF01.png");
                    }
                    break;
                case "pbPPT2PDF": //PPT转PDF
                    if (IsLeave)
                    {
                        this.pbPPT2PDF.BackgroundImage = Image.FromFile(spath + "pbPPT2PDF02.png");
                    }
                    else
                    {
                        this.pbPPT2PDF.BackgroundImage = Image.FromFile(spath + "pbPPT2PDF01.png");
                    }
                    break;
                case "pbPdfSplit": //PDF分割
                    if (IsLeave)
                    {
                        this.pbPdfSplit.BackgroundImage = Image.FromFile(spath + "pbPdfSplit02.png");
                    }
                    else
                    {
                        this.pbPdfSplit.BackgroundImage = Image.FromFile(spath + "pbPdfSplit01.png");
                    }
                    break;
                case "pbPDFMerge": //PDF合并
                    if (IsLeave)
                    {
                        this.pbPDFMerge.BackgroundImage = Image.FromFile(spath + "pbPDFMerge02.png");
                    }
                    else
                    {
                        this.pbPDFMerge.BackgroundImage = Image.FromFile(spath + "pbPDFMerge01.png");
                    }
                    break;
                case "pbPDFCompress": //PDF压缩
                    if (IsLeave)
                    {
                        this.pbPDFCompress.BackgroundImage = Image.FromFile(spath + "pbPDFCompress02.png");
                    }
                    else
                    {
                        this.pbPDFCompress.BackgroundImage = Image.FromFile(spath + "pbPDFCompress01.png");
                    }
                    break;
                case "pbPDFDecode": //PDF密码解除
                    if (IsLeave)
                    {
                        this.pbPDFDecode.BackgroundImage = Image.FromFile(spath + "pbPDFDecode02.png");
                    }
                    else
                    {
                        this.pbPDFDecode.BackgroundImage = Image.FromFile(spath + "pbPDFDecode01.png");
                    }
                    break;
                case "pbPDFGetImg": //PDF图片获取
                    if (IsLeave)
                    {
                        this.pbPDFGetImg.BackgroundImage = Image.FromFile(spath + "pbPDFGetImg02.png");
                    }
                    else
                    {
                        this.pbPDFGetImg.BackgroundImage = Image.FromFile(spath + "pbPDFGetImg01.png");
                    }
                    break;
                default:
                    break;
            }
        }

        public string FuncName = "pbFile2Word";
        private void pbFile2Word_MouseClick(object sender, MouseEventArgs e)
        {
            if (this.btnStart.Enabled == false && pbIndexBackGround.Visible == false)
            {
                return;
            }

            FuncName = ((Control)sender).Name;
            foreach (Control c in plLeft.Controls)
            {
                if (c.Name != FuncName && c.Name != "pb_01" && c.Name != "pb_02" && c.Name != "pb_03" && c.Name != "pbLeft_Bottom")
                    c.BackgroundImage = Image.FromFile(spath + c.Name + "01.png");
            }

            switch (FuncName)
            {
                case "pbFile2Word": //文件转Word
                    this.pbFile2Word.BackgroundImage = Image.FromFile(spath + "pbFile2Word03.png");
                    format = Convert01.FORMAT.File2WORD;
                    showUserDefine(0); //是否显示左侧按钮的额外自定义内容
                    break;
                case "pbFile2Excel": //文件转Excel
                    this.pbFile2Excel.BackgroundImage = Image.FromFile(spath + "pbFile2Excel03.png");
                    format = Convert01.FORMAT.File2EXCEL;
                    showUserDefine(0); //是否显示左侧按钮的额外自定义内容
                    break;
                case "pbFile2PPT": //文件转PPT
                    this.pbFile2PPT.BackgroundImage = Image.FromFile(spath + "pbFile2PPT03.png");
                    format = Convert01.FORMAT.File2PPT;
                    showUserDefine(1); //是否显示左侧按钮的额外自定义内容
                    break;
                case "pbFile2HTML": //文件转HTML
                    this.pbFile2HTML.BackgroundImage = Image.FromFile(spath + "pbFile2HTML03.png");
                    format = Convert01.FORMAT.File2HTML;
                    showUserDefine(0); //是否显示左侧按钮的额外自定义内容
                    break;
                case "pbFile2TXT": //文件转TXT
                    this.pbFile2TXT.BackgroundImage = Image.FromFile(spath + "pbFile2TXT03.png");
                    format = Convert01.FORMAT.File2TXT;
                    showUserDefine(0); //是否显示左侧按钮的额外自定义内容
                    break;
                case "pbFile2IMG": //文件转图片
                    //Win8和Win10操作系统需要安装一个图片浏览器,否则会转换结束会打不开
                    this.pbFile2IMG.BackgroundImage = Image.FromFile(spath + "pbFile2IMG03.png");
                    format = Convert01.FORMAT.File2IMG;
                    showUserDefine(0); //是否显示左侧按钮的额外自定义内容                    
                    break;
                case "pbIMG2PDF": //图片转PDF
                    this.pbIMG2PDF.BackgroundImage = Image.FromFile(spath + "pbIMG2PDF03.png");
                    format = Convert01.FORMAT.IMG2PDF;
                    showUserDefine(2); //是否显示左侧按钮的额外自定义内容
                    break;
                case "pbDoc2PDF": //Word转PDF
                    this.pbDoc2PDF.BackgroundImage = Image.FromFile(spath + "pbDoc2PDF03.png");
                    format = Convert01.FORMAT.DOC2PDF;
                    showUserDefine(0); //是否显示左侧按钮的额外自定义内容
                    break;
                case "pbExcel2PDF": //Excel转PDF
                    this.pbExcel2PDF.BackgroundImage = Image.FromFile(spath + "pbExcel2PDF03.png");
                    format = Convert01.FORMAT.Excel2PDF;
                    showUserDefine(0); //是否显示左侧按钮的额外自定义内容
                    break;
                case "pbPPT2PDF": //PPT转PDF
                    this.pbPPT2PDF.BackgroundImage = Image.FromFile(spath + "pbPPT2PDF03.png");
                    format = Convert01.FORMAT.PPT2PDF;
                    showUserDefine(0); //是否显示左侧按钮的额外自定义内容

                    break;
                case "pbPdfSplit": //PDF分割
                    //按指定页面分割(针对单个页面有效,其它的是针对所有页面的)
                    if (Convert01.g_splitsettings.split_page_mode == Convert01.tsplitpagemode.spCustomize)
                    {
                        Convert01.g_splitsettings.split_page_mode = Convert01.tsplitpagemode.spEveryPage; // 默认"文件分割"鼠标右键的RadioButton默认为第1个
                    }

                    this.pbPdfSplit.BackgroundImage = Image.FromFile(spath + "pbPdfSplit03.png");
                    format = Convert01.FORMAT.PDFSplit;
                    showUserDefine(0); //是否显示左侧按钮的额外自定义内容
                    ////ShowDefaultPageBox();
                    break;
                case "pbPDFMerge": //PDF合并,免费版只合并前5个文件(而不是前5页)
                    this.pbPDFMerge.BackgroundImage = Image.FromFile(spath + "pbPDFMerge03.png");
                    format = Convert01.FORMAT.PDFMerge;
                    showUserDefine(3); //是否显示左侧按钮的额外自定义内容
                    break;
                case "pbPDFCompress": //PDF压缩
                    this.pbPDFCompress.BackgroundImage = Image.FromFile(spath + "pbPDFCompress03.png");
                    format = Convert01.FORMAT.PDFCompress;
                    showUserDefine(0); //是否显示左侧按钮的额外自定义内容
                    break;
                case "pbPDFDecode": //PDF密码解除
                    this.pbPDFDecode.BackgroundImage = Image.FromFile(spath + "pbPDFDecode03.png");
                    format = Convert01.FORMAT.PDFDecode;
                    showUserDefine(0); //是否显示左侧按钮的额外自定义内容
                    break;
                case "pbPDFGetImg": //PDF图片获取
                    this.pbPDFGetImg.BackgroundImage = Image.FromFile(spath + "pbPDFGetImg03.png");
                    format = Convert01.FORMAT.PDFGetImg;
                    showUserDefine(0); //是否显示左侧按钮的额外自定义内容
                    break;
                default:
                    break;
            }
        }

        //右键菜单哪些有效
        private void rightMenuEnable()
        {
            if (format == Convert01.FORMAT.File2IMG //文件转图片
                || format == Convert01.FORMAT.PDFGetImg //文件获取图片
               )
            {
                tsmiPicFormat.Enabled = true; //输出设置方案(仅对“文件转图片”有效)                
            }
            else
            {
                tsmiPicFormat.Enabled = false; //输出设置方案(仅对“文件转图片”有效)
            }

            if (format == Convert01.FORMAT.PDFCompress //PDF压缩
                || format == Convert01.FORMAT.PDFDecode //PDF密码解除
                || format == Convert01.FORMAT.IMG2PDF //图片转PDF
                )
            {
                tsmiPageSet.Enabled = false; //页数提取方案
            }
            else
            {
                tsmiPageSet.Enabled = true; //页数提取方案
            }

            if (btnStart.Enabled) // 开始转换
            {
                tsmiStart.Enabled = true;
            }
            else
            {
                tsmiStart.Enabled = false;
            }
            if (btnStop.Enabled) // 停止转换 
            {
                tsmiStop.Enabled = true;
            }
            else
            {
                tsmiStop.Enabled = false;
            }
            if (btnClear.Enabled) // 清空列表
            {
                tsmiClear.Enabled = true;
            }
            else
            {
                tsmiClear.Enabled = false;
            }
        }

        private void plNavigator_MouseEnter(object sender, EventArgs e)
        {
            pbNavigatorMouseEnterLeave(sender, true); //设置背景图片, False-鼠标离开, True-鼠标进入
        }

        private void plNavigator_MouseLeave(object sender, EventArgs e)
        {
            pbNavigatorMouseEnterLeave(sender, false); //设置背景图片, False-鼠标离开, True-鼠标进入
        }

        private void MainInfo01_Activated(object sender, EventArgs e)
        {
            CheckFormIsOpen();
            Thread trd = new Thread(onlyRunOnce);//KongMengyuan增加,2015-11-09,依据郑侃炜新的要求所作,如果网址出错会死在这里,所以要
            trd.IsBackground = true;
            trd.Start();
        }
        private void onlyRunOnce()
        {
            if (Program.MainInfo_LoadFinish == "Finish")
            {
                return;
            }

            Program.MainInfo_LoadFinish = "Finish";

            pbFile2Word_MouseClick(this.pbFile2Word, null); //默认点击左侧第一个按钮“PDF转Word”

            //安装之前再去掉注释,便于方便程序员调试程序,KongMengyuan(全文搜索这句话,在发布之前全部去掉注释)
            //英文版免费(两个条件同时满足则免费：1、安装操作系统时是英文版 2、Config.ini里面的language=en),免费版条件
            //if (Program.iniLanguage == "en")
            if (Program.iniLanguage == "en" && (System.Globalization.CultureInfo.InstalledUICulture.Name).Substring(0, 2).ToLower() == "en")
            {
                isReg = true;
                this.plTop.BackgroundImage = Image.FromFile(spath + "header_02.png");
                this.plTop.Tag = "Genuine Version"; //正式版本
            }
            else
            {
                if (new reg().Is_Reg())
                {
                    isReg = true;
                    if (string.Equals(this.plTop.Tag, "Genuine Version") == false)
                    {
                        this.plTop.BackgroundImage = Image.FromFile(spath + "header_02.png");
                        this.plTop.Tag = "Genuine Version"; //正式版本
                    }
                }
                else
                {
                    isReg = false;
                    if (string.Equals(this.plTop.Tag, "Free Trial") == false)
                    {
                        this.plTop.BackgroundImage = Image.FromFile(spath + "header_01.png");
                        this.plTop.Tag = "Free Trial"; //免费试用版本
                    }
                }
            }

            this.uctblPage.BackGroundText = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_24"); // "请输入,如2-17";

            //读取已保存的文件输出路径(目录路径)
            string targetDir = ini.read_ini("TargetDir");
            if (targetDir != string.Empty)
            {
                outFolder = targetDir;
                //测试一下目录是否存在,如果不存在则建立一个,如果建立不成功,就重新指向桌面(Win8和Win10都要注意测这个地方)
                try
                {
                    Directory.CreateDirectory(outFolder);
                }
                catch
                {
                    outFolder = System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\";//桌面路径
                }
            }
            else
            {
                //outFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Favorites); //收藏夹路径
                outFolder = System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\";//桌面路径
            }

            //输出路径默认选择
            string out_ = ini.read_ini("Out");
            if (string.IsNullOrEmpty(out_) || out_ == "1")
            {
                outFolderSelect = "rdoOldPath";
                this.pb_Folder.BackgroundImage = Image.FromFile(spath + "pb_Folder02.png");
            }
            else
            {
                //this.btnBrowse.IsEnable = true;
                //this.btnBrowse.ButtonBackIMG = Properties.Resources.look;
                outFolderSelect = "rdoNewPath";
                this.pb_Folder.BackgroundImage = Image.FromFile(spath + "pb_Folder01.png");
            }

            //修改版本号的注意事项：
            //1、全局用的版本号不要放在frmSplash.cs里面,因为MainInfo是使用线程启动的,所以要放在这里才可以
            //2、此处写数字的版本号,因为以后自动更新时不会更改config.ini这个文件,里面的版本号可能永远不会变,所以版本号必须写在程序里面,同时在MainInfo01.onlyRunOnce里面重新赋值
            //3、查找整个解决方案的“PDF Convert, Version=6.3.0.0”,全部修改为对应的版本就可以了(每个窗体有对应的resx,比如RegDlg.cs有3个对应的文件: Redlg.resx,RegDlg.en.resx,RegDlg.zh-CN.resx)
            //  如何向winform里面添加额外的resx文件：比如frmAutoUpdate.cs需要添加frmAutoUpdate.en.resx和frmAutoUpdate.zh-CN.resx
            //                     鼠标右击顶部的项目“PDF Convert”-添加-新建项--资源文件--输入和窗体相同的名称,再加一个语言的标志符,比如加en或zh-CN
            //4、同时修改AssemblyInfo.cs里面的“[assembly: AssemblyVersion("6.3")]”为对应的版本号
            Program.httpVersion = "6.3";// Version.version;
            //5、把版本号更新进入Config.ini里面
            ini.write_ini("Version", Program.httpVersion);
            //在点击右上角的“购买”时打开页面会卡一下,而点击右下角的"在线教程"就不卡(放在这里加载目的是只调用一次CPU序列号,这样就只检查一次硬件,以后直接调用,系统会不卡)
            Program.httpMachineCode = new reg().get_machine_code();
            //MessageBox.Show(Program.httpMachineCode);
            Program.httpRegCode = new reg().get_reg_code();
            Program.encodingCode = new reg().get_machine_code();
            Program.cpu_hardDisk = ""; //为了加快显示主界面,取值放在了MainInfo01.cs里面了(以线程方式执行,不影响客户操作)
            Program.cpu_hardDisk_Hash = "";//为了加快显示主界面,取值放在了MainInfo01.cs里面了(以线程方式执行,不影响客户操作)
            Program.setupName = "";//为了加快显示主界面,取值放在了MainInfo01.cs里面了(以线程方式执行,不影响客户操作)

            //KongMengyuan,2015-11-01,在WinXP下面不运行这三句话,加上它就不运行,但是Win7和Win8可以运行
            //ChangeWindowMessageFilter函数导致程序在xp中不能启动
            //windows xp版本的user32.dll没有ChangeWindowMessageFilter，windows vista以后的有。ChangeWindowMessageFilter的功能应该很少用到，考虑弃用这个api
            //弃用ChangeWindowMessageFilter，虽然程序以管理员身份运行时无法实现拖拽操作，但也只有这样了
            //Environment.OSVersion.ToString();，获得系统的版本号，NT 5.1是xp，NT 6.1是win7
            //VS2013的旗舰版在Win64位上安装需要额外安装支持,比较麻烦,只需要安装高级版或专业版就可以了,以后我们只要安装VS2013专业版就够用了
            //以后就用VS2015了,因为操作系统在更新,如果不使用新版本的话,许多微软的dll对应不上
            //以下三句话是KongMengyuan注释的
            //ChangeWindowMessageFilter(WM_DROPFILES, MSGFLT_ADD);m
            //ChangeWindowMessageFilter(WM_COPYDATA, MSGFLT_ADD);
            //ChangeWindowMessageFilter(0x0049, MSGFLT_ADD);

            if (Program.osVersion != "WinXP")
            {
                ChangeWindowMessageFilter(WM_DROPFILES, MSGFLT_ADD);
                ChangeWindowMessageFilter(WM_COPYDATA, MSGFLT_ADD);
                ChangeWindowMessageFilter(0x0049, MSGFLT_ADD);
            }

            this.pbFile2Word.Tag = "EnabledTrue";

            ManagementClass c = new ManagementClass(
                    new ManagementPath("Win32_Processor"));
            // Get the properties in the class
            ManagementObjectCollection moc = c.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                PropertyDataCollection properties = mo.Properties;
                //cpuNumber = System.Convert.ToInt32(properties["NumberOfCores"].Value);
                //cpuLogicalNumber = System.Convert.ToInt32(properties["NumberOfLogicalProcessors"].Value);
                //2015-12-16,KongMengyuan修改,查看是几核的CPU,一般都是4核的,不过虚拟机要注意只有1核,有时WinXP 64位的会出错,这点要测试
                try
                {
                    cpuNumber = System.Convert.ToInt32(properties["NumberOfCores"].Value);  //WinXP 64位此处代码会出错,如出错时使用默认值即可
                }
                catch
                { }
                try
                {
                    cpuLogicalNumber = System.Convert.ToInt32(properties["NumberOfLogicalProcessors"].Value);  //WinXP 64位此处代码会出错,如出错时使用默认值即可
                }
                catch
                { }
                cpuNumber = cpuNumber >= cpuLogicalNumber ? cpuNumber : cpuLogicalNumber; //2015-12-16,KongMengyuan注释,查看是几核的CPU,一般都是4核的,不过虚拟机要注意只有1核,有时WinXP 64位的会出错,这点要测试
                if (cpuNumber > 1)
                {
                    cpuNumber = cpuNumber - 1;
                }
            }

            thread = new Thread[cpuNumber];
            for (int i = 0; i < thread.Length; i++)
            {
                thread[i] = new Thread(new ParameterizedThreadStart(WorkThread));
                thread[i].IsBackground = true;
                thread[i].Start(i);
            }

            //图片是否合并
            string isMerger = ini.read_ini("isMerger");
            this.rbIsMerger1.Checked = true;

            //使用消息队列,把结果传给控件(因为控件赋值不可以放在线程里面),KongMengYuan,2016-01-06
            object[] pList = { this, System.EventArgs.Empty };
            this.tbBrw.BeginInvoke(new System.EventHandler(UpdateUI), pList);//这两行是切换线程的,这样就可以给当前线程的控件传值了

            //安装之前再去掉注释,便于方便程序员调试程序,KongMengyuan(全文搜索这句话,在发布之前全部去掉注释)
            //自动更新,自动下载,在发布之前再将注释去掉,否则不停的上传统计(而实际是没有意义的数据),影响正常统计数据.
            Thread trd = new Thread(statisticsPost);//KongMengyuan增加,2015-11-09,依据郑侃炜新的要求所作,如果网址出错会死在这里,所以要
            trd.Start();
        }

        #region 自动上传至PHP网址,安装包信息,当前程序信息
        private void statisticsPost()
        {
            //KongMengyuan,2015-11-09,郑侃炜提出：注释掉统计相关代码，原来写的代码有点差，而且影响到软件效率，因此建议先注释掉。
            //补充一个最简单的统计，统计如下内容：
            //软件安装包文件名；
            //每次软件启动时开启一个线程，向服务器POST一个包。
            //Setup_Filename = String.SplitPath(SessionVar.Expand("%SourceFilename%")).Filename ..  String.SplitPath(SessionVar.Expand("%SourceFilename%")).Extension; --安装包文件名和后缀
            //INIFile.SetValue(SessionVar.Expand("%AppFolder%\\set.ini"), "Install", "SetupName",Setup_Filename );
            //POST地址： http://tj.sjhfrj.com/tj/ver1/ UTF-8编码无BOM        
            //相关参数：
            //Softname：软件名，“迅捷PDF转换器”
            //Version：版本号，
            //SetupName：安装名文件名
            //MachineID：机器码，可以编码为“mac地址|cpu号”，或是hash后上报
            //Action：行为，一般为”start“即可

            Program.cpu_hardDisk = macAddress() + "|" + cpuSeries();
            Program.cpu_hardDisk_Hash = Program.cpu_hardDisk.GetHashCode().ToString(); //得到Hash值,hash方法是一对多的
            Program.setupName = setupNameGet();//得到安装包的文件名
            //MessageBox.Show(setupName);
            WebClient w = new WebClient();
            System.Collections.Specialized.NameValueCollection VarPost = new System.Collections.Specialized.NameValueCollection();
            VarPost.Add("Softname", Program.appProgName);
            VarPost.Add("Version", Program.httpVersion);
            VarPost.Add("SetupName", Program.setupName); // 安装名文件名,从Set.ini里面获取,在安装包Setup Factory里面自动生成Set.ini
            VarPost.Add("MachineID", Program.cpu_hardDisk); //机器码，可以编码为“mac地址|cpu号”，或是hash后上报
            //VarPost.Add("machineID", cpu_hardDisk_Hash); //机器码，可以编码为“mac地址|cpu号”，或是hash后上报
            VarPost.Add("Action", "start");
            try
            {
                w.UploadValues("http://tj.sjhfrj.com/tj/ver1/", "POST", VarPost); //PHP统计专用,注意不要和自动更新混淆掉
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            //自动更新(后台查找PHP网址,核对Ver,发现有返回信息立刻更新),因为需要用到上面的Program.setupName等公共变量,所以此处程序放在这里
            Thread trd_php = new Thread(DownloadFile_Quiet);
            trd_php.Start();
        }

        private string macAddress()
        {
            //KongMengyuan,2015-11-09,没有使用此原有程序内容,而是自己重新编写的,参考代码：F:\Work_Hudun\SVN\BaseCS_FrameWork20\BaseCS_Test20\Main.cs

            //得到本地IP地址(第1个)：
            //因为在win7中已经启用了IPv6,所以 ipe.AddressList[0]返回的是IPv6格式的地址,应该使用 ipe.AddressList[1] 返回 IPv4 格式地址如果有多个网卡,可以设断点监视 ipa的值,看是否是需要的地址
            //System.Net.IPHostEntry myEntry = System.Net.Dns.GetHostEntry; //(System.Net.Dns.GetHostName());
            //string ipAddress = myEntry.AddressList[0].ToString();
            string HostName = System.Net.Dns.GetHostName();
            System.Net.IPHostEntry IpEntry = System.Net.Dns.GetHostEntry(HostName); //得到主机IP

            //Win7可能会有多个IP地址
            //string strIPAddr = IpEntry.AddressList[3].ToString();
            int i = 0;
            int j = IpEntry.AddressList.GetUpperBound(0);
            string tmpIP = "";
            while (i <= j)
            {
                tmpIP = IpEntry.AddressList[i].ToString();
                double num = 0;
                if (System.Double.TryParse(tmpIP.Replace(".", ""), out num))
                {
                    //是一个数字
                    break;
                }
                else
                {
                    //不是一个数字
                }
                i++;
            }

            //得到指定IP地址的MAC地址
            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
            string remoteHostNameAddress = tmpIP.Trim();            //构造Ping实例
            Ping pingSender = new Ping();            //Ping选项设置
            PingOptions options = new PingOptions();
            options.DontFragment = true;            //测试数据
            string data = "test data abcabc";
            byte[] buffer = Encoding.ASCII.GetBytes(data);            //设置超时时间
            int timeout = 120;            //调用同步send方法发送数据，将返回结果保存至PingReply实例
            PingReply reply = pingSender.Send(remoteHostNameAddress, timeout, buffer, options);
            string macFirst = ""; //只记录第一个MAC地址
            if (reply.Status == IPStatus.Success)
            {
                foreach (NetworkInterface adapter in adapters)
                {
                    if (macFirst == "")
                    {
                        macFirst = adapter.GetPhysicalAddress().ToString();
                    }
                }
            }
            return macFirst;
        }

        private string cpuSeries()
        {
            //using System.Management;//项目下边的 引用--添加引用--框架--System.Management
            //using System.Management.Instrumentation;
            //获取cpu和硬盘的序列号，得到机器码
            string cpuInfo = "";//cpu序列号
            try
            {
                ManagementClass cimobject = new ManagementClass("Win32_Processor");
                ManagementObjectCollection moc = cimobject.GetInstances();//KongMengyuan,2015-11-10,后面的GetInstances有时会弹出System.NullReferenception: 未将对象引用设置到对象的实例中
                string cpuFirst = ""; //第一个CPU序列号
                foreach (ManagementObject mo in moc)
                {
                    cpuInfo = mo.Properties["ProcessorId"].Value.ToString();
                    //MessageBox.Show("cpu序列号：" + cpuInfo.ToString());
                    if (cpuFirst == "")
                    {
                        cpuFirst = cpuInfo.ToString();
                    }
                }
            }
            catch
            {
                return "unknown";

            }

            return cpuInfo.ToString();
        }

        [DllImport("kernel32.dll")]
        public extern static int GetPrivateProfileString(string segName, string keyName, string sDefault, StringBuilder buffer, int nSize, string fileName);

        private string setupNameGet()
        {
            //得到安装包的文件名
            //StringBuilder temp = new StringBuilder();
            //return GetPrivateProfileString("Intall", "SetupName", "pdf_setup_6.0_201511091426.exe", temp, 255, "Set.ini").ToString();

            IniFile ini = new IniFile(Application.StartupPath + "\\Set.ini");
            //判断返回值，避免第一次运行时为空出错
            if ((ini.IniReadValue("Install", "SetupName") != ""))
            {
                return ini.IniReadValue("Install", "SetupName");
            }
            return "没有发现Set.ini";
        }
        #endregion

        #region 自动更新,自动下载,自动下载
        //修改文件属性为隐藏属性
        public void SetFileAttributes(string aFilePath)
        {
            //if (File.GetAttributes(aFilePath).ToString().IndexOf("ReadOnly") != -1) //只读属性
            if ((File.GetAttributes(aFilePath).ToString().IndexOf("Hidden") == -1)) //不是隐藏属性
            {
                File.SetAttributes(aFilePath, FileAttributes.Hidden);//修改为隐藏属性
            }
        }

        //后台暗中下载最新版本(静默下载)
        private void DownloadFile_Quiet()
        {
            //向http网址的php文件Post,返回值获取新版本的网址,将下载文件修改为隐藏,然后再直接提示是否更新版本
            string URL = string.Empty;

            WebClient w = new WebClient();
            System.Collections.Specialized.NameValueCollection VarPost = new System.Collections.Specialized.NameValueCollection();
            VarPost.Add("Softname", Program.appProgName);
            VarPost.Add("Version", Program.httpVersion);
            VarPost.Add("SetupName", Program.setupName); // 安装名文件名,从Set.ini里面获取,在安装包Setup Factory里面自动生成Set.ini
            VarPost.Add("MachineID", Program.cpu_hardDisk); //机器码，可以编码为“mac地址|cpu号”，或是hash后上报
            //VarPost.Add("machineID", cpu_hardDisk_Hash); //机器码，可以编码为“mac地址|cpu号”，或是hash后上报
            VarPost.Add("Action", "start");
            byte[] byRemoteInfo = null;
            try
            {
                byRemoteInfo = w.UploadValues("http://tj.sjhfrj.com/update/ver1/", "POST", VarPost); //在网页服务器的index.php端判断,如果传入的Version小于服务器定义的Version则返回字符串,否则返回空字符串(系统便不会再更新)
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            //以字符串方式显示
            //string sRemoteInfo = System.Text.Encoding.Default.GetString(byRemoteInfo);//输出为 {"newVersion":"6.1","url":"http:\/\/download.xjpdf.com\/pdf2word.exe","optional":true,"updateInfo":"\u6700\u65b0\u7248\uff1a6.1\r\n\u4fee\u590d\u4ee5\u4e0b\u95ee\u9898\uff1a(1)xxxxxx\r\n(2)xxxxx\r\n(3)xxxxx"}
            //Console.WriteLine(sRemoteInfo);
            //以struct方式显示,Byte转Struct(此处编码未成功,以后看有没有解决的办法)
            //phpReturnStructure prs = new phpReturnStructure();
            //object prs = BytesToStuct(byRemoteInfo, byRemoteInfo.GetType());
            //Console.WriteLine("新版本为{0} 更新信息为{1}", prs.newVersion, prs.updateInfo);
            string sRemoteInfo = "";
            try
            {
                sRemoteInfo = System.Text.Encoding.Default.GetString(byRemoteInfo);//输出为 {"newVersion":"6.1","url":"http:\/\/download.xjpdf.com\/pdf2word.exe","optional":true,"updateInfo":"\u6700\u65b0\u7248\uff1a6.1\r\n\u4fee\u590d\u4ee5\u4e0b\u95ee\u9898\uff1a(1)xxxxxx\r\n(2)xxxxx\r\n(3)xxxxx"}
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            //string sInfo = sRemoteInfo.Replace("\\", "");//代替“\”为“”(因为传过来的字符串全部应该是/,网址方式的字符格式)

            string newVersion = string.Empty;//最新版本
            bool optional = false; //是否自动更新
            if (sRemoteInfo != "")
            {
                //取出下载的安装包http地址
                ////右截取：str.Substring(str.Length-i,i) 返回，返回右边的i个字符
                //string downHttp = sInfo.Substring(sInfo.IndexOf("http")); //右截取
                ////左截取：str.Substring(0,i) 返回，返回左边的i个字符
                //downHttp = downHttp.Substring(0, downHttp.IndexOf('"')); //左截取
                ////找到http
                //URL = downHttp;

                newVersion = remoteInfoSplit(sRemoteInfo, "newVersion", "string"); //新版本
                URL = remoteInfoSplit(sRemoteInfo, "url", "string").Replace("\\", ""); //取出下载的安装包http地址
                optional = System.Convert.ToBoolean(remoteInfoSplit(sRemoteInfo, "optional", "bool")); //是否自动更新,optional为True时,可选更新(提示按钮有2个“确定、取消”)；为False时,强制更新(提示按钮只有一个“确定”).bool型不是字符串,所以没有双引号
                string updateInfo = DeUnicode(remoteInfoSplit(sRemoteInfo, "updateInfo", "string")).Replace(@"\r\n", "\r\n"); //后面的中文字符.把字符串中的\r\n重新代替一遍,否则可能不认为是回车符.使用\r\n不换行解决方法.\r\n是windows下的换行符号,\n是linux下的换行符
            }
            else
            {
                return;
            }

            //if (!Directory.Exists(d.ToString())) //这种写法在WinXP和纯净版Win7也不识别(但是雨林木风的Ghost就可以识别这种写法)
            //检查路径是否存在,可能Win10等系统检测不出来,这一点要注意,所以直接创建
            DirectoryInfo directoryInfo = new DirectoryInfo(Application.StartupPath + @"\Update");
            try
            {
                directoryInfo.Create();
            }
            catch
            { }

            //string filename = Application.StartupPath + @"\Update\PDF_Update" + DateTime.Now.ToString("yyyy-MM-dd") + ".exe";
            string filename = Application.StartupPath + @"\Update\PDF_Update.exe";
            float percent = 0;
            try
            {
                System.Net.HttpWebRequest Myrq = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(URL);
                System.Net.HttpWebResponse myrp = (System.Net.HttpWebResponse)Myrq.GetResponse();
                long totalBytes = myrp.ContentLength;
                System.IO.Stream st = myrp.GetResponseStream();
                System.IO.Stream so = new System.IO.FileStream(filename, System.IO.FileMode.Create);
                long totalDownloadedByte = 0;
                byte[] by = new byte[1024];
                int osize = st.Read(by, 0, (int)by.Length);
                while (osize > 0)
                {
                    totalDownloadedByte = osize + totalDownloadedByte;
                    System.Windows.Forms.Application.DoEvents();
                    so.Write(by, 0, osize);
                    osize = st.Read(by, 0, (int)by.Length);

                    percent = (float)totalDownloadedByte / (float)totalBytes * 100;
                    //label1.Text = "当前补丁下载进度" + percent.ToString() + "%";
                    System.Windows.Forms.Application.DoEvents(); //必须加注这句代码，否则label1将因为循环执行太快而来不及显示信息
                }
                so.Close();
                st.Close();
                //文件下载完毕,将属性改成隐藏
                SetFileAttributes(filename);

                //if (MessageBox.Show("最新版本的安装包已经下载完成，是否退出程序立刻更新", "更新提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                frmAutoUpdate01 frm = new frmAutoUpdate01();
                //frm.TopMost = true; //异步的窗口ShowDialog是不会阻塞主窗口的，你只能在主窗口线程创建。使用这个属性就可以把窗体一直放在所有窗体的最前面,这样也可以起到这种效果
                frm.StartPosition = FormStartPosition.CenterParent;//放置于父窗体中间位置
                frm.Location = this.PointToScreen(new Point(400, this.lstFile.Location.Y + 30));
                frm.newVersion = newVersion;
                if (optional == false)
                {
                    frm.cancelShow = false;
                }
                else
                {
                    frm.cancelShow = true;
                }
                DialogResult dr = frm.ShowDialog(); //此处代码的目的是
                if (dr == System.Windows.Forms.DialogResult.OK)
                {
                    System.Diagnostics.Process.Start(filename);
                    Application.Exit();
                    System.Diagnostics.Process.GetCurrentProcess().Kill(); //退出系统
                }
            }
            catch (System.Exception)
            {
                //MessageBox.Show("文件是只读的，请把原文件 " + filename + " 删除");
                //throw;
                try
                {
                    File.Delete(filename);//直接删除其中的文件
                }
                catch
                { }
            }
        }

        //\U编码转换成中文, m=>代码的意思就是找到所有的\uxxxx,转换为short类型，如果不能转就原样输出, DeUnicode("\u6700")返回“最”
        static public string DeUnicode(string s)
        {
            //using System.Text.RegularExpressions; //Regex调用
            //using System.Globalization;//CultureInfo调用
            Regex reUnicode = new Regex(@"\\u([0-9a-fA-F]{4})", RegexOptions.Compiled);
            return reUnicode.Replace(s, m =>
            {
                short c;
                if (short.TryParse(m.Groups[1].Value, System.Globalization.NumberStyles.HexNumber, CultureInfo.InvariantCulture, out c))
                {
                    return "" + (char)c;
                }
                return m.Value;
            });
        }

        //替换byte转struct(不知道如何写它,如果以后知道了就简单了),依据传回的字符串,返回对应的值
        //sRemoteStr: 传回的字符串, sStr: 关键词, sKeywordType: 关键词类型(string,bool), iMoveSpan: 关键词后面的多余字符串长度, sMarkStr_Right:右边标志符
        private string remoteInfoSplit(string sRemoteStr, string sStr, string sKeywordType)
        {
            int iMoveSpan_Left = 0;
            switch (sKeywordType)
            {
                case "string":
                    iMoveSpan_Left = 3; //查找字符串","
                    break;
                default:
                    iMoveSpan_Left = 2; //查找字符串,"
                    break;
            }
            string newStr = sRemoteStr.Substring(sRemoteStr.IndexOf(sStr)); //右截取
            newStr = newStr.Substring(sStr.Length + iMoveSpan_Left, newStr.IndexOf('}') - (sStr.Length + iMoveSpan_Left) - 1); //左截取

            int endIndex = 0;
            switch (sKeywordType)
            {
                case "string":
                    //左截取：str.Substring(0,i) 返回，返回左边的i个字符
                    endIndex = newStr.IndexOf('"'); //右边标志符
                    break;
                default:
                    //左截取：str.Substring(0,i) 返回，返回左边的i个字符
                    endIndex = newStr.IndexOf(','); //右边标志符
                    break;
            }
            if (endIndex > 0)
            {
                newStr = newStr.Substring(0, endIndex); //左截取
            }
            return newStr;
        }
        #endregion

        //切换线程,通过线程消息队列得到数值(因为控件赋值不可以放在线程里面)
        private void UpdateUI(object o, System.EventArgs e)
        {
            Thread.Sleep(1000);
            this.tbBrw.Text = GetShortName(outFolder); //长路径如何变成短路径,2016-02-29,KongMengyuan

            this.toolTip1.SetToolTip(this.tbBrw, this.tbBrw.Text);//增加TextBox的提示
            this.btnAddFiles.Focus();
        }

        //长路径如何变成短路径,2016-02-29,KongMengyuan
        [DllImport("kernel32", EntryPoint = "GetShortPathName", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetShortPathName(string longPath, StringBuilder shortPath, int bufSize);
        //长路径如何变成短路径,2016-02-29,KongMengyuan
        public static string GetShortName(string sLongFileName)
        {
            var buffer = new StringBuilder(259);
            int len = GetShortPathName(sLongFileName, buffer, buffer.Capacity);
            if (len == 0) throw new System.ComponentModel.Win32Exception();
            return buffer.ToString();
        }

        private void btnAddFiles_MouseEnter(object sender, EventArgs e)
        {
            this.btnAddFiles.BackgroundImage = Image.FromFile(spath + "btn_tjwj03.png");
        }

        private void btnAddFiles_MouseLeave(object sender, EventArgs e)
        {
            this.btnAddFiles.BackgroundImage = Image.FromFile(spath + "btn_tjwj01.png");
        }

        private void btnAddFiles_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                uctblPage.Text = string.Empty;//清空定制页面框,避免上次的数据残留带到这里来
                this.btnAddFiles.BackgroundImage = Image.FromFile(spath + "btn_tjwj03.png");

                openFileDialog.FileName = "";
                if (format == Convert01.FORMAT.File2WORD) //文件转Word
                {
                    openFileDialog.Filter = "Any(*.pdf,*.xls,*.xlsx,*.ppt,*.pptx)|*.pdf;*.xls;*.xlsx;*.ppt;*.pptx;";
                }
                else if (format == Convert01.FORMAT.File2EXCEL) //文件转Excel
                {
                    openFileDialog.Filter = "Any(*.pdf,*.ppt,*.pptx,*.doc,*.docx)|*.pdf;*.ppt;*.pptx;*.doc;*.docx";
                }
                else if (format == Convert01.FORMAT.File2PPT) //文件转PPT
                {
                    openFileDialog.Filter = "Any(*.pdf,*.xls,*.xlsx,*.doc,*.docx)|*.pdf;*.xls;*.xlsx;*.doc;*.docx";
                }
                else if (format == Convert01.FORMAT.File2HTML) //文件转HTML
                {
                    openFileDialog.Filter = "Any(*.pdf,*.ppt,*.pptx,*.doc,*.docx,*.xls,*.xlsx)|*.pdf;*.ppt;*.pptx;*.doc;*.docx;*.xls;*.xlsx";
                }
                else if (format == Convert01.FORMAT.File2TXT) //文件转TXT
                {
                    openFileDialog.Filter = "Any(*.pdf,*.ppt,*.pptx,*.doc,*.docx,*.xls,*.xlsx)|*.pdf;*.ppt;*.pptx;*.doc;*.docx;*.xls;*.xlsx";
                }
                else if (format == Convert01.FORMAT.File2IMG) //文件转图片
                {
                    openFileDialog.Filter = "Any(*.pdf,*.ppt,*.pptx,*.doc,*.docx,*.xls,*.xlsx)|*.pdf;*.ppt;*.pptx;*.doc;*.docx;*.xls;*.xlsx";
                }
                else if (format == Convert01.FORMAT.IMG2PDF) //图片转PDF
                {
                    openFileDialog.Filter = "Picture(*.jpg,*.jpeg,*.gif,*.bmp,*.png,*.tif,*.tiff)|*.jpg;*.jpeg;*.gif;*.bmp;*.png;*.tif;*.tiff";
                }
                else if (format == Convert01.FORMAT.DOC2PDF) //Word转PDF
                {
                    openFileDialog.Filter = "Word(*.doc,*.docx)|*.doc;*.docx";
                }
                else if (format == Convert01.FORMAT.Excel2PDF) //Excel转PDF
                {
                    openFileDialog.Filter = "Excel(*.xls,*.xlsx)|*.xls;*.xlsx";
                }
                else if (format == Convert01.FORMAT.PPT2PDF) //PPT转PDF
                {
                    openFileDialog.Filter = "PowerPoint(*.ppt,*.pptx)|*.ppt;*.pptx";
                }
                else if (format == Convert01.FORMAT.PDFSplit) //PDF分割
                {
                    openFileDialog.Filter = "PDF(*.pdf)|*.pdf";
                }
                else if (format == Convert01.FORMAT.PDFMerge) //PDF合并
                {
                    openFileDialog.Filter = "PDF(*.pdf)|*.pdf";
                }
                else if (format == Convert01.FORMAT.PDFCompress) //PDF压缩
                {
                    openFileDialog.Filter = "PDF(*.pdf)|*.pdf";
                }
                else if (format == Convert01.FORMAT.PDFDecode) //PDF密码解除
                {
                    openFileDialog.Filter = "PDF(*.pdf)|*.pdf";
                }
                else if (format == Convert01.FORMAT.PDFGetImg) //PDF图片获取
                {
                    openFileDialog.Filter = "PDF(*.pdf)|*.pdf";
                }
                bool show_flag = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    foreach (string file_name in openFileDialog.FileNames)
                    {
                        if (diclst.ContainsKey(file_name))
                        {
                            string sTip = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_01"); //提示
                            string sOld = string.Empty;
                            if (show_flag && openFileDialog.FileNames.Length == 1)
                            {
                                show_flag = false;
                                sOld = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_02"); //您添加的文件 $S 已存在,我们将会自动过滤这些文件!
                                sOld = Program.strReplace(sOld, "$S", new string[] { Path.GetFileName(file_name) }); //Path需引用 using System.IO;
                                MessageBox.Show(sOld, sTip, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else if (show_flag && openFileDialog.FileNames.Length != 1)
                            {
                                show_flag = false;
                                sOld = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_03"); //您添加的部分文件已存在,我们将会自动过滤这些文件!
                                MessageBox.Show(sOld, sTip, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            continue;

                        }
                        ItemInfomation info = new ItemInfomation(file_name);
                        lstFile.ConversionPageDefaultText = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_04"); //中文: 全部, 英文: All
                        lstFile.AddFile(info);//添加文件进入ListView(填加文件)
                        diclst.Add(file_name, false);

                        backGroundShowHide();//隐藏或显示主界面提示图
                        SetThreeButtonValidate(true);//开始转换,停止转换,清空列表,三个按钮的有效还是失效: true-有效,false-失效
                    }
                }
            }
        }

        #region 是否显示左侧按钮的额外自定义内容
        private void showUserDefine(int itemSelect)
        {
            try
            {
                //itemSelect: 0-普通按钮,1-文件转PPT,2-图片转PDF,3-PDF合并
                //放置“文件转PPT”的"PPT大小设置",“图片转PDF”的"将所有图片合并成一个PDF文件",“PDF合并”百分比,都放在Panel4上(原设计人员定义的,最麻烦的是原设计人员将其Dock定义为Fill,这样即使找到lblPageSize也看不到,已经修改这种作法了)
                switch (itemSelect)
                {
                    case 0: //0-普通按钮,1-文件转PPT,2-图片转PDF,3-PDF合并,目前3不用考虑了
                    case 3: //0-普通按钮,1-文件转PPT,2-图片转PDF,3-PDF合并,目前3不用考虑了
                        this.panel4.Visible = false;
                        lstFile.Location = new Point(0, 3);//new Point(20, 20);
                        lstFile.Height = 576;
                        break;
                    case 1: //0-普通按钮,1-文件转PPT,2-图片转PDF,3-PDF合并,目前3不用考虑了
                        this.panel4.Visible = true;
                        this.plPPT.Visible = true; //显示“文件转PPT”自定义功能
                        this.plMerger.Visible = false; //隐藏“PDF合并”自定义功能
                        this.plPPT_box.Visible = true;
                        this.plPPT.Height = 42;
                        this.plPPT_box.Location = new Point(this.plPPT.Location.X + this.plPPT.Width / 2 - this.plPPT_box.Width / 2, 0);
                        this.panel4.Height = this.plPPT.Height;
                        lstFile.Location = new Point(0, 3);//new Point(20, 20);
                        lstFile.Height = 530;

                        break;
                    case 2: //0-普通按钮,1-文件转PPT,2-图片转PDF,3-PDF合并,目前3不用考虑了
                        this.panel4.Visible = true;
                        this.plPPT.Visible = false; //显示“文件转PPT”自定义功能
                        this.plMerger.Visible = true; //隐藏“PDF合并”自定义功能
                        this.plMerger.Height = 42;
                        this.plMerger_box.Location = new Point(this.plMerger.Location.X + this.plMerger.Width / 2 - this.plMerger_box.Width / 2, 3);
                        this.panel4.Height = this.plMerger.Height;
                        lstFile.Location = new Point(0, 3);//new Point(20, 20);
                        lstFile.Height = 530;
                        break;
                }
            }
            catch
            {
                //线程间操作无效: 从不是创建控件“panel4”的线程访问它。
            }
        }
        #endregion

        public void UpdateProcess(TempClass info)
        {
            if (info.index < 0) return;
            this.syncContext.Post(SetProcess, info);
        }

        public void UpdateProgress(string mess)
        {
            try
            {
                Invoke(new MethodInvoker(delegate()
                {
                    //labProgressUC.ProgressStr = mess;
                }));
            }
            catch (Exception ex)
            {

            }
        }

        private void SetProcess(object obj)
        {
            TempClass tmp = (TempClass)obj;
            if (tmp.index < 0) return;
            this.lstFile.SetStausPV(tmp.index, tmp.cur);
        }

        #region 隐藏或显示主界面提示图
        private void backGroundShowHide()
        {
            if (lstFile.Items.Count > 0)
            {
                if (this.pbIndexBackGround.Visible == true) //隐藏主页面提示图
                {
                    this.pbIndexBackGround.Visible = false;  //隐藏主页面提示图
                    //SetThreeButtonValidate(true);//开始转换,停止转换,清空列表,三个按钮的有效还是失效: true-有效,false-失效
                }
            }
            else
            {
                this.pbIndexBackGround.Visible = true;  //显示主页面提示图
                SetThreeButtonValidate(false);//开始转换,停止转换,清空列表,三个按钮的有效还是失效: true-有效,false-失效
            }
        }
        #endregion
        //开始转换,停止转换,清空列表,三个按钮的有效还是失效: true-有效,false-失效
        public void SetThreeButtonValidate(bool xValidate)
        {
            btnStart.Enabled = xValidate;
            btnStop.Enabled = xValidate;
            btnClear.Enabled = xValidate;
            if (xValidate)
            {
                this.btnStart.BackgroundImage = Image.FromFile(spath + "btn_kszh01.png");//开始转换
                this.btnStop.BackgroundImage = Image.FromFile(spath + "btn_tzzh01.png"); //停止转换
                this.btnClear.BackgroundImage = Image.FromFile(spath + "btn_qklb01.png");//清空列表
            }
            else
            {
                this.btnStart.BackgroundImage = Image.FromFile(spath + "btn_kszh04.png");//开始转换
                this.btnStop.BackgroundImage = Image.FromFile(spath + "btn_tzzh04.png"); //停止转换
                this.btnClear.BackgroundImage = Image.FromFile(spath + "btn_qklb04.png");//清空列表
            }
        }

        private void ShowDefaultPageBox()
        {
            if (format != Convert01.FORMAT.PDFSplit)
            {
                return;
            }
            int index = -1;
            for (int i = 0; i < lstFile.Items.Count; i++)
            {
                ItemInfomation Info = ((ItemInfomation)lstFile.Items[i].Tag);
                if (Info != null && Info.Status == StatusType.Ready)
                {
                    index = i;
                }
            }
            if (index != -1)
            {
                lstFile.Items[index].Selected = true;
                pltext.Location =
                    new Point(lstFile.Items[index].SubItems[3].Bounds.Left + lstFile.Location.X + 2 + 7,
                        lstFile.Items[index].SubItems[3].Bounds.Top + lstFile.Location.Y + 1 + 8);
                this.pltext.Visible = true;
                this.pltext.Width = lstFile.Items[index].SubItems[3].Bounds.Width - 14;
                int height = this.pltext.Height;

                //if (lstFile.Items[index].SubItems[3].Text != rm.GetString("ALL"))
                //{
                //    comboBoxPage.Text = lstFile.Items[index].SubItems[3].Text;
                //}
                //else
                //{
                //    comboBoxPage.Text = string.Empty;
                //}

                m_EditIndex = lstFile.Items[index].Index;
            }
        }

        #region 重画,定制RadioButton按钮(蓝圈红点:外边框为蓝圈,内部为红点)
        //KongMengyuan,2015-11-20,定制按钮RadioButton个性化
        private void radioButton_Paint(object sender, PaintEventArgs e)
        {
            RadioButton rButton = (RadioButton)sender;
            Graphics g = e.Graphics;
            Rectangle radioButtonrect = new Rectangle(0, 0, 12, 12);

            switch (System.Convert.ToInt16(rButton.Font.Size))
            {
                case 9://如果RadioButton的字体为9号字体,则 y 的偏移量为0
                    radioButtonrect = new Rectangle(0, 0, 12, 12);
                    break;
                case 10://如果RadioButton的字体为10号字体,则 y 的偏移量为3,也可为7,这个依据实际情况调整(如果圆点向上就调大数字,反之则调小即可)
                    radioButtonrect = new Rectangle(0, 5, 12, 12);
                    break;
                case 12://如果RadioButton的字体为12号字体,则 y 的偏移量为5
                    radioButtonrect = new Rectangle(0, 7, 12, 12);
                    break;
                case 14://如果RadioButton的字体为12号字体,则 y 的偏移量为5
                    radioButtonrect = new Rectangle(0, 7, 12, 12);
                    break;
            }

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias; //抗锯齿处理

            //圆饼背景
            using (SolidBrush brush = new SolidBrush(Color.White))
            {
                g.FillEllipse(brush, radioButtonrect);
            }

            if (rButton.Checked)
            {
                //内部,红点为红色
                radioButtonrect.Inflate(-2, -2);//矩形内缩2单位
                g.FillEllipse(Brushes.Red, radioButtonrect);
                radioButtonrect.Inflate(2, 2);//还原
            }

            //圆形边边框,外框为蓝色
            using (Pen pen = new Pen(Color.Gray))
            {
                g.DrawEllipse(pen, radioButtonrect);
            }
        }
        #endregion

        private void pb_Folder_MouseMove(object sender, MouseEventArgs e)
        {
            //cursorPosition(); //显示当前坐标位置,在编程时定位控件坐标位置使用,正式使用之前将其注释掉

            //鼠标移动到不同的区域显示不同光标
            int locationFolder1_X_1 = 80; //原始文件夹,X1的位置
            int locationFolder1_X_2 = 150; //原始文件夹,X2的位置
            int locationFolder1_Y_1 = 0; //原始文件夹,Y1的位置
            int locationFolder1_Y_2 = 29; //原始文件夹,Y2的位置
            int locationFolder2_X_1 = 160; //自定义文件夹,X1的位置
            int locationFolder2_X_2 = 230; //自定义文件夹,X2的位置
            int locationFolder2_Y_1 = 0; //自定义文件夹,Y1的位置
            int locationFolder2_Y_2 = 29; //自定义文件夹,Y2的位置
            int locationFolder3_X_1 = 240; //选择文件夹,X1的位置
            int locationFolder3_X_2 = 265; //选择文件夹,X2的位置
            int locationFolder3_Y_1 = 0; //选择文件夹,Y1的位置
            int locationFolder3_Y_2 = 29; //选择文件夹,Y2的位置
            switch (Program.iniLanguage.ToLower())
            {
                case "zh-cn":
                    break;
                case "en":
                    locationFolder1_X_1 = 651 - this.pb_Folder.Left; //原始文件夹,X1的位置
                    locationFolder1_X_2 = 755 - this.pb_Folder.Left; //原始文件夹,X2的位置
                    locationFolder1_Y_1 = 0; //原始文件夹,Y1的位置
                    locationFolder1_Y_2 = 30; //原始文件夹,Y2的位置
                    locationFolder2_X_1 = 777 - this.pb_Folder.Left; //自定义文件夹,X1的位置
                    locationFolder2_X_2 = 838 - this.pb_Folder.Left; //自定义文件夹,X2的位置
                    locationFolder2_Y_1 = 0; //自定义文件夹,Y1的位置
                    locationFolder2_Y_2 = 30; //自定义文件夹,Y2的位置
                    locationFolder3_X_1 = 857 - this.pb_Folder.Left; //选择文件夹,X1的位置
                    locationFolder3_X_2 = 875 - this.pb_Folder.Left; //选择文件夹,X2的位置
                    locationFolder3_Y_1 = 0; //选择文件夹,Y1的位置
                    locationFolder3_Y_2 = 30; //选择文件夹,Y2的位置
                    break;
                case "ja":
                    locationFolder1_X_1 = 678 - this.pb_Folder.Left; //原始文件夹,X1的位置
                    locationFolder1_X_2 = 762 - this.pb_Folder.Left; //原始文件夹,X2的位置
                    locationFolder1_Y_1 = 0; //原始文件夹,Y1的位置
                    locationFolder1_Y_2 = 53; //原始文件夹,Y2的位置
                    locationFolder2_X_1 = 777 - this.pb_Folder.Left; //自定义文件夹,X1的位置
                    locationFolder2_X_2 = 860 - this.pb_Folder.Left; //自定义文件夹,X2的位置
                    locationFolder2_Y_1 = 0; //自定义文件夹,Y1的位置
                    locationFolder2_Y_2 = 53; //自定义文件夹,Y2的位置
                    locationFolder3_X_1 = 873 - this.pb_Folder.Left; //选择文件夹,X1的位置
                    locationFolder3_X_2 = 890 - this.pb_Folder.Left; //选择文件夹,X2的位置
                    locationFolder3_Y_1 = 0; //选择文件夹,Y1的位置
                    locationFolder3_Y_2 = 53; //选择文件夹,Y2的位置
                    break;
                case "zh-tw":
                    locationFolder1_X_1 = 673 - this.pb_Folder.Left; //原始文件夹,X1的位置
                    locationFolder1_X_2 = 739 - this.pb_Folder.Left; //原始文件夹,X2的位置
                    locationFolder1_Y_1 = 0; //原始文件夹,Y1的位置
                    locationFolder1_Y_2 = 30; //原始文件夹,Y2的位置
                    locationFolder2_X_1 = 752 - this.pb_Folder.Left; //自定义文件夹,X1的位置
                    locationFolder2_X_2 = 790 - this.pb_Folder.Left; //自定义文件夹,X2的位置
                    locationFolder2_Y_1 = 0; //自定义文件夹,Y1的位置
                    locationFolder2_Y_2 = 30; //自定义文件夹,Y2的位置
                    locationFolder3_X_1 = 806 - this.pb_Folder.Left; //选择文件夹,X1的位置
                    locationFolder3_X_2 = 830 - this.pb_Folder.Left; //选择文件夹,X2的位置
                    locationFolder3_Y_1 = 0; //选择文件夹,Y1的位置
                    locationFolder3_Y_2 = 30; //选择文件夹,Y2的位置
                    break;
                default:
                    break;
            }

            if (e.Location.X >= locationFolder1_X_1 && e.Location.X < locationFolder1_X_2 && e.Location.Y > locationFolder1_Y_1 && e.Location.Y < locationFolder1_Y_2)
            {
                pb_Folder.Cursor = System.Windows.Forms.Cursors.Hand;
            }
            else if (e.Location.X >= locationFolder2_X_1 && e.Location.X < locationFolder2_X_2 && e.Location.Y > locationFolder2_Y_1 && e.Location.Y < locationFolder2_Y_2)
            {
                pb_Folder.Cursor = System.Windows.Forms.Cursors.Hand;
            }
            else if (e.Location.X >= locationFolder3_X_1 && e.Location.X < locationFolder3_X_2 && e.Location.Y > locationFolder3_Y_1 && e.Location.Y < locationFolder3_Y_2)
            {
                pb_Folder.Cursor = System.Windows.Forms.Cursors.Hand;
            }
            else
            {
                pb_Folder.Cursor = System.Windows.Forms.Cursors.Default;
            }
        }

        private void pb_Folder_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                int locationFolder1_X_1 = 80; //原始文件夹,X1的位置
                int locationFolder1_X_2 = 150; //原始文件夹,X2的位置
                int locationFolder1_Y_1 = 0; //原始文件夹,Y1的位置
                int locationFolder1_Y_2 = 29; //原始文件夹,Y2的位置
                int locationFolder2_X_1 = 160; //自定义文件夹,X1的位置
                int locationFolder2_X_2 = 230; //自定义文件夹,X2的位置
                int locationFolder2_Y_1 = 0; //自定义文件夹,Y1的位置
                int locationFolder2_Y_2 = 29; //自定义文件夹,Y2的位置
                int locationFolder3_X_1 = 240; //选择文件夹,X1的位置
                int locationFolder3_X_2 = 265; //选择文件夹,X2的位置
                int locationFolder3_Y_1 = 0; //选择文件夹,Y1的位置
                int locationFolder3_Y_2 = 29; //选择文件夹,Y2的位置
                //此处注意参考 pb_Folder_MouseMove , 两处的鼠标位置定义是相同的                
                switch (Program.iniLanguage.ToLower())
                {
                    case "zh-cn":
                        break;
                    case "en":
                        locationFolder1_X_1 = 651 - this.pb_Folder.Left; //原始文件夹,X1的位置
                        locationFolder1_X_2 = 755 - this.pb_Folder.Left; //原始文件夹,X2的位置
                        locationFolder1_Y_1 = 0; //原始文件夹,Y1的位置
                        locationFolder1_Y_2 = 30; //原始文件夹,Y2的位置
                        locationFolder2_X_1 = 777 - this.pb_Folder.Left; //自定义文件夹,X1的位置
                        locationFolder2_X_2 = 838 - this.pb_Folder.Left; //自定义文件夹,X2的位置
                        locationFolder2_Y_1 = 0; //自定义文件夹,Y1的位置
                        locationFolder2_Y_2 = 30; //自定义文件夹,Y2的位置
                        locationFolder3_X_1 = 857 - this.pb_Folder.Left; //选择文件夹,X1的位置
                        locationFolder3_X_2 = 875 - this.pb_Folder.Left; //选择文件夹,X2的位置
                        locationFolder3_Y_1 = 0; //选择文件夹,Y1的位置
                        locationFolder3_Y_2 = 30; //选择文件夹,Y2的位置
                        break;
                    case "ja":
                        locationFolder1_X_1 = 678 - this.pb_Folder.Left; //原始文件夹,X1的位置
                        locationFolder1_X_2 = 762 - this.pb_Folder.Left; //原始文件夹,X2的位置
                        locationFolder1_Y_1 = 0; //原始文件夹,Y1的位置
                        locationFolder1_Y_2 = 53; //原始文件夹,Y2的位置
                        locationFolder2_X_1 = 777 - this.pb_Folder.Left; //自定义文件夹,X1的位置
                        locationFolder2_X_2 = 860 - this.pb_Folder.Left; //自定义文件夹,X2的位置
                        locationFolder2_Y_1 = 0; //自定义文件夹,Y1的位置
                        locationFolder2_Y_2 = 53; //自定义文件夹,Y2的位置
                        locationFolder3_X_1 = 873 - this.pb_Folder.Left; //选择文件夹,X1的位置
                        locationFolder3_X_2 = 890 - this.pb_Folder.Left; //选择文件夹,X2的位置
                        locationFolder3_Y_1 = 0; //选择文件夹,Y1的位置
                        locationFolder3_Y_2 = 53; //选择文件夹,Y2的位置
                        break;
                    case "zh-tw":
                        locationFolder1_X_1 = 673 - this.pb_Folder.Left; //原始文件夹,X1的位置
                        locationFolder1_X_2 = 739 - this.pb_Folder.Left; //原始文件夹,X2的位置
                        locationFolder1_Y_1 = 0; //原始文件夹,Y1的位置
                        locationFolder1_Y_2 = 30; //原始文件夹,Y2的位置
                        locationFolder2_X_1 = 752 - this.pb_Folder.Left; //自定义文件夹,X1的位置
                        locationFolder2_X_2 = 790 - this.pb_Folder.Left; //自定义文件夹,X2的位置
                        locationFolder2_Y_1 = 0; //自定义文件夹,Y1的位置
                        locationFolder2_Y_2 = 30; //自定义文件夹,Y2的位置
                        locationFolder3_X_1 = 806 - this.pb_Folder.Left; //选择文件夹,X1的位置
                        locationFolder3_X_2 = 830 - this.pb_Folder.Left; //选择文件夹,X2的位置
                        locationFolder3_Y_1 = 0; //选择文件夹,Y1的位置
                        locationFolder3_Y_2 = 30; //选择文件夹,Y2的位置
                        break;
                    default:
                        break;
                }
                if (e.Location.X >= locationFolder1_X_1 && e.Location.X < locationFolder1_X_2 && e.Location.Y > locationFolder1_Y_1 && e.Location.Y < locationFolder1_Y_2)
                {
                    outFolderSelect = "rdoOldPath"; // rdoOldPath-原文件夹, rdoNewPath-自定义文件夹
                    this.pb_Folder.BackgroundImage = Image.FromFile(spath + "pb_Folder02.png");
                }
                else if (e.Location.X >= locationFolder2_X_1 && e.Location.X < locationFolder2_X_2 && e.Location.Y > locationFolder2_Y_1 && e.Location.Y < locationFolder2_Y_2)
                {
                    //KongMengyuan增加,2015-11-10,在转换过程中切换选择原始选择的文件夹不再更改（直到转换结束）
                    if (this.btnStart.Enabled == false && pbIndexBackGround.Visible == false)
                    {
                        return;
                    }
                    outFolderSelect = "rdoNewPath"; // rdoOldPath-原文件夹, rdoNewPath-自定义文件夹
                    this.pb_Folder.BackgroundImage = Image.FromFile(spath + "pb_Folder01.png");

                    if (DialogResult.OK == folderBrowserDialog.ShowDialog())
                    {
                        outFolder = folderBrowserDialog.SelectedPath;
                        this.tbBrw.Text = GetShortName(outFolder); //长路径如何变成短路径,2016-02-29,KongMengyuan
                        this.toolTip1.SetToolTip(this.tbBrw, this.tbBrw.Text);//增加TextBox的提示
                    }
                }
                else if (e.Location.X >= locationFolder3_X_1 && e.Location.X < locationFolder3_X_2 && e.Location.Y > locationFolder3_Y_1 && e.Location.Y < locationFolder3_Y_2)
                {
                    if (DialogResult.OK == folderBrowserDialog.ShowDialog())
                    {
                        outFolder = folderBrowserDialog.SelectedPath;
                        this.tbBrw.Text = GetShortName(outFolder); //长路径如何变成短路径,2016-02-29,KongMengyuan
                        //this.toolTip1.SetToolTip(this.tbBrw, this.tbBrw.Text);//增加TextBox的提示
                    }
                }
                lstFile.Focus();
            }
        }

        private void MainInfo01_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, 274, 61440 + 9, 0);
            }
        }

        private void frmDragAndDrop(object sender, MouseEventArgs e)
        {
            //鼠标按住控件拖拽窗体移动,在引用的控件上面加入事件MouseDown
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, 274, 61440 + 9, 0);
            }
        }

        private void pbQQ_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                visitHttpAddress("qq");//访问www.xjpdf.com
            }
        }

        #region 访问www.xjpdf.com, visitHttpAddress
        public void visitHttpAddress(string httpType)
        {
            switch (httpType)
            {
                case "help":
                    try
                    {
                        Process.Start("http://www.xjpdf.com/software/pdfConvert/help/?version=" + Program.httpVersion);
                    }
                    catch
                    {
                        Process.Start("iexplore.exe", "http://www.xjpdf.com/software/pdfConvert/help/?version=" + Program.httpVersion);
                    }
                    break;
                case "qq":
                    try
                    {
                        Process.Start("http://www.xjpdf.com/software/pdfConvert/qq/?version=" + Program.httpVersion);
                    }
                    catch
                    {
                        Process.Start("iexplore.exe", "http://www.xjpdf.com/software/pdfConvert/qq/?version=" + Program.httpVersion);
                    }
                    break;
                case "buy":
                    try
                    {
                        Process.Start("http://www.xjpdf.com/software/pdfConvert/buy/?version=" + Program.httpVersion + "&machine=" +
                                      Program.httpMachineCode);
                    }
                    catch
                    {
                        Process.Start("iexplore.exe", "http://www.xjpdf.com/software/pdfConvert/buy/?version=" + Program.httpVersion + "&machine=" +
                                      Program.httpMachineCode);
                    }
                    break;
                case "jiaocheng":
                    try
                    {
                        Process.Start("http://www.xjpdf.com/software/pdfConvert/jiaocheng/?version=" + Program.httpVersion);
                    }
                    catch
                    {
                        Process.Start("iexplore.exe", "http://www.xjpdf.com/software/pdfConvert/jiaocheng/?version=" + Program.httpVersion);
                    }
                    break;
                case "guanyu":
                    try
                    {
                        Process.Start("http://www.xjpdf.com/software/pdfConvert/guanyu/?version=" + Program.httpVersion);
                    }
                    catch
                    {
                        Process.Start("iexplore.exe", "http://www.xjpdf.com/software/pdfConvert/guanyu/?version=" + Program.httpVersion);
                    }
                    break;
            }
        }
        #endregion

        private void pbHelp_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                visitHttpAddress("help");//访问www.xjpdf.com
            }
        }

        private void pbBuy2_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                visitHttpAddress("buy");//访问www.xjpdf.com
            }
        }

        private void pbBuy1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                visitHttpAddress("buy");//访问www.xjpdf.com
            }
        }

        private void btnFolder_MouseEnter(object sender, EventArgs e)
        {
            this.btnFolder.BackgroundImage = Image.FromFile(spath + "btn_tjwjj02.png");
        }

        private void btnFolder_MouseLeave(object sender, EventArgs e)
        {
            this.btnFolder.BackgroundImage = Image.FromFile(spath + "btn_tjwjj01.png");
        }

        private void btnStart_MouseEnter(object sender, EventArgs e)
        {
            if (((Control)sender).Enabled != false)
            {
                this.btnStart.BackgroundImage = Image.FromFile(spath + "btn_kszh02.png");
            }
        }

        private void btnStart_MouseLeave(object sender, EventArgs e)
        {
            if (((Control)sender).Enabled != false)
            {
                this.btnStart.BackgroundImage = Image.FromFile(spath + "btn_kszh01.png");
            }
        }

        private void btnStop_MouseEnter(object sender, EventArgs e)
        {
            if (((Control)sender).Enabled != false)
            {
                this.btnStop.BackgroundImage = Image.FromFile(spath + "btn_tzzh02.png");
            }
        }

        private void btnStop_MouseLeave(object sender, EventArgs e)
        {
            if (((Control)sender).Enabled != false)
            {
                this.btnStop.BackgroundImage = Image.FromFile(spath + "btn_tzzh01.png");
            }
        }

        private void btnClear_MouseEnter(object sender, EventArgs e)
        {
            if (((Control)sender).Enabled != false)
            {
                this.btnClear.BackgroundImage = Image.FromFile(spath + "btn_qklb02.png");
            }
        }

        private void btnClear_MouseLeave(object sender, EventArgs e)
        {
            if (((Control)sender).Enabled != false)
            {
                this.btnClear.BackgroundImage = Image.FromFile(spath + "btn_qklb01.png");
            }
        }

        private void pbReg_MouseEnter(object sender, EventArgs e)
        {
            this.pbReg.BackgroundImage = Image.FromFile(spath + "btn_reg_02.png");
        }

        private void pbBuy1_MouseEnter(object sender, EventArgs e)
        {
            this.pbBuy1.BackgroundImage = Image.FromFile(spath + "btn_buy_02.png");
        }

        private void pbClose_MouseEnter(object sender, EventArgs e)
        {
            this.pbClose.BackgroundImage = Image.FromFile(spath + "close_02.png");
        }

        private void pbClose_MouseLeave(object sender, EventArgs e)
        {
            this.pbClose.BackgroundImage = Image.FromFile(spath + "close_01.png");
        }

        private void btnFolder_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                uctblPage.Text = string.Empty;//清空定制页面框,避免上次的数据残留带到这里来
                this.btnFolder.BackgroundImage = Image.FromFile(spath + "btn_tjwjj03.png");

                if (DialogResult.OK == folderBrowserDialog.ShowDialog())
                {
                    GetFolder(folderBrowserDialog.SelectedPath);
                }
            }
        }

        /// <summary>
        /// 获取文件夹下的PDF文件
        /// </summary>
        /// <param name="filePath">文件夹路径</param>
        private void GetFolder(string filePath)
        {
            DirectoryInfo folder = new DirectoryInfo(filePath);
            bool show_flag = true;
            foreach (FileInfo file in folder.GetFiles())
            {
                string extensions = file.Extension.ToLower();
                string fileName = string.Empty;
                switch (format)
                {
                    case Convert01.FORMAT.File2WORD:
                        {
                            if (extensions == ".pdf" || extensions == ".xls" || extensions == ".xlsx" ||
                                extensions == ".ppt" || extensions == ".pptx")
                            {
                                fileName = file.FullName;
                            }
                        }
                        break;
                    case Convert01.FORMAT.File2EXCEL:
                        {
                            if (extensions == ".pdf" || extensions == ".docx" || extensions == ".doc" ||
                                extensions == ".ppt" || extensions == ".pptx")
                            {
                                fileName = file.FullName;
                            }
                        }
                        break;
                    case Convert01.FORMAT.File2PPT:
                        {
                            if (extensions == ".pdf" || extensions == ".docx" || extensions == ".doc" ||
                                extensions == ".xls" || extensions == ".xlsx")
                            {
                                fileName = file.FullName;
                            }
                        }
                        break;
                    case Convert01.FORMAT.File2HTML:
                        {
                            if (extensions == ".pdf" || extensions == ".docx" || extensions == ".doc" ||
                                extensions == ".xls" || extensions == ".xlsx" || extensions == ".ppt" ||
                                extensions == ".pptx")
                            {
                                fileName = file.FullName;
                            }
                        }
                        break;
                    case Convert01.FORMAT.IMG2PDF:
                        {
                            if (extensions == ".jpg" || extensions == ".jpeg" || extensions == ".gif" ||
                                extensions == ".bmp" || extensions == ".png" || extensions == ".tiff" ||
                                extensions == ".tif")
                            {
                                fileName = file.FullName;
                            }
                        }
                        break;
                    case Convert01.FORMAT.File2TXT:
                        {
                            if (extensions == ".pdf" || extensions == ".docx" || extensions == ".doc" ||
                                extensions == ".xls" || extensions == ".xlsx" || extensions == ".ppt" ||
                                extensions == ".pptx")
                            {
                                fileName = file.FullName;
                            }
                        }
                        break;
                    case Convert01.FORMAT.File2IMG:
                        {
                            if (extensions == ".pdf" || extensions == ".docx" || extensions == ".doc" ||
                                extensions == ".xls" || extensions == ".xlsx" || extensions == ".ppt" ||
                                extensions == ".pptx")
                            {
                                fileName = file.FullName;
                            }
                        }
                        break;

                    case Convert01.FORMAT.DOC2PDF:
                        {
                            if (extensions == ".docx" || extensions == ".doc")
                            {
                                fileName = file.FullName;
                            }
                        }
                        break;

                    case Convert01.FORMAT.PPT2PDF:
                        {
                            if (extensions == ".ppt" || extensions == ".pptx")
                            {
                                fileName = file.FullName;
                            }
                        }
                        break;
                    case Convert01.FORMAT.Excel2PDF:
                        {
                            if (extensions == ".xls" || extensions == ".xlsx")
                            {
                                fileName = file.FullName;
                            }
                        }
                        break;
                    case Convert01.FORMAT.PDFSplit:
                        {
                            if (extensions == ".pdf")
                            {
                                fileName = file.FullName;
                            }
                        }
                        break;
                    case Convert01.FORMAT.PDFDecode:
                        {
                            if (extensions == ".pdf")
                            {
                                fileName = file.FullName;
                            }
                        }
                        break;
                    case Convert01.FORMAT.PDFMerge:
                        {
                            if (extensions == ".pdf")
                            {
                                fileName = file.FullName;
                            }
                        }
                        break;
                    case Convert01.FORMAT.PDFCompress:
                        {
                            if (extensions == ".pdf")
                            {
                                fileName = file.FullName;
                            }
                        }
                        break;
                    case Convert01.FORMAT.PDFGetImg:
                        {
                            if (extensions == ".pdf")
                            {
                                fileName = file.FullName;
                            }
                        }
                        break;
                }
                if (string.IsNullOrEmpty(fileName)) continue;
                if (diclst.ContainsKey(fileName))
                {
                    string sTip = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_01"); //提示
                    string sOld = string.Empty;
                    if (show_flag && openFileDialog.FileNames.Length == 1)
                    {
                        show_flag = false;
                        sOld = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_02"); //您添加的文件 $S 已存在,我们将会自动过滤这些文件!
                        sOld = Program.strReplace(sOld, "$S", new string[] { Path.GetFileName(fileName) }); //Path需引用 using System.IO;
                        MessageBox.Show(sOld, "Tips", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else if (show_flag && openFileDialog.FileNames.Length != 1)
                    {
                        show_flag = false;
                        sOld = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_03"); //您添加的部分文件已存在,我们将会自动过滤这些文件!
                        MessageBox.Show(sOld, sTip, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    continue;
                }
                if (!fileName.Contains("$"))
                {
                    ItemInfomation info = new ItemInfomation(fileName);
                    lstFile.ConversionPageDefaultText = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_04"); //中文: 全部, 英文: All
                    lstFile.AddFile(info);//添加文件进入ListView(填加文件)
                    diclst.Add(fileName, false);

                    backGroundShowHide();//隐藏或显示主界面提示图
                    SetThreeButtonValidate(true);//开始转换,停止转换,清空列表,三个按钮的有效还是失效: true-有效,false-失效
                }
            }
        }

        private void btnClear_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                clearListView();//清空列表,因为是公用的,所以写成函数
            }
        }

        private void WorkThread(object pram)
        {
            try
            {
                //队列线程索引
                int thr_index = System.Convert.ToInt32(pram);

                ListViewItem lv = null;
                Convert01 ins;
                while (true)
                {
                    if (isClose) break;
                    lock (fileQueue)
                    {
                        if (fileQueue != null && fileQueue.Count > 0)
                        {
                            lv = fileQueue.Dequeue();
                        }
                    }
                    if (lv != null && ((ItemInfomation)lv.Tag).Status != StatusType.Done)
                    {
                        string fileName = ((ItemInfomation)lv.Tag).FileFullPath;
                        string sourseName = Path.GetFileNameWithoutExtension(fileName);
                        string sourcePath = Path.GetDirectoryName(fileName) + "\\" + sourseName;
                        if (sourcePath.Contains("\\\\"))
                        {
                            try
                            {
                                sourcePath = sourcePath.Replace("\\\\", "\\");
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                            }
                        }
                        //string path = this.rdoPath.Checked ? sourcePath : this.txtOutPath.OutText + "\\" + sourseName;
                        string path = string.Empty;
                        if (outFolderSelect == "rdoOldPath")
                        {
                            path = sourcePath;
                        }
                        else
                        {
                            path = outFolder + "\\" + sourseName;
                        }

                        if (path.Contains("\\\\"))
                        {
                            try
                            {
                                path = path.Replace("\\\\", "\\");
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                            }
                        }
                        if (new FileInfo(fileName).Exists == false)
                        {
                            string sOld = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_20"); //转换的源文件不存在。
                            MessageBox.Show(sOld);
                            ItemInfomation info = (ItemInfomation)lv.Tag;
                            info.Status = StatusType.Done;
                            info.PersentValue = 0;
                            btnStart.Enabled = true;
                            btnStart.BackgroundImage = Image.FromFile(spath + "btn_kszh01.png");
                        }
                        else
                        {
                            ins = new Convert01(fileName, path, format, this);
                            ins.ErrorMessageEvent += ins_ErrorMessageEvent;
                            ItemInfomation info = (ItemInfomation)lv.Tag;
                            info.Status = StatusType.Start;
                            try
                            {
                                lstFile.SetStausPV(lv.Index, 0);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                            }
                            info.FileFullConvertPath = fileName;
                            lv.Tag = info;
                            if (!dicThreadManagement.ContainsKey(fileName))
                            {
                                dicThreadManagement.Add(fileName, thr_index);
                            }
                            if (!ins.Can_work())
                            {
                                diclst.Remove(fileName);
                                int fileIndex = fileName.LastIndexOf("\\");
                                string name = fileName.Substring(fileIndex + 1, fileName.Length - fileIndex - 1);
                                //if (ins.CloseDirect == false) //KongMengyuan,2015-10-30,如果弹出窗体直接关闭则不理会它
                                {
                                    this.lstFile.RemoveFile(lv.Index);
                                }
                                lv = null;
                                if (lstFile.IsAllFinished)
                                {
                                    btnStart.Enabled = true;
                                    btnStart.BackgroundImage = Image.FromFile(spath + "btn_kszh01.png");
                                }
                                //if (ins.Get_err_msg() != string.Empty)
                                //{
                                //    MessageBox.Show(ins.Get_err_msg(), rm.GetString("Tips"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                                //    break;
                                //}
                                if (ins.CloseDirect == false) //KongMengyuan,2015-10-30,如果弹出窗体直接关闭则不理会它
                                {
                                    if (ins.OtherMessage != "")
                                    {
                                        MessageBox.Show(ins.OtherMessage);
                                    }
                                    else
                                    {
                                        //MessageBox.Show(name + "读取错误！");
                                        //2015-11-04,KongMengyuan,添加加密的ppt、word、excel后，转换弹出提示“文件读取错误。”
                                        //控件目前只要打不开,就无法判断,控件不支持,无法判断此doc是用户随便命名（比如zip修改扩展名为doc),还是加密文件,总之是打不开
                                        string sOld = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_21"); // $S 文件读取错误,请检查文件是否加密！
                                        sOld = Program.strReplace(sOld, "$S", new string[] { name }); //Path需引用 using System.IO;
                                        MessageBox.Show(sOld);
                                    }
                                }
                            }
                            else
                            {
                                ins.Save(this, Path.GetExtension(fileName), lv.Index, lv); //开始转换,KongMengyuan注释,2015-11-19
                                Console.WriteLine("MainInfo:" + fileName);
                            }
                            // dicThreadManagement.Remove(fileName);
                            ins.Close();

                            //if (isStart)
                            //{
                            //    PopReg();
                            //    isStart = false;
                            //}
                            //KongMengyuan修改,2015-11-03,参考互盾PDFCon
                            if (isStart || NeedToPop == false)
                            {
                                //OutputDir = ins.outPath;
                                //if (MessageBox.Show("是否打开转换完成的文件存放目录？", "转换完成", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                                //{
                                //    Process.Start(OutputDir.Substring(0, OutputDir.IndexOf("\\") + 1));
                                //}
                                if (ins.Count > 5 && ins.targetFormat != Convert01.FORMAT.PDFMerge && ins.targetFormat != Convert01.FORMAT.PDFCompress && ins.targetFormat != Convert01.FORMAT.PDFGetImg && ins.Can_work())
                                {
                                    //2015-11-30,KongMengyuan注释,时隐时现的bug,当把页面先在“选择页面”里面输入一个“1”(只要小于5)的数字(第1次弹出RegTips页面),再重新引入同样的文件时,即使大于5页也不会再有"RegTips"提示了,后来发现这是对的,因为不需要频繁的显示"请您注册",只显示一次就行了
                                    NeedToPop = true;
                                }
                                isStart = false;
                            }

                            if (NeedToPop)
                            {
                                PopReg();
                            }
                        }
                    }

                    Thread.Sleep(500);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }

        public void PopReg()
        {
            if (!isReg)
            {
                this.syncContext.Post(RegTigs, null);
            }
        }

        private void RegTigs(object obj)
        {
            //KongMengyuan修改,2015-11-03,参考互盾PDFCon
            if (lstFile.IsAllFinished && !Poped)// && fileQueue.Count == 0)
            {
                Poped = true;
                RegTips01 frm = new RegTips01();
                frm.StartPosition = FormStartPosition.CenterParent;
                frm.Location = this.PointToScreen(new Point(400, this.lstFile.Location.Y + 30));
                DialogResult dr = frm.ShowDialog();
                if (dr == System.Windows.Forms.DialogResult.OK)
                {
                    RegDlg01 reg = new RegDlg01();
                    reg.StartPosition = FormStartPosition.CenterParent;
                    reg.Location = this.PointToScreen(new Point(250, lstFile.Location.Y - 10));
                    reg.ShowDialog();
                    if (new reg().Is_Reg())
                    {
                        isReg = true;
                    }
                    else
                    {
                        isReg = false;
                    }
                }
            }
        }

        private void ins_ErrorMessageEvent(object sender, ErrorMessageArgs args)
        {
            MessageBox.Show(args.message);
        }

        private void MainInfo01_FormClosing(object sender, FormClosingEventArgs e)
        {
            WinformClose();
        }

        private void pbMinimize_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                this.WindowState = FormWindowState.Minimized;
            }
        }

        private void lstFile_DragDrop(object sender, DragEventArgs e)
        {
            bool show_flag = true;
            if (((string[])e.Data.GetData(DataFormats.FileDrop)) != null)
            {
                foreach (string file_name in ((string[])e.Data.GetData(DataFormats.FileDrop)))
                {
                    string fileExt = Path.GetExtension(file_name).ToLower();
                    if (fileExt == ".pdf" || fileExt == ".xls" || fileExt == ".xlsx" || fileExt == ".ppt" || fileExt == ".pptx" || fileExt == ".doc" ||
                        fileExt == ".docx" || fileExt == ".jpg" || fileExt == ".jpeg" || fileExt == ".gif" || fileExt == ".bmp" || fileExt == ".png" || fileExt == ".tiff" || fileExt == ".tif")
                    {
                        if (diclst.ContainsKey(file_name))
                        {
                            string sTip = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_01"); //提示
                            string sOld = string.Empty;
                            if (show_flag && openFileDialog.FileNames.Length == 1)
                            {
                                show_flag = false;
                                sOld = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_02"); //您添加的文件 $S 已存在,我们将会自动过滤这些文件!
                                sOld = Program.strReplace(sOld, "$S", new string[] { Path.GetFileName(file_name) }); //Path需引用 using System.IO;
                                MessageBox.Show(sOld, sTip, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else if (show_flag && openFileDialog.FileNames.Length != 1)
                            {
                                show_flag = false;
                                sOld = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_03"); //您添加的部分文件已存在,我们将会自动过滤这些文件!
                                MessageBox.Show(sOld, sTip, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            continue;
                        }
                        ItemInfomation info = new ItemInfomation(file_name);
                        lstFile.ConversionPageDefaultText = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_04"); //中文: 全部, 英文: All
                        lstFile.AddFile(info);//添加文件进入ListView(填加文件)
                        diclst.Add(file_name, false);
                    }
                    else if (string.IsNullOrEmpty(fileExt))
                    {
                        GetFolder(file_name);
                    }
                }
            }
            lstFile.Invalidate();
        }

        private void lstFile_DragEnter(object sender, DragEventArgs e)
        {
            //Win8的64位和Win10的64位(32位的未测试)安装完直接运行不能拖拽，再次打开就可以拖拽,KongMengyuan注释,2015-12-03
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
                if (this.pbIndexBackGround.Visible == true)
                {
                    this.pbIndexBackGround.Visible = false;  //隐藏提示图标

                    SetThreeButtonValidate(true);//开始转换,停止转换,清空列表,三个按钮的有效还是失效: true-有效,false-失效
                }
            }
            else
            {
                e.Effect = e.AllowedEffect;
            }
        }

        private void lstFile_DragLeave(object sender, EventArgs e)
        {
            lstFile.InsertionMark.Index = -1;
        }

        private void lstFile_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                rightMenuEnable();//右键菜单哪些有效

                //string expendFileName = returnFileName(lstFile.SelectedItems[0].SubItems[1].Text, false);
                //if (expendFileName.ToLower() == "pdf")
                {
                    if (this.lstFile.SelectedItems.Count > 0)
                    {
                        this.tsmiStart.Visible = true; //显示"文件转Word"鼠标右键功能
                        this.contextMenuStrip1.Show(this, e.Location);

                        string iniSection = this.Name;

                        //pdf分割,pdf合并
                        if (format == Convert01.FORMAT.PDFSplit)
                        {
                            //分割目前针对所有的文件操作,但是“指定页面分割”是针对单个页面的.否则就得单独加一列来记录对应文件的页面设置,它不像其它的只是转成页数就可以了,它有5项设置,比较多.
                            this.tsmiPageSet.Enabled = true;// 页数提取方案
                            this.tsmiPicFormat.Enabled = false;// 设置图片输出格式
                            this.tsmiPageSet.Text = Program.Language_Load(Program.iniLanguage, iniSection, "tsmiPageSet.Text1");// tsmiPageSet.Text1-页数提取方案,tsmiPageSet.Text2-输出设置方案
                        }
                        else if (format == Convert01.FORMAT.PDFMerge)
                        {
                            this.tsmiPageSet.Enabled = true;// 页数提取方案
                            this.tsmiPicFormat.Enabled = false;// 设置图片输出格式
                            this.tsmiPageSet.Text = Program.Language_Load(Program.iniLanguage, iniSection, "tsmiPageSet.Text2");// tsmiPageSet.Text1-页数提取方案,tsmiPageSet.Text2-输出设置方案
                        }
                        else if (format == Convert01.FORMAT.PDFGetImg || format == Convert01.FORMAT.File2IMG)
                        {
                            this.tsmiPageSet.Enabled = false;// 页数提取方案
                            this.tsmiPicFormat.Enabled = true;// 设置图片输出格式
                        }
                        else if (format == Convert01.FORMAT.PDFGetImg)
                        {
                            this.tsmiPageSet.Enabled = false;// 页数提取方案
                            this.tsmiPicFormat.Enabled = false;// 设置图片输出格式
                        }

                        //如果是多选的话,自定义的页面提取就不出现(多选的情况比较麻烦,因为有些如果已经设置好了,是否保存,是否使用统一的,还有如果部分超出部分不超出如何处理)
                        if (this.lstFile.SelectedItems.Count > 1)
                        {
                            this.tsmiPageSet.Enabled = false;// 页数提取方案
                        }
                    }
                }
                //else
                //{
                //    this.tsmiStart.Visible = false; //显示"文件转Word"鼠标右键功能
                //}
                //MessageBox.Show(returnFileName(lstFile.SelectedItems[0].SubItems[1].Text,false));//返回文件名的前后部分
            }

            if (e.Button == System.Windows.Forms.MouseButtons.Left && format != Convert01.FORMAT.IMG2PDF)
            {
                for (int i = 0; i < lstFile.Items.Count; i++)
                {
                    if (lstFile.Items[i].Selected)
                    {
                        ItemInfomation info = (ItemInfomation)lstFile.Items[i].Tag;
                        if (info != null)
                        {
                            if (info.Status != StatusType.Ready)
                                return;
                        }
                    }

                    if (lstFile.Items[i].SubItems[3].Bounds.Contains(e.X, e.Y))
                    {
                        //KongMengyuan修改,20015-11-05,当页码输入过多,鼠标离开当前位置,再重新回来时,会发现底部的数据两边有显示(两边溢出)
                        //位于表格顶部
                        //pltext.Location =
                        //    new Point(lstFile.SelectedItems[0].SubItems[3].Bounds.Left + lstFile.Location.X + 2,
                        //        lstFile.SelectedItems[0].SubItems[3].Bounds.Top + lstFile.Location.Y + 2);
                        //位于表格中间
                        pltext.Location =
                            new Point(lstFile.SelectedItems[0].SubItems[3].Bounds.Left + lstFile.Location.X + 2,
                                lstFile.SelectedItems[0].SubItems[3].Bounds.Top + lstFile.Location.Y + 1 + 8);
                        this.pltext.Width = lstFile.SelectedItems[0].SubItems[3].Bounds.Width;

                        this.pltext.Visible = true;
                        int height = this.pltext.Height;
                        if (lstFile.SelectedItems[0].SubItems[3].Text != Program.Language_Load(Program.iniLanguage, this.Name, "MSG_04")) //中文: 全部, 英文: All
                        {
                            uctblPage.Text = lstFile.SelectedItems[0].SubItems[3].Text;
                        }
                        else
                        {
                            uctblPage.Text = string.Empty;
                        }

                        m_EditIndex = lstFile.SelectedItems[0].Index;
                        return;
                    }
                }
            }
            if (!string.IsNullOrEmpty(uctblPage.Text))
            {
                if (lstFile.SelectedItems.Count > 0 && lstFile.SelectedItems[0].Index == m_EditIndex)
                {
                    string s = uctblPage.Text.Replace("\r\n", string.Empty);
                    s = Convert01.strMergeStr1(s);
                    lstFile.SelectedItems[0].SubItems[3].Text = s;
                    uctblPage.Text = string.Empty;
                }

            }
            pltext.Visible = false;
        }

        private void comboBoxPage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    if (string.IsNullOrEmpty(uctblPage.Text.Trim()))
                    {
                        uctblPage.Text = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_04"); //中文: 全部, 英文: All
                    }
                    string s = Convert01.strMergeStr1(uctblPage.Text);
                    uctblPage.Text = s;
                    lstFile.SelectedItems[0].SubItems[3].Text = uctblPage.Text;
                    pltext.Visible = false;
                    lstFile.Focus();
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }
        }

        private void comboBoxPage_Leave(object sender, EventArgs e)
        {
            //string text = comboBoxPage.Text;
            //try
            //{
            //    if (text != Program.Language_Load(Program.iniLanguage, this.Name, "MSG_04")) //中文: 全部, 英文: All
            //    {
            //        if (VerifyPageSet01(text) == false && text.Length > 0)
            //        //if (VerifyPageSetOld(text) == false && text.Length > 0)
            //        {
            //            MessageBox.Show(Program.Language_Load(Program.iniLanguage, this.Name, "MSG_06")); //请输入正确的页码格式
            //            comboBoxPage.Text = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_04"); //中文: 全部, 英文: All
            //        }
            //        else if (text.Length > 0)
            //        {
            //            lstFile.SelectedItems[0].SubItems[3].Text = comboBoxPage.Text;
            //            comboBoxPage.Text = string.Empty;
            //        }
            //    }
            //}
            //catch
            //{
            //    if (lstFile.SelectedItems.Count > 0)
            //    {
            //        lstFile.SelectedItems[0].SubItems[3].Text = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_04"); //中文: 全部, 英文: All
            //    }
            //}
            //pltext.Visible = false;

            //comboBoxPage_Validated(sender, e);

            //KongMengyuan,2016-01-08,微软Select的Bug,快速点击切换时,可能会把上条选中的数字切换过来,如果慢速就没有这个问题了(这个和当初产品经理夏勇设计的鼠标移到ListView上面显示全称,有时会因为移动快而定位不准,是一个道理)
            resetPageSet();
        }
        private void resetPageSet()
        {
            if (lstFile.SelectedItems.Count <= 0)
            {
                return;
            }
            string name = lstFile.SelectedItems[0].SubItems[1].Text;
            int pageCount = System.Convert.ToInt32(lstFile.SelectedItems[0].SubItems[2].Text);

            if (!MainInfo01.isReg) //免费版只转换前5页
            {
                if (pageCount >= Program.pageFreeConvert)
                {
                    pageCount = Program.pageFreeConvert;
                }
            }

            //利用正则表达式将字符串中的数字提取出来分别保存在整型数组中,提取字符串中的数字
            string s = this.uctblPage.Text;  //测试数据"1 2234 34";
            this.uctblPage.Text = s;
            s = Convert01.strRemoveTrim(s, true); //去除字符串前后两边的多余符号和空格"  ；;,，-?*  "            

            if (Convert01.VerifyNumber01(s, true) == false || verifyValidate(s, pageCount) == false || s == string.Empty) //校验数字是否正确
            {
                uctblPage.Text = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_04"); //中文: 全部, 英文: All
                lstFile.SelectedItems[0].SubItems[3].Text = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_04"); //中文: 全部, 英文: All
                return;
            }
            s = Convert01.strMergeStr1(s);
            uctblPage.Text = s;
            lstFile.SelectedItems[0].SubItems[3].Text = s;
            return;
        }
        private void comboBoxPage_Validated(object sender, EventArgs e)
        {
            //int inputMaxCount = 0;
            ////KongMengyuan,2016-01-08,微软的Select的Bug,快速点击切换时,可能会把上条选中的数字切换过来,如果慢速就没有这个问题了(这个和当初产品经理夏勇设计的鼠标移到ListView上面显示全称,有时会因为移动快而定位不准,是一个道理)
            //if (lstFile.SelectedItems.Count <= 0)
            //{
            //    return;
            //}
            //string name = lstFile.SelectedItems[0].SubItems[1].Text;
            //int pageCount = System.Convert.ToInt32(lstFile.SelectedItems[0].SubItems[2].Text);

            //if (!MainInfo01.isReg) //免费版只转换前5页
            //{
            //    if (pageCount >= Program.pageFreeConvert)
            //    {
            //        pageCount = Program.pageFreeConvert;
            //    }
            //}

            ////利用正则表达式将字符串中的数字提取出来分别保存在整型数组中,提取字符串中的数字
            //string s = this.comboBoxPage.Text;  //测试数据"1 2234 34";

            //s = Convert01.strRemoveTrim(s, true); //去除字符串前后两边的多余符号和空格"  ；;,，-?*  "
            //lstFile.SelectedItems[0].SubItems[3].Text = s;

            //if (Convert01.VerifyNumber01(s, true) == false || verifyValidate(s, pageCount) == false) //校验数字是否正确
            //{
            //    comboBoxPage.Text = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_04"); //中文: 全部, 英文: All
            //    lstFile.SelectedItems[0].SubItems[3].Text = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_04"); //中文: 全部, 英文: All
            //    return;
            //}

            //MatchCollection vMatchs = Regex.Matches(s, @"(\d+)"); //需要引用 using System.Text.RegularExpressions; //Regex调用
            //int[] vInts = new int[vMatchs.Count];
            //for (int i = 0; i < vMatchs.Count; i++)
            //{
            //    if (vMatchs[i].Value.Length > "99999".Length)
            //    {
            //        string sTip = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_01"); //提示
            //        string sOld = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_22"); //所输入的数字 $S 大于99999,目前不支持这么大的数字
            //        sOld = Program.strReplace(sOld, "$S", new string[] { vMatchs[i].ToString() }); //替换Language.ini字符串里面的通用代替符号
            //        MessageBox.Show(sOld + "\r\n" + "\r\n" + this.AccessibleName, sTip, MessageBoxButtons.OK, MessageBoxIcon.Error);  // 字符之间加入回车符
            //        comboBoxPage.Text = string.Empty;
            //    }
            //    vInts[i] = int.Parse(vMatchs[i].Value);
            //}
            //int uBound = vInts.GetUpperBound(0); //取得数组的最大上标

            //string errString = string.Empty;

            //for (int i = 0; i <= uBound; i++)
            //{
            //    inputMaxCount = vInts[i];
            //    //Console.WriteLine(vInts[i]);
            //    if (inputMaxCount > pageCount)
            //    {
            //        errString += ", " + inputMaxCount.ToString();
            //    }
            //    //所输入的数字必须是从小到大排列的
            //    try
            //    {
            //        if (System.Convert.ToInt32(vInts[i + 1]) < System.Convert.ToInt32(vInts[i]))
            //        {
            //            string sTip = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_01"); //提示
            //            errString = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_23"); //所输入的数字必须是从小到大排列的，请重新输入！                            
            //            MessageBox.Show(errString + "\r\n" + "\r\n" + name, sTip, MessageBoxButtons.OK, MessageBoxIcon.Error);  // 字符之间加入回车符
            //            lstFile.SelectedItems[0].SubItems[3].Text = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_04"); //中文: 全部, 英文: All                
            //            //break;
            //            return;
            //        }
            //    }
            //    catch
            //    { }
            //}
            //if (errString != string.Empty)
            //{
            //    //去掉字符串左边的逗号和空格
            //    errString = errString.Substring(2, errString.Length - 2);
            //    string sTip = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_01"); //提示
            //    string sOld = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_07"); //输入的页码 $S 已经超过可以转换的最大页数 $S ,请检查输入的页码范围
            //    sOld = Program.strReplace(sOld, "$S", new string[] { errString, pageCount.ToString() }); //替换Language.ini字符串里面的通用代替符号
            //    MessageBox.Show(sOld + "\r\n" + "\r\n" + name, sTip, MessageBoxButtons.OK, MessageBoxIcon.Error);  // 字符之间加入回车符
            //    lstFile.SelectedItems[0].SubItems[3].Text = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_04"); //中文: 全部, 英文: All
            //}
            //comboBoxPage.Text = string.Empty;
        }


        #region 校验是否超过最大值,同时是否从小到大排列
        /// <summary>
        /// 2016-01-27,KongMengyuan
        /// 校验是否超过最大值,同时是否从小到大排列
        /// 测试样例:
        /// 1,5,7
        /// 1,7,5
        /// </summary>
        /// <param name="enterNumber">输入的组合字符串</param>
        /// <returns>是否正确</returns>
        private bool verifyValidate(string enterNumber, int pageCount)
        {
            int inputMaxCount = 0;
            //KongMengyuan,2016-01-08,微软的Select的Bug,快速点击切换时,可能会把上条选中的数字切换过来,如果慢速就没有这个问题了(这个和当初产品经理夏勇设计的鼠标移到ListView上面显示全称,有时会因为移动快而定位不准,是一个道理)
            //int pageCount = this.pageCount;

            //if (!MainInfo01.isReg) //免费版只转换前5页
            //{
            //    if (pageCount >= Program.pageFreeConvert)
            //    {
            //        pageCount = Program.pageFreeConvert;
            //    }
            //}

            //利用正则表达式将字符串中的数字提取出来分别保存在整型数组中,提取字符串中的数字
            string s = enterNumber;  //测试数据"1 2234 34";

            s = Convert01.strRemoveTrim(s, false); //去除字符串前后两边的多余符号和空格"  ；;,，-?*  "

            MatchCollection vMatchs = Regex.Matches(s, @"(\d+)"); //需要引用 using System.Text.RegularExpressions; //Regex调用
            int[] vInts = new int[vMatchs.Count];
            for (int i = 0; i < vMatchs.Count; i++)
            {
                if (vMatchs[i].Value.Length > "99999".Length)
                {
                    string sTip = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_01"); //提示
                    string sOld = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_22"); //所输入的数字 $S 大于99999,目前不支持这么大的数字
                    sOld = Program.strReplace(sOld, "$S", new string[] { vMatchs[i].ToString() }); //替换Language.ini字符串里面的通用代替符号
                    MessageBox.Show(sOld + "\r\n" + "\r\n" + this.AccessibleName, sTip, MessageBoxButtons.OK, MessageBoxIcon.Error);  // 字符之间加入回车符
                    return false;
                }
                vInts[i] = int.Parse(vMatchs[i].Value);
            }
            int uBound = vInts.GetUpperBound(0); //取得数组的最大上标

            string errString = string.Empty;

            for (int i = 0; i <= uBound; i++)
            {
                inputMaxCount = vInts[i];
                //Console.WriteLine(vInts[i]);
                if (inputMaxCount > pageCount)
                {
                    errString += ", " + inputMaxCount.ToString();
                }

                //所输入的数字必须是从小到大排列的
                try
                {
                    if (System.Convert.ToInt32(vInts[i + 1]) < System.Convert.ToInt32(vInts[i]))
                    {
                        string sTip = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_01"); //提示
                        string sOld = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_23"); //所输入的数字必须是从小到大排列的，请重新输入！                            
                        MessageBox.Show(sOld + "\r\n" + "\r\n" + this.AccessibleName, sTip, MessageBoxButtons.OK, MessageBoxIcon.Error);  // 字符之间加入回车符
                        return false;
                    }
                }
                catch
                { }
            }
            if (errString != string.Empty)
            {
                //去掉字符串左边的逗号和空格
                errString = errString.Substring(2, errString.Length - 2);
                string sTip = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_01"); //提示
                string sOld = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_07"); //输入的页码 $S 已经超过可以转换的最大页数 $S ,请检查输入的页码范围
                sOld = Program.strReplace(sOld, "$S", new string[] { errString, pageCount.ToString() }); //替换Language.ini字符串里面的通用代替符号
                MessageBox.Show(sOld + "\r\n" + "\r\n" + this.AccessibleName, sTip, MessageBoxButtons.OK, MessageBoxIcon.Error);  // 字符之间加入回车符
                return false;
            }
            return true;
        }
        #endregion

        //支持输入形如“1-5;7,9;15-20,25”这样的样式，表示将文件分割为“1-5”“7,9”“15-20,25”三个文件（用分号分割）
        private bool VerifyPageSet01(string text)
        {
            text = text.Replace('；', ';');
            text = text.Replace('，', ',');
            text = text.Replace(" ", "");
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] != ',' && text[i] != ';' && text[i] != '-' && (text[i] < '0' || text[i] > '9'))
                {
                    return false;
                }
            }
            if (text.Replace("-", "").Replace(",", "").Replace("；", "").Length == 0)
            {
                return false;
            }

            return true;
        }
        //支持输入形如“1-5”或"7,12"这样的样式,文件保存为一个文档
        private bool VerifyPageSetOld(string text)
        {
            text = text.Replace('，', ',');
            text = text.Replace(" ", "");
            for (int i = 0; i < text.Length; i++)
                if (text[i] != ',' && text[i] != '-' && (text[i] < '0' || text[i] > '9'))
                {
                    return false;
                }

            if (text.Replace("-", "").Replace(",", "").Length == 0)
            {
                return false;
            }

            return true;
        }

        private void lstFile_OnDeleteButtonClicked(int index)
        {
            if (index >= 0)
            {

                ItemInfomation Info = ((ItemInfomation)lstFile.Items[index].Tag);
                diclst.Remove(Info.FileFullPath);
                if (dicThreadManagement.Count > 0 && dicThreadManagement.ContainsKey(Info.FileFullPath))
                {
                    int i = dicThreadManagement[Info.FileFullPath];
                    //dicThreadManagement.Remove(Info.FileFullPath);
                    //终止当前线程
                    thread[i].Abort();
                }

                //KongMengyuan增加,2015-11-17
                string sTip = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_01"); //提示
                string sOld = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_08"); //是否删除文件
                if (MessageBox.Show(sOld + "\r\n" + "\r\n" + lstFile.SelectedItems[0].SubItems[1].Text, sTip, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    this.lstFile.RemoveFile(index);
                }
                else
                {
                    return;
                }

                //KongMengyuan修改,20015-11-06,注释掉下面这段就可以了.注释原因：添加大量文件后，开始转换，转换到一半停止转换，再点击删除其中的一个文件。文件被删除，同时从未转换开始的文件开始转换（已经暂停的不自动转换）。
                ////开启新线程
                //for (int j = 0; j < thread.Length; j++)
                //{
                //    if (thread[j].ThreadState == System.Threading.ThreadState.Stopped)
                //    {
                //        thread[j] = new Thread(new ParameterizedThreadStart(WorkThread));
                //        thread[j].IsBackground = true;
                //        thread[j].Start(j);
                //    }
                //}


                // dicThreadManagement.Remove(Info.FileFullPath);
                if (lstFile.IsAllFinished)
                {
                    btnStart.Enabled = true;
                    btnStart.BackgroundImage = Image.FromFile(spath + "btn_kszh01.png");
                }

                backGroundShowHide();//隐藏或显示主界面提示图
            }
        }

        private void lstFile_OnOpenDirectoryButtonClicked(int index)
        {
            ItemInfomation Info = ((ItemInfomation)lstFile.Items[index].Tag);
            string filePath = Info.FileFullPath;
            if (Info.Status == StatusType.Done)
            {
                filePath = Info.FileFullConvertPath;
            }
            try
            {
                string path = @"/select," + filePath + "";
                System.Diagnostics.Process.Start("explorer.exe", path);
            }
            catch
            { }
        }

        private void lstFile_OnOpenFileButtonClicked(int index)
        {
            ItemInfomation Info = ((ItemInfomation)lstFile.Items[index].Tag);
            string filePath = Info.FileFullPath;
            if (Info.Status == StatusType.Done)
            {
                filePath = Info.FileFullConvertPath;
            }
            try
            {
                System.Diagnostics.Process.Start(filePath);
                ////发送请求信息
                //TempUrl t = new TempUrl("主程序", "列表打开[" + filePath + "]文件");
                //PostURL(t);
            }
            catch (Exception ex)
            {
                MessageBox.Show(Program.Language_Load(Program.iniLanguage, this.Name, "MSG_09")); //是否删除文件
            }
        }

        private void lstFile_OnStatusButtonClicked(int index, StatusType status)
        {
            this.pltext.Visible = false;

            //创建目录时如果目录已存在，则不会重新创建目录，且不会报错。创建目录时会自动创建路径中各级不存在的目录。(防止指定好的目录在转换之前被删除)
            string activeDir = tbBrw.Text;
            string newPath = System.IO.Path.Combine(activeDir, "");
            System.IO.Directory.CreateDirectory(newPath);

            if (status == StatusType.Start)
            {
                ((ItemInfomation)lstFile.Items[index].Tag).Status = StatusType.Pause;
            }
            else if (status == StatusType.Pause)
            {

                ((ItemInfomation)lstFile.Items[index].Tag).Status = StatusType.Start;
            }
            else if (status == StatusType.Ready)
            {
                if (!VerifyList(index))
                {
                    fileQueue.Enqueue(this.lstFile.Items[index]);
                    ((ItemInfomation)lstFile.Items[index].Tag).Status = StatusType.Start;
                    lstFile.SetStausPV(index, ((ItemInfomation)lstFile.Items[index].Tag).PersentValue);
                    //开启新线程
                    //2015-12-16,KongMengyuan增加,目前默认是只转换CPU个数少1核的进程数,发现虚拟机里“WinXP 64位”点击“开始转换”时此处出错,跟踪发现是"thread.length"不识别(编译时"目标平台"为X86和X64也都出错)。
                    int threadLength = cpuNumber;
                    //检查窗体打开的初始化线程是否结束(如果在打开窗体后立刻执行转换就会产生错误,要等待初始化的进程先结束)
                    while (true)
                    {
                        try
                        {
                            threadLength = thread.Length;
                            break;
                        }
                        catch
                        {
                            Thread.Sleep(500);
                            //threadLength = 0; //设置为0就不开始转换了,所以此处就没有用了
                        }
                    }
                    //检查窗体打开的初始化线程是否结束(如果在打开窗体后立刻执行转换就会产生错误,要等待初始化的进程先结束)
                    while (true)
                    {
                        //如果 outFolder 为空,则说明线程onlyRunOnce()还没有运行结束,要等待它首先运行结束
                        if (string.IsNullOrEmpty(outFolder) == false || string.Equals(outFolder, "") == false)
                        {
                            break;
                        }
                    }
                    for (int j = 0; j < threadLength; j++) //2015-12-14,KongMengyuan注释,目前默认是只转换CPU个数少1核的进程数,发现虚拟机里“WinXP 64位”点击“开始转换”时此处出错,跟踪发现是"thread.length"不识别(编译时"目标平台"为X86和X64也都出错)。
                    {
                        //Win8的64位和Win10的64位(32位的未测试)此处不是Stopped而是Aborted,KongMengyuan注释,2016-03-04
                        //if (thread[j].ThreadState == System.Threading.ThreadState.Stopped)
                        if (thread[j].ThreadState == System.Threading.ThreadState.Stopped || thread[j].ThreadState == System.Threading.ThreadState.Aborted || thread[j].ThreadState == System.Threading.ThreadState.Unstarted)
                        {
                            thread[j] = new Thread(new ParameterizedThreadStart(WorkThread));
                            thread[j].IsBackground = true;
                            thread[j].Start(j);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 验证列表是否包含其他类型文件
        /// </summary>
        private bool VerifyList(int index = -1)
        {
            bool result = false;
            if (index == -1)
            {
                foreach (ListViewItem lv in lstFile.Items)
                {
                    string suffix = lv.SubItems[1].Text;
                    ItemInfomation Info = ((ItemInfomation)lv.Tag);
                    if (Info != null && Info.Status == StatusType.Done)
                    {
                        suffix = Info.FileFullConvertPath;
                    }

                    // KongMengyuan增加,2015-11-05,特殊测试: 用户添加文件后,同时用户又在原文件夹删除了该原文件,但是文件名已经保存在“PDF转换器”里面了,此时点击开始转换,程序就没有响应了
                    string fileName = ((ItemInfomation)lv.Tag).FileFullPath;
                    if (!File.Exists(fileName))   //判断文件是否存在
                    {
                        string sOld = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_19"); //文件已经不存在了，请检查！
                        MessageBox.Show(fileName + "\r\n" + "\r\n" + sOld);
                        diclst.Remove(((ItemInfomation)lv.Tag).FileFullPath);
                        lv.Remove();
                    }
                    else
                    {
                        if (!IsMatched(suffix, format))
                        {
                            // KongMengyuan注释掉下面的代码,2015-11-05,添加大量文件后,点击开始转换,转换成功了部分,此时停止转换,再点击开始转换,已经转换成功的文件会弹出提示同时把文件删除.(应该保留在当前页面,便于用户点击后面的"打开"按钮打开)
                            // 原代码：下面的代码是把已经转换好的文件移除后继续转换剩余的文件
                            //if (!result)
                            //{
                            //    MessageBox.Show(string.Format(rm.GetString("msg3"), GetTaskName()), rm.GetString("Tips"),
                            //        MessageBoxButtons.OK, MessageBoxIcon.Information);
                            //    //MessageBox.Show(string.Format("您选择的是{0}，但您添加的文件中含有其他类型的文件,我们将会移除相应的文件", GetTaskName()), "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            //    result = true;
                            //}
                            //diclst.Remove(((ItemInfomation)lv.Tag).FileFullPath);
                            //lv.Remove();
                        }
                    }
                }
            }
            else
            {
                if (lstFile.Items.Count > 0)
                {
                    string suffix = lstFile.Items[index].SubItems[1].Text;
                    if (!IsMatched(suffix, format))
                    {
                        if (!result)
                        {
                            string sTip = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_01"); //提示
                            string sOld = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_10"); //您选择的是 $S，但您添加的文件中含有其他类型的文件,我们将会移除相应的文件
                            sOld = Program.strReplace(sOld, "$S", new string[] { GetTaskName() }); //替换Language.ini字符串里面的通用代替符号
                            MessageBox.Show(sOld, sTip,
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            //MessageBox.Show(string.Format("您选择的是{0}，但您添加的文件中含有其他类型的文件,我们将会移除相应的文件", GetTaskName()), "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            result = true;
                        }
                        diclst.Remove(((ItemInfomation)lstFile.Items[index].Tag).FileFullPath);
                        this.lstFile.RemoveFile(index);
                    }
                }
            }
            return result;
        }

        private bool IsMatched(string file_name, Convert01.FORMAT format)
        {
            bool result = false;

            string suffix = Path.GetExtension(file_name).ToUpper();
            if (format == Convert01.FORMAT.File2WORD)
            {
                if (suffix == ".PDF" || suffix == ".XLS" || suffix == ".XLSX" || suffix == ".PPT" || suffix == ".PPTX")
                {
                    result = true;
                }
            }
            else if (format == Convert01.FORMAT.File2EXCEL)
            {
                if (suffix == ".PDF" || suffix == ".DOC" || suffix == ".DOCX" || suffix == ".PPT" || suffix == ".PPTX")
                {
                    result = true;
                }
            }
            else if (format == Convert01.FORMAT.File2PPT)
            {
                if (suffix == ".PDF" || suffix == ".DOC" || suffix == ".DOCX" || suffix == ".XLS" || suffix == ".XLSX")
                {
                    result = true;
                }
            }
            else if (format == Convert01.FORMAT.File2IMG || format == Convert01.FORMAT.File2HTML ||
                     format == Convert01.FORMAT.File2TXT)
            {
                if (suffix == ".PDF" || suffix == ".DOC" || suffix == ".DOCX" || suffix == ".PPT" ||
                    suffix == ".PPTX" || suffix == ".XLS" || suffix == ".XLSX")
                {
                    result = true;
                }
            }

            else if (format == Convert01.FORMAT.IMG2PDF)
            {
                if (suffix == ".JPG" || suffix == ".JPEG" || suffix == ".GIF" || suffix == ".BMP" ||
                    suffix == ".PNG" || suffix == ".TIF" || suffix == ".TIFF")
                {
                    return result = true;
                    ;
                }
            }
            else if (format == Convert01.FORMAT.DOC2PDF)
            {
                if (suffix == ".DOC" || suffix == ".DOCX")
                {
                    result = true;
                }
            }
            else if (format == Convert01.FORMAT.Excel2PDF)
            {
                if (suffix == ".XLS" || suffix == ".XLSX")
                {
                    result = true;
                }
            }

            else if (format == Convert01.FORMAT.PPT2PDF)
            {
                if (suffix == ".PPT" || suffix == ".PPTX")
                {
                    result = true;
                }
            }

            else if (format == Convert01.FORMAT.PDFSplit)
            {
                if (suffix == ".PDF")
                {
                    result = true;
                }
            }

            else if (format == Convert01.FORMAT.PDFDecode)
            {
                if (suffix == ".PDF")
                {
                    result = true;
                }
            }

            else if (format == Convert01.FORMAT.PDFMerge)
            {
                if (suffix == ".PDF")
                {
                    result = true;
                }
            }

            else if (format == Convert01.FORMAT.PDFCompress)
            {
                if (suffix == ".PDF")
                {
                    result = true;
                }
            }

            else if (format == Convert01.FORMAT.PDFGetImg)
            {
                if (suffix == ".PDF")
                {
                    result = true;
                }
            }
            return result;
        }
        private string GetTaskName()
        {
            string tackName = string.Empty;
            string sOld1 = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_11"); //文件转
            string sOld2 = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_12"); //转
            string sOld3 = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_13"); //图片
            string sOld4 = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_14"); //PDF合并
            switch (format)
            {
                case Convert01.FORMAT.File2WORD:
                    {
                        tackName = sOld1 + " Word";
                    }
                    break;
                case Convert01.FORMAT.File2EXCEL:
                    {
                        tackName = sOld1 + " EXCEL";
                    }
                    break;
                case Convert01.FORMAT.File2PPT:
                    {
                        tackName = sOld1 + " PPT";
                    }
                    break;
                case Convert01.FORMAT.File2HTML:
                    {
                        tackName = sOld1 + " HTML";
                    }
                    break;
                case Convert01.FORMAT.IMG2PDF:
                    {
                        tackName = sOld3 + sOld2 + " PDF";
                    }
                    break;
                case Convert01.FORMAT.File2TXT:
                    {
                        tackName = sOld1 + " TXT";
                    }
                    break;
                case Convert01.FORMAT.File2IMG:
                    {
                        tackName = sOld1 + sOld3;
                    }
                    break;

                case Convert01.FORMAT.DOC2PDF:
                    {
                        tackName = "Word " + sOld2 + " PDF";
                    }
                    break;

                case Convert01.FORMAT.PPT2PDF:
                    {
                        tackName = "PPT " + sOld2 + " PDF";
                    }
                    break;
                case Convert01.FORMAT.PDFSplit:
                    {
                        tackName = "PDF Split " + sOld2 + " PDF";
                    }
                    break;
                case Convert01.FORMAT.PDFDecode:
                    {
                        tackName = "PDF Decode " + sOld2 + " PDF";
                    }
                    break;
                case Convert01.FORMAT.PDFMerge:
                    {
                        tackName = sOld4;
                    }
                    break;
                case Convert01.FORMAT.PDFCompress:
                    {
                        tackName = "PDF Compress " + sOld2 + " PDF";
                    }
                    break;
                case Convert01.FORMAT.Excel2PDF:
                    {
                        tackName = "Excel " + sOld2 + " PDF";
                    }
                    break;
                case Convert01.FORMAT.PDFGetImg:
                    {
                        tackName = "PDF Get " + sOld2 + " IMG";
                    }
                    break;
            }
            return tackName;
        }

        //停止转换
        private void btnStop_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                stopConversion();
            }
        }

        //开始转换
        private void btnStart_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (uctblPage.Text != string.Empty)
                {
                    comboBoxPage_Leave(sender, e);
                }

                //创建目录时如果目录已存在，则不会重新创建目录，且不会报错。创建目录时会自动创建路径中各级不存在的目录。(防止指定好的目录在转换之前被删除)                
                while (true)
                {
                    //如果 outFolder 为空,则说明线程onlyRunOnce()还没有运行结束,要等待它首先运行结束
                    if (string.IsNullOrEmpty(outFolder) == false)
                    {
                        break;
                    }
                }
                string activeDir = outFolder;// tbBrw.Text;
                string newPath = System.IO.Path.Combine(activeDir, "");
                System.IO.Directory.CreateDirectory(newPath);

                startConversion();//开始转换
            }
        }
        private void startConversion()
        {
            //KongMengyuan修改,2015-11-03,保留当前输出文件夹(在文件转换过程中不允许更改它)
            this.btnStart.BackgroundImage = Image.FromFile(spath + "btn_kszh04.png");

            if (lstFile.Items.Count < 1)//如果没有文件则不能点击
            {
                SetThreeButtonValidate(false);//开始转换,停止转换,清空列表,三个按钮的有效还是失效: true-有效,false-失效
                return;
            }

            //KongMengyuan修改,2015-11-03,参考互盾PDFCon
            if (btnStart.Enabled == false)
            {
                return;
            }

            //检查窗体打开的初始化线程是否结束(如果在打开窗体后立刻执行转换就会产生错误,要等待初始化的进程先结束)
            while (true)
            {
                //如果 outFolder 为空,则说明线程onlyRunOnce()还没有运行结束,要等待它首先运行结束
                if (string.IsNullOrEmpty(outFolder) == false)
                {
                    break;
                }
            }

            DirectoryInfo d = new DirectoryInfo(outFolder);
            //if (d.Exists == false) //KongMengyuan修改,2015-11-04,由于WinXP和纯净版的Win7对于Directory.Exists不起作用,所以使用另外一种写法(因为文件打开时是删除不了文件夹的)
            //if (!Directory.Exists(d.ToString())) //这种写法在WinXP和纯净版Win7也不识别(但是雨林木风的Ghost就可以识别这种写法)
            string sOld = string.Empty;
            if (d.ToString().Length < 1)
            {
                sOld = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_15"); //您选择的输出文件夹不存在
                MessageBox.Show(sOld);
                return;
            }
            if (lstFile.Items.Count < 1)
            {
                sOld = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_16"); //请先添加文件
                MessageBox.Show(sOld);
                return;
            }

            if (format == Convert01.FORMAT.PDFMerge)
            {
                if (lstFile.Items.Count < 2)
                {
                    sOld = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_17"); //请添加2个或2个以上文件进行合并
                    MessageBox.Show(sOld);
                    return;
                }
            }

            bool resetPoped = false;
            //检查是否有未完成的任务
            if (lstFile.Items.Count > 0)
            {
                resetPoped = true;
                bool ok = false;
                foreach (ListViewItem lv in lstFile.Items)
                {
                    string suffix = lv.SubItems[1].Text;
                    ItemInfomation Info = ((ItemInfomation)lv.Tag);

                    if (Info.Status != StatusType.Done)
                    {
                        ok = true;
                    }
                    if (Info.Status == StatusType.Pause)
                    {
                        resetPoped = false;
                    }
                }
                if (!ok)
                {
                    //开始转换,停止转换,这两个按钮失效,清空列表有效
                    this.btnStart.Enabled = false;
                    this.btnStop.Enabled = false;
                    this.btnStart.BackgroundImage = Image.FromFile(spath + "btn_kszh04.png");//开始转换
                    this.btnStop.BackgroundImage = Image.FromFile(spath + "btn_tzzh04.png"); //停止转换

                    sOld = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_18"); //任务已经全部完成
                    MessageBox.Show(sOld);
                    return;
                }
            }

            if (resetPoped)
            {
                Poped = false;
                NeedToPop = false;
            }
            isStart = true;
            this.pltext.Visible = false;
            //if (lstFile.Items.Count > 0 && this.lstFile.IsAllFinished==true)
            //2016-02-26，KongMengYuan修改
            if (lstFile.Items.Count > 0)
            {
                this.btnStart.Enabled = false;
                this.btnStart.BackgroundImage = Image.FromFile(spath + "btn_kszh04.png");
            }
            for (int i = 0; i < lstFile.Items.Count; i++)
            {
                fileQueue.Enqueue(this.lstFile.Items[i]);
            }

            //MessageBox.Show(thread.Length.ToString());//2015-12-14,KongMengyuan注释,此处在WinXP_64位上面点击“开始转换”时出错
            //2015-12-16,KongMengyuan增加,目前默认是只转换CPU个数少1核的进程数,发现虚拟机里“WinXP 64位”点击“开始转换”时此处出错,跟踪发现是"thread.length"不识别(编译时"目标平台"为X86和X64也都出错)。
            int threadLength = cpuNumber;
            //检查窗体打开的初始化线程是否结束(如果在打开窗体后立刻执行转换就会产生错误,要等待初始化的进程先结束)
            //while (true)
            //{
            //    try
            //    {
            //        threadLength = thread.Length;
            //    }
            //    catch
            //    {
            //        Thread.Sleep(500);
            //        //threadLength = 0; //设置为0就不开始转换了,所以此处就没有用了
            //    }
            //}
            //检查窗体打开的初始化线程是否结束(如果在打开窗体后立刻执行转换就会产生错误,要等待初始化的进程先结束)
            while (true)
            {
                //如果 outFolder 为空,则说明线程onlyRunOnce()还没有运行结束,要等待它首先运行结束
                if (string.IsNullOrEmpty(outFolder) == false || string.Equals(outFolder, "") == false)
                {
                    break;
                }
            }
            for (int j = 0; j < threadLength; j++) //2015-12-14,KongMengyuan注释,目前默认是只转换CPU个数少1核的进程数,发现虚拟机里“WinXP 64位”点击“开始转换”时此处出错,跟踪发现是"thread.length"不识别(编译时"目标平台"为X86和X64也都出错)。
            {
                //检查窗体打开的初始化线程是否结束(如果在打开窗体后立刻执行转换就会产生错误,要等待初始化的进程先结束)
                while (true)
                    try
                    {
                        if (thread[j] == null) //此处线程检测只有这种写法是正确的,检测thread[j].ThreadState是不正确的
                        {
                            Thread.Sleep(500);
                        }
                        else
                        {
                            break;
                        }
                    }
                    catch
                    {
                        Thread.Sleep(500);
                    }

                //Win8的64位和Win10的64位(32位的未测试)此处不是Stopped而是Aborted(但Win7的64位,考虑问题要全面,但V6.2就全部通过,也很奇怪),KongMengyuan注释,2016-03-04
                //if (thread[j].ThreadState == System.Threading.ThreadState.Stopped)
                if (thread[j].ThreadState == System.Threading.ThreadState.Stopped || thread[j].ThreadState == System.Threading.ThreadState.Aborted || thread[j].ThreadState == System.Threading.ThreadState.Unstarted)
                {
                    Console.WriteLine("Thread:" + j);
                    thread[j] = new Thread(new ParameterizedThreadStart(WorkThread));
                    thread[j].IsBackground = true;
                    thread[j].Start(j);
                }
            }
        }

        private void stopConversion()
        {
            if (lstFile.Items.Count < 1)//如果没有文件则不能点击
            {
                SetThreeButtonValidate(false);//开始转换,停止转换,清空列表,三个按钮的有效还是失效: true-有效,false-失效
                return;
            }
            this.btnStop.BackgroundImage = Image.FromFile(spath + "btn_tzzh03.png");
            //2015-12-16,KongMengyuan增加,目前默认是只转换CPU个数少1核的进程数,发现虚拟机里“WinXP 64位”点击“开始转换”时此处出错,跟踪发现是"thread.length"不识别(编译时"目标平台"为X86和X64也都出错)。
            int threadLength = cpuNumber;
            //由于是多线程的,下面这段加上就会死机了, 此处不能加下面这段,2015-01-11
            ////检查窗体打开的初始化线程是否结束(如果在打开窗体后立刻执行转换就会产生错误,要等待初始化的进程先结束)
            //while (true)
            //{
            //    try
            //    {
            //        threadLength = thread.Length;
            //    }
            //    catch
            //    {
            //        //threadLength = 0; //设置为0就不开始转换了,所以此处就没有用了
            //        Thread.Sleep(500);
            //    }
            //}
            //检查窗体打开的初始化线程是否结束(如果在打开窗体后立刻执行转换就会产生错误,要等待初始化的进程先结束)
            while (true)
            {
                //如果 outFolder 为空,则说明线程onlyRunOnce()还没有运行结束,要等待它首先运行结束
                if (string.IsNullOrEmpty(outFolder) == false || string.Equals(outFolder, "") == false)
                {
                    break;
                }
            }

            for (int j = 0; j < threadLength; j++) //2015-12-14,KongMengyuan注释,目前默认是只转换CPU个数少1核的进程数,发现虚拟机里“WinXP 64位”点击“开始转换”时此处出错,跟踪发现是"thread.length"不识别(编译时"目标平台"为X86和X64也都出错)。
            {
                try
                {
                    thread[j].Abort();
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }
            this.btnStart.Enabled = true;
            this.btnStart.BackgroundImage = Image.FromFile(spath + "btn_kszh01.png");
        }

        private void pbReg_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (string.IsNullOrEmpty(Program.httpMachineCode))
                {
                    return;
                }
                //检查窗体打开的初始化线程是否结束(如果在打开窗体后立刻执行转换就会产生错误,要等待初始化的进程先结束)
                while (true)
                {
                    //如果 outFolder 为空,则说明线程onlyRunOnce()还没有运行结束,要等待它首先运行结束
                    if (string.IsNullOrEmpty(outFolder) == false || string.Equals(outFolder, "") == false)
                    {
                        break;
                    }
                    else
                    {
                        //System.Threading.Thread.Sleep(2000); //此句不起作用
                        return;
                    }
                }
                this.pbReg.BackgroundImage = Image.FromFile(spath + "btn_reg_04.png");
                RegDlg01 frm = new RegDlg01();
                frm.StartPosition = FormStartPosition.CenterParent;
                frm.Location = this.PointToScreen(new Point(250, lstFile.Location.Y - 10));
                frm.ShowDialog();
                if (Program.dialogClose == false) //KongMengyuan增加,2015-11-11,判断注册页面窗体是直接关闭还是激活后关闭
                {
                    return;
                }
                if (new reg().Is_Reg())
                {
                    isReg = true;
                    this.plTop.BackgroundImage = Image.FromFile(spath + "header_02.png");
                    this.plTop.Tag = "Genuine Version"; //正式版本
                }
                else
                {
                    this.plTop.BackgroundImage = Image.FromFile(spath + "header_01.png");
                    this.plTop.Tag = "Free Trial"; //免费试用版本
                    isReg = false;
                }
            }
        }

        public void PostURL(TempUrl obj)
        {
            this.syncContext.Post(URL, obj);
        }

        public void URL(object obj)
        {
            if (obj != null)
            {
                TempUrl url = obj as TempUrl;
                Version.Post("http://all.jsocr.com/", Version.GetParamName("Data"), Program.appProgName, Version.GetParamName("Version"), Program.encodingCode, url.Target, url.MehodObject);
            }
        }

        private void lstFile_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 46)//回车键13空格键32删除键46
            {
                //    int iCount = lstFile.SelectedItems.Count;
                //    if (iCount>0)
                //    {
                //        for (int j = 0; j <= iCount; j++)
                //        {
                //            int index = j.
                //            ItemInfomation Info = ((ItemInfomation)lstFile.Items[index].Tag);
                //            diclst.Remove(Info.FileFullPath);
                //            if (dicThreadManagement.Count > 0 && dicThreadManagement.ContainsKey(Info.FileFullPath))
                //            {
                //                int i = dicThreadManagement[Info.FileFullPath];
                //                //dicThreadManagement.Remove(Info.FileFullPath);
                //                //终止当前线程
                //                thread[i].Abort();
                //            }

                //            //KongMengyuan增加,2015-11-17
                //            string sTip = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_01"); //提示
                //            string sOld = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_08"); //是否删除文件
                //            if (MessageBox.Show(sOld + "\r\n" + "\r\n" + lstFile.SelectedItems[0].SubItems[1].Text, sTip, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                //            {
                //                this.lstFile.RemoveFile(index);
                //            }
                //            else
                //            {
                //                return;
                //            }

                //            //KongMengyuan修改,20015-11-06,注释掉下面这段就可以了.注释原因：添加大量文件后，开始转换，转换到一半停止转换，再点击删除其中的一个文件。文件被删除，同时从未转换开始的文件开始转换（已经暂停的不自动转换）。
                //            ////开启新线程
                //            //for (int j = 0; j < thread.Length; j++)
                //            //{
                //            //    if (thread[j].ThreadState == System.Threading.ThreadState.Stopped)
                //            //    {
                //            //        thread[j] = new Thread(new ParameterizedThreadStart(WorkThread));
                //            //        thread[j].IsBackground = true;
                //            //        thread[j].Start(j);
                //            //    }
                //            //}


                //            // dicThreadManagement.Remove(Info.FileFullPath);
                //            if (lstFile.IsAllFinished)
                //            {
                //                btnStart.Enabled = true;
                //                btnStart.BackgroundImage = Image.FromFile(spath + "btn_kszh01.png");
                //            }

                //            backGroundShowHide();//隐藏或显示主界面提示图
                //        }
                //    }                
            }
        }

        //返回文件名的前后部分,KongMengyuan,2016-10-20 isFrontName：True-文件名的前部分,False-文件名的后部分
        private string returnFileName(string fileNameAll, bool isFrontName)
        {
            //string fileNameAll = lstFile.SelectedItems[0].SubItems[1].Text;
            int filenameStart = fileNameAll.LastIndexOf(".");
            //只获取纯文件名(不带扩展名)
            string fileName = fileNameAll.Substring(0, filenameStart);
            //只获取扩展名
            string expandName = fileNameAll.Substring(filenameStart + 1, fileNameAll.Length - filenameStart - 1);
            if (isFrontName)
                return fileName;
            else
                return expandName;
        }

        private void lstFile_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.lstFile.SelectedItems.Count <= 0)
            {
                this.lstFile.ContextMenuStrip = null;
            }
            else
            {
                this.lstFile.ContextMenuStrip = this.contextMenuStrip1;
            }
        }

        private void tsmiStart_Click(object sender, EventArgs e)
        {
            startConversion();
        }

        private void tsmiPageSet_Click(object sender, EventArgs e)
        {
            //string expendFileName = returnFileName(lstFile.SelectedItems[0].SubItems[1].Text, false);
            //if (expendFileName.ToLower() == "pdf")
            {
                if (this.lstFile.SelectedItems.Count > 0)
                {
                    if (format == Convert01.FORMAT.PDFSplit) // PDF分割
                    {
                        frmMain01Setting01 frm = new frmMain01Setting01();
                        frm.StartPosition = FormStartPosition.CenterParent;
                        frm.Tag = ""; //借用它来传送窗体变量,先清空它
                        frm.pageCount = System.Convert.ToInt32(lstFile.SelectedItems[0].SubItems[2].Text);
                        frm.AccessibleName = lstFile.SelectedItems[0].SubItems[1].Text;
                        DialogResult dr = frm.ShowDialog();
                        if (dr == System.Windows.Forms.DialogResult.OK)
                        {
                            if (frm.Tag.ToString() == "")
                            {
                                lstFile.SelectedItems[0].SubItems[3].Text = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_04"); //中文: 全部, 英文: All
                            }
                            else
                            {
                                lstFile.SelectedItems[0].SubItems[3].Text = frm.Tag.ToString();
                            }
                        }
                    }

                    if (format == Convert01.FORMAT.PDFMerge) // PDF合并
                    {
                        frmMain01Setting02 frm = new frmMain01Setting02();
                        frm.StartPosition = FormStartPosition.CenterParent;
                        frm.Tag = ""; //借用它来传送窗体变量,先清空它
                        frm.pageCount = System.Convert.ToInt32(lstFile.SelectedItems[0].SubItems[2].Text);
                        frm.AccessibleName = lstFile.SelectedItems[0].SubItems[1].Text;
                        DialogResult dr = frm.ShowDialog();
                        if (dr == System.Windows.Forms.DialogResult.OK)
                        {
                            if (frm.Tag.ToString() == "")
                            {
                                lstFile.SelectedItems[0].SubItems[3].Text = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_04"); //中文: 全部, 英文: All
                            }
                            else
                            {
                                lstFile.SelectedItems[0].SubItems[3].Text = frm.Tag.ToString();
                            }
                        }
                    }
                    if (format == Convert01.FORMAT.File2WORD // 文件转Word
                        || format == Convert01.FORMAT.File2EXCEL // 文件转Excel
                        || format == Convert01.FORMAT.File2PPT // 文件转PPT
                        || format == Convert01.FORMAT.File2HTML // 文件转HTML
                        || format == Convert01.FORMAT.File2TXT // 文件转TXT
                        || format == Convert01.FORMAT.File2IMG // 文件转图片
                        || format == Convert01.FORMAT.DOC2PDF // Word转PDF
                        || format == Convert01.FORMAT.Excel2PDF // Excel转PDF
                        || format == Convert01.FORMAT.PPT2PDF // PPT转PDF
                        || format == Convert01.FORMAT.PDFGetImg // PDF图片获取
                        )
                    {
                        frmMain01Setting03 frm = new frmMain01Setting03();
                        frm.StartPosition = FormStartPosition.CenterParent;
                        frm.Tag = ""; //借用它来传送窗体变量,先清空它
                        frm.pageCount = System.Convert.ToInt32(lstFile.SelectedItems[0].SubItems[2].Text);
                        frm.AccessibleName = lstFile.SelectedItems[0].SubItems[1].Text;
                        DialogResult dr = frm.ShowDialog();
                        if (dr == System.Windows.Forms.DialogResult.OK)
                        {
                            if (frm.Tag.ToString() == "")
                            {
                                lstFile.SelectedItems[0].SubItems[3].Text = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_04"); //中文: 全部, 英文: All
                            }
                            else
                            {
                                lstFile.SelectedItems[0].SubItems[3].Text = frm.Tag.ToString();
                            }
                        }
                    }
                }
            }
            //else
            //{
            //    //文件转Excel,提取图片
            //}
        }

        private void tsmiClear_Click(object sender, EventArgs e)
        {
            clearListView();//清空列表,因为是公用的,所以写成函数
        }

        private void clearListView()
        {
            if (lstFile.Items.Count < 1)//如果没有文件则不能点击
            {
                SetThreeButtonValidate(false);//开始转换,停止转换,清空列表,三个按钮的有效还是失效: true-有效,false-失效
                return;
            }
            ClearListTips01 frm = new ClearListTips01();
            frm.StartPosition = FormStartPosition.CenterParent;
            frm.Location = this.PointToScreen(new Point(400, this.lstFile.Location.Y + 30));
            DialogResult dr = frm.ShowDialog();
            if (dr == System.Windows.Forms.DialogResult.OK)
            {
                diclst.Clear();
                this.lstFile.Items.Clear();
                fileQueue.Clear();
                //this.comboBoxPage.Visible = false;
                this.btnClear.Enabled = true;
                this.btnClear.BackgroundImage = Image.FromFile(spath + "btn_qklb01.png");
                this.pltext.Visible = false;
                //2015-12-16,KongMengyuan增加,目前默认是只转换CPU个数少1核的进程数,发现虚拟机里“WinXP 64位”点击“开始转换”时此处出错,跟踪发现是"thread.length"不识别(编译时"目标平台"为X86和X64也都出错)。
                int threadLength = cpuNumber;
                //检查窗体打开的初始化线程是否结束(如果在打开窗体后立刻执行转换就会产生错误,要等待初始化的进程先结束)
                while (true)
                {
                    try
                    {
                        threadLength = thread.Length;
                        break;
                    }
                    catch
                    {
                        Thread.Sleep(500);
                        //threadLength = 0; //设置为0就不开始转换了,所以此处就没有用了
                    }
                }

                //检查窗体打开的初始化线程是否结束(如果在打开窗体后立刻执行转换就会产生错误,要等待初始化的进程先结束)
                while (true)
                {
                    //如果 outFolder 为空,则说明线程onlyRunOnce()还没有运行结束,要等待它首先运行结束
                    if (string.IsNullOrEmpty(outFolder) == false || string.Equals(outFolder, "") == false)
                    {
                        break;
                    }
                }

                for (int i = 0; i < threadLength; i++) //2015-12-14,KongMengyuan注释,目前默认是只转换CPU个数少1核的进程数,发现虚拟机里“WinXP 64位”点击“开始转换”时此处出错,跟踪发现是"thread.length"不识别(编译时"目标平台"为X86和X64也都出错)。
                {
                    thread[i].Abort();
                }
            }

            backGroundShowHide();//隐藏或显示主界面提示图
        }

        private void tsmiStop_Click(object sender, EventArgs e)
        {
            stopConversion();
        }

        private void tsmiPicBMP_Click(object sender, EventArgs e)
        {
            Convert01.pictureFormat = Convert01.picFormat.picFormatBMP;
        }

        private void tsmiPicEMF_Click(object sender, EventArgs e)
        {
            Convert01.pictureFormat = Convert01.picFormat.picFormatEMF;
        }

        private void tsmiPicGIF_Click(object sender, EventArgs e)
        {
            Convert01.pictureFormat = Convert01.picFormat.picFormatGIF;
        }

        private void tsmiPicJPG_Click(object sender, EventArgs e)
        {
            Convert01.pictureFormat = Convert01.picFormat.picFormatJPG;
        }

        private void tsmiPicPNG_Click(object sender, EventArgs e)
        {
            Convert01.pictureFormat = Convert01.picFormat.picFormatPNG;
        }

        private void tsmiPicTIF_Click(object sender, EventArgs e)
        {
            Convert01.pictureFormat = Convert01.picFormat.picFormatTIF;
        }

        private void tsmiPicWMF_Click(object sender, EventArgs e)
        {
            Convert01.pictureFormat = Convert01.picFormat.picFormatWMF;
        }

        private void plBottom_MouseMove(object sender, MouseEventArgs e)
        {
            //cursorPosition(); //显示当前坐标位置,在编程时定位控件坐标位置使用,正式使用之前将其注释掉
        }
        //显示当前坐标位置,在编程时定位控件坐标位置使用,正式使用之前将其注释掉
        private void cursorPosition()
        {
            lblCursorPosition.Visible = true;
            Point pt = this.PointToClient(Control.MousePosition); //将指定屏幕点的位置计算成工作区坐标
            string sPos = pt.X.ToString() + "," + pt.Y.ToString();
            lblCursorPosition.Text = sPos;
            //如果不需要光标跟随显示,就直接把下边两行注释掉(如果位置在最下边或最右边,可能会显示在屏幕外面)
            //lblCursorPosition.Left = pt.X + 10;
            //lblCursorPosition.Top = pt.Y + 20;
        }

        private void test_frmAutoUpdate01(object sender, EventArgs e)
        {
            //测试额外frmAutoUpdate01专用,需增加一个测试按钮
            //private void button1_Click(object sender, EventArgs e)
            //{
            //    test_frmAutoUpdate01(sender, e);
            //}
            frmAutoUpdate01 frm = new frmAutoUpdate01();
            //frm.TopMost = true; //异步的窗口ShowDialog是不会阻塞主窗口的，你只能在主窗口线程创建。使用这个属性就可以把窗体一直放在所有窗体的最前面,这样也可以起到这种效果
            frm.StartPosition = FormStartPosition.CenterParent;//放置于父窗体中间位置
            frm.Location = this.PointToScreen(new Point(400, this.lstFile.Location.Y + 30));
            if (MessageBox.Show("调试1个按钮还是2个按钮: Yes-1,No-2", "测试frmAutoUpdate窗体是否显示正常", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
            {
                frm.cancelShow = true; //是否显示“跳过”按钮,在调试时要注意把它修改为false进行调试
            }
            DialogResult dr = frm.ShowDialog(); //此处代码的目的是
        }

        private void lstFile_SelectedIndexChanged(object sender, EventArgs e)
        {
            //增加这段代码的原因：1、测试人员候福涛先点中某一行比如第2行,点击"页码选择,会出现一个输入框,不再次点击让用户输入,而是直接按键盘的上下键移动鼠标到其它行比如第7行 2、鼠标直接再点击原来第2行的"页码选择"输入框输入数据。结果：输入的数据库变成了第7行的数据(按道理这种情况是对的,因为输入框的数据是针对选择行的,而不是当前所在行的,但是要避免这种情况)
            //避免选择行数使用键盘变化
            if (selectLineChange == 0)
            {
                selectLineChange = lstFile.SelectedItems[0].Index;
            }
            if (selectLineChange != lstFile.SelectedItems[0].Index && selectLineChange != 0)
            {
                if (pltext.Visible)
                {
                    pltext.ResetText();
                    pltext.Visible = false;
                }
                selectLineChange = lstFile.SelectedItems[0].Index;
            }
        }

        private void lblTestAutoUpdate_Click(object sender, EventArgs e)
        {
            //安装之前要注释,测试自动更新界面,便于方便程序员调试程序,KongMengyuan(全文搜索这句话,在发布之前全部去掉注释)
            DialogResult dr = MessageBox.Show("测试人员专用(不需要翻译成多语言): \r\n  是-1个按钮 \r\n  否-2个按钮", "", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            switch (dr.ToString())
            {
                case "Yes":
                    frmAutoUpdate01 frm1 = new frmAutoUpdate01();
                    frm1.cancelShow = false;
                    frm1.StartPosition = FormStartPosition.CenterParent;//放置于父窗体中间位置
                    frm1.Location = this.PointToScreen(new Point(400, this.lstFile.Location.Y + 30));
                    frm1.ShowDialog();
                    break;
                case "No":
                    frmAutoUpdate01 frm2 = new frmAutoUpdate01();
                    frm2.cancelShow = true;
                    frm2.StartPosition = FormStartPosition.CenterParent;//放置于父窗体中间位置
                    frm2.Location = this.PointToScreen(new Point(400, this.lstFile.Location.Y + 30));
                    frm2.ShowDialog();
                    break;
                default:
                    RegTips01 frm3 = new RegTips01();
                    frm3.StartPosition = FormStartPosition.CenterParent;//放置于父窗体中间位置
                    frm3.Location = this.PointToScreen(new Point(400, this.lstFile.Location.Y + 30));
                    frm3.ShowDialog();
                    break;

            }
        }

        private void lblTestDownloadFile_Quiet_Click(object sender, EventArgs e)
        {
            Program.httpVersion = "6.0";
            DownloadFile_Quiet();
        }

    }

    public class TempClass
    {
        public int index { get; set; }
        public int cur { get; set; }

        public TempClass(int index, int cur)
        {
            this.index = index;
            this.cur = cur;
        }
    }

    public class TempUrl
    {
        public string Target { get; set; }
        public string MehodObject { get; set; }

        public TempUrl(string Target, string MehodObject)
        {
            this.Target = Target;
            this.MehodObject = MehodObject;
        }
    }

}
