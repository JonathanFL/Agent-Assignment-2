using System;
using System.Diagnostics;
using System.Windows.Data;
using System.Windows.Media;

namespace AgentAssignment5
{
    class AgentIdToColorValueConverter : IValueConverter    
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Debug.Assert(targetType == typeof(Brush));
            string id = value as string;
            if (id == null)
                id = "";
            // 007 requires special treatment ...
            return (id == "007" ? Brushes.Blue : Brushes.Black);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
