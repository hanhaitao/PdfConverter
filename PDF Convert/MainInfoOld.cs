using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Management;//项目下边的 引用--添加引用--框架--System.Management
using System.Runtime.InteropServices; //运行Windows的API,引用DLL专用
using System.Globalization;//CultureInfo调用
using System.Diagnostics;

using System.Net.NetworkInformation; //C# winform点击按钮获取指定ip的mac地址
using System.Net; //KongMengyuan增加,2015-11-09,statisticsPost向服务器POST使用

using System.Text.RegularExpressions; //Regex调用

//using Controls;
using System.Resources;
//using Microsoft.Win32; //关于注册表的命名空间


namespace PDF_Convert
{
    //RegCode=358915218264
    public partial class MainInfoOld : Form
    {
        [DllImport("user32")]
        public static extern int ReleaseCapture();//用鼠标拖动窗体,移动窗体
        [DllImport("user32")]
        public static extern int SendMessage(IntPtr hwnd, int msg, int wp, int lp);//用鼠标拖动窗体,移动窗体

        //物理CPU核数
        private int cpuNumber = 1;
        //逻辑CPU核数
        private int cpuLogicalNumber = 1;
        //文件队列
        public Queue<ListViewItem> fileQueue = new Queue<ListViewItem>();
        //导航栏目
        private ConvertOld.FORMAT format = new ConvertOld.FORMAT();
        private SynchronizationContext syncContext = null;
        //列表集合
        public Dictionary<string, bool> diclst = new Dictionary<string, bool>();
        public bool isClose = false;
        //是否已注册
        public static bool isReg = false;
        private ini_config ini = new ini_config("config.ini");
        public static ResourceManager rm = new ResourceManager(typeof(MainInfoOld));
        private Thread[] thread;
        //线程管理当前文件集合
        public Dictionary<string, int> dicThreadManagement = new Dictionary<string, int>();
        //private string encodingCode = string.Empty;

        private string iniLanguage = string.Empty; //读取Config.ini文件里面的language

