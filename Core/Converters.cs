using System;
using System.Globalization;
using System.Windows.Data;

namespace ERDT.Core.Converters
{
    public class DefaultCharNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return "Select a profile";
            }
            return (value as CharacterData).Name;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }

    public class DefaultCharDeathsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return "First Time? ;)";
            }
            return (value as CharacterData).Deaths;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }

    public class RadioIndexToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return false;

            var selectedIndex = System.Convert.ToInt32(value);
            var targetIndex = System.Convert.ToInt32(parameter);

            return selectedIndex == targetIndex;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return null;

            var isChecked = (bool)value;
            var targetIndex = System.Convert.ToInt32(parameter);

            return isChecked ? targetIndex : Binding.DoNothing;
        }
    }
}
