using System;
using System.Collections.Generic;
using System.Text;

//需要调用API函数，所以必须创建System.Runtime.InteropServices命名空间以提供可用于访问 .NET 中的 COM 对象和本机 API 的类的集合
using System.IO;
using System.Runtime.InteropServices; //运行Windows的API,引用DLL专用

namespace PDF_Convert
{
    public class IniFile
    {
        //KongMengYuan增加,2015-11-09,目的是读取ini文件,此程序原有的读写ini文件方法太麻烦了

        public string path;				//INI文件名

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section,string key,
					string val,string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section,string key,string def,
					StringBuilder retVal,int size,string filePath);

        //声明读写INI文件的API函数
        public IniFile(string INIPath)
        {
            path = INIPath;
        }

        //类的构造函数，传递INI文件名
        public void IniWriteValue(string Section,string Key,string Value)
        {
            WritePrivateProfileString(Section,Key,Value,this.path);
        }

        //写INI文件
        public string IniReadValue(string Section,string Key)
        {
            StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileString(Section,Key,"",temp,255,this.path);
            return temp.ToString();
        }

        //读取INI文件指定
	}
}
