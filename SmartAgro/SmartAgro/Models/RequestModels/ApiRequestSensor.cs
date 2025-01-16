using SmartAgroAPI.Models;

namespace SmartAgroAPI.DataTransferObjects
{
    public class ApiRequestSensor
    {

        public ApiRequestSensor()
        {

        }
        
        public int SensorId { get; set; }
        public int CategoryId { get; set; }

        public Guid UsuarioId { get; set; }
        public string? SensorName { get; set; }

        public DateTime? DataColheita { get; set; }

        public decimal? Latitude { get; set; }

        public decimal? Longitude { get; set; }

        public decimal? UmidadeSoloIdeal { get; set; }

        public decimal? TemperaturaArIdeal { get; set; }

        public decimal? UmidadeArIdeal { get; set; }

        public decimal? LuminosidadeIdeal { get; set; }

        public decimal? TemperaturaSoloIdeal { get; set; }

        public decimal? PhSoloIdeal { get; set; }

        public virtual List<ApiRequestLogsSensor>? SensorLogs { get; set; } = null;


        public ApiRequestLogsSensor LatestLog
        {
            get
            {
                if (SensorLogs == null) return null!;
                return SensorLogs?.OrderByDescending(x => x.DataAtualizacao).First()! ?? new ApiRequestLogsSensor();
            }
        }

        public virtual Categoria? SensorCategory { get; set; }

    }
}
