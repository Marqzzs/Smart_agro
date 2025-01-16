using SmartAgroAPI.DataTransferObjects;

namespace SmartAgro.DataTransferObjects
{
    public class ApiRequestEditSensor
    {
        public ApiRequestEditSensor()
        {
            
        }

        public ApiRequestEditSensor(ApiRequestSensor sensor)
        {
            Id = sensor.SensorId;
            Nome = sensor.SensorName;
            DataColheita =DateOnly.FromDateTime( sensor.DataColheita.Value);
            Latitude = sensor.Latitude;
            Longitude = sensor.Longitude;
            CategoriaId = sensor.CategoryId;
            UsuarioId = sensor.UsuarioId;
            UmidadeSoloIdeal = sensor.UmidadeSoloIdeal;
            TemperaturaArIdeal = sensor.TemperaturaArIdeal;
            UmidadeArIdeal = sensor.UmidadeArIdeal;
            LuminosidadeIdeal = sensor.LuminosidadeIdeal;
            TemperaturaSoloIdeal = sensor.TemperaturaSoloIdeal;
            PhSoloIdeal = sensor.PhSoloIdeal;
        }
        public int Id { get; set; }

        public string? Nome { get; set; }

        public DateOnly? DataColheita { get; set; }

        public decimal? Latitude { get; set; }

        public decimal? Longitude { get; set; }

        public int CategoriaId { get; set; }
        public Guid UsuarioId { get; set; }

        public decimal? UmidadeSoloIdeal { get; set; }

        public decimal? TemperaturaArIdeal { get; set; }

        public decimal? UmidadeArIdeal { get; set; }

        public decimal? LuminosidadeIdeal { get; set; }

        public decimal? TemperaturaSoloIdeal { get; set; }

        public decimal? PhSoloIdeal { get; set; }
    }
}
