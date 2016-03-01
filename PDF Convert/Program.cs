using System;
using System.Collections.Generic;
using System.Globalization;
using System.Resources;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32; //关于注册表的命名空间

using System.Text.RegularExpressions;

using System.Runtime.InteropServices; //获取操作系统语言

namespace PDF_Convert
{
    public static class Program
    {
        //static ResourceManager rm = new ResourceManager(typeof(MainInfoOld));
        static ini_config ini = new ini_config("config.ini");
        public static string osVersion; //KongMengyuan修改,2015-11-04,WinXP不能检测UAC(因为Vista以后版本才有它),Win10必须避过UAC
        public static string MainInfo_LoadFinish; //窗体是否加载结束
        private static ApplicationContext context = null; //窗体加载使用

        //KongMengyuan增加,郑总提出来：在点击右上角的“购买”时打开页面会卡一下,而点击右下角的"在线教程"就不卡,改变写法(提取机器码浪费了多余的时间)
        public static string httpVersion = string.Empty;
        public static string httpMachineCode = string.Empty;
        public static string httpRegCode = string.Empty;
        public static string encodingCode = string.Empty;
        public static string cpu_hardDisk = string.Empty;
        public static string cpu_hardDisk_Hash = string.Empty;
        public static string appProgName = "迅捷PDF转换器";       //窗体的快捷方式显示,任务栏的显示内容. 发布成为不同的文件名,2015-12-25,KongMengyuan
        //public static string formTitle = "多用户发布专用标题"; //窗体的快捷方式显示,任务栏的显示内容. 发布成为不同的文件名,2015-12-25,KongMengyuan
        public static string setupName = string.Empty;
        public static bool dialogClose = false;
        public static string progTitle = string.Empty;
        public static int pageFreeConvert = 5; //免费版只转换前5页

        //读取Config.ini文件里面的language.如果把语言切换定义在这里的话,用户切换不同语言必须重新启动系统才有效,这是不行的,所以不在这里定义全局语言切换(但是如果放在其它程序,比如Convert01.cs里面在调用时会不停的访问ini,这会降低效率)
        public static string iniLanguage = string.Empty;

