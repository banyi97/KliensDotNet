using Autofac;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using UwpClient.Models.Services;
using UwpClient.Models.Services.Interfaces;
using UwpClient.Views;
using Windows.Globalization;
using Windows.Storage;
using Windows.UI.Popups;

namespace UwpClient.ViewModels
{
    public class AuthViewModel : ReactiveObject
    {
        private readonly NavigationService _navigationService;
        private readonly IRestApiService _apiService;
        private FbAuthService _fbAuthService;
        public AuthViewModel(NavigationService navigationService,IRestApiService apiService, FbAuthService authService)
        {
            _navigationService = navigationService ?? throw new NullReferenceException();
            _apiService = apiService ?? throw new NullReferenceException();
            _fbAuthService = authService ?? throw new NullReferenceException();

            var container = App.Container.Resolve<ApplicationDataContainer>();
            SelectedLang = container.Values["appLang"] as string;
            SignInWithFacebookCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var res = await _fbAuthService.SignInWithFacebook();
                if (res == true)
                {
                    try
                    {
                        var aut = await _apiService.SignInWithFacebook(_fbAuthService.AccessToken);
                        if(aut.Succeeded == true)
                        {
                            container.Values["AuthToken"] = "Bearer " + aut.AuthToken;
                            _navigationService.Navigate_App(typeof(MainPageView));
                            return;
                        }
                        await new MessageDialog(aut.Error).ShowAsync();
                    }
                    catch(Exception e)
                    {
                        await new MessageDialog(e.Message).ShowAsync();
                    }
                }
                await new MessageDialog("Error").ShowAsync();
            });
        }
        public ReactiveCommand<Unit, Unit> SignInWithFacebookCommand { get; }
        bool init = false;
        private string _selectedLang;
        public string SelectedLang { get => _selectedLang; set => SetLang(value); }
        
        private async void SetLang(string lang)
        {
            if(init == false)
            {
                this.RaiseAndSetIfChanged(ref _selectedLang, lang);
                init = true;
                return;
            }
            //https://stackoverflow.com/questions/32715690/c-sharp-change-app-language-programmatically-uwp-realtime
            var container = App.Container.Resolve<ApplicationDataContainer>();
            container.Values["appLang"] = lang;
            ApplicationLanguages.PrimaryLanguageOverride = lang;
            Windows.ApplicationModel.Resources.Core.ResourceContext.GetForCurrentView().Reset();
            Windows.ApplicationModel.Resources.Core.ResourceContext.GetForViewIndependentUse().Reset();
            await Task.Delay(100);
            _navigationService.Navigate_App(typeof(Auth));
        }

        public void GoToSignInPage()
        {
            _navigationService.Navigate_App(typeof(SignInView));
        }

        public void GoToRegisterPage()
        {
            _navigationService.Navigate_App(typeof(RegisterView));
        }
    }
}
