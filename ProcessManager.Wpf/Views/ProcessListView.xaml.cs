using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using MvvmCross.Platforms.Wpf.Views;
using ProcessManager.Core.ViewModels;

namespace ProcessManager.Wpf.Views
{
    /// <summary>
    /// Interaction logic for ProcessListView.xaml
    /// </summary>
    public partial class ProcessListView : MvxWpfView
    {
        public ProcessListView()
        {
            InitializeComponent();
        }

        private void KillProcess(object sender, RoutedEventArgs e)
        {
            try
            {((ProcessListViewModel)ViewModel).KillProcess((ProcessViewModel)ListView.SelectedItem);}
            catch{}
        }


        private void ChangePriority(object sender, RoutedEventArgs e)
        {
            if ((ProcessViewModel)ListView.SelectedItem != null)
            {
                ((ProcessViewModel)ListView.SelectedItem).Priority =
                    (ProcessPriorityClass)PriorityComboBox.SelectedItem;
            }
        }
    }
}
