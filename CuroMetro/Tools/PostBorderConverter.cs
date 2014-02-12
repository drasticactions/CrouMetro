using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using CrouMetro.Core.Entity;

namespace Croumetro.Tools
{
    public class PostBorderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var item = value as PostEntity;
            if (item == null) return null;
            if (item.SpreadStatus != null)
            {
                return new SolidColorBrush(Colors.Red);
            }
            return item.InReplyToUserID > 0 ? new SolidColorBrush(Colors.Blue) : null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}