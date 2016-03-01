using System;
using System.Collections.Generic;
using System.Text;
using System.Management;
using System.Security.Cryptography;
using System.Runtime.InteropServices; //运行Windows的API,引用DLL专用
using Microsoft.Win32;

namespace PDF_Convert
{
    partial class reg
    {
        public static string get_SetupDisk_code(string logicalDisk)
        {
            string vsn = string.Empty ;
            string vsl_All = string.Empty;
            try
            {
                //ManagementObject disk = new ManagementObject("win32_logicaldisk.deviceid=\"c:\""); //在Win10虚拟机上面取不到
                System.Management.ManagementClass mc = new ManagementClass("Win32_LogicalDisk");
                ManagementObjectCollection moc = mc.GetInstances();
                int m = 0;
                foreach (ManagementObject mo in moc)
                {
                    if (((string)mo["Caption"]).ToUpper() == logicalDisk.ToUpper())
                    {
                        vsn = (string)mo["VolumeSerialNumber"];
                    }
                    m++;
                    vsl_All = vsl_All + "第" + m.ToString() + "盘" + (string)mo["Caption"] + "码" + vsn;
                }
            }
            catch (Exception)
            {
                vsn = "12D85F3"; //如果没有取到则默认一个数字,12D85F3=19760627
            }
            int i = Convert.ToInt32(vsn, 16);//将十六进制转换为十进制i
            vsn = i.ToString();
            //return vsl_All;
            return vsn;
        }

        public int get_MACaddress_code()
        {
            //try //KongMengyuan增加try,因为不知为什么有时会弹出System.NullReferenception: 未将对象引用设置到对象的实例中
            //注意此处如果使用了try,可能会产生两个主页面,测试时看一下主窗体后面是否有隐藏的窗体,KongMengyuan,2015-11-10
            {
                string cpu_id = "";
                string md5 = "";

                try
                {

                    ////获取网卡硬件地址
                    //ManagementObjectCollection obj = new ManagementClass("Win32_Processor").GetInstances(); //KongMengyuan,2015-11-10,后面的GetInstances有时会弹出System.NullReferenception: 未将对象引用设置到对象的实例中
                    ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                    ManagementObjectCollection obj = mc.GetInstances();

                    foreach (ManagementObject mo in obj)
                    {
                        //cpu_id = mo.Properties["ProcessorId"].Value.ToString();
                        if ((bool)mo["IPEnabled"] == true)
                        {
                            cpu_id = mo.Properties["MacAddress"].Value.ToString();

                        }
                        mo.Dispose();
                    }
                }
                catch
                {
                    return 0;
                }

                if (cpu_id == string.Empty)
                    return 0;
                if (cpu_id.Length == 16)
                {
                    cpu_id = string.Format("{0}-{1}-00000000-00000000", cpu_id.Substring(0, 8), cpu_id.Substring(8, 8));
                }
                else if (cpu_id.Length == 32)
                {
                    cpu_id = string.Format("{0}-{1}-{2}-{3}", cpu_id.Substring(0, 8), cpu_id.Substring(8, 8), cpu_id.Substring(16, 8),
                        cpu_id.Substring(24, 8));
                }

                md5 = get_md5(cpu_id);


                //计算----------------------------------------------------------------------

                string x = "", cpu_code = "";
                int y, z;
                for (int i = 1; i <= 27; i += 3)
                {
                    x = md5.Substring(i + 2, 1);
                    y = Encoding.ASCII.GetBytes(x)[0];
                    z = y % encode[(i + 2) / 3 - 1];
                    if (0 == z)
                        z = 8;
                    cpu_code = cpu_code + z.ToString();
                }
                return int.Parse(cpu_code);
            }
        }
        public int get_cpu_code()
        {
            //try //KongMengyuan增加try,因为不知为什么有时会弹出System.NullReferenception: 未将对象引用设置到对象的实例中
            //注意此处如果使用了try,可能会产生两个主页面,测试时看一下主窗体后面是否有隐藏的窗体,KongMengyuan,2015-11-10
            {
                string cpu_id = "";
                string md5 = "";

                //ManagementObjectCollection obj = new ManagementClass("Win32_Processor").GetInstances(); //KongMengyuan,这句话不知为什么有时会弹出System.NullReferenception: 未将对象引用设置到对象的实例中
                //foreach (ManagementObject mo in obj)
                //{
                //    cpu_id = mo.Properties["ProcessorId"].Value.ToString();
                //    break;
                //}

                try
                {
                    ManagementObjectCollection obj = new ManagementClass("Win32_Processor").GetInstances(); //KongMengyuan,2015-11-10,后面的GetInstances有时会弹出System.NullReferenception: 未将对象引用设置到对象的实例中
                    foreach (ManagementObject mo in obj)
                    {
                        cpu_id = mo.Properties["ProcessorId"].Value.ToString();
                        break;
                    }
                }
                catch
                {
                    return 0;
                }

                if (cpu_id == string.Empty)
                    return 0;
                if (cpu_id.Length == 16)
                {
                    cpu_id = string.Format("{0}-{1}-00000000-00000000", cpu_id.Substring(0, 8), cpu_id.Substring(8, 8));
                }
                else if (cpu_id.Length == 32)
                {
                    cpu_id = string.Format("{0}-{1}-{2}-{3}", cpu_id.Substring(0, 8), cpu_id.Substring(8, 8), cpu_id.Substring(16, 8),
                        cpu_id.Substring(24, 8));
                }

                md5 = get_md5(cpu_id);


                //计算----------------------------------------------------------------------

                string x = "", cpu_code = "";
                int y, z;
                for (int i = 1; i <= 27; i += 3)
                {
                    x = md5.Substring(i + 2, 1);
                    y = Encoding.ASCII.GetBytes(x)[0];
                    z = y % encode[(i + 2) / 3 - 1];
                    if (0 == z)
                        z = 8;
                    cpu_code = cpu_code + z.ToString();
                }
                return int.Parse(cpu_code);
            }
            //catch
            //{
            //    return 0;
            //}
        }

