using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MvvmCross.Platforms.Wpf.Views;
using MvvmCross.ViewModels;
using ProcessManager.Core.ViewModels;

namespace ProcessManager.Wpf.Views
{
    /// <summary>
    /// Interaction logic for ProcessView.xaml
    /// </summary>
    [MvxViewFor(typeof(ProcessViewModel))]
    public partial class ProcessView : MvxWpfView
    {
        public ProcessView()
        {
            InitializeComponent();
        }
    }
}
