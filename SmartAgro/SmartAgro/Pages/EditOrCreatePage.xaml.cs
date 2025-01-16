using Microsoft.Maui.Devices.Sensors;
using Newtonsoft.Json;
using SmartAgro.DataTransferObjects;
using SmartAgro.Models.RequestModels;
using SmartAgro.Services;
using SmartAgro.Utils;
using SmartAgro.ViewModels;
using System.Collections.ObjectModel;
using System.Globalization;

namespace SmartAgro.Pages;

public partial class EditOrCreatePage : ContentPage
{

    public EditOrCreateViewModel vm = new EditOrCreateViewModel();
    private CancellationTokenSource _cancelTokenSource;
    private bool _isCheckingLocation;
    private string apiKey = "d98a3ffaf90646da977124924242911";
    static readonly HttpClient client = new HttpClient();
    private Location location = new Location();
    public DateTime? DataSelecionada { get; set; }

    private ApiRequestEditSensor _sensor;

    private bool _isUpdating;

    public EditOrCreatePage(ApiRequestEditSensor? sensor = null)
	{
		InitializeComponent();

        BindingContext = vm;

        _sensor = sensor;

        _isUpdating = sensor != null;

        if (_isUpdating)
        {
            LoadExistingData();

            SensorButton.Text = "Editar";
            LabelSensor.Text = $"Editando - {_sensor.Nome}";
        }

        DataSelecionada = DateTime.Now;
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        await GetCurrentLocation();
    }


    public async Task GetCurrentLocation()
    {
        try
        {
            _isCheckingLocation = true;

            // Configurando a solicitação de localização
            GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));

            _cancelTokenSource = new CancellationTokenSource();

            // Obtendo a localização atual
            location = await Geolocation.Default.GetLocationAsync(request, _cancelTokenSource.Token);

            if (location != null)
            {
                Console.WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}");

                //Garantindo que as coordenadas usem pontos decimais, por algum motivo nao estava indo sem 😎
                string latitude = location.Latitude.ToString(CultureInfo.InvariantCulture);
                string longitude = location.Longitude.ToString(CultureInfo.InvariantCulture);

                string url = $"https://api.weatherapi.com/v1/current.json?q={latitude},{longitude}&key={apiKey}";

                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();

                ApiRequestWeather weatherData = JsonConvert.DeserializeObject<ApiRequestWeather>(responseBody);

