using SmartAgro.Models;
using SmartAgro.Utils;
using SmartAgroAPI.DataTransferObjects;
using System.Collections.ObjectModel;

namespace SmartAgro.ViewModels
{
    public partial class SensorViewModel
    {
        public SensorViewModel()
        {

        }
        public ObservableCollection<SensorInfo> Sensors { get; set; } = new ObservableCollection<SensorInfo>();

        public SensorViewModel(ApiRequestSensor requestSensor)
        {

            var todayLogs = requestSensor.SensorLogs!.Where(x => x.DataAtualizacao!.Value.Date == Data.SelectedDate.Date).ToList();

            SensorInfo UmidadeArIdeal = new SensorInfo();
            UmidadeArIdeal.Nome = "Umidade do ar";
            UmidadeArIdeal.NivelAtual = todayLogs.Count() > 0 ? todayLogs.Average(x => x.UmidadeAr) : 0;
            UmidadeArIdeal.NivelIdeal = (decimal)requestSensor.UmidadeArIdeal!;
            UmidadeArIdeal.Sufixo = "%";
            UmidadeArIdeal.Icon = "\xf043;";
            UmidadeArIdeal.Color = "#8BC9EE";
            UmidadeArIdeal.Logs = todayLogs.Select(x => new ChartModel((decimal)x.UmidadeAr!, x.DataAtualizacao!.Value)).ToList();

            SensorInfo UmidadeSoloIdeal = new SensorInfo();
            UmidadeSoloIdeal.Nome = "Umidade do solo";
            UmidadeSoloIdeal.NivelAtual = todayLogs.Count() > 0 ? todayLogs.Average(x => x.UmidadeSolo) : 0;
            UmidadeSoloIdeal.NivelIdeal = (decimal)requestSensor.UmidadeSoloIdeal!;
            UmidadeSoloIdeal.Icon = "\xf043";
            UmidadeSoloIdeal.Sufixo = "%";
            UmidadeSoloIdeal.Color = "#8BC9EE";
            UmidadeSoloIdeal.Logs = todayLogs.Select(x => new ChartModel((decimal)x.UmidadeSolo!, x.DataAtualizacao!.Value)).ToList();

            SensorInfo TemperaturaArIdeal = new SensorInfo();
            TemperaturaArIdeal.Nome = "Temperatura do ar";
            TemperaturaArIdeal.NivelAtual = todayLogs.Count() > 0 ? todayLogs.Average(x => x.TemperaturaAr) : 0;
            TemperaturaArIdeal.NivelIdeal = (decimal)requestSensor.TemperaturaArIdeal!;
            TemperaturaArIdeal.Icon = "\xf72e;";
            TemperaturaArIdeal.Sufixo = "°C";
            TemperaturaArIdeal.Color = "#EC7474";
            TemperaturaArIdeal.Logs = todayLogs.Select(x => new ChartModel((decimal)x.TemperaturaAr!, x.DataAtualizacao!.Value)).ToList();

            SensorInfo TemperaturaSoloIdeal = new SensorInfo();
            TemperaturaSoloIdeal.Nome = "Temperatura do solo";
            TemperaturaSoloIdeal.NivelAtual = todayLogs.Count() > 0 ? todayLogs.Average(x => x.TemperaturaSolo) : 0;
            TemperaturaSoloIdeal.NivelIdeal = (decimal)requestSensor.TemperaturaSoloIdeal!;
            TemperaturaSoloIdeal.Icon = "\xf2c9;";
            TemperaturaSoloIdeal.Sufixo = "°C";
            TemperaturaSoloIdeal.Color = "#EC7474";
            TemperaturaSoloIdeal.Logs = todayLogs.Select(x => new ChartModel((decimal)x.TemperaturaSolo!, x.DataAtualizacao!.Value)).ToList();

            SensorInfo PhSoloIdeal = new SensorInfo();
            PhSoloIdeal.Nome = "Ph do solo";
            PhSoloIdeal.NivelAtual = todayLogs.Count() > 0 ? todayLogs.Average(x => x.PhSolo) : 0;
            PhSoloIdeal.NivelIdeal = (decimal)requestSensor.PhSoloIdeal!;
            PhSoloIdeal.Icon = "pH";
            PhSoloIdeal.Color = "#C4A02B";
            PhSoloIdeal.Logs = todayLogs.Select(x => new ChartModel((decimal)x.PhSolo!, x.DataAtualizacao!.Value)).ToList();

            SensorInfo QualidadeAr = new SensorInfo();
            QualidadeAr.Nome = "Qualidade do ar";
            QualidadeAr.NivelAtual = todayLogs.Count() > 0 ? todayLogs.Average(x => x.QualidadeAr) : 0;
            QualidadeAr.NivelIdeal = 200;
            QualidadeAr.Icon = "\x2600;";
            QualidadeAr.Sufixo = " IQA";
            QualidadeAr.Color = "#8BC9EE";
            QualidadeAr.Logs = todayLogs.Select(x => new ChartModel((decimal)x.QualidadeAr!, x.DataAtualizacao!.Value)).ToList();

            SensorInfo LuminosidadeIdeal = new SensorInfo();
            LuminosidadeIdeal.Nome = "Luminosidade";
            LuminosidadeIdeal.NivelAtual = todayLogs.Count() > 0 ? todayLogs.Average(x => x.Luminosidade) : 0;
            LuminosidadeIdeal.NivelIdeal = (decimal)requestSensor.LuminosidadeIdeal!;
            LuminosidadeIdeal.Icon = "\x2600;";
            LuminosidadeIdeal.Sufixo = " lux";
            LuminosidadeIdeal.Color = "#ECA674";
            LuminosidadeIdeal.Logs = todayLogs.Select(x => new ChartModel((decimal)x.Luminosidade!, x.DataAtualizacao!.Value)).ToList();

            Sensors.Add(UmidadeArIdeal);
            Sensors.Add(UmidadeSoloIdeal);
            Sensors.Add(TemperaturaArIdeal);
            Sensors.Add(TemperaturaSoloIdeal);
            Sensors.Add(PhSoloIdeal);
            Sensors.Add(QualidadeAr);
            Sensors.Add(LuminosidadeIdeal);

        }
    }
}
