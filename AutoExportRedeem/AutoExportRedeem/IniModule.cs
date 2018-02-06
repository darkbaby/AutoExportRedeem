using System;
using System.Runtime.InteropServices;
using System.Text;

namespace AutoExportRedeem
{
    class IniFile
    {
        public string path;

        //[DllImport("kernel32")]
        //private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        //[DllImport("kernel32")]
        //private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        static extern long WritePrivateProfileString(string Section, string Key, string Value, string FilePath);

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        static extern int GetPrivateProfileString(string Section, string Key, string Default, StringBuilder RetVal, int Size, string FilePath);


        /// <summary>
        /// INIFile Constructor.
        /// </summary>
        /// <PARAM name="INIPath"></PARAM>
        public IniFile(string INIPath)
        {
            path = INIPath;
        }
        /// <summary>
        /// Write Data to the INI File
        /// </summary>
        /// <PARAM name="Section"></PARAM>
        /// Section name
        /// <PARAM name="Key"></PARAM>
        /// Key Name
        /// <PARAM name="Value"></PARAM>
        /// Value Name
        public void IniWriteValue(string Section, string Key, string Value)
        {
            WritePrivateProfileString(Section, Key, Value, this.path);
        }

        /// <summary>
        /// Read Data Value From the Ini File
        /// </summary>
        /// <PARAM name="Section"></PARAM>
        /// <PARAM name="Key"></PARAM>
        /// <PARAM name="Path"></PARAM>
        /// <returns></returns>
        public string IniReadValue(string Section, string Key)
        {
            StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileString(Section, Key, "", temp, 255, this.path);
            return temp.ToString();

        }
    }

    public class SettingBean
    {
        public String dataSource { get; set; }
        public String databaseName { get; set; }
        public String username { get; set; }
        public String password { get; set; }
        public int databaseTimeout { get; set; }

        public String dataSourceLog { get; set; }
        public String databaseNameLog { get; set; }
        public String usernameLog { get; set; }
        public String passwordLog { get; set; }
        public int databaseTimeoutLog { get; set; }

        public String localFolder { get; set; }
        public String localPath { get; set; }

        public String networkDrive { get; set; }
        public String networkPath { get; set; }
        public String networkUsername { get; set; }
        public String networkPassword { get; set; }

        public String fileFormatName { get; set; }
        public String folderStructure { get; set; }
    }
}
