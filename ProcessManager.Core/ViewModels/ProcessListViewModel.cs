using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MvvmCross.Commands;
using MvvmCross.ViewModels;

namespace ProcessManager.Core.ViewModels
{
    public class ProcessListViewModel : MvxViewModel
    {
        private const int _sleepTime = 10000;
        private static int _lockFlag = 0;
        private Thread _refreshThread;
        private CancellationTokenSource _tokenSource;
        private CancellationToken _token;

        public ProcessListViewModel()
        {
            _refreshThread = new Thread(RefreshThreadJob);
            StartStopCommand = new MvxCommand(StartStop);
            RefreshCommand = new MvxCommand(RefreshProcessList);
            CancelCommand = new MvxCommand(Cancel);
        }

        public override async Task Initialize()
        {
            await base.Initialize();
            _tokenSource = new CancellationTokenSource();
            _token = _tokenSource.Token;
            Process[] processCollection = Process.GetProcesses();
            var newProcessList = new ObservableCollection<ProcessViewModel>();
            foreach (var process in processCollection)
            {
                var processViewModel = new ProcessViewModel(_token);
                processViewModel.CurrentProcess = process;
                processViewModel.Token = _token;
                newProcessList.Add(processViewModel);
            }

            ProcessList =
                new ObservableCollection<ProcessViewModel>(newProcessList.OrderBy(p => p.CurrentProcess.ProcessName));
            //SelectedProcess = ProcessList[0];
            _refreshThread.Start();
        }


        private ObservableCollection<ProcessViewModel> _processList = new ObservableCollection<ProcessViewModel>();

        private readonly object ProcessListLock = new object();

        public ObservableCollection<ProcessViewModel> ProcessList
        {
            get => _processList;
            set => SetProperty(ref _processList, value);
        }

        private bool _shouldRefresh = true;

        public bool ShouldRefresh
        {
            get => _shouldRefresh;
            set
            {
                SetProperty(ref _shouldRefresh, value);
                if (value && !_refreshThread.IsAlive)
                {
                    _refreshThread.Start();
                }
            }
        }

        public void KillProcess(ProcessViewModel processModel)
        {
            if (processModel != null)
            {
                processModel.CurrentProcess.Kill();
            }
        }

        public IMvxCommand RefreshCommand { get; set; }

        public void RefreshProcessList()
        {
            if (Interlocked.CompareExchange(ref _lockFlag, 1, 0) == 0)
            {
                try
                {
                    Process[] processCollection = Process.GetProcesses();
                    List<Process> processToAdd = new List<Process>();
                    var processListResult = new ObservableCollection<ProcessViewModel>();
                    foreach (var process in processCollection)
                    {
                        bool contain = false;
                        foreach (var processViewModel in ProcessList)
                        {
                            if (processViewModel.CurrentProcess.Id == process.Id)
                            {
                                contain = true;
                                processListResult.Add(processViewModel);
                                break;
                            }
                        }

                        if (!contain)
                        {
                            processToAdd.Add(process);
                        }
                    }

                    foreach (var processViewModel in ProcessList)
                    {
                        if (processViewModel.ShouldSustain)
                        {
                            bool contain = false;

                            foreach (var processViewModelResult in processListResult)
                            {
                                if (processViewModel.CurrentProcess.Id == processViewModelResult.CurrentProcess.Id)
                                {
                                    contain = true;
                                }
                            }

                            if (!contain)
                            {
                                processListResult.Add(processViewModel);
                            }
                        }
                    }

                    foreach (var process in processToAdd)
                    {
                        bool contain = false;
                        foreach (var processViews in processListResult)
                        {
                            if (processViews.CurrentProcess.Id == process.Id)
                            {
                                contain = true;
                            }
                        }

                        if (!contain)
                        {
                            var processViewModel = new ProcessViewModel(_token);
                            processViewModel.CurrentProcess = process;
                            processViewModel.Token = _token;
                            processListResult.Add(processViewModel);
                        }
                    }

                    ProcessList =
                        new ObservableCollection<ProcessViewModel>(
                            processListResult.OrderBy(v => v.CurrentProcess.ProcessName));
                }
                finally
                {
                    Interlocked.Decrement(ref _lockFlag);
                }
            }
        }

        private void RefreshThreadJob()
        {
            while (ShouldRefresh)
            {
                RefreshProcessList();
                Thread.Sleep(_sleepTime);
            }
        }

        public IMvxCommand StartStopCommand { get; set; }

        void StartStop()
        {
            ShouldRefresh = !ShouldRefresh;
        }

        public IMvxCommand CancelCommand { get; set; }

        void Cancel()
        {
            _tokenSource.Cancel(false);
            _tokenSource = new CancellationTokenSource();
            _token = _tokenSource.Token;
            foreach (var processViewModel in ProcessList)
            {
                processViewModel.Token = _token;
            }
        }

        private ProcessViewModel _selectedProcess;

        public ProcessViewModel SelectedProcess
        {
            get => _selectedProcess;
            set => SetProperty(ref _selectedProcess, value);
        }
    }
}