        /// <summary>
        /// 应用程序的主入口点,包括一些初始化操作,启动窗体等。
        /// </summary>
        [STAThread]
        static void Main()
        {
            appProgName = Encrypt.Refresh();
            //MessageBox.Show("程序运行到这里了 11111");
            CheckOSVersion(); //检查操作系统的版本
            //MessageBox.Show("程序运行到这里了 22222");
            try
            {
                SkipUAC(); //解决UAC的控制问题,单独编一个程序,DotNetFrameWork是2.0或4.5都过的,但是放在这里就不行,2015-11-03查出原因,是因为app.manifest里面的这条语句在影响<requestedExecutionLevel level="requireAdministrator" uiAccess="false" />
            }
            catch
            {
                //Win7 32位有时会卡在这里,所以这句话如果出错就略过
            }
            Application.EnableVisualStyles(); //样式设置
            Application.SetCompatibleTextRenderingDefault(false); //样式设置

            iniLanguage = ini.read_ini("language").ToLower();
            if (string.IsNullOrEmpty(iniLanguage))
            {
                //iniLanguage = System.Globalization.CultureInfo.InstalledUICulture.Name.ToLower();
                //获取操作系统语言 using System.Runtime.InteropServices;
                string sLanguage = string.Empty;
                sLanguage = System.Threading.Thread.CurrentThread.CurrentCulture.Name.ToLower();
                if (string.IsNullOrEmpty(sLanguage) == true || string.Equals(sLanguage, "") == true)
                    sLanguage = System.Globalization.CultureInfo.InstalledUICulture.Name.ToLower();
                if (sLanguage.Substring(0,2).ToLower() == "en")
                    sLanguage = "en";
                if (sLanguage.Substring(0, 2).ToLower() == "ja" || sLanguage.Substring(2).ToLower() == "jp")
                    sLanguage = "ja";
                if (sLanguage.Substring(0, 2).ToLower() == "zh-TW".ToLower() || sLanguage.Substring(2).ToLower() == "zh-HK".ToLower())
                    sLanguage = "zh-tw";
                if (string.IsNullOrEmpty(sLanguage) == true || string.Equals(sLanguage, "") == true)
                    sLanguage = "zh-cn";
                iniLanguage = sLanguage;
            }
            if (iniLanguage.Substring(0, 2).ToLower() == "ja" || iniLanguage.Substring(2).ToLower() == "jp")
                iniLanguage = "ja";
            if (iniLanguage.Substring(0, 2).ToLower() == "zh-TW".ToLower() || iniLanguage.Substring(2).ToLower() == "zh-HK".ToLower())
                iniLanguage = "zh-tw";


            try
            {
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(iniLanguage);
            }
            catch
            {
                //如果ini里面的language定义错误,则默认语言版本为中文,此时尽量不默认为英文,因为目前英文版是免费的,免得有的用户不小心发现这个秘密"英文版免费"
                iniLanguage = "zh-cn";
            }
            License();

            RunForm();                               //启动窗体
        }
        static void CheckOSVersion()
        {
            //操作系统__________________版本号
            //Windows 10________________6.2     默认安装.Net4.5
            //Windows 8	________________6.2     默认安装.Net4.5 (同时也携带有.Net3.5,只是不是默认安装)
            //Windows 7 ________________6.1     默认安装.Net3.5
            //Windows Server 2008 R2____6.1
            //Windows Server 2008_______6.0
            //Windows Vista_____________6.0
            //Windows Server 2003 R2____5.2
            //Windows Server 2003_______5.2
            //Windows XP X64____________5.2     手动安装.Net2.0 SP2 64位
            //Windows XP________________5.1     手动安装.Net2.0或.Net2.0 SP2或.Net3.5,不能安装.Net4.0
            //Windows 2000______________5.0

            osVersion = "";
            osVersion = Environment.OSVersion.ToString(); // 操作系统版本
            //MessageBox.Show("看是否到这里了,KongMengyuan测试,操作系统版本为 " + osVersion);
            if (osVersion.Contains("NT 5.1") || osVersion.Contains("NT 5.2"))
            {
                osVersion = "WinXP";
            }
            if (osVersion.Contains("NT 6.1"))
            {
                osVersion = "Win7";
            }
        }
        static void SkipUAC()
        {
            //---------------解决UAC的控制问题-----------End
            if (osVersion == "WinXP") //WinXP不能检测UAC(因为Vista以后版本才有它),Win10必须避过UAC
            {
                return;
            }
            //KongMengyuan,2015-11-03,放在这里不知道为什么不好用,但是单独作一个程序就是好用的
            //UAC：打开一个文件时,控制面板\用户帐户和家庭安全\用户帐户\更改用户帐户控制设置\始终通知,当打开任何文件时都提示了
            //自从Windows Vista起就有了UAC，即用户帐户控制，提高了安全性之余也为用户增添了不少麻烦。有些常用程序每次打开也会有UAC提示，因此有不少人直接关闭了UAC。
            //有没有办法在不关闭UAC的情况下又可以不触发UAC打开一些常用程序呢？答案是肯定的。
            //一般的方法：在“开始”菜单里，搜索 regedit ，打开注册表编辑器。
            //手动修改注册表：HKEY_CURRENT_USERS\Software\Microsoft\Windows NT\CurrentVersion\AppCompatFlags\Layers 键下面新建字符串值，
            //数值名字是程序的全路径，数值数据是“RunAsInvoker”（不带双引号）。
            //如果安装的是雨林木风的win7 ghost就怎么也不提示,但是Win10就有这个问题了,单独作一个程序是过的,但是放在这个软件里面就行了,2015-11-03查出原因,是因为app.manifest里面的这条语句在影响<requestedExecutionLevel level="requireAdministrator" uiAccess="false" />
            object obj = Microsoft.Win32.Registry.LocalMachine
            .OpenSubKey("SOFTWARE")
            .OpenSubKey("Microsoft")
            .OpenSubKey("Windows")
            .OpenSubKey("CurrentVersion")
            .OpenSubKey("Policies")
            .OpenSubKey("System")
            .GetValue("EnableLUA");
            if ((int)obj == 0)
            {
                Console.WriteLine("关闭"); //UAC是“从不检查”
            }
            else
            {
                Console.WriteLine("开启"); //UAC是“一直检查”,但是雨林木风的Win7 Ghost处于开启状态也不检查
                //操作注册表
                RegistryKey rsg = null; //声明一个变量
                //if (Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft").SubKeyCount <= 0)
                //{
                //    Registry.LocalMachine.DeleteSubKey("SOFTWARE\\Microsoft"); //删除 
                //    Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft"); //创建
                //}
                //rsg = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft", true); //true表可以修改
                //rsg.SetValue("HoanReg", "写入的值"); //写入
                //rsg.Close(); //关闭

                if (Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows NT\\CurrentVersion\\AppCompatFlags").SubKeyCount <= 0)
                {
                    try
                    {
                        rsg.CreateSubKey("Layers"); //生成一个新项目
                    }
                    catch //如果已经生成则不去理会它
                    { }
                    rsg = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows NT\\CurrentVersion\\AppCompatFlags\\Layers", true); //true表可以修改
                    //rsg.SetValue("RunAsInvoker", "Release"); //写入

                    //获取当前最径的最后路径位置(方便写入注册表)
                    string currentPath = Application.StartupPath;
                    string tmpPath = "";
                    int i = currentPath.Length;
                    int j = 0;
                    while (i > 0)
                    {
                        j++;
                        i--;
                        if (currentPath.Substring(i, 1) == "/" || currentPath.Substring(i, 1) == "\\")
                        {
                            break;
                        }
                    }
                    tmpPath = currentPath.Substring(i, j).Replace("\\", "").Replace("/", ""); //替换掉\\或/,同时把当前程序最后的路径位置取出来
                    rsg.SetValue("RunAsInvoker", tmpPath); //写入

                    rsg.Close(); //关闭
                }
                else
                {
                    try
                    {
                        rsg = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows NT\\CurrentVersion", true); //true表可以修改
                        rsg.CreateSubKey("AppCompatFlags"); //生成一个新项目
                        rsg = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows NT\\CurrentVersion\\AppCompatFlags", true); //true表可以修改
                        rsg.CreateSubKey("Layers"); //生成一个新项目
                    }
                    catch //如果已经生成则不去理会它
                    { }
                    rsg = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows NT\\CurrentVersion\\AppCompatFlags\\Layers", true); //true表可以修改
                    //rsg.SetValue("RunAsInvoker", "Release"); //写入

                    //获取当前路径的最后路径位置(方便写入注册表)
                    string currentPath = Application.StartupPath;
                    string tmpPath = "";
                    int i = currentPath.Length;
                    int j = 0;
                    while (i > 0)
                    {
                        j++;
                        i--;
                        if (currentPath.Substring(i, 1) == "/" || currentPath.Substring(i, 1) == "\\")
                        {
                            break;
                        }
                    }
                    tmpPath = currentPath.Substring(i, j).Replace("\\", "").Replace("/", ""); //替换掉\\或/,同时把当前程序最后的路径位置取出来
                    rsg.SetValue("RunAsInvoker", tmpPath); //写入

                    rsg.Close(); //关闭

                    //System.Environment.CurrentDirectory 
                    //获取和设置当前目录(该进程从中启动的目录)的完全限定目录。 
                    //System.IO.Directory.GetCurrentDirectory() 搜索
                    //获取应用程序的当前工作目录。
                    //System.AppDomain.CurrentDomain.BaseDirectory 
                    //获取程序的基目录。 
                    //System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase 
                    //获取和设置包括该应用程序的目录的名称。 
                    //System.Windows.Forms.Application.StartupPath 
                    //获取启动了应用程序的可执行文件的路径 
                }
            }
            //---------------解决UAC的控制问题-----------End
        }

        public static void License()
        {
            string licensePath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\\Aspose.Total.lic";
            Aspose.Pdf.License pdf_license = new Aspose.Pdf.License();
            pdf_license.SetLicense(licensePath);

            Aspose.Words.License word_license = new Aspose.Words.License();
            word_license.SetLicense(licensePath);

            Aspose.Cells.License excel_license = new Aspose.Cells.License();
            excel_license.SetLicense(licensePath);

            Aspose.Slides.License ppt_license = new Aspose.Slides.License();
            ppt_license.SetLicense(licensePath);
        }

        static void RunForm()
        {
            bool mutex_succ;
            Mutex mutex = new Mutex(false, "XJ_PDF_CONVERT2", out mutex_succ);
            if (!mutex_succ)
            {
                //MessageBox.Show("本程序已经在运行了", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //string iniLanguage = ini.read_ini("language");
                string sTip = Language_Load(iniLanguage, "Program.cs", "MSG_01"); //提示                                
                string sOld = Language_Load(iniLanguage, "Program.cs", "MSG_02"); //您添加的文件 $S 已存在,我们将会自动过滤这些文件!
                MessageBox.Show(sOld, sTip, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string form_type = new ini_config("config.ini").read_ini("formtype");

            //Application.Run(new MainInfoOld()); //没有frmSplash窗体,直接运行MainInfo主窗体(这种情况如果在主窗体启动时有检查CPU序列号等硬件就会变得比较慢,所以把检测CPU序列号放在frmSplash窗体里面)

            //加载Splash窗体方法1,在frmSplash窗体里面运行MainInfo窗体.两种方法都不可以使用gif动画,只有在frmSplash里面使用线程加载MainInfo窗体才可以使用动画,但是那样如果MainInfo再使用线程就会引起线程冲突)
            Application.Run(new frmSplash());

            //不采用下面的方法原因：下面的常规加载方法在frmSplash和MainInfo主窗体之间会有一个空闲等待,而MainInfo放在frmSplash里面就不存在这个问题了(后者界面就比较流畅)
            ////加载Splash窗体方法2,在Program里面运行MainInfo窗体.两种方法都不可以使用gif动画,只有在frmSplash里面使用线程加载MainInfo窗体才可以使用动画,但是那样如果MainInfo再使用线程就会引起线程冲突)
            //frmSplash sp = new frmSplash(); //启动窗体
            //sp.Show(); //显示启动窗体
            //context = new ApplicationContext();
            //context.Tag = sp;
            //Application.Idle += new EventHandler(Application_Idle); //注册程序运行空闲去执行主程序窗体相应初始化代码
            //Application.Run(context);

            try
            {
                mutex.ReleaseMutex();
            }
            catch { }
        }

        //初始化等待处理函数
        private static void Application_Idle(object sender, EventArgs e)
        {
            Application.Idle -= new EventHandler(Application_Idle);
            if (context.MainForm == null)
            {
                //MainInfoOld mw = new MainInfoOld();
                MainInfo01 mw = new MainInfo01();
                context.MainForm = mw;
                mw.InitializeLifetimeService();
                //主窗体要做的初始化事情在这里，该方法在主窗体里应该申明为public
                frmSplash sp = (frmSplash)context.Tag;
                sp.Close();                                 //关闭启动窗体 
                mw.Show();                                  //启动主程序窗体
            }
        }

        //每个位置的语言定义和Program.cs的都一样,注意要修改成一致
        //为什么不使用全局变量: 如果把语言切换定义成全局变量的话,用户切换不同语言必须重新启动系统才有效,这是不行的,所以不定义全局语言切换
        public static string Language_Load(string iniLanguage, string ini_Section, string ini_Key)
        {
            //英文版免费

            string strRtn = string.Empty;
            IniFile ini_Language = new IniFile(Application.StartupPath + "\\Set.ini");
            //节(section) 节用方括号括起来,单独占一行. 键(key)又名属性(property),单独占一行用等号连接键名和键值. 注释(comment) 使用英文分号（;）开头，单独占一行
            switch (iniLanguage.ToLower())
            {
                case "zh-cn":
                    ini_Language = new IniFile(Application.StartupPath + "\\zh-CN\\" + "ChineseSimplified.ini");
                    strRtn = ini_Language.IniReadValue(ini_Section, ini_Key);

                    if (string.IsNullOrEmpty(strRtn))
                    {
                        string sOld = Language_Load(iniLanguage, "Program.cs", "MSG_03"); // ChineseSimplified.ini里面的$S缺少关键字$S, 请与开发人员联系.
                        sOld = strReplace(sOld, "$S", new string[] { ini_Section, ini_Key }); //Path需引用 using System.IO;
                        return sOld;
                    }
                    break;
                case "en":
                    ini_Language = new IniFile(Application.StartupPath + "\\en\\" + "English.ini");
                    strRtn = ini_Language.IniReadValue(ini_Section, ini_Key);

                    if (string.IsNullOrEmpty(strRtn))
                    {
                        string sOld = Language_Load(iniLanguage, "Program.cs", "MSG_03"); // English.ini lost the key $S of section $S, please contact with the vendor.
                        sOld = strReplace(sOld, "$S", new string[] { ini_Section, ini_Key }); //Path需引用 using System.IO;
                        return sOld;
                    }
                    break;
                case "ja":
                    ini_Language = new IniFile(Application.StartupPath + "\\Ja\\" + "Japanese.ini");
                    strRtn = ini_Language.IniReadValue(ini_Section, ini_Key);

                    if (string.IsNullOrEmpty(strRtn))
                    {
                        string sOld = Language_Load(iniLanguage, "Program.cs", "MSG_03"); // English.ini lost the key $S of section $S, please contact with the vendor.
                        sOld = strReplace(sOld, "$S", new string[] { ini_Section, ini_Key }); //Path需引用 using System.IO;
                        return sOld;
                    }
                    break;
                case "zh-tw":
                    ini_Language = new IniFile(Application.StartupPath + "\\zh-TW\\" + "ChineseTraditional.ini");
                    strRtn = ini_Language.IniReadValue(ini_Section, ini_Key);

                    if (string.IsNullOrEmpty(strRtn))
                    {
                        string sOld = Language_Load(iniLanguage, "Program.cs", "MSG_03"); // English.ini lost the key $S of section $S, please contact with the vendor.
                        sOld = strReplace(sOld, "$S", new string[] { ini_Section, ini_Key }); //Path需引用 using System.IO;
                        return sOld;
                    }
                    break;
                default:
                    ini_Language = new IniFile(Application.StartupPath + "\\zh-CN\\" + "ChineseTraditional.ini");
                    strRtn = ini_Language.IniReadValue(ini_Section, ini_Key);

                    if (string.IsNullOrEmpty(strRtn))
                    {
                        string sOld = Language_Load(iniLanguage, "Program.cs", "MSG_03"); //English.ini lost the key MSG_01 of section MainInfo01, please contact with the vendor.
                        return sOld;
                    }
                    break;
            }
            return strRtn;
        }

        //字符串操作：查找并替换固定字符串(依序替换),KongMengYuan,2016-01-06
        public static string strReplace(string sOld, string sRemove, string[] sReplace)
        {
            //测试语句：MessageBox.Show(strReplace("$Sabcd$Sefghi$Sjklmnopqrstuvwxyz", "$S", new string[]{ "替换AAA替换", "替换BBB替换", "替换DDD替换" }));
            //string s_Old = "$Sabcd$Sefghi$Sjklmnopqrstuvwxyz";
            //string s_Remove = "$S";
            //string[] s_Replace = new string[] { "替换AAA替换", "替换BBB替换", "替换DDD替换" };
            int count = 0;
            int startIndex = 0;

            int iCountNew = sReplace.Length; //得到数组维数,待替换新字符串的数组长度

            //得到指定字符串的个数,正则表达式使用需引用 using System.Text.RegularExpressions;
            MatchCollection vMatchs = Regex.Matches(sOld, @"<" + sRemove + ">");
            string pattern = @"(\" + sRemove + ")";
            MatchCollection mc = Regex.Matches(sOld, pattern); //使用正则表达式查找欲替换的相同字符串个数
            int iCountOld = mc.Count;

            //待转换的字符串个数 > 新字符串个数,则返回错误,而实际情况: 是用户自己修改了Language.ini,比如English.ini里面某个字符串错误而导致的
            if (iCountNew != iCountOld)
            {
                return "Error, the quantity of s_Remove not equal to the quantity of s_Replace.GetUpperBound(), please check the string" + "\r\n" + sOld; //字符之间加入回车符
            }
            while (true)
            {
                int y = sOld.IndexOf(sRemove, startIndex);
                if (y != -1)
                {
                    count++;
                    startIndex = y + 1;
                }
                else
                {
                    break;
                }
                sOld = (sOld.Remove(y, sRemove.Length).Insert(y, sReplace[count - 1]));
                startIndex = startIndex - sRemove.Length + (sReplace[count - 1]).Length;
            }
            for (int i = 0; i <= count; i++)
            {
                //Console.WriteLine("$S在字符串中出现了{0}次", count);
                //Console.ReadLine();
            }
            return sOld;
        }

        #region 各个版本之间的更新内容
        //V6.3版本更新内容（2016-01-04至2016-02-28,6.5周时间,春节放假12天,修改测试Bug用时1.5周）,KongMengyuan：
        //    加载速度	更改窗口加载方式，解决启动速度慢的问题。
        //    多语言    制作英文版、繁体中文，并在对应的操作系统中测试。安装包也应当适应多语言。
        //              其中英文版可以完全免费。
        //    右键菜单	在PDF文件右键增加“转换到Word文件”菜单，并在其他文件右键菜单也增加类似的功能。
        //    页面	    支持输入形如“1-5;7,9;15-20,25”这样的样式，表示将文件分割为“1-5”“7,9”“15-20,25”三个文件（用分号分割）。并且在设置界面中，支持更复杂的设置。
        //    安装包	KongMY增加：
        //    1、普通用户安装没有桌面快捷方式，所有程序里面也没有快捷方式
        //    2、获取CPU的序列号不用了,使用网卡序列号,要看无线网卡是否好用,无网卡时会不会出错（出错也可以不理会,因为毕竟是网络环境才可以转换）,解决返回找不到序列号返回0 的问题.KongMY修改为先查CPU序号,没查到再查网卡MAC地址,要同时修改客服的注册号生成程序
        //    3、MainInfo使用线程通知（把慢的东西放在线程里，结束时发一个通知给其它使用的地方）
        //--------------------
        //V6.2版本更新内容（2015-11-13至2015-11-27,2周时间,修改测试Bug用时1.5周）,KongMengyuan：
        //    界面有很大变化
        //    增加了自动更新功能
        //    进度条更改显示方式
        //    增加了总页数
        //    加快了程序的显示速度
        //    文件转Word增加分文档功能
        //    主程序MainInfo_Load的写法完全不同(代码)
        //    所有的网页访问方式重新整理,速度加快(代码)
        //    对ListViewPlus部分代码重新封装(加快显示,同时修改对应的Bug)
        #endregion
    }
}
