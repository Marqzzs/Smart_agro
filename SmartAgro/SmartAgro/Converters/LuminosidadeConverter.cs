using System.Globalization;

namespace SmartAgro.Converters
{
    public class LuminosidadeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal luminosidade)
            {
                return $" {luminosidade / 1000:F0} klux"; // Converte para milhares e adiciona o sufixo
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
