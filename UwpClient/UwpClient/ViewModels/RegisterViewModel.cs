using Autofac;
using ReactiveUI;
using SharedLibrary.Dtos;
using SharedLibrary.Models;
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
    public class RegisterViewModel : ReactiveObject
    {
        private readonly NavigationService _navigationService;
        private readonly IRestApiService _apiService;

        public RegisterViewModel(NavigationService navigationService, IRestApiService apiService)
        {
            _navigationService = navigationService ?? throw new NullReferenceException();
            _apiService = apiService ?? throw new NullReferenceException();

            var valid = this.WhenAnyValue(
                x => x.Email, x => x.Password, x => x.Name, x => x.DateOfBirth, x => x.Gender, 
                (email, password, name, date, gender) =>
                    new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$").IsMatch(email) &&
                    new Regex(@"([a-zA-Z0-9]{6,})").IsMatch(password) &&
                    new Regex(@"([a-z]{1,})").IsMatch(password) &&
                    new Regex(@"([A-Z]{1,})").IsMatch(password) &&
                    new Regex(@"([0-9]{1,})").IsMatch(password) &&
                    new Regex(@"([a-zA-Z]{2,})").IsMatch(name) &&
                    gender is Gender
           );
            RegisterCommand = ReactiveCommand.CreateFromTask(async () => {
                var gender = this.Gender == SharedLibrary.Models.Gender.Male ? SharedLibrary.Models.Gender.Male : SharedLibrary.Models.Gender.Female;
                var dto = new RegisterDto { Email = this.Email, Name = this.Name, Password = this.Password, Gender = gender, DateOfBirth = this.DateOfBirth.DateTime };
                try
                {
                    var res = await _apiService.Register(dto);
                    if (res.Succeeded == true)
                    {
                        var container = App.Container.Resolve<ApplicationDataContainer>();
                        container.Values["AuthToken"] = "Bearer " + res.AuthToken;
                        _navigationService.Navigate_App(typeof(MainPageView));
                        return;
                    }
                    await new MessageDialog($"Error: {res.Error}").ShowAsync();
                }
                catch(Exception e)
                {
                    await new MessageDialog($"Error: {e.Message}").ShowAsync();
                }
            }, valid);
        }
       public ReactiveCommand<Unit, Unit> RegisterCommand { get; }

        private string _email = String.Empty;
        private string _password = String.Empty;
        private string _name = String.Empty;
        private DateTimeOffset _dateOfBirth = new DateTimeOffset(DateTime.Now.AddYears(-18).AddDays(-1));
        private Gender? _gender = null;        

       public DateTimeOffset MaxYear { get => new DateTimeOffset(DateTime.Now.AddYears(-18)); }

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
       public string Name
       {
           get => _name;
           set => this.RaiseAndSetIfChanged(ref _name, value);
       }
       public DateTimeOffset DateOfBirth
       {
           get => _dateOfBirth;
           set => this.RaiseAndSetIfChanged(ref _dateOfBirth, value);
       }
       public Gender? Gender
       {
            get => _gender;
            set => this.RaiseAndSetIfChanged(ref _gender, value);
       }

       public void GoBack()
       {
           _navigationService.NavigateBack_App();
       }      
    }
}
