using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmCross.ViewModels;
using ProcessManager.Core.ViewModels;

namespace ProcessManager.Core
{
    public class App : MvxApplication
    {
        public override void Initialize()
        {
            RegisterAppStart<ProcessListViewModel>();
            base.Initialize();
        }
    }
}
