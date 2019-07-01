using Autofac;
using ReactiveUI;
using SharedLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UwpClient.Models.Services;
using UwpClient.Models.Services.Interfaces;
using UwpClient.Views;
using Windows.Storage;
using Windows.UI.Popups;

namespace UwpClient.ViewModels
{

    public class SignInViewModel : ReactiveObject
    {
        private readonly NavigationService _navigationService;
        private readonly IRestApiService _apiService;

        public SignInViewModel(NavigationService navigationService, IRestApiService apiService)
        {
            _navigationService = navigationService ?? throw new NullReferenceException();
            _apiService = apiService ?? throw new NullReferenceException();

            var valid = this.WhenAnyValue(x => x.Email, x => x.Password, (email, password) =>
                    new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$").IsMatch(email) &&
                    new Regex(@"([a-zA-Z0-9]{6,})").IsMatch(password));

            SignInCommand = ReactiveCommand.CreateFromTask(async () => {
                var dto = new SignInDto { Email = this.Email, Password = this.Password };
                try
                {
                    var res = await _apiService.SignIn(dto);
                    if (res.Succeeded == true)
                    {
                        var container = App.Container.Resolve<ApplicationDataContainer>();
                        container.Values["AuthToken"] = "Bearer " + res.AuthToken;
                        _navigationService.Navigate_App(typeof(MainPageView));
                        return;
                    }
                    await new MessageDialog($"Error: {res.Error}").ShowAsync();
                }
                catch (Exception e)
                {
                    await new MessageDialog(e.Message).ShowAsync();
                }             
            }, valid);
        }
        public ReactiveCommand<Unit, Unit> SignInCommand { get; }

        private string _email = String.Empty;
        private string _password = String.Empty;

        public string Email
        {
            get => _email;
            set => this.RaiseAndSetIfChanged(ref _email, value);
        }
        public string Password
        {
            get => _password;
            set => this.RaiseAndSetIfChanged(ref _password, value);
        }

        public void GoBack()
        {
            _navigationService.NavigateBack_App();
        }
    }
}