        private string get_md5(string str, int bit = 32, bool lower_case = true)
        {
            byte[] md5_16;
            string md5 = "";
            MD5CryptoServiceProvider md5_csp = new MD5CryptoServiceProvider();

            md5_16 = md5_csp.ComputeHash(Encoding.ASCII.GetBytes(str));

            for (int i = 0; i < md5_16.Length; i++)
            {
                if (32 == bit)
                    md5 += System.Convert.ToString(md5_16[i], 16).PadLeft(2, '0');
                else
                    md5 += System.Convert.ToString(md5_16[i], 16);
            }

            return lower_case ? md5 : md5.ToUpper();

        }

        public string get_machine_code()
        {
            //因为现在是自动更新的，如果改变机器码的算法，生成的对应码会变化，这样以前的未到期的所有软件在使用时都会变化，这样会需要客服配合，可能在安装初期会有井喷式的客服需求，所以需要彻底决定是否改变机器码的提取方法
            //回答: 自动更新除非我们产品有重大BUG，否则不会启用
            //2016-02-26,KongMengyuan,由于网卡有时会更换,另外笔记本的插拔式网卡如果拔下来会查不到,所以机器码更改为"CPU+安装盘序列号"(取前10位,CPU后4位+安装盘序列号后5位,Hash后再取后9位)
            //return get_cpu_code().ToString(); //取得CPU序列号
            //KongMengYuan修改,2016-01-07,在Win10下面有时会取不到CPU序列号
            string machineCode = string.Empty;
            string macAddressCode = string.Empty;
            string cpuCode = get_cpu_code().ToString();
            if (cpuCode == "0" || string.IsNullOrEmpty(cpuCode))
            {
                machineCode = "887316298"; //有时客户的机器码获取不到,此时设置成上海互盾KongMengyuan的机器号
            }
            try
            {
                macAddressCode = get_MACaddress_code().ToString(); //取得网卡序列号,因为Win10下面取不到CPU序列号,所以全部使用网卡序列号
            }
            catch
            {
                macAddressCode = "网卡没有发现,此处要提示客户出错了";//2016-02-26目前不考虑这种方法,所以不需要转换这条提示为多语言
            }
            string setupDiskCode = get_SetupDisk_code("C:").ToString();//取得安装盘序列号,目前先取C盘的序列号(如果客户安装在了U盘上面电脑插的U盘个数不同则显示的对应盘符也会变化),以后修改为安装盘的序列号(这样免得客户安装为不同的安装盘时序列号变化,那时会要求客服协助,这样比较麻烦,还是取C盘的序列号比较实用)
            //机器码=CPU+安装盘序列号(取前10位,CPU后4位+安装盘序列号后5位,Md5后再取后9位)
            //不能使用Hash,因为两次启动计算机会不同,所以只能MD5
            //machineCode = (cpuCode.Substring(cpuCode.Length - 5, 5) + setupDiskCode.Substring(setupDiskCode.Length - 5, 5)).GetHashCode().ToString(); //先取后10位,不知为什么有时前面会有一个负号
            //machineCode = machineCode.Substring(machineCode.Length - 9, 9);//再取后9位
            machineCode = get_md5("19760627"+cpuCode+ setupDiskCode).ToString(); //前面加一个固定字符,MD5生成的都是128位16个字节
            machineCode = machineCode.Substring(machineCode.Length - 8, 8);//再取后8位,吴德有老师
            machineCode =Convert.ToInt32(machineCode.Substring(machineCode.Length - 8, 8), 16).ToString();//将十六进制转换为十进制i
            //如果长度大于10位,则取10位长度,否则取绝对值的数字字符串
            if (machineCode.Length>10)
            {
            machineCode = machineCode.Substring(machineCode.Length - 10, 10); //取后10位
            }
            else
            {
                machineCode = System.Math.Abs(Convert.ToInt32(machineCode)).ToString(); //不知为什么在Win7的32位上面有时前面会有负号,非常奇怪
            }

            return machineCode;
        }

