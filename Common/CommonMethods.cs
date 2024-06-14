using Common.Config;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Common
{
    public  class CommonMethods 
    {
        public CommonMethods()
        {

        }
        #region SystemInfo
        public static string SoftVersion {  get; set; }
        #endregion


        #region ConfigInfo
        public static string ConfigPath = Environment.CurrentDirectory + "\\Config\\Device.ini";
        ////PLCIpAddress
        //public static string PLCIpAddress {  get; set; }= IniConfigHelper.ReadIniData("设备参数", "PLCIP地址", "NULL", ConfigPath);

        ////CamIpAddress
        //public static string CamIpAddress{ get; set; } = IniConfigHelper.ReadIniData("设备参数", "CamIP地址", "NULL", ConfigPath);


        #endregion

        #region Delegate
        //系统日志添加
        public static Action<int,string> AddSysLog { get; set; }

        #endregion

    }
}
