using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using System.Threading;
using System.Runtime.InteropServices; //运行Windows的API,引用DLL专用
using System.Diagnostics; //Process引用时

namespace PDF_Convert
{
    public partial class RegDlg01 : Form
    {
        [DllImport("user32")]
        public static extern int ReleaseCapture();//用鼠标拖动窗体,移动窗体
        [DllImport("user32")]
        public static extern int SendMessage(IntPtr hwnd, int msg, int wp, int lp);//用鼠标拖动窗体,移动窗体

        private string iniLanguage = string.Empty; //读取Config.ini文件里面的language
        private string spath01 = string.Empty;
        ini_config ini = new ini_config("config.ini");

        public RegDlg01()
        {
            //引入多语言
            iniLanguage = ini.read_ini("language");
            if (string.IsNullOrEmpty(iniLanguage))
                iniLanguage = System.Globalization.CultureInfo.InstalledUICulture.Name;

            InitializeComponent();
        }

        private void pbRegCodeGet_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                this.pbRegCodeGet.BackgroundImage = Image.FromFile(spath01 + "RegCodeGet_03.png");

                try
                {
                    Process.Start("http://www.xjpdf.com/pdfcode");
                }
                catch
                {
                    Process.Start("iexplore.exe", "http://www.xjpdf.com/pdfcode");
                }
            }
        }

        private void pbRegCodeGet_MouseEnter(object sender, EventArgs e)
        {
            if (((Control)sender).Enabled != false)
            {
                this.pbRegCodeGet.BackgroundImage = Image.FromFile(spath01 + "RegCodeGet_02.png");
            }
        }

        private void pbRegCodeGet_MouseLeave(object sender, EventArgs e)
        {
            if (((Control)sender).Enabled != false)
            {
                this.pbRegCodeGet.BackgroundImage = Image.FromFile(spath01 + "RegCodeGet_01.png");
            }
        }

        private void pbActive_MouseClick(object sender, MouseEventArgs e)
        {
            //写入注册码失败! 把config.ini改为只读，然后点击注册会产生这种错误。改进的方法是：再次注册，或者把注册码直接写入config.ini的“RegCode=”后面。先把config.ini用Word打开，再向config.ini写任何内容都不可以（这种情况是只读的）。
            //郑侃炜：你查下文档,微软有推荐配置文件的写出目录,往其目录写的话是没有权限问题的
            //测试用例：
            //   1、Administrator安装,普通用户User1使用 
            //   2、普通用户User1安装,Administrator使用 
            //   3、普通用户User1安装,普通用户User2使用 
            //   4、找一个写入config.ini的地方(比如主界面的“设置输出目录”),设置一下,让config.ini会变化,再在此处点击"开始激活"
            //可能的Bug
            //Administrator安装,普通用户User1使用,正常
            //普通用户User1安装,Administrator使用,输入注册码后,其它所有用户都可使用
            //普通用户User1安装,普通用户User2使用,不输入注册码,为免费版
            //普通用户User2安装,普通用户User1使用,不输入注册码,为正式版,奇怪吧(这里要测试是为什么)
            //Win10 64位安装完成后,目录是只读的,所以config.ini也就不可以修改了(但其它系统就不成问题)。在Win10 64位的操作系统里面,如果文件夹是只读的,里面的文件也不可以修改。Win10的普通用户安装的软件,对于config.ini等任何文件没有写入权限(可以右击文件---属性---安全---编辑,增加当前用户后,再所"写入"打上勾）

            //想重新变回试用码,只要把Release目录下面的config.ini里面的RegCode后面的数字清空就行了,KongMengyuan,2015-11-06           
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                registerCodeEnter();
            }
        }

        private void RegDlg01_Load(object sender, EventArgs e)
        {
            this.Refresh();
            this.Text = Program.appProgName;
            txtMachineCode.Text = Program.httpMachineCode;
            this.txtMachineCode.ReadOnly = true;//机器码是自动生成的,不允许手工输入
            this.txtMachineCode.BackColor = System.Drawing.Color.White; //TextBox如果设置为ReadOnly=true,同时窗体是透明的,那么它也是透明的,所以要把背景颜色重新设置一下
            //this.Icon = new Icon(Image.FromFile(Application.StartupPath + "images\\logo.ico"));
            switch (iniLanguage.ToLower())
            {
                case "zh-cn":
                    SetZhCn();
                    break;
                case "en":
                    SetEn();
                    break;
                case "ja":
                    SetJa();
                    break;
                case "zh-tw":
                    SetZhTw();
                    break;
                default:
                    SetZhCn();
                    break;
            }

            this.FormBorderStyle = 0; //窗体无边框
            this.BackColor = System.Drawing.SystemColors.Control; this.TransparencyKey = this.BackColor; //两者设置相同颜色,即可让窗体透明 
            this.StartPosition = FormStartPosition.CenterParent;

            if (MainInfo01.isReg)
            {
                string regCode = ini.read_ini("RegCode");
                string code = string.Empty;
                if (!string.IsNullOrEmpty(regCode))
                {

                    for (int i = 0; i < regCode.Length; i++)
                    {
                        code += "*";
                    }
                }
                else
                {
                    //加一个默认的数值,因为英文版免费,所以不需要输入注册码
                    code = "********";
                }
                txtRegCode.Text = code;
                txtRegCode.Enabled = false;
                if (this.txtRegCode.Text != "")
                {
                    this.pbActive.BackgroundImage = Image.FromFile(spath01 + "Active_04.png"); //不可点击
                    this.pbActive.Enabled = false;
                }
            }
            else
            {
                //this.txtRegCode.PasswordChar = '*'; //单引号表示Char型
                this.txtRegCode.PasswordChar = System.Convert.ToChar(0);
            }
        }

        private void RegDlg01_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, 274, 61440 + 9, 0);
            }
        }

        /// <summary>
        /// 设置简体中文语言
        /// </summary>
        private void SetZhCn()
        {
            this.Size = new Size(494, 362);//窗体界面固定大小(防止程序员不小心改变了大小,因为是多语言界面,窗体大小是固定的)\
            spath01 = Application.StartupPath + "\\zh-CN\\RegDlg01\\";
            this.pbBuy.BackgroundImage = Image.FromFile(spath01 + "Buy_01.png");
            this.pbRegCodeGet.BackgroundImage = Image.FromFile(spath01 + "RegCodeGet_01.png");
            this.pbActive.BackgroundImage = Image.FromFile(spath01 + "Active_01.png");
            this.pbClose.BackgroundImage = Image.FromFile(Application.StartupPath + "\\images\\close_01.png"); //取消,窗体右上角关闭按钮
            if (!MainInfo01.isReg)
            {
                this.BackgroundImage = Image.FromFile(spath01 + "RegDlg_01.png"); //窗体背景
            }
            else
            {
                this.BackgroundImage = Image.FromFile(spath01 + "RegDlg_02.png"); //窗体背景
            }

            this.pbBuy.Location = new Point(50, 196); //购买正式版
            this.pbBuy.Size = new Size(124, 36);
            this.pbRegCodeGet.Location = new Point(185, 196); //获取注册码
            this.pbRegCodeGet.Size = new Size(124, 36);
            this.pbActive.Location = new Point(320, 196); //开始激活
            this.pbActive.Size = new Size(124, 36);

            this.txtMachineCode.Location = new Point(155, 112);
            this.txtRegCode.Location = new Point(155, 150);

            this.pbQQ.Location = new Point(31, 333);
            this.pbHelp.Location = new Point(200, 333);
            this.pbClose.Location = new Point(452, 12);

            //Thread.CurrentThread.CurrentUICulture = new CultureInfo("zh-CN");
            //this.BackgroundImage = Properties.Resources.dialog_clear;
            //this.label2.Text = rm.GetString("label2.Text"); //您确定要清除列表中所有项目吗？
            //this.btnConfirm.ButtonText = rm.GetString("btnConfirm.ButtonText"); //确定            

            //this.label2.Left = (int)(this.Width / 2 - this.label2.Width / 2);
            //this.label2.Location = new Point(46, 75);
            //this.btnConfirm.Location = new Point(46, 117);
            //this.btnOver.Location = new Point(193, 117);
        }

        /// <summary>
        /// 设置英文语言
        /// </summary>
        private void SetEn()
        {
            this.Size = new Size(520, 362);
            this.pbClose.Location = new Point(482, 12);
            spath01 = Application.StartupPath + "\\en\\RegDlg01\\";
            this.pbBuy.BackgroundImage = Image.FromFile(spath01 + "Buy_01.png");
            this.pbRegCodeGet.BackgroundImage = Image.FromFile(spath01 + "RegCodeGet_01.png");
            this.pbActive.BackgroundImage = Image.FromFile(spath01 + "Active_01.png");
            this.pbClose.BackgroundImage = Image.FromFile(Application.StartupPath + "\\images\\close_01.png"); //取消,窗体右上角关闭按钮
            if (!MainInfo01.isReg)
            {
                this.BackgroundImage = Image.FromFile(spath01 + "RegDlg_01.png"); //窗体背景
            }
            else
            {
                this.BackgroundImage = Image.FromFile(spath01 + "RegDlg_02.png"); //窗体背景
            }

            this.txtMachineCode.Location = new Point(165, 110);
            this.txtRegCode.Location = new Point(165, 148);

            this.pbBuy.Location = new Point(50, 196); //购买正式版
            this.pbBuy.Size = new Size(123, 36);
            this.pbRegCodeGet.Location = new Point(181, 196); //获取注册码
            this.pbRegCodeGet.Size = new Size(124, 36);
            this.pbRegCodeGet.Size = new System.Drawing.Size(153, 36);
            this.pbActive.Location = new Point(341, 196); //开始激活
            this.pbActive.Size = new Size(125, 35);

            this.pbQQ.Location = new Point(50, 323);
            this.pbQQ.Width = 120;
            this.pbHelp.Location = new Point(223, 323);
            this.pbHelp.Width = 50;

            //this.label2.Left = (int)(this.Width / 2 - this.label2.Width / 2);
            //this.label2.Location = new Point(46, 75);
        }

        /// <summary>
        /// 设置日文语言
        /// </summary>
        private void SetJa()
        {
            this.Size = new Size(582, 420);//窗体界面固定大小(防止程序员不小心改变了大小,因为是多语言界面,窗体大小是固定的)\
            spath01 = Application.StartupPath + "\\Ja\\RegDlg01\\";
            this.pbBuy.BackgroundImage = Image.FromFile(spath01 + "Buy_01.png");
            this.pbRegCodeGet.BackgroundImage = Image.FromFile(spath01 + "RegCodeGet_01.png");
            this.pbActive.BackgroundImage = Image.FromFile(spath01 + "Active_01.png");
            this.pbClose.BackgroundImage = Image.FromFile(Application.StartupPath + "\\images\\close_01.png"); //取消,窗体右上角关闭按钮
            if (!MainInfo01.isReg)
            {
                this.BackgroundImage = Image.FromFile(spath01 + "RegDlg_01.png"); //窗体背景
            }
            else
            {
                this.BackgroundImage = Image.FromFile(spath01 + "RegDlg_02.png"); //窗体背景
            }

            this.pbBuy.Location = new Point(60, 196); //购买正式版
            this.pbBuy.Size = new Size(142, 36);
            this.pbRegCodeGet.Location = new Point(222, 196); //获取注册码
            this.pbRegCodeGet.Size = new Size(150, 36);
            this.pbActive.Location = new Point(380, 196); //开始激活
            this.pbActive.Size = new Size(160, 36);

            this.txtMachineCode.Location = new Point(185, 105);
            this.txtMachineCode.Size = new System.Drawing.Size(350, 32);
            this.txtRegCode.Location = new Point(185, 145);
            this.txtRegCode.Size = new System.Drawing.Size(350, 32);
            this.pbQQ.Location = new Point(10, 385);
            this.pbQQ.Size = new System.Drawing.Size(180, 27);
            this.pbHelp.Location = new Point(210, 385);
            this.pbHelp.Size = new System.Drawing.Size(110, 27);
            this.pbClose.Location = new Point(545, 12);
        }

        /// <summary>
        /// 设置繁体中文
        /// </summary>
        private void SetZhTw()
        {
            this.Size = new Size(494, 362);//窗体界面固定大小(防止程序员不小心改变了大小,因为是多语言界面,窗体大小是固定的)\
            spath01 = Application.StartupPath + "\\zh-TW\\RegDlg01\\";
            this.pbBuy.BackgroundImage = Image.FromFile(spath01 + "Buy_01.png");
            this.pbRegCodeGet.BackgroundImage = Image.FromFile(spath01 + "RegCodeGet_01.png");
            this.pbActive.BackgroundImage = Image.FromFile(spath01 + "Active_01.png");
            this.pbClose.BackgroundImage = Image.FromFile(Application.StartupPath + "\\images\\close_01.png"); //取消,窗体右上角关闭按钮
            if (!MainInfo01.isReg)
            {
                this.BackgroundImage = Image.FromFile(spath01 + "RegDlg_01.png"); //窗体背景
            }
            else
            {
                this.BackgroundImage = Image.FromFile(spath01 + "RegDlg_02.png"); //窗体背景
            }
            //Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");
            //this.BackgroundImage = Properties.Resources.dialog_clear;
            //this.label2.Text = rm.GetString("label2.Text"); //您确定要清除列表中所有项目吗？
            //this.btnConfirm.ButtonText = rm.GetString("btnConfirm.ButtonText"); //确定
            //this.btnOver.ButtonText = rm.GetString("btnOver.ButtonText"); //取消

            //this.label2.Left = (int)(this.Width / 2 - this.label2.Width / 2);
            //this.label2.Location = new Point(46, 75);
            //this.btnConfirm.Location = new Point(46, 117);
            //this.btnOver.Location = new Point(193, 117);
        }

        private void pbActive_MouseEnter(object sender, EventArgs e)
        {
            this.pbActive.BackgroundImage = Image.FromFile(spath01 + "Active_02.png"); //取消
        }

        private void pbClose_MouseEnter(object sender, EventArgs e)
        {
            this.pbClose.BackgroundImage = Image.FromFile(Application.StartupPath + "\\images\\close_02.png"); //取消,窗体右上角关闭按钮
        }

        private void pbClose_MouseLeave(object sender, EventArgs e)
        {
            this.pbClose.BackgroundImage = Image.FromFile(Application.StartupPath + "\\images\\close_01.png"); //取消,窗体右上角关闭按钮
        }

        private void pbActive_MouseLeave(object sender, EventArgs e)
        {
            this.pbActive.BackgroundImage = Image.FromFile(spath01 + "Active_01.png"); //取消
        }

        private void pbClose_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        private void pbHelp_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                try
                {
                    Process.Start("http://www.xjpdf.com/software/pdfConvert/buy/?version=" + Program.httpVersion + "&machine=" + Program.httpMachineCode);
                }
                catch
                {
                    Process.Start("iexplore.exe", "http://www.xjpdf.com/software/pdfConvert/buy/?version=" + Program.httpVersion + "&machine=" + Program.httpMachineCode);
                }
                //发送请求信息 
                try
                {
                    Version.Post("http://all.jsocr.com/", Version.GetParamName("Data"), Program.appProgName, Version.GetParamName("Version"), new reg().get_reg_code(), "注册窗口", "购买正式版");
                }
                catch
                { }
            }
        }

        private void txtMachineCode_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                Clipboard.SetText(txtMachineCode.Text); //把文本复制到粘贴板
                //Clipboard.SetImage(Image.FromFile("d:\\a.jpg"));//把图片复制到剪贴板
                string msg01 = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_01"); //提示
                string msg02 = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_05"); //已复制到粘贴板,可在其它地方直接粘贴使用
                MessageBox.Show(msg02, msg01, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void txtRegCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13)//回车键13空格键32,如果按了回车键就把窗体关闭
            {
                //MouseEventArgs n;
                //pbRegCodeGet_MouseClick(sender, n);
                registerCodeEnter();
            }
        }

        private void registerCodeEnter()
        {
            //测试人员用,生成注册码,内部人员使用
            string regCode = new reg().get_reg_code("710519949"); //后面是测试用的本机序号
            Console.WriteLine(regCode);

            this.pbActive.BackgroundImage = Image.FromFile(spath01 + "Active_03.png");
            if (new reg().get_reg_code() == this.txtRegCode.Text)
            {
                ini_config ini = new ini_config("config.ini"); //这句话一定要加,如果不加的话,所有操作系统都可以写入,但是Win10 64位普通用户就不可以,保存时说文件只读(从外部修改也是只读的),加上这句话Win10也可以读写了,2015-12-25,KongMengyuan增加
                if (new reg().write_reg_code(this.txtRegCode.Text))
                {
                    string msg01 = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_01"); //提示
                    string msg02 = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_02"); //写入注册码成功!
                    MessageBox.Show(msg02, msg01, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //发送请求信息 
                    Version.Post("http://all.jsocr.com/", Version.GetParamName("Data"), Program.appProgName, Version.GetParamName("Version"), new reg().get_reg_code(), "注册窗口", "激活成功" + this.txtRegCode.Text);
                }
                else
                {
                    //MessageBox.Show("写入注册码失败!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    string msg01 = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_01"); //提示
                    string msg02 = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_03"); //写入注册码失败!
                    MessageBox.Show(msg02, msg01, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //发送请求信息 
                    Version.Post("http://all.jsocr.com/", Version.GetParamName("Data"), Program.appProgName, Version.GetParamName("Version"), new reg().get_reg_code(), "注册窗口", "激活失败" + this.txtRegCode.Text);
                }
            }
            else
            {
                string msg01 = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_01"); //提示
                string msg02 = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_04"); //您输入的注册码不正确，如果您无注册码，请及时购买！
                MessageBox.Show(msg02, msg01, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                try
                {
                    Process.Start("http://www.xjpdf.com/software/pdfConvert/buy/?version=" + Program.httpVersion + "&machine=" + Program.httpMachineCode);
                }
                catch
                {
                    Process.Start("iexplore.exe", "http://www.xjpdf.com/software/pdfConvert/buy/?version=" + Program.httpVersion + "&machine=" + Program.httpMachineCode);
                }
                return;
            }

            this.DialogResult = DialogResult.Abort;
            Program.dialogClose = true; //KongMengyuan增加,2015-11-11,判断注册页面窗体是直接关闭还是激活后关闭
            this.Close();
        }

        private void pbQQ_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                try
                {
                    Process.Start("http://www.xjpdf.com/software/pdfConvert/qq/?version=" + Program.httpVersion);
                }
                catch
                {
                    Process.Start("iexplore.exe", "http://www.xjpdf.com/software/pdfConvert/qq/?version=" + Program.httpVersion);
                }
            }
        }

        private void RegDlg01_MouseMove(object sender, MouseEventArgs e)
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

        private void pbBuy_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                this.pbBuy.BackgroundImage = Image.FromFile(spath01 + "Buy_03.png");

                try
                {
                    Process.Start("http://www.xjpdf.com/software/pdfConvert/buy/?version=" + Program.httpVersion + "&machine=" + Program.httpMachineCode);
                }
                catch
                {
                    Process.Start("iexplore.exe", "http://www.xjpdf.com/software/pdfConvert/buy/?version=" + Program.httpVersion + "&machine=" + Program.httpMachineCode);
                }

            }
        }

        private void pbBuy_MouseEnter(object sender, EventArgs e)
        {
            if (((Control)sender).Enabled != false)
            {
                this.pbBuy.BackgroundImage = Image.FromFile(spath01 + "Buy_02.png");
            }
        }

        private void pbBuy_MouseLeave(object sender, EventArgs e)
        {
            if (((Control)sender).Enabled != false)
            {
                this.pbBuy.BackgroundImage = Image.FromFile(spath01 + "Buy_01.png");
            }
        }

    }
}
