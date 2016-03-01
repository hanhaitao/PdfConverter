using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics; //Process引用时
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Runtime.InteropServices; //运行Windows的API,引用DLL专用
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace PDF_Convert
{
    public partial class RegDlgOld : Form
    {
        //KongMengyuan增加,郑总提出来：在点击右上角的“购买”时打开页面会卡一下,而点击右下角的"在线教程"就不卡,改变写法(提取机器码浪费了多余的时间)
        private string httpVersion = "";
        private string httpMachineCode = "";
        private string httpRegCode = "";

        ini_config ini = new ini_config("config.ini");
        private string iniLanguage = string.Empty; //读取Config.ini文件里面的language
        //public static ResourceManager rm = new ResourceManager(typeof(MainInfo)); //2015-12-17,KongMengyuan注释
        public static ResourceManager rm = new ResourceManager(typeof(RegDlgOld));

        [DllImport("user32")]
        public static extern int ReleaseCapture();//用鼠标拖动窗体,移动窗体
        [DllImport("user32")]
        public static extern int SendMessage(IntPtr hwnd, int msg, int wp, int lp);//用鼠标拖动窗体,移动窗体

        public RegDlgOld()
        {
            iniLanguage = ini.read_ini("language");
            if (string.IsNullOrEmpty(iniLanguage))
                iniLanguage = System.Globalization.CultureInfo.InstalledUICulture.Name;            
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(iniLanguage);
            InitializeComponent();
        }

        private void RegDlg_Load(object sender, EventArgs e)
        {
            //txtMachineCode.OutText = new reg().get_machine_code();
            txtMachineCode.OutText = Program.httpMachineCode;

            //this.btnReg.ButtonText = rm.GetString("Purchase");
            this.btnActive.ButtonText = rm.GetString("btnActive.Text");
            //this.labQQ.Text = rm.GetString("btnQQ");
            //this.labPhone.Text = rm.GetString("btnPhone");
            if (MainInfoOld.isReg)
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
                txtRegCode.OutText = code;
                txtRegCode.Enabled = false;
                //this.label2.Text = rm.GetString("Activated");
                this.btnActive.ButtonText = rm.GetString("Activated");
                ////this.btnReg.Visible = false;
                //this.label2.Visible = false;
                //label3.Text = "(已激活)";
                //label3.Location = new Point(240,82);
                //lblTitle.Location = new Point(200, 41);
                //this.btnActive.Enabled = false;

                if (this.txtRegCode.OutText != "")
                {
                    this.btnActive.Enabled = false;
                }

                txtMachineCode.BackImg = Properties.Resources.text;
                this.StartPosition = FormStartPosition.CenterParent;
            }
            switch (iniLanguage.ToLower())
            {
                case "zh-cn":
                    SetZhCn();
                    //pbLogo.BackgroundImage = Properties.Resources.logo_03;
                    //this.btnReg.Location = new Point(400, 106);
                    //this.btnQQ.ButtonImage = Properties.Resources.qqcn;
                    //this.btnQQ.Size = new Size(202, 38);
                    break;
                case "en":
                    //pbLogo.BackgroundImage = Properties.Resources.logo_05;
                    //this.btnReg.Location = new Point(430, 106);
                    //this.btnQQ.ButtonImage = Properties.Resources.emailen;
                    //this.btnQQ.Size = new Size(222, 27);
                    //this.btnQQ.Location = new Point(132, 311);
                    //this.btnQQ.FromType = 6;
                    //this.btnPhone.Location = new Point(368, 311);
                    //this.btnPhone.Size = new Size(278, 27);
                    SetEn();
                    break;
                default:
                    SetZhCn();
                    break;
            }

            //KongMengyuan增加,郑总提出来：在点击右上角的“购买”时打开页面会卡一下,而点击右下角的"在线教程"就不卡,改变写法(提取机器码浪费了多余的时间)
            httpVersion = Program.httpVersion;
            httpMachineCode = Program.httpMachineCode;
            httpRegCode = Program.httpRegCode;
            this.FormBorderStyle = 0; //窗体无边框
            this.BackColor = System.Drawing.SystemColors.Control; this.TransparencyKey = this.BackColor; //两者设置相同颜色,即可让窗体透明 
            //this.Opacity = 0.5;//窗体的透明度为50%

            this.Text = Program.appProgName; //窗体的快捷方式显示,任务栏的显示内容
        }

        private void pbClose_Click(object sender, EventArgs e)
        {
            Program.dialogClose = false; //KongMengyuan增加,2015-11-11,判断注册页面窗体是直接关闭还是激活后关闭
            this.Close();
        }

        private void btnActive_MouseEnter(object sender, EventArgs e)
        {
            this.btnActive.BackgroundImage = Properties.Resources.RegDlg_btn_dialog_ac_02;
        }
        private void btnActive_MouseLeave(object sender, EventArgs e)
        {
            this.btnActive.BackgroundImage = Properties.Resources.RegDlg_btn_dialog_ac_01;
        }

        private void btnActive_MouseClick(object sender, MouseEventArgs e)
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
                //测试人员用,生成注册码,内部人员使用
                string regCode = new reg().get_reg_code("710519949"); //后面是测试用的本机序号
                Console.WriteLine(regCode);

                this.btnActive.BackgroundImage = Properties.Resources.RegDlg_btn_dialog_ac_03;
                if (new reg().get_reg_code() == this.txtRegCode.OutText)
                {
                    ini_config ini = new ini_config("config.ini"); //这句话一定要加,如果不加的话,所有操作系统都可以写入,但是Win10 64位普通用户就不可以,保存时说文件只读(从外部修改也是只读的),加上这句话Win10也可以读写了,2015-12-25,KongMengyuan增加
                    if (new reg().write_reg_code(this.txtRegCode.OutText))
                    {
                        //MessageBox.Show("写入注册码成功!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        MessageBox.Show(rm.GetString("RegisteredSuccessfully"), rm.GetString("Tips"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                        //发送请求信息 
                        Version.Post("http://all.jsocr.com/", Version.GetParamName("Data"), Program.appProgName, Version.GetParamName("Version"), new reg().get_reg_code(), "注册窗口", "激活成功" + this.txtRegCode.OutText);
                    }
                    else
                    {
                        //MessageBox.Show("写入注册码失败!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        MessageBox.Show(rm.GetString("RegistrationFailed"), rm.GetString("Tips"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                        //发送请求信息 
                        Version.Post("http://all.jsocr.com/", Version.GetParamName("Data"), Program.appProgName, Version.GetParamName("Version"), new reg().get_reg_code(), "注册窗口", "激活失败" + this.txtRegCode.OutText);
                    }
                }
                else
                {
                    //MessageBox.Show("您输入的注册码不正确，如果您无注册码，请及时购买！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    MessageBox.Show(rm.GetString("msg13"), rm.GetString("Tips"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //Process.Start("iexplore.exe", "http://www.xjpdf.com/software/pdfConvert/buy/?version=" + Version.version + "&machine=" + new reg().get_machine_code()); //KongMengyuan增加,2015-11-04,常规作法是转到购买页面
                    //KongMengyuan修改,郑总提出来：在点击右上角的“购买”时打开页面会卡一下,而点击右下角的"在线教程"就不卡,改变写法(提取机器码浪费了多余的时间)
                    Process.Start("iexplore.exe", "http://www.xjpdf.com/software/pdfConvert/buy/?version=" + httpVersion + "&machine=" + httpMachineCode);
                    return;
                }

                this.DialogResult = DialogResult.Abort;
                Program.dialogClose = true; //KongMengyuan增加,2015-11-11,判断注册页面窗体是直接关闭还是激活后关闭
                this.Close();
            }
        }

        private void btnReg_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                //Process.Start("http://www.xjpdf.com/software/pdfConvert/buy/?version=" + Version.version + "&machine=" + new reg().get_machine_code());
                try
                {
                    //Process.Start("http://www.xjpdf.com/software/pdfConvert/buy/?version=" + Version.version + "&machine=" + new reg().get_machine_code());
                    //KongMengyuan修改,郑总提出来：在点击右上角的“购买”时打开页面会卡一下,而点击右下角的"在线教程"就不卡,改变写法(提取机器码浪费了多余的时间)
                    Process.Start("http://www.xjpdf.com/software/pdfConvert/buy/?version=" + httpVersion + "&machine=" + httpMachineCode);
                }
                catch
                {
                    //Process.Start("iexplore.exe", "http://www.xjpdf.com/software/pdfConvert/buy/?version=" + Version.version + "&machine=" + new reg().get_machine_code());
                    //KongMengyuan修改,郑总提出来：在点击右上角的“购买”时打开页面会卡一下,而点击右下角的"在线教程"就不卡,改变写法(提取机器码浪费了多余的时间)
                    Process.Start("iexplore.exe", "http://www.xjpdf.com/software/pdfConvert/buy/?version=" + httpVersion + "&machine=" + httpMachineCode);
                }
                //发送请求信息 
                Version.Post("http://all.jsocr.com/", Version.GetParamName("Data"), Program.appProgName, Version.GetParamName("Version"), new reg().get_reg_code(), "注册窗口", "购买正式版");
            }
        }

        private void RegDlg_Paint(object sender, PaintEventArgs e)
        {
            //ControlPaint.DrawBorder(e.Graphics, this.ClientRectangle,
            //     Color.FromArgb(45, 164, 244), 2, ButtonBorderStyle.Solid,
            //     Color.FromArgb(45, 164, 244), 2, ButtonBorderStyle.Solid,
            //     Color.FromArgb(45, 164, 244), 2, ButtonBorderStyle.Solid,
            //     Color.FromArgb(45, 164, 244), 2, ButtonBorderStyle.Solid);
            ////DrawBroder画不上底部边框 用DrawLine补充上
            //e.Graphics.DrawLine(new Pen(Color.FromArgb(45, 164, 244), 3), 0, this.Height - 2, this.Width, this.Height - 2);
        }

        private void labQQ_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                switch (iniLanguage.ToLower())
                {
                    case "zh-cn":
                        //Process.Start("iexplore.exe", "http://www.xjpdf.com/software/pdfConvert/help/?version=" + Version.version);
                        try
                        {
                            Process.Start("http://www.xjpdf.com/software/pdfConvert/help/?version=" + Version.version);
                        }
                        catch
                        {
                            Process.Start("iexplore.exe", "http://www.xjpdf.com/software/pdfConvert/help/?version=" + Version.version);
                        }
                        break;
                    case "en":
                        //Process.Start("iexplore.exe", "http://www.xjpdf.com/software/pdfConvert/help/?version=" + Version.version);
                        try
                        {
                            Process.Start("http://www.xjpdf.com/software/pdfConvert/help/?version=" + Version.version);
                        }
                        catch
                        {
                            Process.Start("iexplore.exe", "http://www.xjpdf.com/software/pdfConvert/help/?version=" + Version.version);
                        }
                        break;
                    default:
                        //Process.Start("iexplore.exe", "http://www.xjpdf.com/software/pdfConvert/help/?version=" + Version.version);
                        try
                        {
                            Process.Start("http://www.xjpdf.com/software/pdfConvert/help/?version=" + Version.version);
                        }
                        catch
                        {
                            Process.Start("iexplore.exe", "http://www.xjpdf.com/software/pdfConvert/help/?version=" + Version.version);
                        }
                        break;
                }
            }
        }

        private void RegDlg_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, 274, 61440 + 9, 0);
            }
        }

        private void ucQQ_MouseClick(object sender, MouseEventArgs e)
        {
            //KongMY备注: 下面代码并没有起作用,而是直接进入了控件原始的代码,那里在起作用
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                switch (iniLanguage.ToLower())
                {
                    case "zh-cn":
                        try
                        {
                            Process.Start("http://www.xjpdf.com/software/pdfConvert/qq/?version=" + Program.httpVersion);
                        }
                        catch
                        {
                            Process.Start("iexplore.exe", "http://www.xjpdf.com/software/pdfConvert/qq/?version=" + Program.httpVersion);
                        }
                        break;
                    case "en":
                        try
                        {
                            Process.Start("http://www.xjpdf.com/software/pdfConvert/qq/?version=" + Program.httpVersion);
                        }
                        catch
                        {
                            Process.Start("iexplore.exe", "http://www.xjpdf.com/software/pdfConvert/qq/?version=" + Program.httpVersion);
                        }
                        break;
                    default:
                        try
                        {
                            Process.Start("http://www.xjpdf.com/software/pdfConvert/qq/?version=" + Program.httpVersion);
                        }
                        catch
                        {
                            Process.Start("iexplore.exe", "http://www.xjpdf.com/software/pdfConvert/qq/?version=" + Program.httpVersion);
                        }
                        break;
                }
            }
        }

        private void ucBuy_Click(object sender, EventArgs e)
        {
            //Process.Start("iexplore.exe", "http://www.xjpdf.com/software/pdfConvert/help/?version=" + Version.version);
            try
            {
                Process.Start("http://www.xjpdf.com/software/pdfConvert/help/?version=" + Version.version);
            }
            catch
            {
                Process.Start("iexplore.exe", "http://www.xjpdf.com/software/pdfConvert/help/?version=" + Version.version);
            }
            //Process.Start("http://www.xjpdf.com/software/pdfConvert/buy/?version=" + Version.version + "&machine=" + new reg().get_machine_code());
            try
            {
                //Process.Start("http://www.xjpdf.com/software/pdfConvert/buy/?version=" + Version.version + "&machine=" + new reg().get_machine_code());
                //KongMengyuan修改,郑总提出来：在点击右上角的“购买”时打开页面会卡一下,而点击右下角的"在线教程"就不卡,改变写法(提取机器码浪费了多余的时间)
                Process.Start("http://www.xjpdf.com/software/pdfConvert/buy/?version=" + httpVersion + "&machine=" + httpMachineCode);
            }
            catch
            {
                //Process.Start("iexplore.exe", "http://www.xjpdf.com/software/pdfConvert/buy/?version=" + Version.version + "&machine=" + new reg().get_machine_code());
                //KongMengyuan修改,郑总提出来：在点击右上角的“购买”时打开页面会卡一下,而点击右下角的"在线教程"就不卡,改变写法(提取机器码浪费了多余的时间)
                Process.Start("iexplore.exe", "http://www.xjpdf.com/software/pdfConvert/buy/?version=" + httpVersion + "&machine=" + httpMachineCode);
            }

            //发送请求信息 
            Version.Post("http://all.jsocr.com/", Version.GetParamName("Data"), Program.appProgName, Version.GetParamName("Version"), new reg().get_reg_code(), "注册窗口", "购买正式版");
        }

        private void ucBuy_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                //Process.Start("http://www.xjpdf.com/software/pdfConvert/buy/?version=" + Version.version + "&machine=" + new reg().get_machine_code());
                try
                {
                    //Process.Start("http://www.xjpdf.com/software/pdfConvert/buy/?version=" + Version.version + "&machine=" + new reg().get_machine_code());
                    //KongMengyuan修改,郑总提出来：在点击右上角的“购买”时打开页面会卡一下,而点击右下角的"在线教程"就不卡,改变写法(提取机器码浪费了多余的时间)
                    Process.Start("http://www.xjpdf.com/software/pdfConvert/buy/?version=" + httpVersion + "&machine=" + httpMachineCode);
                }
                catch
                {
                    //Process.Start("iexplore.exe", "http://www.xjpdf.com/software/pdfConvert/buy/?version=" + Version.version + "&machine=" + new reg().get_machine_code());
                    //KongMengyuan修改,郑总提出来：在点击右上角的“购买”时打开页面会卡一下,而点击右下角的"在线教程"就不卡,改变写法(提取机器码浪费了多余的时间)
                    Process.Start("iexplore.exe", "http://www.xjpdf.com/software/pdfConvert/buy/?version=" + httpVersion + "&machine=" + httpMachineCode);
                }
                //发送请求信息 
                Version.Post("http://all.jsocr.com/", Version.GetParamName("Data"), Program.appProgName, Version.GetParamName("Version"), new reg().get_reg_code(), "注册窗口", "购买正式版");
            }
        }

        private void ucBuy_MouseUp(object sender, MouseEventArgs e)
        {
            //Process.Start("http://www.xjpdf.com/software/pdfConvert/buy/?version=" + Version.version + "&machine=" + new reg().get_machine_code());
            try
            {
                //Process.Start("http://www.xjpdf.com/software/pdfConvert/buy/?version=" + Version.version + "&machine=" + new reg().get_machine_code());
                //KongMengyuan修改,郑总提出来：在点击右上角的“购买”时打开页面会卡一下,而点击右下角的"在线教程"就不卡,改变写法(提取机器码浪费了多余的时间)
                Process.Start("http://www.xjpdf.com/software/pdfConvert/buy/?version=" + httpVersion + "&machine=" + httpMachineCode);
            }
            catch
            {
                //Process.Start("iexplore.exe", "http://www.xjpdf.com/software/pdfConvert/buy/?version=" + Version.version + "&machine=" + new reg().get_machine_code());
                //KongMengyuan修改,郑总提出来：在点击右上角的“购买”时打开页面会卡一下,而点击右下角的"在线教程"就不卡,改变写法(提取机器码浪费了多余的时间)
                Process.Start("iexplore.exe", "http://www.xjpdf.com/software/pdfConvert/buy/?version=" + httpVersion + "&machine=" + httpMachineCode);
            }
            //发送请求信息 
            Version.Post("http://all.jsocr.com/", Version.GetParamName("Data"), Program.appProgName, Version.GetParamName("Version"), new reg().get_reg_code(), "注册窗口", "购买正式版");
        }

        private void ucQQ_MouseClick(object sender, EventArgs e)
        {
            switch (iniLanguage.ToLower())
            {
                case "zh-cn":
                    //Process.Start("iexplore.exe", "http://www.xjpdf.com/software/pdfConvert/help/?version=" + Version.version);
                    try
                    {
                        Process.Start("http://www.xjpdf.com/software/pdfConvert/help/?version=" + Version.version);
                    }
                    catch
                    {
                        Process.Start("iexplore.exe", "http://www.xjpdf.com/software/pdfConvert/help/?version=" + Version.version);
                    }
                    break;
                case "en":
                    //Process.Start("iexplore.exe", "http://www.xjpdf.com/software/pdfConvert/help/?version=" + Version.version);
                    try
                    {
                        Process.Start("http://www.xjpdf.com/software/pdfConvert/help/?version=" + Version.version);
                    }
                    catch
                    {
                        Process.Start("iexplore.exe", "http://www.xjpdf.com/software/pdfConvert/help/?version=" + Version.version);
                    }
                    break;
                default:
                    //Process.Start("iexplore.exe", "http://www.xjpdf.com/software/pdfConvert/help/?version=" + Version.version);
                    try
                    {
                        Process.Start("http://www.xjpdf.com/software/pdfConvert/help/?version=" + Version.version);
                    }
                    catch
                    {
                        Process.Start("iexplore.exe", "http://www.xjpdf.com/software/pdfConvert/help/?version=" + Version.version);
                    }
                    break;
            }
        }

        private void ucQQ_Click(object sender, EventArgs e)
        {
        }

        private void ucQQ_Load(object sender, EventArgs e)
        {

        }

        private void pbClose_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                this.pbClose.BackgroundImage = Properties.Resources.close_03;
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        private void btnRegCodeGet_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                try
                {
                    Process.Start("http://www.xjpdf.com/software/pdfConvert/buy/?version=" + httpVersion + "&machine=" + httpMachineCode);
                }
                catch
                {
                    Process.Start("iexplore.exe", "http://www.xjpdf.com/software/pdfConvert/buy/?version=" + httpVersion + "&machine=" + httpMachineCode);
                }
            }
        }

        private void btnRegCodeGet_MouseEnter(object sender, EventArgs e)
        {
            this.btnRegCodeGet.BackgroundImage = Properties.Resources.RegDlg_btn_dialog_get_02;
        }

        private void btnRegCodeGet_MouseLeave(object sender, EventArgs e)
        {
            this.btnRegCodeGet.BackgroundImage = Properties.Resources.RegDlg_btn_dialog_get_01;
        }

        private void pbClose_MouseEnter(object sender, EventArgs e)
        {
            this.pbClose.BackgroundImage = Properties.Resources.close_02;
        }

        private void pbClose_MouseLeave(object sender, EventArgs e)
        {
            this.pbClose.BackgroundImage = Properties.Resources.close_01;
        }

        private void RegDlg_Activated(object sender, EventArgs e)
        {
            this.lblTitle.Text = Program.progTitle;
        }

        /// <summary>
        /// 设置简体中文语言
        /// </summary>
        private void SetZhCn()
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("zh-CN");
            this.label1.Text = rm.GetString("label1");//可永久无限制使用      可在3台电脑上使用      可终身免费升级
            this.label3.Text = rm.GetString("label3"); //输入注册码，点击“开始激活”开启正式版
            this.label4.Text = rm.GetString("label4");//注册码：
            this.label5.Text = rm.GetString("label5");//机器码：
            this.label6.Text = rm.GetString("label6");//感谢对正版的支持与购买，激活后您将获得软件完整功能。
            this.btnActive.ButtonText = rm.GetString("btnActive");//开始激活
            this.btnRegCodeGet.ButtonText = rm.GetString("btnRegCodeGet");//获取注册码
            this.lblBuy.Text = rm.GetString("lblBuy");//购买软件
            this.lblQQ.Text = rm.GetString("lblQQ");//在线QQ：4006685572
            this.lblTelOnline.Text = rm.GetString("lblTelOnline");//咨询热线  400-668-5572

            //以下的位置定义,中英文语言位置不同,要注意
            this.pbClose.Location = new Point(453, 12);
            this.pictureBox1.Location = new Point(112, 58);
            this.label3.Location = new Point(135, 60);
            this.label5.Location = new Point(49, 108);
            this.label4.Location = new Point(49, 152);
            this.label6.Location = new Point(28, 254);
            this.label1.Location = new Point(72, 289);

            //this.label6.Left = (int)(this.Width / 2 - this.label6.Width / 2);
            //this.label1.Left = (int)(this.Width / 2 - this.label1.Width / 2);
            this.pbTelOnline.Location = new Point(18, 331);
            this.lblTelOnline.Location = new Point(37, 334);
            this.pbBuy.Location = new Point(201, 331);
            this.lblBuy.Location = new Point(222, 334);
            this.pbQQ.Location = new Point(314, 332);
            this.lblQQ.Location = new Point(328, 334);

            this.txtMachineCode.Location = new Point(118, 102);
            this.txtRegCode.Location = new Point(118, 146);
            this.btnRegCodeGet.Location = new Point(100, 196);
            this.btnActive.Location = new Point(260, 196);
        }

        /// <summary>
        /// 设置英文语言
        /// </summary>
        private void SetEn()
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");
            this.BackgroundImage = Properties.Resources.RegDlg_bg;
            this.label1.Text = rm.GetString("label1");//可永久无限制使用      可在3台电脑上使用      可终身免费升级
            this.label3.Text = rm.GetString("label3"); //输入注册码，点击“开始激活”开启正式版
            this.label4.Text = rm.GetString("label4");//注册码：
            this.label5.Text = rm.GetString("label5");//机器码：
            this.label6.Text = rm.GetString("label6");//感谢对正版的支持与购买，激活后您将获得软件完整功能。
            this.btnActive.ButtonText = rm.GetString("btnActive");//开始激活
            this.btnRegCodeGet.ButtonText = rm.GetString("btnRegCodeGet");//获取注册码
            this.lblBuy.Text = rm.GetString("lblBuy");//购买软件
            this.lblQQ.Text = rm.GetString("lblQQ");//在线QQ：4006685572
            this.lblTelOnline.Text = rm.GetString("lblTelOnline");//咨询热线  400-668-5572

            //以下的位置定义,中英文语言位置不同,要注意
            this.pbClose.Location = new Point(453, 12);
            this.pictureBox1.Location = new Point(112, 58);
            this.label3.Location = new Point(135, 60);
            this.label5.Location = new Point(35, 108);
            this.label4.Location = new Point(10, 152);
            //this.label6.Location = new Point(28, 254);
            this.label1.Location = new Point(72, 289);

            this.label6.Left = 3; //(int)(this.Width / 2 - this.label6.Width / 2);
            this.label1.Left = 50; //(int)(this.Width / 2 - this.label1.Width / 2);
            this.label6.Top = 254;
            this.label1.Top = 289;            
            this.pbTelOnline.Location = new Point(18, 331);
            this.lblTelOnline.Location = new Point(37, 334);
            this.pbBuy.Location = new Point(201, 331);
            this.lblBuy.Location = new Point(222, 334);
            this.pbQQ.Location = new Point(314, 332);
            this.lblQQ.Location = new Point(328, 334);

            this.txtMachineCode.Location = new Point(160, 102);
            this.txtRegCode.Location = new Point(160, 146);
            this.btnRegCodeGet.Location = new Point(100, 196);
            this.btnActive.Location = new Point(260, 196);
        }
    }
}
