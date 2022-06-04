using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace OrganizationBankingSystem.Core.Converters
{
    public class DecreaseConverter : MarkupExtension, IValueConverter
    {
        private static DecreaseConverter _instance;

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Convert.ToInt32(value) - System.Convert.ToInt32(parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion IValueConverter Members

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _instance ??= new DecreaseConverter();
        }
    }
}