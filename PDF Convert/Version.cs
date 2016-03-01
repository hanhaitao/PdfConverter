using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace PDF_Convert
{
    public static class Version
    {
        static ini_config ini = new ini_config("config.ini");
        public static string version = string.IsNullOrEmpty(ini.read_ini("Version")) ? Application.ProductVersion : ini.read_ini("Version");
        private static string cells_ver = "7.7.1.0";
        private static string pdf_ver = "8.8.0.0";
        private static string slides_ver = "8.2.0.0";
        private static string words_ver = "13.12.0.0";
        public static Dictionary<string, string> dic_ver = new Dictionary<string, string>();

        static Version()
        {
            dic_ver.Add("exe_main", version);
            dic_ver.Add("dll_cells", cells_ver);
            dic_ver.Add("dll_pdf", pdf_ver);
            dic_ver.Add("dll_slides", slides_ver);
            dic_ver.Add("dll_words", words_ver);

        }

        /// <summary>
        /// POST请求在线统计
        /// </summary>
        /// <param name="url">请求的链接地址,如：http://statistical.jsocr.com/Default.aspx </param>
        /// <param name="softName">软件名称</param>
        /// <param name="version">版本号</param>
        /// <param name="encoding">机器码</param>
        public static void Post(string url, string softName, string softType, string version, string encoding, string target, string mehodObject)
        {
            try
            {
                WebClient w = new WebClient();

                System.Collections.Specialized.NameValueCollection VarPost = new System.Collections.Specialized.NameValueCollection();
                VarPost.Add("softName", softName.Trim());
                VarPost.Add("softType", softType.Trim());
                VarPost.Add("version", version.Trim());
                VarPost.Add("encoding", encoding.Trim());
                VarPost.Add("target", target.Trim());
                VarPost.Add("mehodObject", mehodObject.Trim());
                VarPost.Add("recordState", "使用");
                VarPost.Add("packageName", "无");
                VarPost.Add("packageVersion", "无");

                //byte[] byRemoteInfo = w.UploadValues(url, "POST", VarPost); //此行代码为2015-11-09郑侃炜提出后注释后,KongMengyuan,2015-11-09,注释原因见下面的注释(2015-11-10发现它并不影响网页的打开速度)

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
            }
            catch { }
        }

        public static string GetParamName(string param)
        {
            string paramName = string.Empty;
            string tmp = new ini_config("config.ini").read_ini(param);
            if (param == "Data")
            {
                paramName = DecryptDES(tmp);
                return paramName;
            }
            else
            {
                paramName = tmp;
            }

            return paramName;
        }

        public static string DecryptDES(string decryptString)
        {
            try
            {
                string tmp = Encoding.Default.GetString(System.Convert.FromBase64String(decryptString));
                if (tmp == string.Empty)
                    return Program.appProgName;
                return tmp;
            }
            catch
            {
                return Program.appProgName;
            }
        }
    }
}