        public string get_reg_code(string machine_code = "")
        {
            //2016-02-29,KongMengyuan,这种算法在反编译时很容易看到原码,以后修改为单独作一个dll(作完后把它"混淆"一下),然后C#引用它,在引用之前检查dll是否正确(在dll里面设置一引起固定变量,看是否存在,如果存在则证明dll是正确的),如果不正确就直接弹出错误.吴德有老师观点：前面搞一个密码区,在验证时查一下密码区,作一个100-200位的随机数密码算法,肯定破解的人猜不到
            if (machine_code == string.Empty)
                machine_code = get_machine_code();

            long param = System.Convert.ToInt64(machine_code);
            string str = "";
            for (int i = 0; i < 100; ++i)
            {
                param = param * 2;
                str = param.ToString();
                if (str.Length <= 12)
                    param = System.Convert.ToInt64(str);
                else
                    param = System.Convert.ToInt64(str.Substring(0, 12));
            }
            return param.ToString();
        }

        public bool write_reg_code(string reg_code)
        {
            return ini.write_ini("RegCode", reg_code);
        }

        public bool Is_Reg()
        {
            return (ini.read_ini("RegCode") == get_reg_code());
        }
    }

    partial class reg
    {
        readonly int[] encode = new int[] { 8, 7, 8, 6, 5, 7, 9, 9, 8 };
        ini_config ini;
        registry register;
        public reg()
        {
            ini = new ini_config("config.ini");
            register = new registry("XJPDF Convert");
        }
    }

    partial class ini_config
    {
        public bool write_ini(string node_name, string str, string section_name = "App")
        {
            return WritePrivateProfileString(section_name, node_name, str, get_app_dic() + ini_name);
        }

        public string read_ini(string node_name, string section_name = "App")
        {
            StringBuilder str_buffer = new StringBuilder(100);
            GetPrivateProfileString(section_name, node_name, "", str_buffer, 100, get_app_dic() + ini_name);
            return str_buffer.ToString();
        }

        private string get_app_dic()
        {
            string app_full_path = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            return app_full_path.Substring(0, app_full_path.LastIndexOf("\\") + 1);
        }
    }

    partial class ini_config
    {
        string ini_name;

        public ini_config(string file_name)
        {
            ini_name = file_name;
        }

        [DllImport("kernel32")]
        private static extern bool WritePrivateProfileString(string section, string key, string val, string filePath);

        [DllImport("kernel32")]
        private static extern bool GetPrivateProfileString(string section, string key, string defVal, System.Text.StringBuilder retVal, int size, string filePath);
    }


    partial class registry
    {
        public int get_reg_int(string item)
        {
            try
            {
                RegistryKey dest = Registry.CurrentUser.OpenSubKey("software\\" + reg_item, false);
                int res = System.Convert.ToInt32(dest.GetValue(item));
                dest.Close();
                return res;
            }
            catch
            {
                return READ_ERROR;
            }
        }

        public byte[] get_reg_byte(string item)
        {
            try
            {
                RegistryKey dest = Registry.CurrentUser.OpenSubKey("software\\" + reg_item, false);
                byte[] res = (byte[])dest.GetValue(item);
                dest.Close();
                return res;
            }
            catch
            {
                return null;
            }
        }


        public string get_reg_string(string item)
        {
            try
            {
                RegistryKey dest = Registry.CurrentUser.OpenSubKey("software\\" + reg_item, false);
                string res = dest.GetValue(item).ToString();
                dest.Close();
                return res;
            }
            catch
            {
                return "";
            }
        }

        public bool set_reg_int(string item, int data)
        {
            try
            {
                if (!is_reg_exist(reg_item))
                {
                    create_reg_item(reg_item);
                }
                RegistryKey dest = Registry.CurrentUser.OpenSubKey("software\\" + reg_item, true);
                dest.SetValue(item, data);
                dest.Close();
                return true;
            }
            catch
            {
                return false;
            }

        }

        public bool set_reg_byte(string item, ref byte[] data)
        {
            try
            {
                if (!is_reg_exist(reg_item))
                {
                    create_reg_item(reg_item);
                }
                RegistryKey dest = Registry.CurrentUser.OpenSubKey("software\\" + reg_item, true);
                dest.SetValue(item, data);
                dest.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool set_reg_string(string item, string data)
        {
            try
            {
                if (!is_reg_exist(reg_item))
                {
                    create_reg_item(reg_item);
                }
                RegistryKey dest = Registry.CurrentUser.OpenSubKey("software" + reg_item, true);
                dest.SetValue(item, data);
                dest.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool is_reg_exist(string item)
        {
            try
            {
                RegistryKey dest = Registry.CurrentUser.OpenSubKey("software", false);
                string[] sub_names = dest.GetSubKeyNames();

                foreach (string sub in sub_names)
                {
                    if (item == sub)
                        goto YES;
                }
                dest.Close();
                return false;
            YES:
                dest.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool create_reg_item(string item)
        {
            try
            {
                RegistryKey dest = Registry.CurrentUser.OpenSubKey("software", true);
                dest.CreateSubKey(item);
                dest.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    partial class registry
    {
        string reg_item;
        public readonly int READ_ERROR = -9999;
        public registry(string item)
        {
            reg_item = item;
        }
    }

}