                inputLocation.Text = $"{weatherData.Location.Name}, {weatherData.Location.Region}";
            }
            else
            {
                Console.WriteLine("Não foi possível obter a localização.");
            }
        }
        catch (FeatureNotSupportedException)
        {
            Console.WriteLine("Recurso de geolocalização não suportado neste dispositivo.");
        }
        catch (FeatureNotEnabledException)
        {
            Console.WriteLine("Recurso de geolocalização está desativado. Ative-o nas configurações.");
        }
        catch (PermissionException)
        {
            Console.WriteLine("Permissão de localização negada.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro inesperado: {ex.Message}");
        }
        finally
        {
            _isCheckingLocation = false;
        }
    }


    public void CancelRequest()
    {
        if (_isCheckingLocation && _cancelTokenSource != null && _cancelTokenSource.IsCancellationRequested == false)
            _cancelTokenSource.Cancel();
    }


    private async void Button_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(inputName.Text) ||

            string.IsNullOrWhiteSpace(inputUmidadeAr.Text) ||
            string.IsNullOrWhiteSpace(inputUmidadeSolo.Text) ||
            string.IsNullOrWhiteSpace(inputTemperaturaAr.Text) ||
            string.IsNullOrWhiteSpace(inputTemperaturaSolo.Text) ||
            string.IsNullOrWhiteSpace(inputPh.Text) ||
            string.IsNullOrWhiteSpace(inputLuminosidade.Text))
        {
            await DisplayAlert("Erro", "Por favor, preencha todos os campos obrigatórios.", "OK");
            return;
        }

        try
        {
            Categoria selectedCategoria = (Categoria)categorias.SelectedItem;
            int categoryId = selectedCategoria.ID;

            var body = new ApiRequestEditSensor
            {
                Nome = inputName.Text,
                CategoriaId = categoryId,
                DataColheita = DateOnly.FromDateTime(datePicker.SelectedDate.Value),
                Latitude = (decimal?)location.Latitude,
                Longitude = (decimal?)location.Longitude,
                UmidadeArIdeal = decimal.TryParse(inputUmidadeAr.Text, out var umidadeAr) ? umidadeAr / 100 : null,
                UmidadeSoloIdeal = decimal.TryParse(inputUmidadeSolo.Text, out var umidadeSolo) ? umidadeSolo / 100 : null,
                TemperaturaArIdeal = decimal.TryParse(inputTemperaturaAr.Text, out var temperaturaAr) ? temperaturaAr / 100: null,
                TemperaturaSoloIdeal = decimal.TryParse(inputTemperaturaSolo.Text, out var temperaturaSolo) ? temperaturaSolo / 100 : null,
                PhSoloIdeal = decimal.TryParse(inputPh.Text, out var ph) ? ph / 100 : null,
                LuminosidadeIdeal = decimal.TryParse(inputLuminosidade.Text, out var luminosidade) ? luminosidade / 100 : null,
                UsuarioId = Data.loggedUser.Id
            };

            if(_sensor != null)
            {
                var response = await ApiService.Client.PatchAsync(ApiRoutes.Sensor + _sensor.Id, ApiService.ToBodyContent(body));

                if (!response.IsSuccessStatusCode)
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    await DisplayAlert("Erro", "Ocorreu um erro ao editar baia, tente mais tarde", "OK");
                    Console.WriteLine($"Erro ao editar baia: {errorContent}");
                }else
                {
                    await DisplayAlert("Sucesso", $"Baia editada com sucesso", "OK");
                    await Navigation.PopAsync();
                }
            }
            else
            {
                var response = await ApiService.Client.PostAsync(ApiRoutes.SensorPost, ApiService.ToBodyContent(body));

                if (!response.IsSuccessStatusCode)
                {
                    await DisplayAlert("Erro", "Ocorreu um erro ao editar baia, tente mais tarde", "OK");
                }

                await DisplayAlert("Sucesso", $"Baia cadastrada com sucesso", "OK");
                
                await Navigation.PopAsync();
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Ocorreu um erro: {ex.Message}", "OK");
        }
    }


    private void TapGestureRecognizer_Tapped_1(object sender, TappedEventArgs e)
    {
        App.Current!.MainPage = new NavigationPage(new HomePage());
    }

    private void LoadExistingData()
    {
        if (_sensor == null) return;

        categorias.SelectedItem = vm.Categorias.FirstOrDefault(c => c.ID == _sensor.CategoriaId);

        inputName.Text = _sensor.Nome;
        datePicker.SelectedDate = new DateTime(_sensor.DataColheita.Value.Year, _sensor.DataColheita.Value.Month, _sensor.DataColheita.Value.Day);

        if (_sensor.Latitude.HasValue && _sensor.Longitude.HasValue)
        {
            GetCurrentLocation();
        }

        inputUmidadeAr.Text = _sensor.UmidadeArIdeal?.ToString(CultureInfo.InvariantCulture);
        inputUmidadeSolo.Text = _sensor.UmidadeSoloIdeal?.ToString(CultureInfo.InvariantCulture);
        inputTemperaturaAr.Text = _sensor.TemperaturaArIdeal?.ToString(CultureInfo.InvariantCulture);
        inputTemperaturaSolo.Text = _sensor.TemperaturaSoloIdeal?.ToString(CultureInfo.InvariantCulture);
        inputPh.Text = _sensor.PhSoloIdeal?.ToString(CultureInfo.InvariantCulture);
        inputLuminosidade.Text = _sensor.LuminosidadeIdeal?.ToString(CultureInfo.InvariantCulture);
    }
}