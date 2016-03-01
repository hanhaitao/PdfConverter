using Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Management;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace FileGrid
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            ItemInfomation info = new ItemInfomation(@"C:\程序员的自我修养.txt");
            listViewPlus1.AddFile(info);
            info = new ItemInfomation(@"C:\程序员的自我修养.xls");
            listViewPlus1.AddFile(info);
            info = new ItemInfomation(@"C:\程序员的自我修养.pdf");
            listViewPlus1.AddFile(info);
            info = new ItemInfomation(@"C:\程序员的自我修养.doc");
            listViewPlus1.AddFile(info);
            info = new ItemInfomation(@"C:\程序员的自我修养.html");
            listViewPlus1.AddFile(info);
            info = new ItemInfomation(@"C:\程序员的自我修养.jpg");
            listViewPlus1.AddFile(info);
            info = new ItemInfomation(@"C:\程序员的自我修养程序员的自我修养程序员的自我修养.ppt");
            listViewPlus1.AddFile(info);
            info = new ItemInfomation(@"C:\程序员的自我修养.url");
            listViewPlus1.AddFile(info);
            info = new ItemInfomation(@"C:\程序员的自我修养.txt");
            listViewPlus1.AddFile(info);
            info = new ItemInfomation(@"C:\程序员的自我修养.txt");
            listViewPlus1.AddFile(info);
            info = new ItemInfomation(@"C:\程序员的自我修养.txt");
            listViewPlus1.AddFile(info);
            info = new ItemInfomation(@"C:\程序员的自我修养.txt");
            listViewPlus1.AddFile(info);
            info = new ItemInfomation(@"C:\程序员的自我修养.txt");
            listViewPlus1.AddFile(info);
            info = new ItemInfomation(@"C:\程序员的自我修养.txt");
            listViewPlus1.AddFile(info);
            get_cpu_code();

            ManagementClass c = new ManagementClass(
            new ManagementPath("Win32_Processor"));
            // Get the properties in the class
            ManagementObjectCollection moc = c.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                PropertyDataCollection properties = mo.Properties;
                //获取内核数代码
                int pcpu = Convert.ToInt16(properties["NumberOfCores"].Value);
                int lcpu = Convert.ToInt16(properties["NumberOfLogicalProcessors"].Value);
            }
        }
        readonly int[] encode = new int[] { 8, 7, 8, 6, 5, 7, 9, 9, 8 };
        private int get_cpu_code()
        {
            string cpu_id = "";
            string md5 = "";
            ManagementObjectCollection obj = new ManagementClass("Win32_Processor").GetInstances();
            foreach (ManagementObject mo in obj)
            {
                cpu_id = mo.Properties["ProcessorId"].Value.ToString();
                break;
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

        private void ucPicButton1_Click(object sender, EventArgs e)
        {
            foreach (Control c in panel3.Controls)
            {
                if (c.Name != ((Control)sender).Name)
                    ((ucPicButton)c).Selected = false;
            }
        }

        private void listViewPlus1_OnDeleteButtonClicked(int index)
        {
            //listViewPlus1.RemoveFile(index);
        }
    }
}
