using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UwpClient.Models.Services;
using UwpClient.Models.Services.Interfaces;
using UwpClient.Views;
using Windows.UI.Xaml.Controls;

namespace UwpClient.ViewModels
{
    public class MainPageViewModel : ReactiveObject
    {
        private readonly NavigationService _navigationService;
        private readonly IRestApiService _apiService;
        public MainPageViewModel(NavigationService navigationService, IRestApiService apiService)
        {
            _navigationService = navigationService ?? throw new NullReferenceException();
            _apiService = apiService ?? throw new NullReferenceException();
        }

        public void SetFrame(Frame frame)
        {
            _navigationService.Frame = frame;
        }

        public void nvTopLevelNav_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                _navigationService.Navigate_Main(typeof(SettingView));
                return;
            }
            else
            {
                var ItemContent = args.InvokedItem as TextBlock;
                if (ItemContent != null)
                {
                    switch (ItemContent.Tag.ToString())
                    {
                        case "Nav_Match":
                            _navigationService.Navigate_Main(typeof(MatchView));
                            break;

                        case "Nav_Me":
                            _navigationService.Navigate_Main(typeof(Profile));
                            break;

                        case "Nav_Message":
                            _navigationService.Navigate_Main(typeof(Message));
                            break;
                    }
                }
            }
        }
    }
}