        private string outpathSelect = ""; //KongMengyuan修改,2015-11-03,保留当前输出文件夹(在文件转换过程中不允许更改它)
        public MainInfoOld()
        {
            Console.WriteLine("查看当前的时间，跟踪为什么程序启动比较慢8： " + DateTime.Now.ToString());
            iniLanguage = ini.read_ini("language");
            if (string.IsNullOrEmpty(iniLanguage))
                iniLanguage = System.Globalization.CultureInfo.InstalledUICulture.Name;
            //encodingCode = new reg().get_machine_code(); //KongMengyuan注释,2015-11-16,此段代码可能影响此主窗体重复打开
            syncContext = SynchronizationContext.Current;
            // Thread.CurrentThread.CurrentUICulture = new CultureInfo(language);
            Control.CheckForIllegalCrossThreadCalls = false;

            InitializeComponent();

            switch (iniLanguage.ToLower())
            {
                case "zh-cn":
                    SetZhCn();
                    //SetEn();
                    break;
                case "en":
                    SetEn();
                    break;
                default:
                    SetZhCn();
                    //SetEn();
                    break;
            }
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
            Console.WriteLine("查看当前的时间，跟踪为什么程序启动比较慢9： " + DateTime.Now.ToString());
            return;
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr ChangeWindowMessageFilter(uint message, uint dwFlag);

        private uint WM_DROPFILES = 0x0233;
        private uint WM_COPYDATA = 0x4A;
        private uint MSGFLT_ADD = 1;

        private void MainInfo_Load(object sender, EventArgs e)
        {
            Console.WriteLine("查看当前的时间，跟踪为什么程序启动比较慢10： " + DateTime.Now.ToString());

            onlyRunOnce();

            Console.WriteLine("查看当前的时间，跟踪为什么程序启动比较慢11： " + DateTime.Now.ToString());
            //this.Text = Program.formTitle; //窗体的快捷方式显示,任务栏的显示内容
        }

        public static object Bytes2Struct(byte[] rawdatas, int startIndex, Type anytype)
        {
            int rawsize = Marshal.SizeOf(anytype);
            if (startIndex + rawsize > rawdatas.Length)
                return null;
            IntPtr buffer = Marshal.AllocHGlobal(rawsize);
            Marshal.Copy(rawdatas, startIndex, buffer, rawsize);
            object retobj = Marshal.PtrToStructure(buffer, anytype);
            Marshal.FreeHGlobal(buffer);
            return retobj;
        }

        //注意这个特性不能少
        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        struct TestStruct
        {
            public int c;
            //字符串，SizeConst为字符串的最大长度
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string str;
            //int数组，SizeConst表示数组的个数，在转换成
            //byte数组前必须先初始化数组，再使用，初始化
            //的数组长度必须和SizeConst一致，例test = new int[6];
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public int[] test;
        }

        private void MainInfo_Activated(object sender, EventArgs e)
        {
            Console.WriteLine("查看当前的时间，跟踪为什么程序启动比较慢12： " + DateTime.Now.ToString());

            //全部使用底图显示中文或英文(因为英文和中文的长度不同,而按钮比较小,有时英文需要折行才可以显示)
            btnAddFiles.ButtonText = ""; //添加文件
            btnFolder.ButtonText = "";   //添加文件夹
            btnStart.ButtonText = "";    //开始转换
            btnStop.ButtonText = "";     //停止转换
            btnClear.ButtonText = "";    //清空列表

            //MessageBox.Show("程序运行到这里MainInfoOld了 77777");
            CheckFormIsOpen();
            //MessageBox.Show("程序运行到这里MainInfoOld了 88888");
            iniLanguage = ini.read_ini("language").ToLower(); //读取Config.ini文件里面的language

            pbIndexBackGround.BringToFront();//BringToFront()置于顶层,SendToBack()置于底层
            backGroundShowHide();//隐藏或显示主界面提示图            

            pltext.Height = 29; //45为填写好高度
            Console.WriteLine("查看当前的时间，跟踪为什么程序启动比较慢13： " + DateTime.Now.ToString());
        }

        private void onlyRunOnce()
        {
            if (Program.MainInfo_LoadFinish == "Finish")
            {
                return;
            }

            //修改版本号的注意事项：
            //1、全局用的版本号不要放在frmSplash.cs里面,因为MainInfoOld是使用线程启动的,所以要放在这里才可以
            //2、此处写数字的版本号,因为以后自动更新时不会更改config.ini这个文件,里面的版本号可能永远不会变,所以版本号必须写在程序里面,同时在MainInfoOld.onlyRunOnce里面重新赋值
            //3、查找整个解决方案的“PDF Convert, Version=6.2.0.0”,全部修改为对应的版本就可以了(每个窗体有对应的resx,比如RegDlg.cs有3个对应的文件: Redlg.resx,RegDlg.en.resx,RegDlg.zh-CN.resx)
            //  如何向winform里面添加额外的resx文件：比如frmAutoUpdate.cs需要添加frmAutoUpdate.en.resx和frmAutoUpdate.zh-CN.resx
            //                     鼠标右击顶部的项目“PDF Convert”-添加-新建项--资源文件--输入和窗体相同的名称,再加一个语言的标志符,比如加en或zh-CN
            //4、同时修改AssemblyInfo.cs里面的“[assembly: AssemblyVersion("6.2")]”为对应的版本号
            Program.httpVersion = "6.2";// Version.version; 
            //5、把版本号更新进入Config.ini里面
            ini.write_ini("Version", Program.httpVersion);

            //以下代码是MainInfo_Load里面的代码,放在这里是为了方便测试页面加载慢的原因: 左侧自画控件,顶部自画控件,
            Console.WriteLine(DateTime.Now.ToString());
            //comboBoxPage.Width = lstFile.Columns[2].Width;
            //lstFile.Controls.Add(comboBoxPage);
            try
            {
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
                //图片是否合并
                string isMerger = ini.read_ini("isMerger");
                //cbIsMerger.Checked = true;
                this.rbIsMerger1.Checked = true;
                //cbIsMerger.Visible = false;
                //cbIsMerger.Checked = isMerger == "1" ? true : false;
                if (new reg().Is_Reg())
                {
                    this.lblTitle.Text = Encrypt.APPTITLE + " " + rm.GetString("OfficialVersion") + " v" +
                                         Program.httpVersion;//Version.version;
                    this.panel1.BackgroundImage = Properties.Resources.header_01; //已注册
                    isReg = true;
                    // pltext.Visible = true;
                }
                else
                {
                    this.lblTitle.Text = Encrypt.APPTITLE + " " + rm.GetString("FreeTrialVersion") + " v" +
                                         Program.httpVersion;// Version.version;
                    this.panel1.BackgroundImage = Properties.Resources.header_02; //未注册
                    isReg = false;
                    //pltext.Visible = false;
                }
                Program.progTitle = this.lblTitle.Text;
                //菜单导航功能按钮选择
                string type = ini.read_ini("Type");
                if (!string.IsNullOrEmpty(type))
                {
                    int select = 0;
                    int.TryParse(type, out select);
                    //MenuSeletect(select);
                    MenuSeletect(0);
                }
                //保存文本框路径
                string targetDir = ini.read_ini("TargetDir");
                if (targetDir != string.Empty)
                {
                    this.txtOutPath.OutText = targetDir;
                }
                else
                {
                    //this.txtOutPath.Text = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Favorites); //收藏夹路径
                    this.txtOutPath.OutText = System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\";//桌面路径
                }
                this.toolTip1.SetToolTip(this.txtOutPath, this.txtOutPath.OutText);//增加TextBox的提示
                this.tbBrw.Text = this.txtOutPath.OutText;
                this.toolTip1.ShowAlways = true;//增加TextBox的提示
                this.toolTip1.AutoPopDelay = 32500;//AutoPopDelay的值不能超过33000,估计它的取值范围应该是小于32767（小于2的15次方）,当大于这个值的话,这个值就用默认值5000来代替.如要延迟更长的时间,用本身的tooltip是不行的,需要自己写
                this.toolTip1.InitialDelay = 1000;
                this.toolTip1.ReshowDelay = 500;
                this.toolTip1.SetToolTip(this.tbBrw, this.tbBrw.Text);//增加TextBox的提示

                //输出路径默认选择
                string out_ = ini.read_ini("Out");
                if (string.IsNullOrEmpty(out_) || out_ == "1")
                {
                    this.rdoNewPath.Checked = false;
                    this.rdoPath.Checked = true;
                    //this.btnBrowse.IsEnable = false;
                    //this.btnBrowse.ButtonBackIMG = Properties.Resources.lookEnable;
                    outpathSelect = "rdoPath";
                }
                else
                {
                    this.rdoNewPath.Checked = true;
                    this.rdoPath.Checked = false;
                    //this.btnBrowse.IsEnable = true;
                    //this.btnBrowse.ButtonBackIMG = Properties.Resources.look;
                    outpathSelect = "rdoNewPath";
                }
                this.txtWidth.Text = ini.read_ini("PicX") == string.Empty ? "700" : ini.read_ini("PicX");
                this.txtHeight.Text = ini.read_ini("PicY") == string.Empty ? "500" : ini.read_ini("PicY");
                thread = new Thread[cpuNumber];
                for (int i = 0; i < thread.Length; i++)
                {
                    thread[i] = new Thread(new ParameterizedThreadStart(WorkThread));
                    thread[i].IsBackground = true;
                    thread[i].Start(i);
                }
                //upbFile2Word.ButtonImage = Properties.Resources.img_09;
                Program.MainInfo_LoadFinish = "Finish";

                //toolTip1.SetToolTip(this.picBrw, "My sdfgdfsg  button1 ");

                this.pbIndexBackGround.Visible = true; //显示主界面初始图片

                //放置“文件转PPT”的"PPT大小设置",“图片转PDF”的"将所有图片合并成一个PDF文件",“PDF合并”百分比,都放在Panel4上(原设计人员定义的,最麻烦的是原设计人员将其Dock定义为Fill,这样即使找到lblPageSize也看不到,已经修改这种作法了)
                this.panel4.Visible = false;
            }
            catch (Exception ex)
            {
                //MessageBox.Show("出错了 (互盾错误号标记201512161652)," + ex.Message);
            }

            //自动更新,自动下载,在发布之前再将注释去掉,否则不停的上传统计(而实际是没有意义的数据),影响正常统计数据
            Thread trd = new Thread(statisticsPost);//KongMengyuan增加,2015-11-09,依据郑侃炜新的要求所作,如果网址出错会死在这里,所以要
            trd.Start();

            ////KongMengyuan增加,郑总提出来：在点击右上角的“购买”时打开页面会卡一下,而点击右下角的"在线教程"就不卡
            //httpVersion = Version.version;
            //httpMachineCode = new reg().get_machine_code();
            //httpRegCode = new reg().get_reg_code();
        }

        #region 自动更新,自动下载
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
                frmAutoUpdateOld frm = new frmAutoUpdateOld();
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

        /// <summary>
        /// 设置简体中文语言
        /// </summary>
        private void SetZhCn()
        {
            this.panel4.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.plMerger.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.plPPT.Dock = System.Windows.Forms.DockStyle.Bottom;

            Thread.CurrentThread.CurrentUICulture = new CultureInfo("zh-CN");
            this.upbFile2Word.ButtonText = rm.GetString("upbFile2Word");
            this.upbFile2Excel.ButtonText = rm.GetString("upbFile2Excel");
            this.upbFile2PPT.ButtonText = rm.GetString("upbFile2PPT");
            this.upbFile2HTML.ButtonText = rm.GetString("upbFile2HTML");
            this.upbIMG2PDF.ButtonText = rm.GetString("upbIMG2PDF");
            this.upbFile2TXT.ButtonText = rm.GetString("upbFile2TXT");
            this.upbFile2IMG.ButtonText = rm.GetString("upbFile2IMG");
            this.upbDoc2PDF.ButtonText = rm.GetString("upbDoc2PDF");
            this.upbPPT2PDF.ButtonText = rm.GetString("upbPPT2PDF");
            this.upbExcel2PDF.ButtonText = rm.GetString("upbExcel2PDF");
            this.btnHelp.ButtonText = rm.GetString("btnHelp");
            this.lblTitle.Text = rm.GetString("lblTitle.Text");
            this.btnAddFiles.ButtonText = rm.GetString("btnAddFiles.Text");
            this.btnFolder.ButtonText = rm.GetString("btnFolder.Text");
            this.btnStart.ButtonText = rm.GetString("btnStart.Text");
            this.btnClear.ButtonText = rm.GetString("btnClear");
            this.lstFile.IndexText = rm.GetString("Number");
            this.lstFile.FileNameText = rm.GetString("FileName");
            this.lstFile.PageCountText = rm.GetString("PageCount"); //此处影响界面显示的"总页数"
            this.lstFile.SelectPageText = rm.GetString("ConversionPages");
            this.lstFile.StatusText = rm.GetString("Status");
            //this.lstFile.OperateText = rm.GetString("Operate");
            //this.cbIsMerger.Text = rm.GetString("cbIsMerger.Text");

            this.lblPPTSize.Text = rm.GetString("PPTSize");
            this.lblWidth.Text = rm.GetString("Width");
            this.lblHeight.Text = rm.GetString("Height");
            this.rdoPath.Text = rm.GetString("rdoPath.Text");
            this.rdoNewPath.Text = rm.GetString("rdoNewPath.Text");
            //this.btnBrowse.ButtonText = rm.GetString("btnBrowse.Text");
            this.labHelp.Text = rm.GetString("btnCourse");
            this.labBuy.Text = rm.GetString("btnBuy");
            this.tsmSoftwareUpgrade.Text = rm.GetString("tsmSoftwareUpgrade");
            this.tsmLanguageSelection.Text = rm.GetString("tsmLanguageSelection");
            //this.comboBoxPage.Text = rm.GetString("PageTips");
            //this.tsmCn.Text = rm.GetString("tsmCn.Text");
            //this.tsmEnglish.Text = rm.GetString("tsmEnglish.Text");
            this.tsmAboutUs.Text = rm.GetString("tsmAboutUs");
            this.tsmBuy.Text = rm.GetString("tsmBuy");
            this.labQQ.Text = rm.GetString("btnQQ");
            this.labPhone.Text = rm.GetString("btnPhone");
            this.comboBoxPage.BackGroundText = rm.GetString("PageTips");
            this.lstFile.ConversionPageDefaultText = rm.GetString("ALL");
            this.lstFile.OpenButtonText = rm.GetString("Open"); //内部的打开小按钮
            this.lstFile.OpenText = rm.GetString("lstFileOpenText"); //顶部的标题头,打开
            this.lstFile.FolderText = rm.GetString("lstFileFolderText"); //顶部的标题头,输出
            this.lstFile.DelText = rm.GetString("lstFileDelText"); //顶部的标题头,删除
            this.rdoPath.Text = rm.GetString("FolderFrom"); //原文件夹
            this.rdoNewPath.Text = rm.GetString("FolderNew"); //自定义
            this.labOut.Text = rm.GetString("OutputDirectory"); //文件保存位置                        
            this.upbPdfSplit.ButtonText = rm.GetString("PDFSplit"); //PDF分割
            this.upbPDFMerge.ButtonText = rm.GetString("PDFMerge"); //PDF合并
            this.upbPDFDecode.ButtonText = rm.GetString("PDFDecode"); //PDF密码解除
            this.upbPDFCompress.ButtonText = rm.GetString("PDFCompress"); //PDF压缩
            this.upbPDFGetImg.ButtonText = rm.GetString("PDFGetImage"); //PDF图片获取
            this.ucPicButtonTop1.ButtonText = rm.GetString("ucPicButtonTop1"); //PDF转换成其他文件
            this.ucPicButtonTop2.ButtonText = rm.GetString("ucPicButtonTop2"); //其他文件转换成PDF
            this.ucPicButtonTop3.ButtonText = rm.GetString("ucPicButtonTop3"); //其他操作

            this.plNavigation.Location = new Point(0, 0);
            this.plNavigation.Size = new System.Drawing.Size(180, 608);
            this.lstFile.Location = new Point(0, 3);
            this.lstFile.Size = new System.Drawing.Size(799, 567);

            this.ucPicButtonTop1.Location = new Point(0, 0);
            this.ucPicButtonTop1.Size = new System.Drawing.Size(180, 43);
            this.upbFile2Word.Location = new Point(0, 43);
            this.upbFile2Word.Size = new System.Drawing.Size(180, 30);
            this.upbFile2Excel.Location = new Point(0, 73);
            this.upbFile2Excel.Size = new System.Drawing.Size(180, 30);
            this.upbFile2PPT.Location = new Point(0, 103);
            this.upbFile2PPT.Size = new System.Drawing.Size(180, 30);
            this.upbFile2HTML.Location = new Point(0, 133);
            this.upbFile2HTML.Size = new System.Drawing.Size(180, 30);
            this.upbFile2TXT.Location = new Point(0, 163);
            this.upbFile2TXT.Size = new System.Drawing.Size(180, 30);
            this.upbFile2IMG.Location = new Point(0, 193);
            this.upbFile2IMG.Size = new System.Drawing.Size(180, 30);

            this.ucPicButtonTop2.Location = new Point(0, 223);
            this.ucPicButtonTop2.Size = new System.Drawing.Size(180, 42);
            this.upbIMG2PDF.Location = new Point(0, 265);
            this.upbIMG2PDF.Size = new System.Drawing.Size(180, 30);
            this.upbDoc2PDF.Location = new Point(0, 295);
            this.upbDoc2PDF.Size = new System.Drawing.Size(180, 30);
            this.upbExcel2PDF.Location = new Point(0, 325);
            this.upbExcel2PDF.Size = new System.Drawing.Size(180, 30);
            this.upbPPT2PDF.Location = new Point(0, 355);
            this.upbPPT2PDF.Size = new System.Drawing.Size(180, 36);

            this.ucPicButtonTop3.Location = new Point(0, 391);
            this.ucPicButtonTop3.Size = new System.Drawing.Size(180, 43);
            this.upbPdfSplit.Location = new Point(0, 434);
            this.upbPdfSplit.Size = new System.Drawing.Size(180, 33);
            this.upbPDFMerge.Location = new Point(0, 467);
            this.upbPDFMerge.Size = new System.Drawing.Size(180, 30);
            this.upbPDFDecode.Location = new Point(0, 497);
            this.upbPDFDecode.Size = new System.Drawing.Size(180, 30);
            this.upbPDFCompress.Location = new Point(0, 527);
            this.upbPDFCompress.Size = new System.Drawing.Size(180, 30);
            this.upbPDFGetImg.Location = new Point(0, 557);
            this.upbPDFGetImg.Size = new System.Drawing.Size(180, 30);
            //pbLogo.BackgroundImage = Properties.Resources.logo_030;
            //pbLogo.Size = new System.Drawing.Size(206, 40);
            //btnPhone.Visible = true;
            //btnQQ.Visible = true;
            plPPT.Location = new Point(0, 94);
            plPPT.Size = new System.Drawing.Size(710, 42);

            //lblPPTSize.Location = new Point(4, 5);
            //lblPPTSize.Size = new System.Drawing.Size(89, 17);
            //lblWidth.Location = new Point(97, 5);
            //lblWidth.Size = new System.Drawing.Size(20, 17);
            //txtWidth.Location = new Point(118, 5);
            //txtWidth.Size = new System.Drawing.Size(59, 21);
            //lblHeight.Location = new Point(185, 5);
            //lblHeight.Size = new System.Drawing.Size(20, 17);
            //txtHeight.Location = new Point(208, 4);
            //txtHeight.Size = new System.Drawing.Size(59, 21);
            //cbIsMerger.Location = new Point(520, 6);
            //cbIsMerger.Size = new System.Drawing.Size(186, 16);
            rdoPath.Location = new Point(99, 2);
            //rdoPath.Size = new System.Drawing.Size(214, 24);
            rdoNewPath.Location = new Point(185, 2);
            //rdoNewPath.Size = new System.Drawing.Size(111, 24);
            //txtOutPath.Location = new Point(283, 110);
            //btnBrowse.Location = new Point(652, 108);
            btnStart.Location = new Point(234, 5);
            //btnCourse.Location = new Point(17, 0);
            //btnCourse.Size = new System.Drawing.Size(115, 38);
            //btnCourse.ButtonTextFont = new System.Drawing.Font("微软雅黑", 8);
            //btnBuy.ButtonTextFont = new System.Drawing.Font("微软雅黑", 9);
            //btnBuy.Location = new Point(144, 0);
            //btnBuy.Size = new System.Drawing.Size(116, 38);
            //btnQQ.Location = new Point(20, 5);
            //btnQQ.Size = new System.Drawing.Size(203, 38);
            //btnQQ.ButtonImage = Properties.Resources.qq;
            //btnQQ.FromType = 2;
            //btnPhone.Location = new Point(474, 0);
            //btnPhone.Size = new System.Drawing.Size(273, 38);
        }

        /// <summary>
        /// 设置英文语言
        /// </summary>
        private void SetEn()
        {
            this.panel4.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.plMerger.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.plPPT.Dock = System.Windows.Forms.DockStyle.Bottom;

            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");
            this.upbFile2Word.ButtonText = rm.GetString("upbFile2Word");
            this.upbFile2Excel.ButtonText = rm.GetString("upbFile2Excel");
            this.upbFile2PPT.ButtonText = rm.GetString("upbFile2PPT");
            this.upbFile2HTML.ButtonText = rm.GetString("upbFile2HTML");
            this.upbIMG2PDF.ButtonText = rm.GetString("upbIMG2PDF");
            this.upbFile2TXT.ButtonText = rm.GetString("upbFile2TXT");
            this.upbFile2IMG.ButtonText = rm.GetString("upbFile2IMG");
            this.upbDoc2PDF.ButtonText = rm.GetString("upbDoc2PDF");
            this.upbPPT2PDF.ButtonText = rm.GetString("upbPPT2PDF");
            this.upbExcel2PDF.ButtonText = rm.GetString("upbExcel2PDF");
            this.btnHelp.ButtonText = rm.GetString("btnHelp");
            this.lblTitle.Text = rm.GetString("lblTitle.Text");
            this.btnAddFiles.ButtonText = rm.GetString("btnAddFiles.Text");
            this.btnFolder.ButtonText = rm.GetString("btnFolder.Text");
            this.btnStart.ButtonText = rm.GetString("btnStart.Text");
            this.btnClear.ButtonText = rm.GetString("btnClear");
            this.lstFile.IndexText = rm.GetString("Number");
            this.lstFile.FileNameText = rm.GetString("FileName");
            this.lstFile.PageCountText = rm.GetString("PageCount"); //此处影响传过来的"总页数"
            this.lstFile.SelectPageText = rm.GetString("ConversionPages");
            this.lstFile.StatusText = rm.GetString("Status");
            //this.lstFile.OperateText = rm.GetString("Operate");
            //this.cbIsMerger.Text = rm.GetString("cbIsMerger.Text");
            this.lbIsMerger.Text = rm.GetString("lbIsMerger.Text"); //将所有图片合并成一个PDF文件,定制你的PPT大小
            this.lblPPTSize.Text = rm.GetString("PPTSize");
            this.lblWidth.Text = rm.GetString("Width");
            this.lblHeight.Text = rm.GetString("Height");
            this.rdoPath.Text = rm.GetString("rdoPath.Text");
            this.rdoNewPath.Text = rm.GetString("rdoNewPath.Text");
            //this.btnBrowse.ButtonText = rm.GetString("btnBrowse.Text");
            this.labHelp.Text = rm.GetString("btnCourse");
            this.labBuy.Text = rm.GetString("btnBuy");
            this.tsmSoftwareUpgrade.Text = rm.GetString("tsmSoftwareUpgrade");
            this.tsmLanguageSelection.Text = rm.GetString("tsmLanguageSelection");
            //this.tsmCn.Text = rm.GetString("tsmCn.Text");
            //this.tsmEnglish.Text = rm.GetString("tsmEnglish.Text");
            this.tsmAboutUs.Text = rm.GetString("tsmAboutUs");
            this.tsmBuy.Text = rm.GetString("tsmBuy");
            this.labQQ.Text = rm.GetString("btnQQ");
            this.labPhone.Text = rm.GetString("btnPhone");
            this.comboBoxPage.BackGroundText = rm.GetString("PageTips");
            this.lstFile.ConversionPageDefaultText = rm.GetString("ALL");
            this.lstFile.OpenButtonText = rm.GetString("Open"); //内部的打开小按钮
            this.lstFile.OpenText = rm.GetString("lstFileOpenText"); //顶部的标题头,打开
            this.lstFile.FolderText = rm.GetString("lstFileFolderText"); //顶部的标题头,输出
            this.lstFile.DelText = rm.GetString("lstFileDelText"); //顶部的标题头,删除
            this.rdoPath.Text = rm.GetString("FolderFrom"); //原文件夹
            this.rdoNewPath.Text = rm.GetString("FolderNew"); //自定义
            this.labOut.Text = rm.GetString("OutputDirectory"); //文件保存位置                        
            this.upbPdfSplit.ButtonText = rm.GetString("PDFSplit"); //PDF分割
            this.upbPDFMerge.ButtonText = rm.GetString("PDFMerge"); //PDF合并
            this.upbPDFDecode.ButtonText = rm.GetString("PDFDecode"); //PDF密码解除
            this.upbPDFCompress.ButtonText = rm.GetString("PDFCompress"); //PDF压缩
            this.upbPDFGetImg.ButtonText = rm.GetString("PDFGetImage"); //PDF图片获取
            this.ucPicButtonTop1.ButtonText = rm.GetString("ucPicButtonTop1"); //PDF转换成其他文件
            this.ucPicButtonTop2.ButtonText = rm.GetString("ucPicButtonTop2"); //其他文件转换成PDF
            this.ucPicButtonTop3.ButtonText = rm.GetString("ucPicButtonTop3"); //其他操作

            this.plNavigation.Location = new Point(0, 0);
            this.plNavigation.Size = new System.Drawing.Size(180, 608);
            this.lstFile.Location = new Point(0, 3);
            this.lstFile.Size = new System.Drawing.Size(799, 567);

            this.ucPicButtonTop1.Location = new Point(0, 0);
            this.ucPicButtonTop1.Size = new System.Drawing.Size(180, 43);
            this.upbFile2Word.Location = new Point(0, 43);
            this.upbFile2Word.Size = new System.Drawing.Size(180, 30);
            this.upbFile2Excel.Location = new Point(0, 73);
            this.upbFile2Excel.Size = new System.Drawing.Size(180, 30);
            this.upbFile2PPT.Location = new Point(0, 103);
            this.upbFile2PPT.Size = new System.Drawing.Size(180, 30);
            this.upbFile2HTML.Location = new Point(0, 133);
            this.upbFile2HTML.Size = new System.Drawing.Size(180, 30);
            this.upbFile2TXT.Location = new Point(0, 163);
            this.upbFile2TXT.Size = new System.Drawing.Size(180, 30);
            this.upbFile2IMG.Location = new Point(0, 193);
            this.upbFile2IMG.Size = new System.Drawing.Size(180, 30);

            this.ucPicButtonTop2.Location = new Point(0, 223);
            this.ucPicButtonTop2.Size = new System.Drawing.Size(180, 42);
            this.upbIMG2PDF.Location = new Point(0, 265);
            this.upbIMG2PDF.Size = new System.Drawing.Size(180, 30);
            this.upbDoc2PDF.Location = new Point(0, 295);
            this.upbDoc2PDF.Size = new System.Drawing.Size(180, 30);
            this.upbExcel2PDF.Location = new Point(0, 325);
            this.upbExcel2PDF.Size = new System.Drawing.Size(180, 30);
            this.upbPPT2PDF.Location = new Point(0, 355);
            this.upbPPT2PDF.Size = new System.Drawing.Size(180, 36);

            this.ucPicButtonTop3.Location = new Point(0, 391);
            this.ucPicButtonTop3.Size = new System.Drawing.Size(180, 43);
            this.upbPdfSplit.Location = new Point(0, 434);
            this.upbPdfSplit.Size = new System.Drawing.Size(180, 33);
            this.upbPDFMerge.Location = new Point(0, 467);
            this.upbPDFMerge.Size = new System.Drawing.Size(180, 30);
            this.upbPDFDecode.Location = new Point(0, 497);
            this.upbPDFDecode.Size = new System.Drawing.Size(180, 30);
            this.upbPDFCompress.Location = new Point(0, 527);
            this.upbPDFCompress.Size = new System.Drawing.Size(180, 30);
            this.upbPDFGetImg.Location = new Point(0, 557);
            this.upbPDFGetImg.Size = new System.Drawing.Size(180, 30);

            //pbLogo.BackgroundImage = Properties.Resources.logo_050;
            //pbLogo.Size = new System.Drawing.Size(206, 40);
            // btnQQ.Visible = false;
            // btnPhone.Visible = false;
            //plPPT.Location = new Point(3, 34);
            //plPPT.Size = new System.Drawing.Size(344, 35);
            //lblPPTSize.Location = new Point(5, 9);
            //lblPPTSize.Size = new System.Drawing.Size(116, 17);
            //lblWidth.Location = new Point(116, 10);
            //lblWidth.Size = new System.Drawing.Size(42, 17);
            //txtWidth.Location = new Point(163, 10);
            //txtWidth.Size = new System.Drawing.Size(59, 21);
            //lblHeight.Location = new Point(228, 11);
            //lblHeight.Size = new System.Drawing.Size(46, 17);
            //txtHeight.Location = new Point(280, 10);
            //txtHeight.Size = new System.Drawing.Size(59, 21);
            //cbIsMerger.Location = new Point(520, 6);
            //cbIsMerger.Size = new System.Drawing.Size(264, 16);
            rdoPath.Location = new Point(99, 2);
            //rdoPath.Size = new System.Drawing.Size(214, 24);
            rdoNewPath.Location = new Point(185, 2);
            //rdoNewPath.Size = new System.Drawing.Size(130, 24);
            //txtOutPath.Location = new Point(148, 119);
            //btnBrowse.Location = new Point(531, 119);
            btnStart.Location = new Point(234, 5);
            //btnCourse.Location = new Point(17, 17);
            //btnCourse.Size = new System.Drawing.Size(80, 38);
            //btnCourse.ButtonTextFont = new System.Drawing.Font("微软雅黑", 12);
            //btnBuy.ButtonTextFont = new System.Drawing.Font("微软雅黑", 12);
            //btnBuy.Location = new Point(103, 17);
            //btnBuy.Size = new System.Drawing.Size(72, 38);
            //btnQQ.Location = new Point(182, 17);
            //btnQQ.Size = new System.Drawing.Size(232, 38);
            //btnQQ.ButtonImage = Properties.Resources.emailnew;
            //btnQQ.FromType = 7;
            //btnPhone.Location = new Point(423, 17);
            //btnPhone.Size = new System.Drawing.Size(273, 38);
        }

        public void URL(object obj)
        {
            if (obj != null)
            {
                TempUrl url = obj as TempUrl;
                Version.Post("http://all.jsocr.com/", Version.GetParamName("Data"), Program.appProgName, Version.GetParamName("Version"), Program.encodingCode, url.Target, url.MehodObject);
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
            //if (lstFile.IsAllFinished && fileQueue.Count == 0)
            //{
            //    RegTips frm = new RegTips();
            //    frm.StartPosition = FormStartPosition.CenterParent;
            //    frm.Location = this.PointToScreen(new Point(400, this.lstFile.Location.Y + 30));
            //    DialogResult dr = frm.ShowDialog();
            //    if (dr == System.Windows.Forms.DialogResult.OK)
            //    {
            //        RegDlg reg = new RegDlg();
            //        reg.StartPosition = FormStartPosition.CenterParent;
            //        reg.Location = this.PointToScreen(new Point(250, lstFile.Location.Y - 10));
            //        reg.ShowDialog();
            //        if (new reg().Is_Reg())
            //        {
            //            this.lblTitle.Text = Encrypt.APPTITLE + " " + rm.GetString("OfficialVersion") + " v" +
            //                                 Version.version;
            //            isReg = true;
            //            // pltext.Visible = true;
            //        }
            //        else
            //        {
            //            this.lblTitle.Text = Encrypt.APPTITLE + " " + rm.GetString("FreeTrialVersion") + " v" +
            //                                 Version.version;
            //            isReg = false;
            //            // pltext.Visible = false;
            //        }
            //    }
            //}

            //KongMengyuan修改,2015-11-03,参考互盾PDFCon
            if (lstFile.IsAllFinished && !Poped)// && fileQueue.Count == 0)
            {
                Poped = true;
                RegTipsOld frm = new RegTipsOld();
                frm.StartPosition = FormStartPosition.CenterParent;
                frm.Location = this.PointToScreen(new Point(400, this.lstFile.Location.Y + 30));
                DialogResult dr = frm.ShowDialog();
                if (dr == System.Windows.Forms.DialogResult.OK)
                {
                    RegDlgOld reg = new RegDlgOld();
                    reg.StartPosition = FormStartPosition.CenterParent;
                    reg.Location = this.PointToScreen(new Point(250, lstFile.Location.Y - 10));
                    reg.ShowDialog();
                    if (new reg().Is_Reg())
                    {
                        this.lblTitle.Text = Encrypt.APPTITLE + " " + rm.GetString("OfficialVersion");// + " v" +  Version.version;
                        this.panel1.BackgroundImage = Properties.Resources.header_01; //已注册
                        isReg = true;
                        // pltext.Visible = true;
                    }
                    else
                    {
                        this.lblTitle.Text = Encrypt.APPTITLE + " " + rm.GetString("FreeTrialVersion");// + " v" +  Version.version;
                        this.panel1.BackgroundImage = Properties.Resources.header_02; //未注册
                        isReg = false;
                        // pltext.Visible = false;
                    }
                    Program.progTitle = this.lblTitle.Text;
                }
            }
        }
        
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
                    labProgressUC.ProgressStr = mess;
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
                    case ConvertOld.FORMAT.File2WORD:
                        {
                            if (extensions == ".pdf" || extensions == ".xls" || extensions == ".xlsx" ||
                                extensions == ".ppt" || extensions == ".pptx")
                            {
                                fileName = file.FullName;
                            }
                        }
                        break;
                    case ConvertOld.FORMAT.File2EXCEL:
                        {
                            if (extensions == ".pdf" || extensions == ".docx" || extensions == ".doc" ||
                                extensions == ".ppt" || extensions == ".pptx")
                            {
                                fileName = file.FullName;
                            }
                        }
                        break;
                    case ConvertOld.FORMAT.File2PPT:
                        {
                            if (extensions == ".pdf" || extensions == ".docx" || extensions == ".doc" ||
                                extensions == ".xls" || extensions == ".xlsx")
                            {
                                fileName = file.FullName;
                            }
                        }
                        break;
                    case ConvertOld.FORMAT.File2HTML:
                        {
                            if (extensions == ".pdf" || extensions == ".docx" || extensions == ".doc" ||
                                extensions == ".xls" || extensions == ".xlsx" || extensions == ".ppt" ||
                                extensions == ".pptx")
                            {
                                fileName = file.FullName;
                            }
                        }
                        break;
                    case ConvertOld.FORMAT.IMG2PDF:
                        {
                            if (extensions == ".jpg" || extensions == ".jpeg" || extensions == ".gif" ||
                                extensions == ".bmp" || extensions == ".png" || extensions == ".tiff" ||
                                extensions == ".tif")
                            {
                                fileName = file.FullName;
                            }
                        }
                        break;
                    case ConvertOld.FORMAT.File2TXT:
                        {
                            if (extensions == ".pdf" || extensions == ".docx" || extensions == ".doc" ||
                                extensions == ".xls" || extensions == ".xlsx" || extensions == ".ppt" ||
                                extensions == ".pptx")
                            {
                                fileName = file.FullName;
                            }
                        }
                        break;
                    case ConvertOld.FORMAT.File2IMG:
                        {
                            if (extensions == ".pdf" || extensions == ".docx" || extensions == ".doc" ||
                                extensions == ".xls" || extensions == ".xlsx" || extensions == ".ppt" ||
                                extensions == ".pptx")
                            {
                                fileName = file.FullName;
                            }
                        }
                        break;

                    case ConvertOld.FORMAT.DOC2PDF:
                        {
                            if (extensions == ".docx" || extensions == ".doc")
                            {
                                fileName = file.FullName;
                            }
                        }
                        break;

                    case ConvertOld.FORMAT.PPT2PDF:
                        {
                            if (extensions == ".ppt" || extensions == ".pptx")
                            {
                                fileName = file.FullName;
                            }
                        }
                        break;
                    case ConvertOld.FORMAT.Excel2PDF:
                        {
                            if (extensions == ".xls" || extensions == ".xlsx")
                            {
                                fileName = file.FullName;
                            }
                        }
                        break;
                    case ConvertOld.FORMAT.PDFSplit:
                        {
                            if (extensions == ".pdf")
                            {
                                fileName = file.FullName;
                            }
                        }
                        break;
                    case ConvertOld.FORMAT.PDFDecode:
                        {
                            if (extensions == ".pdf")
                            {
                                fileName = file.FullName;
                            }
                        }
                        break;
                    case ConvertOld.FORMAT.PDFMerge:
                        {
                            if (extensions == ".pdf")
                            {
                                fileName = file.FullName;
                            }
                        }
                        break;
                    case ConvertOld.FORMAT.PDFCompress:
                        {
                            if (extensions == ".pdf")
                            {
                                fileName = file.FullName;
                            }
                        }
                        break;
                    case ConvertOld.FORMAT.PDFGetImg:
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
                    if (show_flag && openFileDialog.FileNames.Length == 1)
                    {
                        show_flag = false;
                        MessageBox.Show(string.Format(rm.GetString("msg9"), Path.GetFileName(fileName))
                            , rm.GetString("Tips"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                        //MessageBox.Show("您添加的文件 " + Path.GetFileName(fileName) + " 已存在,我们将会自动过滤这些文件!"
                        //    , "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else if (show_flag && openFileDialog.FileNames.Length != 1)
                    {
                        show_flag = false;
                        MessageBox.Show(rm.GetString("msg1")
                            , rm.GetString("Tips"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                        //MessageBox.Show("您添加的部分文件已存在,我们将会自动过滤这些文件!"
                        //    , "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    continue;
                }
                if (!fileName.Contains("$"))
                {
                    ItemInfomation info = new ItemInfomation(fileName);
                    lstFile.ConversionPageDefaultText = rm.GetString("ALL");
                    lstFile.AddFile(info);//添加文件进入ListView(填加文件)
                    diclst.Add(fileName, false);

                    backGroundShowHide();//隐藏或显示主界面提示图
                    SetThreeButtonValidate(true);//开始转换,停止转换,清空列表,三个按钮的有效还是失效: true-有效,false-失效
                }
            }
        }

        /// <summary>
        /// 导航菜单默认选中
        /// </summary>
        /// <param name="index"></param>
        private void MenuSeletect(int select)
        {
            switch (select)
            {
                case 0:
                    upbFile2Word.Selected = true;
                    format = ConvertOld.FORMAT.File2WORD;
                    break;
                case 1:
                    upbFile2Excel.Selected = true;
                    format = ConvertOld.FORMAT.File2EXCEL;
                    break;
                case 2:
                    upbFile2PPT.Selected = true;
                    this.plPPT.Visible = true;
                    format = ConvertOld.FORMAT.File2PPT;
                    break;
                case 3:
                    upbFile2HTML.Selected = true;
                    format = ConvertOld.FORMAT.File2HTML;
                    break;
                case 4:
                    upbIMG2PDF.Selected = true;
                    //this.cbIsMerger.Visible = true;
                    format = ConvertOld.FORMAT.IMG2PDF;
                    break;
                case 5:
                    upbFile2TXT.Selected = true;
                    format = ConvertOld.FORMAT.File2TXT;
                    break;
                case 6:
                    upbFile2IMG.Selected = true;
                    format = ConvertOld.FORMAT.File2IMG;
                    break;
                case 7:
                    upbDoc2PDF.Selected = true;
                    format = ConvertOld.FORMAT.DOC2PDF;
                    break;
                case 8:
                    upbPPT2PDF.Selected = true;
                    format = ConvertOld.FORMAT.PPT2PDF;
                    break;
                case 9:
                    upbExcel2PDF.Selected = true;
                    format = ConvertOld.FORMAT.Excel2PDF;
                    break;
                default:
                    upbFile2Word.Selected = true;
                    format = ConvertOld.FORMAT.File2WORD;
                    break;
            }
        }

        private string GetTaskName()
        {
            string tackName = string.Empty;
            switch (format)
            {
                case ConvertOld.FORMAT.File2WORD:
                    {
                        tackName = rm.GetString("FileTo") + " Word";
                    }
                    break;
                case ConvertOld.FORMAT.File2EXCEL:
                    {
                        tackName = rm.GetString("FileTo") + " EXCEL";
                    }
                    break;
                case ConvertOld.FORMAT.File2PPT:
                    {
                        tackName = rm.GetString("FileTo") + " PPT";
                    }
                    break;
                case ConvertOld.FORMAT.File2HTML:
                    {
                        tackName = rm.GetString("FileTo") + " HTML";
                    }
                    break;
                case ConvertOld.FORMAT.IMG2PDF:
                    {
                        tackName = rm.GetString("IMG") + rm.GetString("Turn") + " PDF";
                    }
                    break;
                case ConvertOld.FORMAT.File2TXT:
                    {
                        tackName = rm.GetString("FileTo") + " TXT";
                    }
                    break;
                case ConvertOld.FORMAT.File2IMG:
                    {
                        tackName = rm.GetString("FileTo") + rm.GetString("IMG");
                    }
                    break;

                case ConvertOld.FORMAT.DOC2PDF:
                    {
                        tackName = "Word " + rm.GetString("Turn") + " PDF";
                    }
                    break;

                case ConvertOld.FORMAT.PPT2PDF:
                    {
                        tackName = "PPT " + rm.GetString("Turn") + " PDF";
                    }
                    break;
                case ConvertOld.FORMAT.PDFSplit:
                    {
                        tackName = "PDF Split " + rm.GetString("Turn") + " PDF";
                    }
                    break;
                case ConvertOld.FORMAT.PDFDecode:
                    {
                        tackName = "PDF Decode " + rm.GetString("Turn") + " PDF";
                    }
                    break;
                case ConvertOld.FORMAT.PDFMerge:
                    {
                        tackName = "PDF合并";
                    }
                    break;
                case ConvertOld.FORMAT.PDFCompress:
                    {
                        tackName = "PDF Compress " + rm.GetString("Turn") + " PDF";
                    }
                    break;
                case ConvertOld.FORMAT.Excel2PDF:
                    {
                        tackName = "Excel " + rm.GetString("Turn") + " PDF";
                    }
                    break;
                case ConvertOld.FORMAT.PDFGetImg:
                    {
                        tackName = "PDF Get " + rm.GetString("Turn") + " IMG";
                    }
                    break;
            }
            return tackName;
        }

        private bool IsMatched(string file_name, ConvertOld.FORMAT format)
        {
            bool result = false;

            string suffix = Path.GetExtension(file_name).ToUpper();
            if (format == ConvertOld.FORMAT.File2WORD)
            {
                if (suffix == ".PDF" || suffix == ".XLS" || suffix == ".XLSX" || suffix == ".PPT" || suffix == ".PPTX")
                {
                    result = true;
                }
            }
            else if (format == ConvertOld.FORMAT.File2EXCEL)
            {
                if (suffix == ".PDF" || suffix == ".DOC" || suffix == ".DOCX" || suffix == ".PPT" || suffix == ".PPTX")
                {
                    result = true;
                }
            }
            else if (format == ConvertOld.FORMAT.File2PPT)
            {
                if (suffix == ".PDF" || suffix == ".DOC" || suffix == ".DOCX" || suffix == ".XLS" || suffix == ".XLSX")
                {
                    result = true;
                }
            }
            else if (format == ConvertOld.FORMAT.File2IMG || format == ConvertOld.FORMAT.File2HTML ||
                     format == ConvertOld.FORMAT.File2TXT)
            {
                if (suffix == ".PDF" || suffix == ".DOC" || suffix == ".DOCX" || suffix == ".PPT" ||
                    suffix == ".PPTX" || suffix == ".XLS" || suffix == ".XLSX")
                {
                    result = true;
                }
            }

            else if (format == ConvertOld.FORMAT.IMG2PDF)
            {
                if (suffix == ".JPG" || suffix == ".JPEG" || suffix == ".GIF" || suffix == ".BMP" ||
                    suffix == ".PNG" || suffix == ".TIF" || suffix == ".TIFF")
                {
                    return result = true;
                    ;
                }
            }
            else if (format == ConvertOld.FORMAT.DOC2PDF)
            {
                if (suffix == ".DOC" || suffix == ".DOCX")
                {
                    result = true;
                }
            }
            else if (format == ConvertOld.FORMAT.Excel2PDF)
            {
                if (suffix == ".XLS" || suffix == ".XLSX")
                {
                    result = true;
                }
            }

            else if (format == ConvertOld.FORMAT.PPT2PDF)
            {
                if (suffix == ".PPT" || suffix == ".PPTX")
                {
                    result = true;
                }
            }

            else if (format == ConvertOld.FORMAT.PDFSplit)
            {
                if (suffix == ".PDF")
                {
                    result = true;
                }
            }

            else if (format == ConvertOld.FORMAT.PDFDecode)
            {
                if (suffix == ".PDF")
                {
                    result = true;
                }
            }

            else if (format == ConvertOld.FORMAT.PDFMerge)
            {
                if (suffix == ".PDF")
                {
                    result = true;
                }
            }

            else if (format == ConvertOld.FORMAT.PDFCompress)
            {
                if (suffix == ".PDF")
                {
                    result = true;
                }
            }

            else if (format == ConvertOld.FORMAT.PDFGetImg)
            {
                if (suffix == ".PDF")
                {
                    result = true;
                }
            }
            return result;
        }

        private void WorkThread(object pram)
        {
            try
            {
                //队列线程索引
                int thr_index = System.Convert.ToInt32(pram);

                ListViewItem lv = null;
                ConvertOld ins;
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
                        //KongMengyuan修改,2015-11-03,保留当前输出文件夹(在文件转换过程中不允许更改它)
                        if (outpathSelect == "rdoNewPath")
                        {
                            rdoNewPath.Checked = true;
                            rdoPath.Checked = false;
                        }
                        else
                        {
                            rdoNewPath.Checked = false;
                            rdoPath.Checked = true;
                        }

                        string fileName = ((ItemInfomation)lv.Tag).FileFullPath;
                        string sourseName = Path.GetFileNameWithoutExtension(fileName);
                        string soursePath = Path.GetDirectoryName(fileName) + "\\" + sourseName;
                        if (soursePath.Contains("\\\\"))
                        {
                            try
                            {
                                soursePath = soursePath.Replace("\\\\", "\\");
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                            }
                        }
                        string path = this.rdoPath.Checked ? soursePath : this.txtOutPath.OutText + "\\" + sourseName;
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
                            MessageBox.Show("转换的源文件不存在。");
                            ItemInfomation info = (ItemInfomation)lv.Tag;
                            info.Status = StatusType.Done;
                            info.PersentValue = 0;
                            btnStart.Enabled = true;
                            btnStart.BackgroundImage = Properties.Resources.btn_kszh_01;
                        }
                        else
                        {
                            ins = new ConvertOld(fileName, path, format, this);
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
                                    btnStart.BackgroundImage = Properties.Resources.btn_kszh_01;
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
                                        MessageBox.Show(name + " 文件读取错误,请检查文件是否加密！");
                                    }
                                }
                            }
                            else
                            {
                                ins.Save(this, Path.GetExtension(fileName), lv.Index, lv); //开始转换,KongMengyuan注释,2015-11-19
                                Console.WriteLine("MainInfoOld:" + fileName);
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
                                if (ins.Count > 5 && ins.targetFormat != ConvertOld.FORMAT.PDFMerge && ins.targetFormat != ConvertOld.FORMAT.PDFCompress && ins.targetFormat != ConvertOld.FORMAT.PDFGetImg && ins.Can_work())
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

        private void ins_ErrorMessageEvent(object sender, ErrorMessageArgs args)
        {
            //Invoke(new MethodInvoker(delegate()
            //{

            //}));
            MessageBox.Show(args.message);
        }

        private void comboBoxPage_Leave(object sender, EventArgs e)
        {
            //string text = comboBoxPage.Text;
            //try
            //{
            //    if (text != rm.GetString("ALL"))
            //    {
            //        //if (lstFile.SelectedItems.Count > 0)
            //        //{
            //        //string[] sp_text = text.Split('-');
            //        //if (sp_text.Length != 2)
            //        //{
            //        //    //MessageBox.Show("选择页数格式错误,请重新输入", "错误", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //        //    lstFile.SelectedItems[0].SubItems[3].Text = rm.GetString("ALL");
            //        //    return;
            //        //}
            //        //if (System.Convert.ToInt32(sp_text[0]) > System.Convert.ToInt32(sp_text[1]))
            //        //{
            //        //    MessageBox.Show(rm.GetString("msg5"), rm.GetString("Error"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            //        //    //MessageBox.Show("起始页应小于等于最终页,请重新输入", "错误", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //        //    lstFile.SelectedItems[0].SubItems[3].Text = rm.GetString("ALL");
            //        //    return;
            //        //}
            //        if (text.Trim() == "")
            //        {
            //            comboBoxPage.Text = "全部";
            //        }
            //        lstFile.SelectedItems[0].SubItems[3].Text = comboBoxPage.Text;
            //        //}
            //    }
            //}
            //catch
            //{
            //    //MessageBox.Show("选择页数格式错误,请重新输入", "错误", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    if (lstFile.SelectedItems.Count > 0)
            //    {
            //        lstFile.SelectedItems[0].SubItems[3].Text = rm.GetString("ALL");
            //    }
            //}
            //pltext.Visible = false;

            //KongMengyuan修改,2015-11-06,把上面的代码注释掉,参考互盾PDFCon
            string text = comboBoxPage.Text;
            try
            {
                if (text != rm.GetString("ALL"))
                {
                    if (VerifyPageSet(text) == false && text.Length > 0)
                    {
                        MessageBox.Show("请输入正确的页码格式");
                        comboBoxPage.Text = "全部";
                    }
                    else if (text.Length > 0)
                    {
                        lstFile.SelectedItems[0].SubItems[3].Text = comboBoxPage.Text;
                    }
                }
            }
            catch
            {
                //MessageBox.Show("选择页数格式错误,请重新输入", "错误", MessageBoxButtons.OK, MessageBoxIcon.Information);
                if (lstFile.SelectedItems.Count > 0)
                {
                    lstFile.SelectedItems[0].SubItems[3].Text = rm.GetString("ALL");
                }
            }
            pltext.Visible = false;
        }

        //KongMengyuan增加,2015-11-06,参考互盾PDFCon
        private bool VerifyPageSet(string text)
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

        private void comboBoxPage_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    if (string.IsNullOrEmpty(comboBoxPage.Text.Trim()))
                    {
                        comboBoxPage.Text = rm.GetString("ALL");
                    }
                    lstFile.SelectedItems[0].SubItems[3].Text = comboBoxPage.Text;
                    pltext.Visible = false;
                    lstFile.Focus();
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }
        }

        public string FuncName = "upbFile2Word";
        private void upbFile2Word_Click(object sender, EventArgs e)
        {
            //KongMengyuan增加,2015-11-03,参考互盾PDFCon
            //每个按钮都把Dock设置为Top,同时在Designer里面手动移动这行的前后位置“this.plNavigation.Controls.Add(this.upbFile2Word);”,就可以更改按钮的顺序了
            if (this.btnStart.Enabled == false && pbIndexBackGround.Visible == false)
            {
                SetButtonUnSelect(FuncName);
                return;
            }

            //cbIsMerger.Visible = false;
            plPPT.Visible = false;
            // comboBoxPage.Enabled = true;
            string name = ((Control)sender).Name;
            switch (name)
            {
                case "upbFile2Word": //文件转Word
                    format = ConvertOld.FORMAT.File2WORD;
                    //contextSplit.Panel1Collapsed = true;//是否折叠,顶部额外显示自定义,控制高度,原设计人员使用的,不易维护,KongMengyuan注释,2015-11-20
                    showUserDefine(0); //是否显示左侧按钮的额外自定义内容
                    break;
                case "upbFile2Excel": //文件转Excel
                    //contextSplit.Panel1Collapsed = true;//是否折叠,顶部额外显示自定义,控制高度,原设计人员使用的,不易维护,KongMengyuan注释,2015-11-20
                    showUserDefine(0); //是否显示左侧按钮的额外自定义内容
                    format = ConvertOld.FORMAT.File2EXCEL;
                    break;
                case "upbFile2PPT": //文件转PPT
                    //以下两行顶部额外显示自定义,控制高度,原设计人员使用的,不易维护,KongMengyuan注释,2015-11-20
                    //contextSplit.Panel1Collapsed = false; //是否折叠
                    //contextSplit.SplitterDistance = 40; //控制高度的
                    showUserDefine(1); //是否显示左侧按钮的额外自定义内容
                    labProgressUC.Visible = false;
                    format = ConvertOld.FORMAT.File2PPT;

                    break;
                case "upbFile2HTML": //文件转HTML
                    //contextSplit.Panel1Collapsed = true;//是否折叠,顶部额外显示自定义,控制高度,原设计人员使用的,不易维护,KongMengyuan注释,2015-11-20
                    showUserDefine(0); //是否显示左侧按钮的额外自定义内容
                    format = ConvertOld.FORMAT.File2HTML;
                    break;
                case "upbIMG2PDF": //图片转PDF
                    labProgressUC.Visible = false;
                    //contextSplit.Panel1Collapsed = false;
                    //contextSplit.SplitterDistance = 20;
                    //// comboBoxPage.Enabled = false;
                    ////contextSplit.Panel1Collapsed = true;
                    showUserDefine(2); //是否显示左侧按钮的额外自定义内容
                    format = ConvertOld.FORMAT.IMG2PDF;
                    break;
                case "upbFile2TXT": //文件转TXT
                    //contextSplit.Panel1Collapsed = true;//是否折叠,顶部额外显示自定义,控制高度,原设计人员使用的,不易维护,KongMengyuan注释,2015-11-20
                    showUserDefine(0); //是否显示左侧按钮的额外自定义内容
                    format = ConvertOld.FORMAT.File2TXT;
                    break;
                case "upbFile2IMG": //文件转图片
                    //contextSplit.Panel1Collapsed = true;//是否折叠,顶部额外显示自定义,控制高度,原设计人员使用的,不易维护,KongMengyuan注释,2015-11-20
                    showUserDefine(0); //是否显示左侧按钮的额外自定义内容
                    format = ConvertOld.FORMAT.File2IMG;
                    break;
                case "upbDoc2PDF": //Word转PDF
                    //contextSplit.Panel1Collapsed = true;//是否折叠,顶部额外显示自定义,控制高度,原设计人员使用的,不易维护,KongMengyuan注释,2015-11-20
                    showUserDefine(0); //是否显示左侧按钮的额外自定义内容
                    format = ConvertOld.FORMAT.DOC2PDF;
                    break;
                case "upbPPT2PDF": //PPT转PDF
                    showUserDefine(0); //是否显示左侧按钮的额外自定义内容
                    format = ConvertOld.FORMAT.PPT2PDF;
                    break;
                case "upbExcel2PDF": //Excel转PDF
                    //contextSplit.Panel1Collapsed = true;//是否折叠,顶部额外显示自定义,控制高度,原设计人员使用的,不易维护,KongMengyuan注释,2015-11-20
                    showUserDefine(0); //是否显示左侧按钮的额外自定义内容
                    format = ConvertOld.FORMAT.Excel2PDF;
                    break;
                case "upbPdfSplit": //PDF分割
                    //contextSplit.Panel1Collapsed = true;//是否折叠,顶部额外显示自定义,控制高度,原设计人员使用的,不易维护,KongMengyuan注释,2015-11-20
                    showUserDefine(0); //是否显示左侧按钮的额外自定义内容
                    format = ConvertOld.FORMAT.PDFSplit;
                    //   MessageBox.Show("请点击页码选择框，进行页码选择");
                    ShowDefaultPageBox();
                    break;
                case "upbPDFMerge": //PDF合并
                    //cbIsMerger.Visible = false;
                    //plPPT.Visible = false;
                    //labProgressUC.Visible = true;
                    //contextSplit.Panel1Collapsed = false;//是否折叠,顶部额外显示自定义,控制高度,原设计人员使用的,不易维护,KongMengyuan注释,2015-11-20
                    //contextSplit.SplitterDistance = 100;
                    labProgressUC.ProgressStr = "0 %";
                    showUserDefine(3); //是否显示左侧按钮的额外自定义内容
                    format = ConvertOld.FORMAT.PDFMerge;
                    break;
                case "upbPDFDecode": //PDF密码解除
                    //contextSplit.Panel1Collapsed = true;//是否折叠,顶部额外显示自定义,控制高度,原设计人员使用的,不易维护,KongMengyuan注释,2015-11-20
                    showUserDefine(0); //是否显示左侧按钮的额外自定义内容
                    format = ConvertOld.FORMAT.PDFDecode;
                    break;
                case "upbPDFCompress": //PDF压缩
                    //contextSplit.Panel1Collapsed = true;//是否折叠,顶部额外显示自定义,控制高度,原设计人员使用的,不易维护,KongMengyuan注释,2015-11-20
                    showUserDefine(0); //是否显示左侧按钮的额外自定义内容
                    format = ConvertOld.FORMAT.PDFCompress;
                    break;
                case "upbPDFGetImg": //PDF图片获取
                    //contextSplit.Panel1Collapsed = true;//是否折叠,顶部额外显示自定义,控制高度,原设计人员使用的,不易维护,KongMengyuan注释,2015-11-20
                    showUserDefine(0); //是否显示左侧按钮的额外自定义内容
                    format = ConvertOld.FORMAT.PDFGetImg;
                    break;
                default:
                    break;
            }
            SetButtonUnSelect(((Control)sender).Name);
        }

        #region 是否显示左侧按钮的额外自定义内容
        private void showUserDefine(int itemSelect)
        {
            //itemSelect: 0-普通按钮,1-文件转PPT,2-图片转PDF,3-PDF合并
            //放置“文件转PPT”的"PPT大小设置",“图片转PDF”的"将所有图片合并成一个PDF文件",“PDF合并”百分比,都放在Panel4上(原设计人员定义的,最麻烦的是原设计人员将其Dock定义为Fill,这样即使找到lblPageSize也看不到,已经修改这种作法了)
            switch (itemSelect)
            {
                case 0: //0-普通按钮,1-文件转PPT,2-图片转PDF,3-PDF合并,目前3不用考虑了
                case 3: //0-普通按钮,1-文件转PPT,2-图片转PDF,3-PDF合并,目前3不用考虑了
                    this.panel4.Visible = false;
                    lstFile.Location = new Point(0, 3);//new Point(20, 20);
                    lstFile.Height = 560;
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
        #endregion

        private void ShowDefaultPageBox()
        {
            if (format != ConvertOld.FORMAT.PDFSplit)
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
                //this.pltext.Height = height - 14;
                if (lstFile.Items[index].SubItems[3].Text != rm.GetString("ALL"))
                {
                    comboBoxPage.Text = lstFile.Items[index].SubItems[3].Text;
                }
                else
                {
                    comboBoxPage.Text = string.Empty;
                }

                m_EditIndex = lstFile.Items[index].Index;
            }
        }

        private void SetButtonUnSelect(string name)
        {
            foreach (Control c in plNavigation.Controls)
            {
                //if (c.Name != name)
                if (c.Name != name && c.Name != "ucPicButtonTop1" && c.Name != "ucPicButtonTop2" && c.Name != "ucPicButtonTop3")
                    ((ucPicButton)c).Selected = false;
            }
        }

        private void pictureBox3_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                this.contextMenuStrip1.Show(pbMenu, 0, pbMenu.Height);
            }
        }

        private void WinformClose()
        {
            ini_config ini = new ini_config("config.ini");
            ini.write_ini("TargetDir", this.txtOutPath.OutText);
            ini.write_ini("PicX", this.txtWidth.Text);
            ini.write_ini("PicY", this.txtHeight.Text);
            ini.write_ini("Type", System.Convert.ToInt32(format).ToString());
            ini.write_ini("Out", this.rdoPath.Checked ? "1" : "0");
            //this.cbIsMerger.Checked = true;
            this.rbIsMerger1.Checked = true;
            ini.write_ini("isMerger", "1");
            isClose = true;
            ////this.Dispose();
            //System.Environment.Exit(-1);
            //this.Close();

            //KongMengyuan修改,2015-11-11,原语句从注册页面返回会错误,而且那种写法也不是C#的WinForm最好的关闭Application方法,应该使用下面语句关闭系统
            Application.Exit();
            System.Diagnostics.Process.GetCurrentProcess().Kill(); //退出系统
        }

        private void btnAddFiles_MouseEnter(object sender, EventArgs e)
        {
            this.btnAddFiles.ButtonBackIMG = Properties.Resources.btn_tjwj_02;
        }

        private void btnAddFiles_MouseLeave(object sender, EventArgs e)
        {
            this.btnAddFiles.ButtonBackIMG = Properties.Resources.btn_tjwj_01;
        }

        private void btnStart_MouseEnter(object sender, EventArgs e)
        {
            if (((Control)sender).Enabled != false)
            {
                this.btnStart.ButtonBackIMG = Properties.Resources.btn_kszh_02;
            }
        }

        private void btnStart_MouseLeave(object sender, EventArgs e)
        {
            if (((Control)sender).Enabled != false)
            {
                this.btnStart.ButtonBackIMG = Properties.Resources.btn_kszh_01;
            }
        }

        private void btnFolder_MouseEnter(object sender, EventArgs e)
        {
            this.btnFolder.ButtonBackIMG = Properties.Resources.btn_tjwjj_02;
        }

        private void btnFolder_MouseLeave(object sender, EventArgs e)
        {
            this.btnFolder.ButtonBackIMG = Properties.Resources.btn_tjwjj_03;
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
                        MessageBox.Show(fileName + "\r\n" + "\r\n" + "文件已经不存在了，请检查！");
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
                            MessageBox.Show(string.Format(rm.GetString("msg3"), GetTaskName()), rm.GetString("Tips"),
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

        /// <summary>
        /// 暂停开始状态事件
        /// </summary>
        /// <param name="index"></param>
        /// <param name="status"></param>
        private void lstFile_OnStatusButtonClicked(int index, StatusType status)
        {
            this.pltext.Visible = false;
            if (status == StatusType.Start)
            {
                ((ItemInfomation)lstFile.Items[index].Tag).Status = StatusType.Pause;
                //发送请求信息
                TempUrl t = new TempUrl("主程序", "列表点击开始");
                PostURL(t);
            }
            else if (status == StatusType.Pause)
            {

                ((ItemInfomation)lstFile.Items[index].Tag).Status = StatusType.Start;
                //发送请求信息
                TempUrl t = new TempUrl("主程序", "列表点击暂停");
                PostURL(t);
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
                    try
                    {
                        threadLength = thread.Length;
                    }
                    catch
                    {
                        //threadLength = 0; //设置为0就不开始转换了,所以此处就没有用了
                    }
                    for (int j = 0; j < threadLength; j++) //2015-12-14,KongMengyuan注释,目前默认是只转换CPU个数少1核的进程数,发现虚拟机里“WinXP 64位”点击“开始转换”时此处出错,跟踪发现是"thread.length"不识别(编译时"目标平台"为X86和X64也都出错)。
                    {
                        if (thread[j].ThreadState == System.Threading.ThreadState.Stopped)
                        {
                            thread[j] = new Thread(new ParameterizedThreadStart(WorkThread));
                            thread[j].IsBackground = true;
                            thread[j].Start(j);
                        }
                    }
                }
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {

            if (DialogResult.OK == folderBrowserDialog.ShowDialog())
            {
                this.txtOutPath.OutText = folderBrowserDialog.SelectedPath;
            }
        }

        private void btnBrowse_MouseEnter(object sender, EventArgs e)
        {
            //this.btnBrowse.ButtonBackIMG = Properties.Resources.lookhover;
        }

        private void btnBrowse_MouseLeave(object sender, EventArgs e)
        {
            //this.btnBrowse.ButtonBackIMG = Properties.Resources.look;
        }

        /// <summary>
        /// 列表删除
        /// </summary>
        /// <param name="index"></param>
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
                if (MessageBox.Show("是否删除文件 " + "\r\n" + "\r\n" + lstFile.SelectedItems[0].SubItems[1].Text, "删除提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
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
                    btnStart.BackgroundImage = Properties.Resources.btn_kszh_01;
                }

                //发送请求信息
                TempUrl t = new TempUrl("主程序", "列表删除[" + Info.FileFullPath + "]文件");
                PostURL(t);

                backGroundShowHide();//隐藏或显示主界面提示图
            }
        }

        /// <summary>
        /// 列表打开文件
        /// </summary>
        /// <param name="index"></param>
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
                //发送请求信息
                TempUrl t = new TempUrl("主程序", "列表打开[" + filePath + "]文件");
                PostURL(t);
            }
            catch (Exception ex)
            {
                MessageBox.Show(rm.GetString("OpenFileButton"));
            }
        }

        /// <summary>
        /// 列表打开文件夹
        /// </summary>
        /// <param name="index"></param>
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
                //发送请求信息
                TempUrl t = new TempUrl("主程序", "列表打开文件夹[" + path + "]");
                PostURL(t);
            }
            catch
            {
            }
        }

