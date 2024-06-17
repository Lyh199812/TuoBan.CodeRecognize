using BaseFramework.CommunicationLib.Library;
using BaseFramework.ConfigLib;
using BaseFramework.DataConvertLib;
using BaseFramework.ToolsLib;
using Common;
using MiniExcelLibs;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using DataFormat = BaseFramework.DataConvertLib.DataFormat;

namespace Base.Start.ViewModel
{
    public class MonitorViewModel:BindableBase 
    {
        public MonitorViewModel() 
        {
            CommonMethods.AddSysLog=new Action<int, string>(AddSysLog);
            #region UpdateTime
             timer = new Timer((state) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    CurrentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                });
            }, null, TimeSpan.Zero, TimeSpan.FromSeconds(0.5));
            #endregion
            LoadInfo();
        }
        #region Base
        Timer timer;
        //Cuurent time
        private string currentTime;

        public string CurrentTime
        {
            get { return currentTime; }
            set { currentTime = value; RaisePropertyChanged(); }
        }
        #endregion
        #region PLC

        #region ---Field
        public static MelsecFXSerialDevice CurDevice;
        public static CancellationTokenSource cts;
        public bool IsFirstConnect = true;
        //大小端
        private DataFormat dataFormat = DataFormat.ABCD;
        //创建通信对象
        private MelsecFxSerial EObj = null;
        //创建连接正常标志位
        private bool isConnected = false;
        #endregion

        #region ---Property


        #endregion

        #region ---Method
        private void LoadInfo()
         {
            CurDevice = LoadDevice(CommonMethods.devicePath);
            if (CurDevice != null)
            {
                CommonMethods.AddSysLog.Invoke(0,"PLC_设备变量配置信息加载成功");
                cts = new CancellationTokenSource();
            }
         }


        /// <summary>
        /// 加载配置信息的方法
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private MelsecFXSerialDevice LoadDevice(string path)
        {
            if (!File.Exists(path))
            {
                CommonMethods.AddSysLog.Invoke(2, "PLC_LoadDevice_设备文件不存在");
                return null;
            }

            List<MelsecFXSerialGroup> MelsecFXSerialGroupList = LoadMelsecFXSerialGroup(CommonMethods.groupPath, CommonMethods.variablePath);

            if (MelsecFXSerialGroupList != null && MelsecFXSerialGroupList.Count > 0)
            {
                try
                {
                    return new MelsecFXSerialDevice()
                    {
                        PortName = CommonMethods.PortName,

                        GroupList = MelsecFXSerialGroupList
                    };
                }
                catch (Exception ex)
                {
                    CommonMethods.AddSysLog.Invoke(2, "PLC_LoadDevice_设备信息加载失败:" + ex.Message);
                    return null;
                }
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// 加载通信组信息
        /// </summary>
        /// <param name="grouppath"></param>
        /// <param name="variablepath"></param>
        /// <returns></returns>
        private List<MelsecFXSerialGroup> LoadMelsecFXSerialGroup(string grouppath, string variablepath)
        {
            if (!File.Exists(grouppath))
            {
                CommonMethods.AddSysLog.Invoke(2, "PLC_LoadMelsecFXSerialGroup_通信组文件不存在");

                return null;
            }

            if (!File.Exists(variablepath))
            {
                CommonMethods.AddSysLog.Invoke(2, "PLC_LoadMelsecFXSerialGroup_通信变量文件不存在");
                return null;
            }

            List<MelsecFXSerialGroup> GpList = null;

            try
            {
                GpList = MiniExcel.Query<MelsecFXSerialGroup>(grouppath).ToList();
            }
            catch (Exception ex)
            {
                CommonMethods.AddSysLog.Invoke(2, "PLC_LoadMelsecFXSerialGroup_通信组加载失败:" + ex.Message);
                return null;
            }
            List<MelsecFXSerialVariable> VarList = null;
            try
            {
                VarList = MiniExcel.Query<MelsecFXSerialVariable>(variablepath).ToList();
            }
            catch (Exception ex)
            {
                CommonMethods.AddSysLog.Invoke(2, "PLC_LoadMelsecFXSerialGroup_通信变量加载失败：" + ex.Message);
                return null;
            }


            if (GpList != null && VarList != null)
            {
                foreach (var group in GpList)
                {
                    group.VarList = VarList.FindAll(c=>c.GroupName  == group.GroupName).ToList();
                }
                return GpList;
            }
            else
            {
                return null;
            }
        }

        private async void PLCCommunication(MelsecFXSerialDevice EObj)
        {
            //DateTime HeartBeatSendTime = DateTime.Now;
            //while (!cts.IsCancellationRequested)
            //{
            //    if (EObj.IsConnected)
            //    {
            //        foreach (var gp in EObj.MelsecFXSerialGroupList)
            //        {
            //            byte[] data = null;
            //            int reqLength = 0;
            //            if (gp.StoreArea == "输入线圈" || gp.StoreArea == "输出线圈")
            //            {
            //                switch (gp.StoreArea)
            //                {
            //                    case "输入线圈":
            //                        data = EObj.ReadInputCoils(gp.Start, gp.Length);
            //                        reqLength = ShortLib.GetByteLengthFromBoolLength(gp.Length);
            //                        break;
            //                    case "输出线圈":
            //                        data = EObj.ReadOutputCoils(gp.Start, gp.Length);
            //                        reqLength = ShortLib.GetByteLengthFromBoolLength(gp.Length);
            //                        break;
            //                    default:
            //                        break;
            //                }

            //                if (data != null && data.Length == reqLength)
            //                {
            //                    foreach (var variable in gp.VarList)
            //                    {
            //                        int start = variable.Start - gp.Start;

            //                        DataType dataType = (DataType)Enum.Parse(typeof(DataType), variable.DataType, true);

            //                        switch (dataType)
            //                        {
            //                            case DataType.Bool:
            //                                variable.VarValue = BitLib.GetBitFromByteArray(data, start, variable.OffsetOrLength);
            //                                break;
            //                            default:
            //                                break;
            //                        }

            //                        EObj.UpdateMelsecFXSerialVariable(variable);
            //                    }
            //                }
            //                else
            //                {
            //                    EObj.IsConnected = false;
            //                    break;
            //                }
            //            }
            //            else
            //            {
            //                switch (gp.StoreArea)
            //                {
            //                    case "输入寄存器":
            //                        data = EObj.ReadInputRegisters(gp.Start, gp.Length);
            //                        reqLength = gp.Length * 2;
            //                        break;
            //                    case "输出寄存器":
            //                        data = EObj.ReadOutputRegisters(gp.Start, gp.Length);
            //                        reqLength = gp.Length * 2;
            //                        break;
            //                    default:
            //                        break;
            //                }
            //                if (data != null && data.Length == reqLength)
            //                {
            //                    foreach (var variable in gp.VarList)
            //                    {
            //                        int start = variable.Start - gp.Start;

            //                        start *= 2;

            //                        DataType dataType = (DataType)Enum.Parse(typeof(DataType), variable.DataType, true);

            //                        switch (dataType)
            //                        {
            //                            case DataType.Bool:
            //                                variable.VarValue = BitLib.GetBitFrom2BytesArray(data, start, variable.OffsetOrLength, dataFormat == DataFormat.BADC || dataFormat == DataFormat.DCBA);
            //                                break;
            //                            case DataType.Byte:
            //                                variable.VarValue = ByteLib.GetByteFromByteArray(data, start);
            //                                break;
            //                            case DataType.Short:
            //                                variable.VarValue = ShortLib.GetShortFromByteArray(data, start);
            //                                break;
            //                            case DataType.UShort:
            //                                variable.VarValue = UShortLib.GetUShortFromByteArray(data, start);
            //                                break;
            //                            case DataType.Int:
            //                                variable.VarValue = IntLib.GetIntFromByteArray(data, start, dataFormat);
            //                                break;
            //                            case DataType.UInt:
            //                                variable.VarValue = UIntLib.GetUIntFromByteArray(data, start, dataFormat);
            //                                break;
            //                            case DataType.Float:
            //                                variable.VarValue = FloatLib.GetFloatFromByteArray(data, start, dataFormat);
            //                                break;
            //                            case DataType.Double:
            //                                variable.VarValue = DoubleLib.GetDoubleFromByteArray(data, start, dataFormat);
            //                                break;
            //                            case DataType.Long:
            //                                variable.VarValue = LongLib.GetLongFromByteArray(data, start, dataFormat);
            //                                break;
            //                            case DataType.ULong:
            //                                variable.VarValue = ULongLib.GetULongFromByteArray(data, start, dataFormat);
            //                                break;
            //                            case DataType.String:
            //                                variable.VarValue = StringLib.GetStringFromByteArrayByEncoding(data, start, variable.OffsetOrLength * 2, Encoding.ASCII);
            //                                break;
            //                            case DataType.ByteArray:
            //                                variable.VarValue = ByteArrayLib.GetByteArrayFromByteArray(data, start, variable.OffsetOrLength * 2);
            //                                break;
            //                            case DataType.HexString:
            //                                variable.VarValue = StringLib.GetHexStringFromByteArray(data, start, variable.OffsetOrLength * 2);
            //                                break;
            //                            default:
            //                                break;
            //                        }
            //                        switch (variable.VarName)
            //                        {
            //                            case "Signal_Heartbeat":
            //                                {
            //                                    TimeSpan timeSpan = DateTime.Now - HeartBeatSendTime;
            //                                    if (timeSpan.Seconds > 1)
            //                                    {
            //                                       // CommonMethods.HeartBeatWrite(variable.VarName, "1");
            //                                    }
            //                                    break;
            //                                }
            //                            default:
            //                                {
            //                                    break;
            //                                }
            //                        }
            //                        variable.VarValue = MigrationLib.GetMigrationValue(variable.VarValue, variable.Scale.ToString(), variable.Offset.ToString()).Content;
            //                        EObj.UpdateMelsecFXSerialVariable(variable);

            //                    }
            //                }
            //                else
            //                {
            //                    EObj.IsConnected = false;
            //                    CommonMethods.AddSysLog.Invoke(1,"PLC_返回数据为空/数据长度错误_断开连接");
            //                    break;
            //                }
            //            }
            //        }
            //        if (EObj.IsConnected)//如果变量读取没出问题
            //        {

            //        }


            //    }
            //    else
            //    {
            //        if (IsFirstConnect)
            //        {
            //            EObj = new ModbusTCP();
            //            IsFirstConnect = false;
            //            //初始化相机
            //            camManager = new CamManager(CommonMethods.frmCam, CommonMethods.CamIpAddress);
            //            var camIniRst = camManager.CamIni();
            //            if (camIniRst.IsSuccess)
            //            {
            //                CommonMethods.AddSysLog($"Cam_相机初始化成功_IP:[{CommonMethods.CamIpAddress}]", 0, true); ;

            //            }
            //            else
            //            {
            //                CommonMethods.AddSysLog($"Cam_相机连接失败_IP:[{CommonMethods.CamIpAddress}]", 2, true); ;

            //            }
            //        }
            //        else
            //        {
            //            EObj.DisConnect();
            //            //重连
            //            await Task.Delay(EObj.ReConnectTime);
            //            CommonMethods.AddSysLog($"PLC_尝试重新连接_IP:[{EObj.IPAddress}]", 0, true); ;

            //        }

            //        EObj.IsConnected = EObj.Connect(EObj.IPAddress, EObj.Port);

            //        if (EObj.IsConnected)
            //        {
            //            CommonMethods.AddSysLog($"PLC_连接成功_IP:[{EObj.IPAddress}]_Port:[{EObj.Port}]", 0, true); ;
            //        }
            //        else
            //        {
            //            CommonMethods.AddSysLog($"PLC_连接失败_IP:[{EObj.IPAddress}]_Port:[{EObj.Port}]", 1, true); ;

            //        }


            //    }
            //}
        }


        /// <summary>
        /// 通用验证
        /// </summary>
        /// <returns></returns>
        private bool CommonValidate()
        {
            if (isConnected == false)
            {
                CommonMethods.AddSysLog(1, "设备未连接，请检查");
                return false;
            }

            return true;
        }
        private void Read(string strDataType,string strMelsecFXSerialVariableAddress,string strCount)

        {
            if (CommonValidate())
            {
                DataType dataType = (DataType)Enum.Parse(typeof(DataType), strDataType);

                switch (dataType)
                {
                    case DataType.Bool:

                        var result1 = EObj.ReadCommon<bool[]>(strMelsecFXSerialVariableAddress, Convert.ToUInt16(strCount));

                        if (result1.IsSuccess)
                        {
                            CommonMethods.AddSysLog(0, "读取成功:" + StringLib.GetStringFromValueArray(result1.Content));
                        }
                        else
                        {
                            CommonMethods.AddSysLog(1, "读取失败:" + result1.Message);
                        }

                        break;
                    case DataType.Byte:
                    case DataType.SByte:
                        var result2 = EObj.ReadCommon<byte[]>(strMelsecFXSerialVariableAddress, Convert.ToUInt16(strCount));

                        if (result2.IsSuccess)
                        {
                            CommonMethods.AddSysLog(0, "读取成功:" + StringLib.GetStringFromValueArray(result2.Content));
                        }
                        else
                        {
                            CommonMethods.AddSysLog(1, "读取失败:" + result2.Message);
                        }

                        break;
                    case DataType.Short:

                        var result3 = EObj.ReadCommon<short[]>(strMelsecFXSerialVariableAddress, Convert.ToUInt16(strCount));

                        if (result3.IsSuccess)
                        {
                            CommonMethods.AddSysLog(0, "读取成功:" + StringLib.GetStringFromValueArray(result3.Content));
                        }
                        else
                        {
                            CommonMethods.AddSysLog(1, "读取失败:" + result3.Message);
                        }

                        break;
                    case DataType.UShort:
                        var result4 = EObj.ReadCommon<ushort[]>(strMelsecFXSerialVariableAddress, Convert.ToUInt16(strCount));

                        if (result4.IsSuccess)
                        {
                            CommonMethods.AddSysLog(0, "读取成功:" + StringLib.GetStringFromValueArray(result4.Content));
                        }
                        else
                        {
                            CommonMethods.AddSysLog(1, "读取失败:" + result4.Message);
                        }

                        break;
                    case DataType.Int:
                        var result5 = EObj.ReadCommon<int[]>(strMelsecFXSerialVariableAddress, Convert.ToUInt16(strCount));

                        if (result5.IsSuccess)
                        {
                            CommonMethods.AddSysLog(0, "读取成功:" + StringLib.GetStringFromValueArray(result5.Content));
                        }
                        else
                        {
                            CommonMethods.AddSysLog(1, "读取失败:" + result5.Message);
                        }
                        break;
                    case DataType.UInt:
                        var result6 = EObj.ReadCommon<uint[]>(strMelsecFXSerialVariableAddress, Convert.ToUInt16(strCount));

                        if (result6.IsSuccess)
                        {
                            CommonMethods.AddSysLog(0, "读取成功:" + StringLib.GetStringFromValueArray(result6.Content));
                        }
                        else
                        {
                            CommonMethods.AddSysLog(1, "读取失败:" + result6.Message);
                        }
                        break;
                    case DataType.Float:
                        var result7 = EObj.ReadCommon<float[]>(strMelsecFXSerialVariableAddress, Convert.ToUInt16(strCount));

                        if (result7.IsSuccess)
                        {
                            CommonMethods.AddSysLog(0, "读取成功:" + StringLib.GetStringFromValueArray(result7.Content));
                        }
                        else
                        {
                            CommonMethods.AddSysLog(1, "读取失败:" + result7.Message);
                        }
                        break;
                    case DataType.Double:
                        var result8 = EObj.ReadCommon<float[]>(strMelsecFXSerialVariableAddress, Convert.ToUInt16(strCount));

                        if (result8.IsSuccess)
                        {
                            CommonMethods.AddSysLog(0, "读取成功:" + StringLib.GetStringFromValueArray(result8.Content));
                        }
                        else
                        {
                            CommonMethods.AddSysLog(1, "读取失败:" + result8.Message);
                        }
                        break;
                    case DataType.Long:
                        var result9 = EObj.ReadCommon<long[]>(strMelsecFXSerialVariableAddress, Convert.ToUInt16(strCount));

                        if (result9.IsSuccess)
                        {
                            CommonMethods.AddSysLog(0, "读取成功:" + StringLib.GetStringFromValueArray(result9.Content));
                        }
                        else
                        {
                            CommonMethods.AddSysLog(1, "读取失败:" + result9.Message);
                        }
                        break;
                    case DataType.ULong:
                        var result10 = EObj.ReadCommon<ulong[]>(strMelsecFXSerialVariableAddress, Convert.ToUInt16(strCount));

                        if (result10.IsSuccess)
                        {
                            CommonMethods.AddSysLog(0, "读取成功:" + StringLib.GetStringFromValueArray(result10.Content));
                        }
                        else
                        {
                            CommonMethods.AddSysLog(1, "读取失败:" + result10.Message);
                        }
                        break;
                    case DataType.String:
                        var result11 = EObj.ReadCommon<string>(strMelsecFXSerialVariableAddress, Convert.ToUInt16(strCount));

                        if (result11.IsSuccess)
                        {
                            CommonMethods.AddSysLog(0, "读取成功:" + result11.Content);
                        }
                        else
                        {
                            CommonMethods.AddSysLog(1, "读取失败:" + result11.Message);
                        }
                        break;
                    default:
                        CommonMethods.AddSysLog(1, "读取失败:不支持的数据类型");
                        break;
                }

            }
        }

         
        private void Write(string strDataType,string strMelsecFXSerialVariableAddress,string strSetValue)

        {
            if (true)
            {
                DataType dataType = (DataType)Enum.Parse(typeof(DataType), strDataType);

                var result = OperateResult.CreateFailResult();

                switch (dataType)
                {
                    case DataType.Bool:
                        result = EObj.WriteCommon(strMelsecFXSerialVariableAddress, BitLib.GetBitArrayFromBitArrayString(strSetValue.Trim()));
                        break;
                    case DataType.Byte:
                    case DataType.SByte:
                        result = EObj.WriteCommon(strMelsecFXSerialVariableAddress, ByteArrayLib.GetByteArrayFromHexString(strSetValue.Trim()));
                        break;
                    case DataType.Short:
                        result = EObj.WriteCommon(strMelsecFXSerialVariableAddress, ShortLib.GetShortArrayFromString(strSetValue.Trim()));
                        break;
                    case DataType.UShort:
                        result = EObj.WriteCommon(strMelsecFXSerialVariableAddress, UShortLib.GetUShortArrayFromString(strSetValue.Trim()));
                        break;
                    case DataType.Int:
                        result = EObj.WriteCommon(strMelsecFXSerialVariableAddress, IntLib.GetIntArrayFromString(strSetValue.Trim()));
                        break;
                    case DataType.UInt:
                        result = EObj.WriteCommon(strMelsecFXSerialVariableAddress, UIntLib.GetUIntArrayFromString(strSetValue.Trim()));
                        break;
                    case DataType.Float:
                        result = EObj.WriteCommon(strMelsecFXSerialVariableAddress, FloatLib.GetFloatArrayFromString(strSetValue.Trim()));
                        break;
                    case DataType.Double:
                        result = EObj.WriteCommon(strMelsecFXSerialVariableAddress, DoubleLib.GetDoubleArrayFromString(strSetValue.Trim()));
                        break;
                    case DataType.Long:
                        result = EObj.WriteCommon(strMelsecFXSerialVariableAddress, LongLib.GetLongArrayFromString(strSetValue.Trim()));
                        break;
                    case DataType.ULong:
                        result = EObj.WriteCommon(strMelsecFXSerialVariableAddress, ULongLib.GetULongArrayFromString(strSetValue.Trim()));
                        break;
                    case DataType.String:
                        result = EObj.WriteCommon(strMelsecFXSerialVariableAddress, strSetValue.Trim());
                        break;
                    default:
                        break;
                }

                if (result.IsSuccess)
                {
                    CommonMethods.AddSysLog(0, "写入成功");
                }
                else
                {
                    CommonMethods.AddSysLog(0, "写入失败");
                }
            }
        }
        #endregion


        #region ---Delegate

        #endregion
        #endregion

        #region Log

        #region ---Field
        //LogList
        private ObservableCollection<OperateLog> logList=new ObservableCollection<OperateLog>();

        public ObservableCollection<OperateLog> LogList
        {
            get { return logList; }
            set
            {
                logList = value;
                RaisePropertyChanged();
            }
        }


        #endregion

        #region ---Property


        #endregion

        #region ---Method
        private void ClearSysLog()
        {
            logList.Clear();
        }
        private  void AddSysLog(int Model,string Meg)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                if (Model == 0)//成功
                {
                    logList.Add(new OperateLog { LogIcon = "\ue626", IconColor = "Green", OperateTime = DateTime.Now.ToString(), OperateInfo = Meg });
                }
                else if (Model == 1)//警告
                {
                    logList.Add(new OperateLog { LogIcon = "\ue616", IconColor = "Orange", OperateTime = DateTime.Now.ToString(), OperateInfo = Meg });
                }
                else//报警
                {
                    logList.Add(new OperateLog { LogIcon = "\ue62a", IconColor = "Red", OperateTime = DateTime.Now.ToString(), OperateInfo = Meg });

                }
            });
        }


        #endregion

        #region ---Delegate

        #endregion
        #endregion
    }
}
