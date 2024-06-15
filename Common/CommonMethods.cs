using BaseFramework.ToolsLib;
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
        //设备参数路径
        public static string devicePath = Environment.CurrentDirectory + "\\Config\\Device.ini";

        //通信组参数路径
        public static string groupPath = Environment.CurrentDirectory + "\\Config\\Group.xlsx";

        //变量路径
        public static string variablePath = Environment.CurrentDirectory + "\\Config\\Variable.xlsx";
        //PLCIpAddress
        public static string PortName { get; set; } = IniConfigHelper.ReadIniData("设备参数", "PLCPortName", "NULL", devicePath);

        //CamIpAddress
        public static string CamIpAddress { get; set; } = IniConfigHelper.ReadIniData("设备参数", "CamIP地址", "NULL", devicePath);


        #endregion

        #region Delegate
        //系统日志添加
        public static Action<int,string> AddSysLog { get; set; }

        #endregion

    }
}
