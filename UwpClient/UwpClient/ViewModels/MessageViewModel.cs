using Autofac;
using DynamicData;
using ReactiveUI;
using Refit;
using SharedLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using UwpClient.Models.Services;
using UwpClient.Models.Services.Interfaces;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

namespace UwpClient.ViewModels
{
    public class MessageViewModel : ReactiveObject
    {
        private readonly NavigationService _navigationService;
        private readonly IRestApiService _apiService;
        public MessageViewModel(NavigationService navigationService, IRestApiService apiService)
        {
            _navigationService = navigationService ?? throw new NullReferenceException();
            _apiService = apiService ?? throw new NullReferenceException();

            SendMessageCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                if (String.IsNullOrWhiteSpace(MyMessage))
                    return;
                try
                {
                    var cont = App.Container.Resolve<ApplicationDataContainer>();
                    await _apiService.SendMessage(_selectedMatchId, new MessageDto { Data = MyMessage }, cont.Values["AuthToken"] as string);
                    MyMessage = String.Empty;
                    await GetMessage(_selectedMatchId);
                }
                catch(Exception e)
                {
                    await new MessageDialog(e.Message).ShowAsync();
                }
            });
        }

        private bool _isNull = true;
        public bool IsNull { get => _isNull; set => this.RaiseAndSetIfChanged(ref _isNull, value); }

        public ReactiveCommand<Unit, Unit> SendMessageCommand { get; }

        private SourceList<MatchDto> _list = null;
        private SourceList<MessageDto> _messageList = null;

        public SourceList<MatchDto> List { get => _list; set => this.RaiseAndSetIfChanged(ref _list, value); }
        public SourceList<MessageDto> MessageList { get => _messageList; set => this.RaiseAndSetIfChanged(ref _messageList, value); }

        private string _selectedMatchId { get; set; }

        private string _myMessage;
        public string MyMessage { get => _myMessage; set => this.RaiseAndSetIfChanged(ref _myMessage, value); }

        public async Task OnInit()
        {
            try
            {
                var cont = App.Container.Resolve<ApplicationDataContainer>();
                var ret = await _apiService.GetMatches(cont.Values["AuthToken"] as string);
                var list = new SourceList<MatchDto>();
                list.AddRange(ret); 
                List = list;
            }
            catch (ApiException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    
                    return;
                }
                await new MessageDialog(e.Message).ShowAsync();
            }
            catch (Exception e)
            {
                await new MessageDialog(e.Message).ShowAsync();
            }
        }

        public async Task GetMessage(string matchId)
        {
            try
            {
                var cont = App.Container.Resolve<ApplicationDataContainer>();
                var res = await _apiService.GetMessages(matchId, cont.Values["AuthToken"] as string);
                var list = new SourceList<MessageDto>();
                list.AddRange(res);
                MessageList = list;
            }
            catch(Exception e)
            {
                await new MessageDialog(e.Message).ShowAsync();
            }
        }

        public async void ItemClick(object sender, ItemClickEventArgs e)
        {
            var match = e.ClickedItem as MatchDto;
            try
            {
                _selectedMatchId = match.Id;
                var cont = App.Container.Resolve<ApplicationDataContainer>();
                var mess = await _apiService.GetMessages(match.Id, cont.Values["AuthToken"] as string);
                var list = new SourceList<MessageDto>();
                IsNull = false;
                list.AddRange(mess);
                MessageList = list;
            }
            catch(Exception exc)
            {
                await new MessageDialog(exc.Message).ShowAsync();
            }
        }
    }
}
