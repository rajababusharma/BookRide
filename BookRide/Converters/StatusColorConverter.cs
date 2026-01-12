using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookRide.Converters
{
    public class StatusColorConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string status)
            {
                if (status.Equals("Active"))
                {
                    return Colors.Green; // Return green color if value is positive
                }
                else
                {
                    return Colors.Red; // Return red color otherwise
                }
            }
            return Colors.Black; // Default color if value is null or not an int
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return Binding.DoNothing; // Not needed for one-way binding
        }
    }
}
