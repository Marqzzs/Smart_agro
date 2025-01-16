using SmartAgro.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAgro.Converters
{
    public class IconConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value == null) return "black";

            var info = value as SensorInfo;
            var nivelIdeal = info.NivelIdeal + 1;
            var nivelAtual = info.NivelAtual + 1;

            var valor = Math.Abs((decimal)nivelIdeal - (decimal)nivelAtual);

            var result = valor / nivelIdeal * 100;

            switch (result)
            {
                case >= 50: return "#C42B2B";
                case >= 40: return "#FF9E4A";
                case >= 30: return "#F5C242";
                case >= 20: return "#AFE899";
                case >= 10: return "#3E8E4A";
                default: return "black";
            }
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
