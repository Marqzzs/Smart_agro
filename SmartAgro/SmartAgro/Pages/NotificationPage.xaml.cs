using CommunityToolkit.Maui.Alerts;
using Newtonsoft.Json;
using SmartAgro.Services;
using SmartAgro.Utils;
using SmartAgro.ViewModels;
using SmartAgroAPI.Models;
using SmartAgroAPI.ViewModels;

namespace SmartAgro.Pages;

public partial class NotificationPage : ContentPage
{

    private const string DefaultLastUpdateDate = "1961-01-01T00:00:00Z";

    public NotificationPage()
    {
        InitializeComponent();
        BindingContext = new NotificationViewModel();
        notificationView.IsVisible = false;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _ = LoadNotificationsAsync(); // Fire-and-forget pattern with explicit task handling
    }

    private async Task<List<Notificacao>> FetchNotificationsAsync()
    {
        try
        {
            var lastUpdateStr = await SecureStorage.GetAsync("LastUpdateDateTime");
            var lastUpdate = !string.IsNullOrWhiteSpace(lastUpdateStr)
                ? DateTime.Parse(lastUpdateStr)
                : DateTime.Parse(DefaultLastUpdateDate);

            var response = await ApiService.Client.GetAsync($"Notification/fetch/{Data.loggedUser.Id}?lastUpdate={lastUpdate:O}");
            var newNotifications = response.IsSuccessStatusCode
                ? await ApiService.DesserializeList<Notificacao>(response)
                : new List<Notificacao>();

            var storedNotificationsJson = await SecureStorage.GetAsync("AllNotifications");
            var storedNotifications = string.IsNullOrEmpty(storedNotificationsJson)
                ? new List<Notificacao>()
                : JsonConvert.DeserializeObject<List<Notificacao>>(storedNotificationsJson);

            var allNotifications = storedNotifications.Concat(newNotifications)
                                                      .DistinctBy(n => n.Id)
                                                      .ToList();

            await SecureStorage.SetAsync("AllNotifications", JsonConvert.SerializeObject(allNotifications));
            await SecureStorage.SetAsync("LastUpdateDateTime", DateTime.UtcNow.ToString("O"));

            return allNotifications;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching notifications: {ex}");
            await Toast.Make("Não foi possível recuperar as informações, cheque sua conexão.").Show();
            return new List<Notificacao>();
        }
    }

    private async Task LoadNotificationsAsync()
    {
        var allNotifications = await FetchNotificationsAsync();

        foreach (var notification in allNotifications)
        {
            notification.nome = MapPropertyNameToDisplayName(notification.Propriedade);

            if (notification.LogsSensor is not null)
            {
                notification.atual = GetDecimalValue(notification.LogsSensor, notification.Propriedade);
                notification.ideal = GetDecimalValue(notification.LogsSensor.Sensor, notification.Propriedade + "Ideal");
            }

            Data.notificationList.Add(notification);
        }

        notificationView.ItemsSource = GroupNotificationsByDate(Data.notificationList.ToList());
        notificationView.IsVisible = true;
        Loading.IsVisible = false;
    }

    private static decimal GetDecimalValue(object obj, string propertyName)
    {
        return obj?.GetType().GetProperty(propertyName)?.GetValue(obj) is decimal value ? value : 0;
    }

    private static string MapPropertyNameToDisplayName(string? propertyName) => propertyName switch
    {
        "TemperaturaAr" => "Temperatura do Ar",
        "TemperaturaSolo" => "Temperatura do Solo",
        "UmidadeAr" => "Umidade do Ar",
        "UmidadeSolo" => "Umidade do Solo",
        "QualidadeAr" => "Qualidade do Ar",
        "PhSolo" => "Ph do Solo",
        "Luminosidade" => "Luminosidade",
        _ => string.Empty
    };

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        if (sender is StackLayout layout && layout.BindingContext is Notificacao notification)
        {
            var sensor = Data.sensorList.FirstOrDefault(x => x.SensorId == notification.LogsSensor?.SensorId);
            if (sensor != null)
            {
                await Navigation.PushAsync(new SensorPage(sensor));
            }
        }
    }

    private static List<NotificationGroupViewModel> GroupNotificationsByDate(IEnumerable<Notificacao> notifications)
    {
        var today = DateTime.UtcNow.Date;
        var oneWeekAgo = today.AddDays(-7);
        var oneMonthAgo = today.AddMonths(-1);

        return new List<NotificationGroupViewModel>
        {
            new("Hoje", notifications.Where(n => (n.DataCriacao!.Value).Date == today).ToList()),
            new("Semana Passada", notifications.Where(n => (n.DataCriacao!.Value) >= oneWeekAgo && (n.DataCriacao.Value) < today).ToList()),
            new("Mês Passado", notifications.Where(n => (n.DataCriacao!.Value) >= oneMonthAgo && (n.DataCriacao.Value) < oneWeekAgo).ToList()),
            new("Há Muito Tempo", notifications.Where(n => (n.DataCriacao!.Value) < oneMonthAgo).ToList())
        };
    }

    private void TapGestureRecognizer_Tapped_1(object sender, TappedEventArgs e)
    {
        Navigation.PopAsync();
    }

}