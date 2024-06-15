using Base.Start.ViewModel;
using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Base.Start.View.Pages
{
    /// <summary>
    /// MonitorPage.xaml 的交互逻辑
    /// </summary>
    public partial class MonitorPage : System.Windows.Controls.UserControl
    {
        public MonitorPage()
        {
            InitializeComponent();
            this.DataContext = new MonitorViewModel();
        }

        
    }
}
