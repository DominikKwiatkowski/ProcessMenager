using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmCross.Commands;
using MvvmCross.ViewModels;
using System.Management;
using System.Threading;

namespace ProcessManager.Core.ViewModels
{
    public class ProcessViewModel : MvxViewModel
    {
        private Thread _sustainThread;

        public CancellationToken Token { get; set; }

        public ProcessViewModel(CancellationToken token)
        {
        }

        public Process CurrentProcess
        {
            get => _currentProcess;
            set
            {
                SetProperty(ref _currentProcess, value);
                try
                {
                    BasicPriority = value.BasePriority;
                    ThreadCount = value.Threads.Count;
                    if (_processStartTime == null)
                    {
                        var timer = new Models.Timer(value.StartTime);
                        timer.TimeElapsed += (s, t) => ProcessStartTime = t;
                        timer.StartAsync();
                    }

                    // Can throw exception
                    MainWindowTitle = value.MainWindowTitle;
                    TotalProcessorTime = value.TotalProcessorTime;
                    UserProcessorTime = value.UserProcessorTime;
                    Priority = value.PriorityClass;
                }
                catch (Exception e)
                {
                }
            }
        }

        private Process _currentProcess;

        public string MainWindowTitle
        {
            get => _mainWindowTitle;
            set => SetProperty(ref _mainWindowTitle, value);
        }

        private string _mainWindowTitle;

        public int ThreadCount
        {
            get => _threadCount;
            set => SetProperty(ref _threadCount, value);
        }

        private int _threadCount;

        public int BasicPriority
        {
            get => _basicPriority;
            set => SetProperty(ref _basicPriority, value);
        }

        private int _basicPriority;

        public int RelaunchCounter
        {
            get => _relaunchCounter;
            set => SetProperty(ref _relaunchCounter, value);
        }

        private int _relaunchCounter;

        private TimeSpan? _processStartTime = null;

        public TimeSpan? ProcessStartTime
        {
            get => _processStartTime;
            set => SetProperty(ref _processStartTime, value);
        }

        private TimeSpan _totalProcessorTime;

        public TimeSpan TotalProcessorTime
        {
            get => _totalProcessorTime;
            set => SetProperty(ref _totalProcessorTime, value);
        }

        private TimeSpan _userProcessorTime;

        public TimeSpan UserProcessorTime
        {
            get => _userProcessorTime;
            set => SetProperty(ref _userProcessorTime, value);
        }

        public ProcessPriorityClass Priority
        {
            get => _priority;
            set
            {
                try
                {
                    CurrentProcess.PriorityClass = value;
                    SetProperty(ref _priority, value);
                }
                catch (Exception e)
                {
                }
            }
        }

        private ProcessPriorityClass _priority;


        private void SustainTask(CancellationToken cancelToken)
        {
            while (ShouldSustain)
            {
                if (cancelToken.IsCancellationRequested)
                {
                    ShouldSustain = false;
                    return;
                }

                if (CurrentProcess.HasExited)
                {
                    try
                    {
                        string fullPath = CurrentProcess.MainModule.FileName;
                        lock (CurrentProcess)
                        {
                            CurrentProcess = Process.Start(fullPath);
                        }

                        RelaunchCounter++;
                    }
                    catch
                    {
                        ShouldSustain = false;
                    }
                }
            }
        }

        private bool _shouldSustain;

        public bool ShouldSustain
        {
            get => _shouldSustain;
            set
            {
                SetProperty(ref _shouldSustain, value);
                if (value)
                {
                    _sustainThread = new Thread(v => SustainTask(Token));
                    _sustainThread.Start();
                }
            }
        }
    }
}