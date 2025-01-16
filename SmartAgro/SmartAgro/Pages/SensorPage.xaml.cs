using Newtonsoft.Json;
using SmartAgro.DataTransferObjects;
using SmartAgro.Models;
using SmartAgro.Models.RequestModels;
using SmartAgro.Utils;
using SmartAgro.ViewModels;
using SmartAgroAPI.DataTransferObjects;
using Syncfusion.Maui.Picker;
using System.Collections.ObjectModel;
using System.Globalization;

namespace SmartAgro.Pages;

public partial class SensorPage : ContentPage
{
    private ApiRequestSensor _sensor;
    static readonly HttpClient client = new HttpClient();
    private string apiKey = "d98a3ffaf90646da977124924242911";

    public SensorPage(ApiRequestSensor sensor)
    {

        InitializeComponent();
        BindingContext = this;
        var date = DateTime.Now;
        LabelDay.Text = $"{date.ToString("dddd")}, {date.ToString("dd")} de {date.ToString("MMMM")}" ;

        _sensor = sensor;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadData();
    }

    private void LoadData()
    {
        LoadDates();
        LoadInfo();
        LoadChart();
        Weather();
    }



    public ObservableCollection<DateTime> dateTimes { get; set; } = new ObservableCollection<DateTime>();
    private void LoadDates()
    {
        dateTimes.Clear();
        datesView.ItemsSource = null;

        var dates = _sensor.SensorLogs.GroupBy(x => x.DataAtualizacao.Value.Date).Select(x => x.Key.Date).ToList();
        foreach (var date in dates.OrderByDescending(x => x))
        {
            dateTimes.Add(date);

        }
        datesView.ItemsSource = dateTimes;
    }

    private void LoadInfo()
    {
        dataColheita.Text = "";
        nomeBaia.Text = _sensor.SensorName;

        int estimado = (_sensor.DataColheita.Value - DateTime.Now).Days;

        dataColheita.Text = estimado.ToString() + " Dias";

        var vm = new SensorViewModel(_sensor);

        infoView.ItemsSource = vm.Sensors;
    }

    private void TapGestureRecognizer_Tapped_Select(object sender, TappedEventArgs e)
    {
        var binding = ((Frame)sender).BindingContext;

        if ((binding as DateTime?).HasValue)
        {
            Data.SelectedDate = (binding as DateTime?)!.Value;
        }

        LoadDates();
        LoadInfo();
    }
    public ObservableCollection<DataPoint> dataPoints { get; set; } = new ObservableCollection<DataPoint>();
    private void LoadChart()
    {

        chartXml.BindingContext = this;
        series.BindingContext = this;
        series.ItemsSource = dataPoints;
    }

    private void TapGestureRecognizer_Tapped_back(object sender, TappedEventArgs e)
    {
        Navigation.PopAsync();
    }

    private void TapGestureRecognizer_Tapped_chart(object sender, TappedEventArgs e)
    {
        var binding = ((StackLayout)sender).BindingContext as SensorInfo;

        if (binding != null)
        {
            dataPoints.Clear();
            chartTitle.Text = binding.Nome + " durante o dia";
            foreach (var item in binding.Logs!.Where(x => x.Date.Date == Data.SelectedDate.Date).OrderBy(x => x.Date).ToList())
            {
                dataPoints.Add(new DataPoint() { Value = item.Value, Date = item.Date });
            }
        }
    }

    private async void Weather()
    {
        string latitude = ((double)_sensor.Latitude).ToString(CultureInfo.InvariantCulture);
        string longitude = ((double)_sensor.Longitude).ToString(CultureInfo.InvariantCulture);

        string url = $"https://api.weatherapi.com/v1/current.json?q={latitude},{longitude}&key={apiKey}";

        HttpResponseMessage response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();

        string responseBody = await response.Content.ReadAsStringAsync();

        ApiRequestWeather weatherData = JsonConvert.DeserializeObject<ApiRequestWeather>(responseBody);

        Condition.Text = $"{weatherData.Current.Condition.Text}";
        TemperatureWeather.Text = $"{weatherData.Current.TempC:F1}ºC";
        img.Source = ImageSource.FromUri(new Uri("https:" + weatherData.Current.Condition.Icon)) ;
        //IconWeather.Text = $"{weatherData.Current.Condition.Icon}";
    }

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        
    }

    private void TapGestureRecognizer_Tapped_1(object sender, TappedEventArgs e)
    {
        Navigation.PushAsync(new EditOrCreatePage( new ApiRequestEditSensor(_sensor)));
    }
}