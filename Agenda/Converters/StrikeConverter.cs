using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Agenda.Converters
{
    public class StrikeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool isCompleted && isCompleted)
                return TextDecorations.Strikethrough;
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

