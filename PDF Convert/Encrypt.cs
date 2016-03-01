using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace PDF_Convert
{
    public static class Encrypt
    {
        public static string APPTITLE;

        static Encrypt()
        {
            string tmp = new ini_config("config.ini").read_ini("Data");
            //string iniLanguage = new ini_config("config.ini").read_ini("language");
            //switch (iniLanguage.ToLower())
            switch (Program.iniLanguage.ToLower())
            {
                case "zh-cn":
                    tmp = new ini_config("config.ini").read_ini("Data");
                    break;
                default:
                    tmp = new ini_config("config.ini").read_ini("EData");
                    break;
            }
            if (tmp == string.Empty)
            {
                APPTITLE = Program.appProgName;
                return;
            }

            APPTITLE = DecryptDES(tmp);
        }
        public static string Refresh()
        {
            string tmp = new ini_config("config.ini").read_ini("Data");
            string iniLanguage = new ini_config("config.ini").read_ini("language");
            switch (iniLanguage.ToLower())
            {
                case "zh-cn":
                    tmp = new ini_config("config.ini").read_ini("Data");
                    break;
                default:
                    tmp = new ini_config("config.ini").read_ini("EData");
                    break;
            }
            if (tmp == string.Empty)
            {
                APPTITLE = Program.appProgName;//"PDF Converter";
                return APPTITLE;
            }

            return APPTITLE = DecryptDES(tmp);
        }

        public static string EncryptDES(string encryptString)
        {
            try
            {
                return System.Convert.ToBase64String(Encoding.Default.GetBytes(encryptString));
            }
            catch
            {
                return encryptString;
            }
        }

        public static string DecryptDES(string decryptString)
        {
            try
            {
                string tmp = Encoding.Default.GetString(System.Convert.FromBase64String(decryptString)); //解码,把Config.ini里面的编码 WHVuSmllIFBERiBDb252ZXJ0ZXI= 解码出来
                //string tmp = EncryptDES("XunJie PDF Converter"); //编码,得到加密后的编码 WHVuSmllIFBERiBDb252ZXJ0ZXI=
                if (tmp == string.Empty)
                    return "PDF Converter";
                return tmp;
            }
            catch
            {
                return "PDF Converter";
            }
        }
    }
}
