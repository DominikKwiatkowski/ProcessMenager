using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmCross.Converters;

namespace ProcessManager.Core.Converters
{
    public class StatusToActionConverter : MvxValueConverter<bool, string>
    {
        protected override string Convert(bool value, Type targetType, object parameter, CultureInfo cultureInfo)
        {
            if (value)
            {
                return "Stop";
            }
            else
            {
                return "Start";
            }
        }
    }
}
