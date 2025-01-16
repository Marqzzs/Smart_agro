using Newtonsoft.Json;
using SmartAgro.Pages;
using SmartAgro.Services;
using SmartAgro.Utils;
using SmartAgroAPI.DataTransferObjects;

namespace SmartAgro
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            Syncfusion.Licensing.SyncfusionLicenseProvider
                .RegisterLicense(SECRETS.SYNCFUSION_KEY);

            MainPage = new LoadingPage();
            InitializeSession();

        }

        private async void InitializeSession()
        {
            MainPage = await GetSession();
        }

        private async Task<Page> GetSession()
        {
            var userData = await SecureStorage.GetAsync("user");
            if (string.IsNullOrWhiteSpace(userData))
            {
                return new LoginPage();

            }

            var user = JsonConvert.DeserializeObject<ApiRequestToken>(userData);

            var tokenExpiration = JWTService.ReadToken(user.Token).ValidTo;
            if (tokenExpiration < DateTime.Now)
            {
                await SecureStorage.SetAsync("user", "");
                return new LoginPage();
            }

            Data.loggedUser = user.user;
            return new NavigationPage(new HomePage());

        }
    }
}
