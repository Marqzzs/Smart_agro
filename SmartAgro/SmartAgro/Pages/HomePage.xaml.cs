using CommunityToolkit.Maui.Alerts;
using Newtonsoft.Json;
using SmartAgro.Services;
using SmartAgro.Utils;
using SmartAgro.ViewModels;
using SmartAgroAPI.DataTransferObjects;

namespace SmartAgro.Pages;

public partial class HomePage : ContentPage
{
    public HomeViewModel vm = new HomeViewModel(new Models.ViewModels.ProfilePageViewModel());

    public HomePage()
    {
        InitializeComponent();
        BindingContext = vm;
        sensorView.IsVisible = false;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        vm.UpdateGreeting();
        GetUserData();

    }

    private void LoadCard(System.Collections.ObjectModel.ObservableCollection<ApiRequestSensor> sensorList)
    {
        if (IsAnyPropertyDangerousToday(sensorList.SelectMany(x => x.SensorLogs).ToList()))
        {
            iconlabel.Text = "\x21;";
            iconlabel.BackgroundColor = Colors.White;
            iconlabel.TextColor = Colors.DarkRed;
            desclabel.Text = "Algumas baias estão apresentando problemas que precisam ser investigados.";
            titlelabel.Text = "Aviso";
            container.BackgroundColor = Color.FromHex("#E37E7E");
            desclabel.TextColor = Colors.DarkRed;
        }
    }

    public bool IsAnyPropertyDangerousToday(List<ApiRequestLogsSensor> list)
    {
        var today = DateTime.Now.Date;
        var todayLogs = list.Where(x => x.DataAtualizacao.Value.Date == today).ToList();
        foreach (var log in todayLogs)
        {
            if (IsOutOfRange(log.Luminosidade, 1000, 100000))
            {
                return true;
            }
            if (IsOutOfRange(log.TemperaturaAr, 5, 25))
            {
                return true;
            }
            if (IsOutOfRange(log.TemperaturaSolo, 5, 25))
            {
                return true;
            }
            if (IsOutOfRange(log.UmidadeSolo, 40, 85))
            {
                return true;
            }
            if (IsOutOfRange(log.UmidadeAr, 30, 80))
            {
                return true;
            }
            if (IsOutOfRange(log.PhSolo, 5, 12))
            {
                return true;
            }
            if (IsOutOfRange(log.QualidadeAr, 100, 200))
            {
                return true;
            }
        }
        return false;

    }

    public static bool IsOutOfRange(decimal? value, decimal min, decimal max)
    {
        return value < min || value > max;
    }

    public async void GetUserData()
    {
        try
        {
            //if (await DisplayAlert("Clean user data", "", "Yes", "No") == true)
            //{
            //    await SecureStorage.SetAsync("AllNotifications", "");
            //    await SecureStorage.SetAsync("user", "");
            //    await SecureStorage.SetAsync("LastUpdateDateTime", "");
            //}

            Data.sensorList.Clear();

            var request = await ApiService.Client.GetAsync(ApiRoutes.HomeRequest + Data.loggedUser.Id.ToString());


            var content = await request.Content.ReadAsStringAsync();

            if (!request.IsSuccessStatusCode)
            {
                Toast.Make("Não foi possível atualizar as informações.");
                content = await SecureStorage.GetAsync("Sensors");

            }
            else
            {
                await SecureStorage.SetAsync("Sensors", content);
            }

            var sensors = JsonConvert.DeserializeObject<List<ApiRequestSensor>>(content!);

            foreach (var sensor in sensors!)
            {
                Data.sensorList.Add(sensor);
            }

            sensorView.ItemsSource = Data.sensorList;
            sensorView.IsVisible = true;
            Loading.IsVisible = false;
            LoadCard(Data.sensorList);

        }
        catch (Exception)
        {

            await Toast.Make("Não foi possível recuperar as informações, cheque sua conexão.").Show();
        }

    }

    private void TapGestureRecognizer_Tapped_SensorPage(object sender, TappedEventArgs e)
    {
        var Obj = ((StackLayout)sender).BindingContext as ApiRequestSensor;
        Navigation.PushAsync(new SensorPage(Obj!));
    }

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        Navigation.PushAsync(new NotificationPage());
    }

    private void TapGestureRecognizer_Tapped_ProfilePage(object sender, TappedEventArgs e)
    {
        Navigation.PushAsync(new ProfilePage());
    }

    private void TapGestureRecognizer_edit_or_create(object sender, TappedEventArgs e)
    {
        Navigation.PushAsync(new EditOrCreatePage());
    }
}