        private void MainInfo_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, 274, 61440 + 9, 0);
            }
        }

        private void MainInfo_FormClosing(object sender, FormClosingEventArgs e)
        {
            WinformClose();
        }

        private void tsmEnglish_Click(object sender, EventArgs e)
        {
            ini.write_ini("language", "en");
            SetEn();
            if (isReg)
            {
                this.lblTitle.Text = Encrypt.Refresh() + " " + rm.GetString("OfficialVersion") + " v" + Program.httpVersion;// Version.version;
                this.panel1.BackgroundImage = Properties.Resources.header_01; //已注册
            }
            else
            {
                this.lblTitle.Text = Encrypt.Refresh() + " " + rm.GetString("FreeTrialVersion") + " v" + Program.httpVersion;// Version.version;
                this.panel1.BackgroundImage = Properties.Resources.header_02; //未注册
            }
            Program.progTitle = this.lblTitle.Text;
            //发送请求信息
            TempUrl t = new TempUrl("主程序", "选择英文");
            PostURL(t);
        }

        private void tsmCn_Click(object sender, EventArgs e)
        {
            ini.write_ini("language", "zh-CN");
            SetZhCn();
            if (isReg)
            {
                this.lblTitle.Text = Encrypt.Refresh() + " " + rm.GetString("OfficialVersion") + " v" + Program.httpVersion;// Version.version;
                this.panel1.BackgroundImage = Properties.Resources.header_01; //已注册
            }
            else
            {
                this.lblTitle.Text = Encrypt.Refresh() + " " + rm.GetString("FreeTrialVersion") + " v" + Program.httpVersion;//Version.version;
                this.panel1.BackgroundImage = Properties.Resources.header_02; //未注册
            }
            Program.progTitle = this.lblTitle.Text;
            //发送请求信息
            TempUrl t = new TempUrl("主程序", "选择中文");
            PostURL(t);
        }

        private void tsmAboutUs_Click(object sender, EventArgs e)
        {
            visitHttpAddress("guanyu");//访问www.xjpdf.com
        }

        private void tsmBuy_Click(object sender, EventArgs e)
        {
            visitHttpAddress("buy");//访问www.xjpdf.com
        }

        private int m_EditIndex;

        private void lstFile_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left && format != ConvertOld.FORMAT.IMG2PDF)
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
                        //pltext.Location =
                        //    new Point(lstFile.SelectedItems[0].SubItems[3].Bounds.Left + lstFile.Location.X + 2 + 7,
                        //        lstFile.SelectedItems[0].SubItems[3].Bounds.Top + lstFile.Location.Y + 1 + 8);
                        //this.pltext.Width = lstFile.SelectedItems[0].SubItems[3].Bounds.Width - 14;

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
                        //this.pltext.Height = height - 14;
                        if (lstFile.SelectedItems[0].SubItems[3].Text != rm.GetString("ALL"))
                        {
                            comboBoxPage.Text = lstFile.SelectedItems[0].SubItems[3].Text;
                        }
                        else
                        {
                            comboBoxPage.Text = string.Empty;
                        }

                        m_EditIndex = lstFile.SelectedItems[0].Index;
                        return;
                    }
                }
            }
            if (!string.IsNullOrEmpty(comboBoxPage.Text))
            {
                if (lstFile.SelectedItems.Count > 0 && lstFile.SelectedItems[0].Index == m_EditIndex)
                {
                    lstFile.SelectedItems[0].SubItems[3].Text = comboBoxPage.Text.Replace("\r\n", string.Empty);
                    comboBoxPage.Text = string.Empty;
                }

            }
            pltext.Visible = false;
        }

        private void btnAddFiles_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                this.btnAddFiles.ButtonBackIMG = Properties.Resources.btn_tjwj_03;

                openFileDialog.FileName = "";
                if (format == ConvertOld.FORMAT.File2WORD)
                {
                    openFileDialog.Filter = "Any文件(*.pdf,*.xls,*.xlsx,*.ppt,*.pptx)|*.pdf;*.xls;*.xlsx;*.ppt;*.pptx;";
                }
                else if (format == ConvertOld.FORMAT.File2EXCEL)
                {
                    openFileDialog.Filter = "Any文件(*.pdf,*.ppt,*.pptx,*.doc,*.docx)|*.pdf;*.ppt;*.pptx;*.doc;*.docx";
                }
                else if (format == ConvertOld.FORMAT.File2PPT)
                {
                    openFileDialog.Filter = "Any文件(*.pdf,*.xls,*.xlsx,*.doc,*.docx)|*.pdf;*.xls;*.xlsx;*.doc;*.docx";
                }
                else if (format == ConvertOld.FORMAT.File2IMG)
                {
                    openFileDialog.Filter =
                        "Any文件(*.pdf,*.ppt,*.pptx,*.doc,*.docx,*.xls,*.xlsx)|*.pdf;*.ppt;*.pptx;*.doc;*.docx;*.xls;*.xlsx";
                }
                else if (format == ConvertOld.FORMAT.File2TXT)
                {
                    openFileDialog.Filter =
                        "Any文件(*.pdf,*.ppt,*.pptx,*.doc,*.docx,*.xls,*.xlsx)|*.pdf;*.ppt;*.pptx;*.doc;*.docx;*.xls;*.xlsx";
                }
                else if (format == ConvertOld.FORMAT.File2HTML)
                {
                    openFileDialog.Filter =
                        "Any文件(*.pdf,*.ppt,*.pptx,*.doc,*.docx,*.xls,*.xlsx)|*.pdf;*.ppt;*.pptx;*.doc;*.docx;*.xls;*.xlsx";
                }
                else if (format == ConvertOld.FORMAT.IMG2PDF)
                {
                    openFileDialog.Filter =
                        "图片文件(*.jpg,*.jpeg,*.gif,*.bmp,*.png,*.tif,*.tiff)|*.jpg;*.jpeg;*.gif;*.bmp;*.png;*.tif;*.tiff";
                }
                else if (format == ConvertOld.FORMAT.DOC2PDF)
                {
                    openFileDialog.Filter = "Word文件(*.doc,*.docx)|*.doc;*.docx";
                }
                else if (format == ConvertOld.FORMAT.Excel2PDF)
                {
                    openFileDialog.Filter = "Excel文件(*.xls,*.xlsx)|*.xls;*.xlsx";
                }
                else if (format == ConvertOld.FORMAT.PPT2PDF)
                {
                    openFileDialog.Filter = "PowerPoint文件(*.ppt,*.pptx)|*.ppt;*.pptx";
                }
                else if (format == ConvertOld.FORMAT.PDFSplit)
                {
                    openFileDialog.Filter = "PDF文件(*.pdf)|*.pdf";
                }
                else if (format == ConvertOld.FORMAT.PDFDecode)
                {
                    openFileDialog.Filter = "PDF文件(*.pdf)|*.pdf";
                }
                else if (format == ConvertOld.FORMAT.PDFMerge)
                {
                    openFileDialog.Filter = "PDF文件(*.pdf)|*.pdf";
                }
                else if (format == ConvertOld.FORMAT.PDFCompress)
                {
                    openFileDialog.Filter = "PDF文件(*.pdf)|*.pdf";
                }
                else if (format == ConvertOld.FORMAT.PDFGetImg)
                {
                    openFileDialog.Filter = "PDF文件(*.pdf)|*.pdf";
                }
                bool show_flag = true;


                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    foreach (string file_name in openFileDialog.FileNames)
                    {
                        if (diclst.ContainsKey(file_name))
                        {
                            if (show_flag && openFileDialog.FileNames.Length == 1)
                            {
                                show_flag = false;
                                MessageBox.Show(string.Format(rm.GetString("msg9"), Path.GetFileName(file_name))
                                    , rm.GetString("Tips"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                                //MessageBox.Show("您添加的文件 " + Path.GetFileName(file_name) + " 已存在,我们将会自动过滤这些文件!"
                                //    , "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else if (show_flag && openFileDialog.FileNames.Length != 1)
                            {
                                show_flag = false;
                                //MessageBox.Show("您添加的部分文件已存在,我们将会自动过滤这些文件!"
                                //    , "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                MessageBox.Show(rm.GetString("msg1")
                                    , rm.GetString("Tips"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            continue;

                        }
                        ItemInfomation info = new ItemInfomation(file_name);
                        lstFile.ConversionPageDefaultText = rm.GetString("ALL");
                        lstFile.AddFile(info);//添加文件进入ListView(填加文件)
                        diclst.Add(file_name, false);

                        backGroundShowHide();//隐藏或显示主界面提示图
                        SetThreeButtonValidate(true);//开始转换,停止转换,清空列表,三个按钮的有效还是失效: true-有效,false-失效
                    }
                }

                //发送请求信息
                TempUrl t = new TempUrl("主程序", "添加文档");
                PostURL(t);
                lstFile.Focus();
            }
        }

        private void btnFolder_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                this.btnFolder.ButtonBackIMG = Properties.Resources.btn_tjwjj_03;

                if (DialogResult.OK == folderBrowserDialog.ShowDialog())
                {
                    GetFolder(folderBrowserDialog.SelectedPath);
                    //发送请求信息
                    TempUrl t = new TempUrl("主程序", "添加文件夹");
                    PostURL(t);
                }
            }
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
                this.btnStart.ButtonBackIMG = Properties.Resources.btn_kszh_01;//开始转换
                this.btnStop.ButtonBackIMG = Properties.Resources.btn_tzzh_01; //停止转换
                this.btnClear.ButtonBackIMG = Properties.Resources.btn_qklb_01;//清空列表
            }
            else
            {
                this.btnStart.ButtonBackIMG = Properties.Resources.btn_kszh_04;//开始转换
                this.btnStop.ButtonBackIMG = Properties.Resources.btn_tzzh_04; //停止转换
                this.btnClear.ButtonBackIMG = Properties.Resources.btn_qklb_04;//清空列表
            }
        }

        private bool isStart;
        private bool NeedToPop; //KongMengyuan增加,2015-11-03,参考互盾PDFCon
        private bool Poped = false; //KongMengyuan增加,2015-11-03,参考互盾PDFCon

        private void btnStart_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (lstFile.Items.Count < 1)//如果没有文件则不能点击
                {
                    SetThreeButtonValidate(false);//开始转换,停止转换,清空列表,三个按钮的有效还是失效: true-有效,false-失效
                    return;
                }
                //    isStart = true;
                //    VerifyList();
                //    this.pltext.Visible = false;
                //    //KongMengyuan,2015-11-12,下面法的不科学,应该发现文件没有的话,直接弹出提示“你输入的原文件不存在”
                //    //if (lstFile.Items.Count > 0 && this.lstFile.IsAllFinished)
                //    //{
                //    //    this.btnStart.Enabled = false;
                //    //    //plNavigation.Enabled = false;
                //    //    //todo
                //    //    //this.btnStart.BackgroundImage = Properties.Resources.startnot;

                //    //}
                //    if (lstFile.Items.Count == 0)
                //    {
                //        MessageBox.Show("欲转换的原文件不存在");
                //        //this.btnStart.Enabled = false;
                //    }
                //    else
                //    {
                //        this.btnStart.Enabled = true;
                //    }

                //    for (int i = 0; i < lstFile.Items.Count; i++)
                //    {
                //        fileQueue.Enqueue(this.lstFile.Items[i]);
                //    }
                //    for (int j = 0; j < thread.Length; j++)
                //    {
                //        if (thread[j].ThreadState == System.Threading.ThreadState.Stopped)
                //        {
                //            Console.WriteLine("Thread:" + j);
                //            thread[j] = new Thread(new ParameterizedThreadStart(WorkThread));
                //            thread[j].IsBackground = true;
                //            thread[j].Start(j);
                //        }
                //    }

                //    //发送请求信息
                //    TempUrl t = new TempUrl("主程序", "启动" + GetTaskName());
                //    PostURL(t);


                //KongMengyuan修改,2015-11-03,保留当前输出文件夹(在文件转换过程中不允许更改它)
                this.btnStart.ButtonBackIMG = Properties.Resources.btn_kszh_03;
                if (rdoNewPath.Checked)
                {
                    outpathSelect = "rdoNewPath";
                }
                else
                {
                    outpathSelect = "rdoPath";
                }

                //KongMengyuan修改,2015-11-03,参考互盾PDFCon
                if (btnStart.Enabled == false)
                {
                    return;
                }

                DirectoryInfo d = new DirectoryInfo(this.txtOutPath.OutText);
                //MessageBox.Show(d.ToString());
                //if (d.Exists == false) //KongMengyuan修改,2015-11-04,由于WinXP和纯净版的Win7对于Directory.Exists不起作用,所以使用另外一种写法(因为文件打开时是删除不了文件夹的)
                //if (!Directory.Exists(d.ToString())) //这种写法在WinXP和纯净版Win7也不识别(但是雨林木风的Ghost就可以识别这种写法)
                if (d.ToString().Length < 1)
                {
                    MessageBox.Show("你选择的输出文件夹不存在。");
                    return;
                }
                if (lstFile.Items.Count < 1)
                {
                    MessageBox.Show("请先添加文件");
                    return;
                }

                if (format == ConvertOld.FORMAT.PDFMerge)
                {
                    if (lstFile.Items.Count < 2)
                    {
                        MessageBox.Show("请添加2个或2个以上文件进行合并");
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
                        MessageBox.Show("任务已经全部完成");
                        return;
                    }
                }

                if (resetPoped)
                {
                    Poped = false;
                    NeedToPop = false;
                }
                isStart = true;
                //VerifyList();
                this.pltext.Visible = false;
                if (lstFile.Items.Count > 0 && this.lstFile.IsAllFinished)
                {
                    //this.btnStart.Enabled = false;
                    this.btnStart.Enabled = false;
                    this.btnStart.BackgroundImage = Properties.Resources.btn_kszh_04;
                    //this.startBtn.ButtonImage = Properties.Resources.btn_kszh4;
                    //plNavigation.Enabled = false;
                    //todo
                    //this.btnStart.BackgroundImage = Properties.Resources.startnot;

                }
                for (int i = 0; i < lstFile.Items.Count; i++)
                {
                    fileQueue.Enqueue(this.lstFile.Items[i]);
                }
                //MessageBox.Show(thread.Length.ToString());//2015-12-14,KongMengyuan注释,此处在WinXP_64位上面点击“开始转换”时出错
                //2015-12-16,KongMengyuan增加,目前默认是只转换CPU个数少1核的进程数,发现虚拟机里“WinXP 64位”点击“开始转换”时此处出错,跟踪发现是"thread.length"不识别(编译时"目标平台"为X86和X64也都出错)。
                int threadLength = cpuNumber;
                try
                {
                    threadLength = thread.Length;
                }
                catch
                {
                    //threadLength = 0; //设置为0就不开始转换了,所以此处就没有用了
                }
                for (int j = 0; j < threadLength; j++) //2015-12-14,KongMengyuan注释,目前默认是只转换CPU个数少1核的进程数,发现虚拟机里“WinXP 64位”点击“开始转换”时此处出错,跟踪发现是"thread.length"不识别(编译时"目标平台"为X86和X64也都出错)。
                {
                    if (thread[j].ThreadState == System.Threading.ThreadState.Stopped)
                    {
                        Console.WriteLine("Thread:" + j);
                        thread[j] = new Thread(new ParameterizedThreadStart(WorkThread));
                        thread[j].IsBackground = true;
                        thread[j].Start(j);
                    }
                }
                //发送请求信息
                TempUrl t = new TempUrl("主程序", "启动" + GetTaskName());
                PostURL(t);
            }
        }

        private void btnStop_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (lstFile.Items.Count < 1)//如果没有文件则不能点击
                {
                    SetThreeButtonValidate(false);//开始转换,停止转换,清空列表,三个按钮的有效还是失效: true-有效,false-失效
                    return;
                }
                this.btnStop.ButtonBackIMG = Properties.Resources.btn_tzzh_03;
                //2015-12-16,KongMengyuan增加,目前默认是只转换CPU个数少1核的进程数,发现虚拟机里“WinXP 64位”点击“开始转换”时此处出错,跟踪发现是"thread.length"不识别(编译时"目标平台"为X86和X64也都出错)。
                int threadLength = cpuNumber;
                try
                {
                    threadLength = thread.Length;
                }
                catch
                {
                    //threadLength = 0; //设置为0就不开始转换了,所以此处就没有用了
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
                this.btnStart.BackgroundImage = Properties.Resources.btn_kszh_01;
            }
        }

        private void btnBrowse_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (DialogResult.OK == folderBrowserDialog.ShowDialog())
                {
                    this.txtOutPath.OutText = folderBrowserDialog.SelectedPath;
                }
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
                            if (show_flag && openFileDialog.FileNames.Length == 1)
                            {
                                show_flag = false;
                                MessageBox.Show(string.Format(rm.GetString("msg9"), Path.GetFileName(file_name))
                                    , rm.GetString("Tips"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                                //MessageBox.Show("您添加的文件 " + Path.GetFileName(file_name) + " 已存在,我们将会自动过滤这些文件!"
                                //    , "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else if (show_flag && openFileDialog.FileNames.Length != 1)
                            {
                                show_flag = false;
                                //MessageBox.Show("您添加的部分文件已存在,我们将会自动过滤这些文件!"
                                //    , "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                MessageBox.Show(rm.GetString("msg1")
                                   , rm.GetString("Tips"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            continue;
                        }
                        ItemInfomation info = new ItemInfomation(file_name);
                        lstFile.ConversionPageDefaultText = rm.GetString("ALL");
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

            //bool show_flag = true;
            //if (((string[]) e.Data.GetData(DataFormats.FileDrop)) != null)
            //{
            //    foreach (string file_name in ((string[]) e.Data.GetData(DataFormats.FileDrop)))
            //    {
            //        string fileExt = Path.GetExtension(file_name);
            //        if (fileExt == ".pdf" || fileExt == ".xls" || fileExt == ".xlsx" || fileExt == ".ppt" ||
            //            fileExt == ".pptx" || fileExt == ".doc" ||
            //            fileExt == ".docx" || fileExt == ".jpg" || fileExt == ".jpeg" || fileExt == ".gif" ||
            //            fileExt == ".bmp" || fileExt == ".png" || fileExt == ".tiff" || fileExt == ".tif")
            //        {
            //            if (diclst.ContainsKey(file_name))
            //            {
            //                if (show_flag && openFileDialog.FileNames.Length == 1)
            //                {
            //                    show_flag = false;
            //                    MessageBox.Show(string.Format(rm.GetString("msg9"), Path.GetFileName(file_name))
            //                        , rm.GetString("Tips"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            //                    //MessageBox.Show("您添加的文件 " + Path.GetFileName(file_name) + " 已存在,我们将会自动过滤这些文件!"
            //                    //    , "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //                }
            //                else if (show_flag && openFileDialog.FileNames.Length != 1)
            //                {
            //                    show_flag = false;
            //                    //MessageBox.Show("您添加的部分文件已存在,我们将会自动过滤这些文件!"
            //                    //    , "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //                    MessageBox.Show(rm.GetString("msg1")
            //                        , rm.GetString("Tips"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            //                }
            //                continue;
            //            }
            //            ItemInfomation info = new ItemInfomation(file_name);
            //            lstFile.ConversionPageDefaultText = rm.GetString("ALL");
            //            lstFile.AddFile(info);//添加文件进入ListView(填加文件)
            //            diclst.Add(file_name, false);
            //        }
            //        else if (string.IsNullOrEmpty(fileExt))
            //        {
            //            GetFolder(file_name);
            //        }
            //    }
            //}
            //lstFile.Invalidate();
        }

        private void lstFile_DragLeave(object sender, EventArgs e)
        {
            lstFile.InsertionMark.Index = -1;
        }

        private void lstFile_DragOver(object sender, DragEventArgs e)
        {
            //// 获得鼠标坐标
            //Point point = lstFile.PointToClient(new Point(e.X, e.Y));
            //// 返回离鼠标最近的项目的索引
            //int index = lstFile.InsertionMark.NearestIndex(point);
            //// 确定光标不在拖拽项目上
            //if (index > -1)
            //{
            //    Rectangle itemBounds = lstFile.GetItemRect(index);
            //    if (point.X > itemBounds.Left + (itemBounds.Width/2))
            //    {
            //        lstFile.InsertionMark.AppearsAfterItem = true;
            //    }
            //    else
            //    {
            //        lstFile.InsertionMark.AppearsAfterItem = false;
            //    }
            //}
            //lstFile.InsertionMark.Index = index;
            //lstFile.Invalidate();
        }

        private void btnReg_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                RegDlgOld frm = new RegDlgOld();
                frm.StartPosition = FormStartPosition.CenterParent;
                frm.Location = this.PointToScreen(new Point(250, lstFile.Location.Y - 10));
                frm.ShowDialog();
                if (new reg().Is_Reg())
                {
                    this.lblTitle.Text = Encrypt.APPTITLE + " " + rm.GetString("OfficialVersion") + " v" +
                                         Program.httpVersion;//Version.version;
                    this.panel1.BackgroundImage = Properties.Resources.header_01; //已注册
                    isReg = true;
                    // pltext.Visible = true;
                }
                else
                {
                    this.lblTitle.Text = Encrypt.APPTITLE + " " + rm.GetString("FreeTrialVersion") + " v" +
                                         Program.httpVersion;//Version.version;
                    this.panel1.BackgroundImage = Properties.Resources.header_02; //未注册
                    isReg = false;
                    // pltext.Visible = false;
                }
                Program.progTitle = this.lblTitle.Text;
                //发送请求信息
                TempUrl t = new TempUrl("主程序", "点击注册");
                PostURL(t);
            }
        }

        private void btnHelp_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                visitHttpAddress("help");//访问www.xjpdf.com
            }
        }

        private void btnClear_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (lstFile.Items.Count < 1)//如果没有文件则不能点击
                {
                    SetThreeButtonValidate(false);//开始转换,停止转换,清空列表,三个按钮的有效还是失效: true-有效,false-失效
                    return;
                }
                ClearListTipsOld frm = new ClearListTipsOld();
                frm.StartPosition = FormStartPosition.CenterParent;
                frm.Location = this.PointToScreen(new Point(400, this.lstFile.Location.Y + 30));
                DialogResult dr = frm.ShowDialog();
                if (dr == System.Windows.Forms.DialogResult.OK)
                {
                    diclst.Clear();
                    this.lstFile.Items.Clear();
                    fileQueue.Clear();
                    //this.comboBoxPage.Visible = false;
                    this.btnStart.Enabled = true;
                    this.btnStart.BackgroundImage = Properties.Resources.btn_qklb_01;
                    this.pltext.Visible = false;
                    //2015-12-16,KongMengyuan增加,目前默认是只转换CPU个数少1核的进程数,发现虚拟机里“WinXP 64位”点击“开始转换”时此处出错,跟踪发现是"thread.length"不识别(编译时"目标平台"为X86和X64也都出错)。
                    int threadLength = cpuNumber;
                    try
                    {
                        threadLength = thread.Length;
                    }
                    catch
                    {
                        //threadLength = 0; //设置为0就不开始转换了,所以此处就没有用了
                    }

                    for (int i = 0; i < threadLength; i++) //2015-12-14,KongMengyuan注释,目前默认是只转换CPU个数少1核的进程数,发现虚拟机里“WinXP 64位”点击“开始转换”时此处出错,跟踪发现是"thread.length"不识别(编译时"目标平台"为X86和X64也都出错)。
                    {
                        thread[i].Abort();
                    }
                }
                //发送请求信息
                TempUrl t = new TempUrl("主程序", "清空列表");
                PostURL(t);

                backGroundShowHide();//隐藏或显示主界面提示图
            }
        }

        private void btnCourse_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                visitHttpAddress("jiaocheng");//访问www.xjpdf.com
            }
        }

        private void btnBuy_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                visitHttpAddress("buy");//访问www.xjpdf.com
            }
        }

        private void btnQQ_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                visitHttpAddress("help");//访问www.xjpdf.com
            }
        }

        private void pbMinimize_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                this.WindowState = FormWindowState.Minimized;
            }
        }

        private void pbClose_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                this.pbClose.BackgroundImage = Properties.Resources.close_03;
                //KongMengyuan,2015-11-11,依据郑总提出,当有文件正在转换时要给用户一个退出提示
                if (this.btnStart.Enabled == false && lstFile.Items.Count > 0)
                {
                    DialogResult dr = MessageBox.Show("有文件正在转换，是否关闭？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dr.ToString() != "Yes") //DialogResult.Cancel
                    {
                        return;
                    }
                }

                WinformClose();
            }
        }

        private void txtWidth_KeyPress(object sender, KeyPressEventArgs e)
        {

            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
        }

        private void txtHeight_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
        }

        private void rdoPath_MouseClick(object sender, MouseEventArgs e)
        {
            //KongMengyuan增加,2015-11-10,在转换过程中切换选择原始选择的文件夹不再更改（直到转换结束）
            if (this.btnStart.Enabled == false)
            {
                //this.rdoNewPath.Checked = true;
                //this.rdoPath.Checked = false;

                return;
                //if (rdoPath.Checked == true)
            }
            else
            {
                outpathSelect = "rdoPath";
                //rdoNewPath.Enabled = true;
            }

            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                //btnBrowse.IsEnable = false;
                //this.btnBrowse.ButtonBackIMG = Properties.Resources.lookEnable;
            }
        }

        private void rdoNewPath_MouseClick(object sender, MouseEventArgs e)
        {
            //KongMengyuan增加,2015-11-10,在转换过程中切换选择原始选择的文件夹不再更改（直到转换结束）
            if (this.btnStart.Enabled == false)
            {
                return;
                //if (rdoPath.Checked == true)
            }
            else
            {
                outpathSelect = "rdoNewPath";
                //rdoNewPath.Enabled = true;
            }

            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                rdoNewPath.Checked = true;
                if (DialogResult.OK == folderBrowserDialog.ShowDialog())
                {
                    this.txtOutPath.OutText = folderBrowserDialog.SelectedPath;
                }
            }
        }

        private void tsmSoftwareUpgrade_Click(object sender, EventArgs e)
        {
            //UpdateTips frm = new UpdateTips();
            //frm.StartPosition = FormStartPosition.CenterParent;
            //frm.Location = this.PointToScreen(new Point(400, this.lstFile.Location.Y + 30));
            ////DialogResult dr = frm.ShowDialog();
            //frm.ShowDialog();
            ////if (frm.DialogResult == DialogResult.OK) //KongMengyuan增加,2015-11-11,判断注册页面窗体是直接关闭还是激活后关闭
            //if (Program.dialogClose == false) //KongMengyuan增加,2015-11-11,判断注册页面窗体是直接关闭还是激活后关闭
            //{
            //    return;
            //}
            MessageBox.Show("您的转换器已经是最新版本了！");

            //发送请求信息
            TempUrl t = new TempUrl("主程序", "在线升级");
            PostURL(t);
        }

        private void regPictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                this.regPictureBox.BackgroundImage = Properties.Resources.btn_reg_04;
                //    if (e.Location.X < 39)
                //    {
                RegDlgOld frm = new RegDlgOld();
                frm.StartPosition = FormStartPosition.CenterParent;
                frm.Location = this.PointToScreen(new Point(250, lstFile.Location.Y - 10));
                frm.ShowDialog();
                if (Program.dialogClose == false) //KongMengyuan增加,2015-11-11,判断注册页面窗体是直接关闭还是激活后关闭
                {
                    return;
                }
                if (new reg().Is_Reg())
                {
                    this.lblTitle.Text = Encrypt.APPTITLE + " " + rm.GetString("OfficialVersion") + " v" +
                                         Version.version;
                    this.panel1.BackgroundImage = Properties.Resources.header_01; //已注册
                    isReg = true;
                    // pltext.Visible = true;
                }
                else
                {
                    this.lblTitle.Text = Encrypt.APPTITLE + " " + rm.GetString("FreeTrialVersion") + " v" +
                                         Version.version;
                    this.panel1.BackgroundImage = Properties.Resources.header_02; //未注册
                    isReg = false;
                    // pltext.Visible = false;
                }
                Program.progTitle = this.lblTitle.Text;
                //发送请求信息
                TempUrl t = new TempUrl("主程序", "点击注册");
                PostURL(t);
                //}
                //else
                //{
                //    if (e.Button == System.Windows.Forms.MouseButtons.Left)
                //    {
                //        visitHttpAddress("buy");//访问www.xjpdf.com

                //        //发送请求信息
                //        //Version.Post("http://all.jsocr.com/", Version.GetParamName("Data"), Program.appProgName,Version.GetParamName("Version"), new reg().get_reg_code(), "注册窗口", "购买正式版");

                //        //KongMengyuan修改,郑总提出来：在点击右上角的“购买”时打开页面会卡一下,而点击右下角的"在线教程"就不卡,改变写法(提取机器码浪费了多余的时间)
                //        //Version.Post("http://all.jsocr.com/", Version.GetParamName("Data"), Program.appProgName,Version.GetParamName("Version"), httpRegCode, "注册窗口", "购买正式版");
                //    }
                //}
            }
        }

        private void buyPictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                this.buyPictureBox.BackgroundImage = Properties.Resources.btn_buy_03;
                visitHttpAddress("buy");//访问www.xjpdf.com

                //KongMengyuan修改,郑总提出来：在点击右上角的“购买”时打开页面会卡一下,而点击右下角的"在线教程"就不卡,改变写法(提取机器码浪费了多余的时间)
                //发送请求信息
                //Version.Post("http://all.jsocr.com/", Version.GetParamName("Data"), Program.appProgName,
                //    Version.GetParamName("Version"), new reg().get_reg_code(), "注册窗口", "购买正式版");
            }
        }

        private void btnStop_MouseEnter(object sender, EventArgs e)
        {
            if (((Control)sender).Enabled != false)
            {
                this.btnStop.ButtonBackIMG = Properties.Resources.btn_tzzh_02;
            }
        }

        private void btnStop_MouseLeave(object sender, EventArgs e)
        {
            if (((Control)sender).Enabled != false)
            {
                this.btnStop.ButtonBackIMG = Properties.Resources.btn_tzzh_01;
            }
        }

        private void btnClear_MouseEnter(object sender, EventArgs e)
        {
            if (((Control)sender).Enabled != false)
            {
                this.btnClear.ButtonBackIMG = Properties.Resources.btn_qklb_02;
            }
        }

        private void btnClear_MouseLeave(object sender, EventArgs e)
        {
            if (((Control)sender).Enabled != false)
            {
                this.btnClear.ButtonBackIMG = Properties.Resources.btn_qklb_01;
            }
        }

        private void picBrw_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (DialogResult.OK == folderBrowserDialog.ShowDialog())
                {
                    this.txtOutPath.OutText = folderBrowserDialog.SelectedPath;
                    this.tbBrw.Text = this.txtOutPath.OutText;
                    this.toolTip1.SetToolTip(this.tbBrw, this.tbBrw.Text);//增加TextBox的提示
                }
            }
        }

        private void pbClose_MouseEnter(object sender, EventArgs e)
        {
            //redPic.Visible = true;
            this.pbClose.BackgroundImage = Properties.Resources.close_02;
        }

        private void pbClose_MouseLeave(object sender, EventArgs e)
        {
            //redPic.Visible = false;
            this.pbClose.BackgroundImage = Properties.Resources.close_01;
        }

        private void qqImg_MouseClick(object sender, MouseEventArgs e)
        {
            //KongMengyuan修改,2015-11-10,参考互盾PDFCon
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                visitHttpAddress("qq");//访问www.xjpdf.com
            }
        }

        private void pictureBuy_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                visitHttpAddress("buy");//访问www.xjpdf.com
            }
        }

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
                w.UploadValues("http://tj.sjhfrj.com/tj/ver1/", "POST", VarPost);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            //自动更新(后台查找PHP网址,核对Ver,发现有返回信息立刻更新),因为需要用到上面的Program.setupName等公共变量,所以此处程序放在这里
            Thread trd_php = new Thread(DownloadFile_Quiet);
            trd_php.Start();
        }

        //定义php返回的结构体
        public struct phpReturnStructure
        {
            public string newVersion;
            public string url;
            public bool optional;
            public string updateInfo;
        }

        //struct转换为byte[]
        public static byte[] StructToBytes(object structObj)
        {
            int size = Marshal.SizeOf(structObj);
            IntPtr buffer = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.StructureToPtr(structObj, buffer, false);
                byte[] bytes = new byte[size];
                Marshal.Copy(buffer, bytes, 0, size);
                return bytes;
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }

        //byte[]转换为struct
        //public static object BytesToStruct(byte[] bytes, Type strcutType)
        //{
        //    int size = Marshal.SizeOf(strcutType);
        //    IntPtr buffer = Marshal.AllocHGlobal(size);
        //    try
        //    {
        //        Marshal.Copy(bytes, 0, buffer, size);
        //        return Marshal.PtrToStructure(buffer, strcutType);
        //    }
        //    finally
        //    {
        //        Marshal.FreeHGlobal(buffer);
        //    }
        //}
        public static object BytesToStuct(byte[] bytes, Type type)
        {
            //得到结构体的大小
            int size = Marshal.SizeOf(type);
            //byte数组长度小于结构体的大小
            if (size > bytes.Length)
            {
                //返回空
                return null;
            }
            //分配结构体大小的内存空间
            IntPtr structPtr = Marshal.AllocHGlobal(size);
            //将byte数组拷到分配好的内存空间
            Marshal.Copy(bytes, 0, structPtr, size);
            //将内存空间转换为目标结构体
            object obj = Marshal.PtrToStructure(structPtr, type);
            //释放内存空间
            Marshal.FreeHGlobal(structPtr);
            //返回结构体
            return obj;
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

        // C# 判断窗体是否已经打开, 避免重复打开同一个窗体
        //判断窗口是否已经打开(这个窗口有时会打开两个,非常奇怪)
        //KongMengyuan增加,2015-11-11,判断多个当前窗口就关闭它(但是多个主窗口只在安装包安装完成后首次运行会有多个主界面出现,但并不是所有机器都有这个问题,只在郑侃炜那里不停的出现,非常奇怪,我们的机器没有这个问题)
        //可能是splash这个页面,因为是多线程写的,可能是郑总的计算机环境哪个变量让它变化了,导致了重新加载frmSplash.或许重新启动的线程在郑总的计算机环境下面认为是另外一个，而我们的计算机环境认为是同一个
        private void CheckFormIsOpen()
        {
            foreach (Form frm in Application.OpenForms)
            {
                if (frm is MainInfoOld)
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

        #region 访问www.xjpdf.com, visitHttpAddress
        public void visitHttpAddress(string httpType)
        {
            TempUrl t = null;
            switch (httpType)
            {
                case "help":
                    switch (iniLanguage) //(ini.read_ini("language").ToLower())
                    {
                        case "zh-cn":
                            try
                            {
                                Process.Start("http://www.xjpdf.com/software/pdfConvert/help/?version=" + Program.httpVersion);
                            }
                            catch
                            {
                                Process.Start("iexplore.exe", "http://www.xjpdf.com/software/pdfConvert/help/?version=" + Program.httpVersion);
                            }
                            break;
                        case "en":
                            try
                            {
                                Process.Start("http://www.xjpdf.com/software/pdfConvert/help/?version=" + Program.httpVersion);
                            }
                            catch
                            {
                                Process.Start("iexplore.exe", "http://www.xjpdf.com/software/pdfConvert/help/?version=" + Program.httpVersion);
                            }
                            break;
                        default:
                            try
                            {
                                Process.Start("http://www.xjpdf.com/software/pdfConvert/help/?version=" + Program.httpVersion);
                            }
                            catch
                            {
                                Process.Start("iexplore.exe", "http://www.xjpdf.com/software/pdfConvert/help/?version=" + Program.httpVersion);
                            }
                            break;
                    }
                    //发送请求信息
                    t = new TempUrl("主程序", "点击在线QQ");
                    PostURL(t);
                    break;
                case "qq":
                    switch (iniLanguage) //(ini.read_ini("language").ToLower())
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
                    //发送请求信息
                    t = new TempUrl("主程序", "点击在线QQ");
                    PostURL(t);
                    break;
                case "buy":
                    switch (iniLanguage) //(ini.read_ini("language").ToLower())
                    {
                        case "zh-cn":
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
                        case "en":
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
                        default:
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
                    }
                    //发送请求信息
                    t = new TempUrl("主程序", "点击购买软件QQ");
                    PostURL(t);
                    break;
                case "jiaocheng":
                    switch (iniLanguage) //(ini.read_ini("language").ToLower())
                    {
                        case "zh-cn":
                            try
                            {
                                Process.Start("http://www.xjpdf.com/software/pdfConvert/jiaocheng/?version=" + Program.httpVersion);
                            }
                            catch
                            {
                                Process.Start("iexplore.exe", "http://www.xjpdf.com/software/pdfConvert/jiaocheng/?version=" + Program.httpVersion);
                            }
                            break;
                        case "en":
                            try
                            {
                                Process.Start("http://www.xjpdf.com/software/pdfConvert/jiaocheng/?version=" + Program.httpVersion);
                            }
                            catch
                            {
                                Process.Start("iexplore.exe", "http://www.xjpdf.com/software/pdfConvert/jiaocheng/?version=" + Program.httpVersion);
                            }
                            break;
                        default:
                            try
                            {
                                Process.Start("http://www.xjpdf.com/software/pdfConvert/jiaocheng/?version=" + Program.httpVersion);
                            }
                            catch
                            {
                                Process.Start("iexplore.exe", "http://www.xjpdf.com/software/pdfConvert/jiaocheng/?version=" + Program.httpVersion);
                            }
                            break;
                    }
                    //发送请求信息
                    t = new TempUrl("主程序", "点击在线教程");
                    PostURL(t);
                    break;
                case "guanyu":
                    switch (iniLanguage) //(ini.read_ini("language").ToLower())
                    {
                        case "zh-cn":
                            try
                            {
                                Process.Start("http://www.xjpdf.com/software/pdfConvert/guanyu/?version=" + Program.httpVersion);
                            }
                            catch
                            {
                                Process.Start("iexplore.exe", "http://www.xjpdf.com/software/pdfConvert/guanyu/?version=" + Program.httpVersion);
                            }
                            break;
                        case "en":
                            try
                            {
                                Process.Start("http://www.xjpdf.com/software/pdfConvert/guanyu/?version=" + Program.httpVersion);
                            }
                            catch
                            {
                                Process.Start("iexplore.exe", "http://www.xjpdf.com/software/pdfConvert/guanyu/?version=" + Program.httpVersion);
                            }
                            break;
                        default:
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
                    //发送请求信息
                    t = new TempUrl("主程序", "点击关于我们");
                    PostURL(t);
                    break;
            }
        }
        public void PostURL(TempUrl obj)
        {
            this.syncContext.Post(URL, obj);
        }
        #endregion

        private void tbBrw_Validated(object sender, EventArgs e)
        {
            ////TextBox失去焦点,也可以把tbBrw_LostFocus里面的代码放在Validated里面,或者放在MouseLeave或leave里面
            ////不能使用的原因,在TextBox失去鼠标的时候,如果光标落在了LstFile上面,就会发生鼠标粘连,这样对于用户来说是不可以的
            ////下面语句放在窗体的Load里
            //this.tbBrw.LostFocus += new System.EventHandler(tbBrw_LostFocus);
            //private void tbBrw_LostFocus(object sender, System.EventArgs e)
            //{
            //    //失去焦点时,检查路径是否存在,可能Win10等系统检测不出来,这一点要注意
            //    string spath = this.tbBrw.Text;
            //    if (spath != string.Empty)
            //    {
            //        DirectoryInfo directoryInfo = new DirectoryInfo(spath);
            //        try
            //        {
            //            directoryInfo.Create();
            //        }
            //        catch
            //        {
            //            MessageBox.Show("在创建目录名时发现非法字符, 请检查");
            //        }
            //    }
            //    else
            //    {
            //        MessageBox.Show("目录名不允许为空");
            //    }
            //}

        }

        private void MainInfo_Paint(object sender, PaintEventArgs e)
        {
            //此处代码不起作用
            //重画“在线教程”的边框,长方形,矩形
            //ControlPaint.DrawBorder(e.Graphics, pbOnlineManual.ClientRectangle,
            //    //Color.DimGray, 2, ButtonBorderStyle.Solid, //左边
            //    Color.FromArgb(255, 255, 255), 1, ButtonBorderStyle.Solid, //左边,226,226,226
            //    Color.FromArgb(255, 255, 255), 1, ButtonBorderStyle.Solid, //上边
            //    Color.FromArgb(255, 255, 255), 1, ButtonBorderStyle.Solid, //右边
            //    Color.FromArgb(255, 255, 255), 1, ButtonBorderStyle.Solid);//底边


            ////重画“购买软件”的边框,长方形,矩形
            //ControlPaint.DrawBorder(e.Graphics, pbBuy.ClientRectangle,
            //    //Color.DimGray, 2, ButtonBorderStyle.Solid, //左边
            //                Color.FromArgb(255, 255, 255), 1, ButtonBorderStyle.Solid, //左边
            //                Color.FromArgb(255, 255, 255), 1, ButtonBorderStyle.Solid, //上边
            //                Color.FromArgb(255, 255, 255), 1, ButtonBorderStyle.Solid, //右边
            //                Color.FromArgb(255, 255, 255), 1, ButtonBorderStyle.Solid);//底边
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

        private void regPictureBox_MouseEnter(object sender, EventArgs e)
        {
            this.regPictureBox.BackgroundImage = Properties.Resources.btn_reg_02;
        }

        private void regPictureBox_MouseLeave(object sender, EventArgs e)
        {
            this.regPictureBox.BackgroundImage = Properties.Resources.btn_reg_01;
        }

        private void buyPictureBox_MouseEnter(object sender, EventArgs e)
        {
            this.buyPictureBox.BackgroundImage = Properties.Resources.btn_buy_02;
        }

        private void buyPictureBox_MouseLeave(object sender, EventArgs e)
        {
            this.buyPictureBox.BackgroundImage = Properties.Resources.btn_buy_01;
        }

        private void comboBoxPage_Validated(object sender, EventArgs e)
        {
            int inputMaxCount = 0;
            string name = lstFile.SelectedItems[0].SubItems[1].Text;
            int pageCount = System.Convert.ToInt32(lstFile.SelectedItems[0].SubItems[2].Text);

            if (!MainInfoOld.isReg) //免费版只转换前5页
            {
                if (pageCount >= Program.pageFreeConvert)
                {
                    pageCount = Program.pageFreeConvert;
                }
            }

            //利用正则表达式将字符串中的数字提取出来分别保存在整型数组中,提取字符串中的数字
            string s = this.comboBoxPage.Text;  //测试数据"1 2234 34";
            MatchCollection vMatchs = Regex.Matches(s, @"(\d+)");
            int[] vInts = new int[vMatchs.Count];
            for (int i = 0; i < vMatchs.Count; i++)
            {
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
            }
            if (errString != string.Empty)
            {
                //去掉字符串左边的逗号和空格
                errString = errString.Substring(2, errString.Length - 2);
                MessageBox.Show("输入的页码“ " + errString + " ”已经超过可以转换的最大页数 "
                    + pageCount.ToString() + " ,请检查输入的页码范围" + "\r\n" + "\r\n" + name, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);  // 字符之间加入回车符
                lstFile.SelectedItems[0].SubItems[3].Text = "全部";
            }
        }

        private void pbOnlineManual_Paint(object sender, PaintEventArgs e)
        {
            //此处代码只有换成红色才能看到效果,其它颜色效果不明显
            //PictureBox p = (PictureBox)sender;
            //Pen pp = new Pen(System.Drawing.Color.LightGray); //new Pen(Color.Red); //#c5c5c5
            //e.Graphics.DrawRectangle(pp, e.ClipRectangle.X, e.ClipRectangle.Y, e.ClipRectangle.X + e.ClipRectangle.Width - 1, e.ClipRectangle.Y + e.ClipRectangle.Height - 1);
        }

    }

    //public class TempClass
    //{
    //    public int index { get; set; }
    //    public int cur { get; set; }

    //    public TempClass(int index, int cur)
    //    {
    //        this.index = index;
    //        this.cur = cur;
    //    }
    //}

    //public class TempUrl
    //{
    //    public string Target { get; set; }
    //    public string MehodObject { get; set; }

    //    public TempUrl(string Target, string MehodObject)
    //    {
    //        this.Target = Target;
    //        this.MehodObject = MehodObject;
    //    }
    //}

}
