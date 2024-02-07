using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace FIFAScripts.MVVM.Converters;

public class ValueToColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {

        int x;
        var isNum = int.TryParse(value?.ToString(), out x);

        if (isNum)
        {
            if (x >= 80)
                return new SolidColorBrush(Colors.Green);

            else if (x > 50)
                return new SolidColorBrush(Colors.YellowGreen);

            else
                return new SolidColorBrush(Colors.OrangeRed);
        }
        
        else return new SolidColorBrush(Colors.White);
        

       
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
