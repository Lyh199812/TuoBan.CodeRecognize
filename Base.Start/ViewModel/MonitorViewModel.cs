using Common;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base.Start.ViewModel
{
    public class MonitorViewModel:BindableBase 
    {
        public MonitorViewModel() 
        {
            CommonMethods.AddSysLog=new Action<int, string>(AddSysLog);
            CommonMethods.AddSysLog.Invoke(0, "1212");
        }

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
