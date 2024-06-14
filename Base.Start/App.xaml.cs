using Common;
using Base.Start.View;
using Base.Start.View.Pages;
using Base.Start.ViewModel;
using Prism.DryIoc;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Base.Start
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            //通过容器的形式获取到需要显示的MainWindow
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            return Container.Resolve<MainView>();
        }
        /// <summary>
        /// 依赖注入的实现，暂时没有，可以不用添加
        /// </summary>
        /// <param name="containerRegistry"></param>
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
           
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = (Exception)e.ExceptionObject;
            CommonMethods.AddSysLog(2,$"未捕获到的异常:{ex}");
            MessageBox.Show($"程序退出 Unhandled exception caught in AppDomain: {ex}");
        }
    }
}
