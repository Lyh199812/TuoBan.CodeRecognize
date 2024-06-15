using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows;
using System.IO;
using MiniExcelLibs;
using Prism.Commands;
using HalconDotNet;
using static ImTools.ImMap;
using Base.Start.View.Pages;
using HandyControl.Tools.Extension;
using Prism.Common;
using Base.Model;
using Common;

namespace Base.Start.ViewModel
{
    public class MainViewModel: BindableBase
    {
       

        public MainViewModel() 
        {
            #region  菜单
            Menus = new List<MenuModel>();
            Menus.Add(new MenuModel
            {
                IsSelected = true,
                MenuHeader = "监控",
                MenuIcon = "\ue620",
                TargetView = "MonitorPage"
            });
            #endregion

            #region SoftVersion
            //软件版本
            Assembly assembly = Assembly.GetEntryAssembly(); // 获取当前应用程序的程序集

            // 获取程序集中的文件版本信息
            AssemblyFileVersionAttribute fileVersionAttribute = assembly.GetCustomAttribute<AssemblyFileVersionAttribute>();
            string fileVersion = fileVersionAttribute?.Version;
            CommonMethods.SoftVersion = fileVersion;
            SoftVersion = "V" + CommonMethods.SoftVersion;
            #endregion


            ShowPage(Menus[0]);
        }

        #region Base
        #region Filed
        private Dictionary<string, object> viewCache = new Dictionary<string, object>();

        #endregion

        #region Property
        // 主窗口数据
        private List<MenuModel> menus;

        public List<MenuModel> Menus
        {
            get { return menus; }
            set { menus = value; RaisePropertyChanged(); }
        }


        //软件版本
        private string softVersion;
        public string SoftVersion
        {
            get { return softVersion; }
            set { softVersion = value; RaisePropertyChanged(); }
        }

        //当前界面
        private object _viewContent;

        public object ViewContent
        {
            get { return _viewContent; }
            set { _viewContent = value; RaisePropertyChanged(); }
        }
        #endregion

        #region Methods
        private void ShowPage(object obj)
        {
            var model = obj as MenuModel;
            if (model != null)
            {
                if (ViewContent != null && ViewContent.GetType().Name == model.TargetView) return;

                if (viewCache.ContainsKey(model.TargetView))
                {
                    ViewContent = viewCache[model.TargetView];
                }
                else
                {
                    Type type = Assembly.Load("Base.Start").GetType("Base.Start.View.Pages." + model.TargetView);
                    ViewContent = Activator.CreateInstance(type);
                    viewCache[model.TargetView] = ViewContent;
                }
            }
        }
        #endregion
        #endregion

    }
}
