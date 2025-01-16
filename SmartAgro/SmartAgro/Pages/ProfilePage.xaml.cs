using CommunityToolkit.Maui.Alerts;
using SmartAgro.Models.RequestModels;
using SmartAgro.Models.ViewModels;
using SmartAgro.Services;
using SmartAgro.Utils;

namespace SmartAgro.Pages;

public partial class ProfilePage : ContentPage
{
    public ProfilePageViewModel vm = new ProfilePageViewModel();
    public ProfilePage()
    {
        InitializeComponent();

        BindingContext = vm;
    }

    private async void EditProfile(object sender, EventArgs e)
    {

        try
        {
            if (string.IsNullOrWhiteSpace(inputNome.Text) || string.IsNullOrWhiteSpace(inputEmail.Text) || string.IsNullOrWhiteSpace(inputTelefone.Text))
            {
                return;
            }

            var body = new ApiRequestUserEdit() { Nome = inputNome.Text, Email = inputEmail.Text, Telefone = inputTelefone.Text };

            var request = await ApiService.Client.PatchAsync(ApiRoutes.User + Data.loggedUser.Id.ToString(), ApiService.ToBodyContent(body));



            if (!request.IsSuccessStatusCode)
            {
                await DisplayAlert("Erro", $"Erro ao editar o perfil", "OK");
                return;
            }

            await DisplayAlert("Sucesso", "Perfil atualizado com sucesso!", "OK");

            RefreshUser();
        }
        catch (Exception)
        {

            await Toast.Make("Não foi possível recuperar as informações, cheque sua conexão.").Show();
        }

    }

    public void RefreshUser()
    {
        Data.loggedUser.Name = inputNome.Text;
        Data.loggedUser.Email = inputEmail.Text;
        Data.loggedUser.Phone = inputTelefone.Text;
    }

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        Navigation.PopAsync();
    }

    private async void TapGestureRecognizer_Tapped_1(object sender, TappedEventArgs e)
    {
        bool confirm = await DisplayAlert("Sair", "Deseja realmente sair do aplicativo?", "Sim", "Não");

        await SecureStorage.SetAsync("user", "");
        Data.loggedUser = null;
        App.Current.MainPage = new LoginPage();

    }
}
