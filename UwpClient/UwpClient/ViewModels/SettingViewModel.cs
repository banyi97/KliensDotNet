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

namespace UwpClient.ViewModels
{
    public class SettingViewModel : ReactiveObject
    {
        private readonly NavigationService _navigationService;
        private readonly IRestApiService _apiService;

        public SettingViewModel(NavigationService navigationService, IRestApiService apiService)
        {
            _navigationService = navigationService ?? throw new NullReferenceException();
            _apiService = apiService ?? throw new NullReferenceException();

            var container = App.Container.Resolve<ApplicationDataContainer>();

            LogoutCommand = ReactiveCommand.Create( () =>
            {               
                container.Values["AuthToken"] = null;
                _navigationService.Navigate_App(typeof(Auth));              

            });
        }

        public ReactiveCommand<Unit, Unit> LogoutCommand { get; }
    }
}
