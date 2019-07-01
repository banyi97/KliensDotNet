using Autofac;
using DynamicData;
using ReactiveUI;
using Refit;
using SharedLibrary.Dtos;
using SharedLibrary.Models;
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

namespace UwpClient.ViewModels
{
    public class MatchViewModel : ReactiveObject
    {
        private readonly NavigationService _navigationService;
        private readonly IRestApiService _apiService;

        public MatchViewModel(NavigationService navigationService, IRestApiService apiService)
        {
            _navigationService = navigationService ?? throw new NullReferenceException(); ;
            _apiService = apiService ?? throw new NullReferenceException();

            var cont = App.Container.Resolve<ApplicationDataContainer>();

            var valid = this.WhenAnyValue(x => x.Error404, (q) => q == true);
            LikeCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                try
                {
                    await _apiService.Liked(_id, cont.Values["AuthToken"] as string);
                    await GetNewPersone();
                }
                catch(Exception e)
                {
                    await new MessageDialog(e.Message).ShowAsync();
                }
            });
            PassCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                try
                {
                    await _apiService.Liked(_id, cont.Values["AuthToken"] as string);
                    await GetNewPersone();
                }
                catch (Exception e)
                {
                    await new MessageDialog(e.Message).ShowAsync();
                }
            });
            SaveSettingCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                try
                {
                    await _apiService.SetSearchParameters(new SearchParameterDto {  }, cont.Values["AuthToken"] as string);
                    await GetNewPersone();
                }
                catch (Exception e)
                {
                    await new MessageDialog(e.Message).ShowAsync();
                }
            });

            LocationCommand = ReactiveCommand.CreateFromTask(async () => 
            {
                try
                {
                    var loc = App.Container.Resolve<LocationService>();
                    await loc.GetAccess();
                }
                catch (Exception)
                {

                }
            });
        }

        private SearchParameterDto _searchParameter;
        public SearchParameterDto SearchParameter { get => _searchParameter; set => this.RaiseAndSetIfChanged(ref _searchParameter, value); }

        public ReactiveCommand<Unit, Unit> LocationCommand { get; }
        public ReactiveCommand<Unit, Unit> LikeCommand { get; }
        public ReactiveCommand<Unit, Unit> PassCommand { get; }
        public ReactiveCommand<Unit, Unit> SaveSettingCommand { get; }
        
        public bool _error404 = false;
        public bool Error404 { get => _error404; set => this.RaiseAndSetIfChanged(ref _error404, value); }
        
        private bool _isPaneOpen = false;
        public bool IsPaneOpen { get => _isPaneOpen; set => this.RaiseAndSetIfChanged(ref _isPaneOpen, value); }

        private bool _man = false;
        public bool Man { get => _man; set => this.RaiseAndSetIfChanged(ref _man, value); }

        private bool _woman = false;
        public bool Woman { get => _woman; set => this.RaiseAndSetIfChanged(ref _woman, value); }

        public async Task OnInit()
        {
            try
            {
                var cont = App.Container.Resolve<ApplicationDataContainer>();
                var ret = await _apiService.GetSearchParameters(cont.Values["AuthToken"] as string);
                var param = new SearchParameterDto
                {
                    MinAge = ret.MinAge,
                    MaxAge = ret.MaxAge,
                    MaxDist = ret.MaxDist
                };
                SearchParameter = param;
                switch (ret.SearchedGender)
                {
                    case Gender.Female:
                        Man = false;
                        Woman = true;
                        break;
                    case Gender.Male:
                        Man = true;
                        Woman = false;
                        break;
                    case Gender.All:
                        Man = true;
                        Woman = true;
                        break;
                    default:
                        break;
                }
            }
            catch(Exception e)
            {
                await new MessageDialog(e.Message).ShowAsync();
            }
        }

        public void ShowButton()
        {
            IsPaneOpen = !IsPaneOpen;
        }

        public async Task GetNewPersone()
        {
            Error404 = false;
            try
            {
                var cont = App.Container.Resolve<ApplicationDataContainer>();
                var ret = await _apiService.GetProfile(cont.Values["AuthToken"] as string);
                if (ret is null || ret.Name is null)
                    return;
                _id = ret.Id;
                Name = ret.Name;
                Description = ret.Description;
                Age = ret.Age;
                School = ret.School;
                Job = ret.Job;
                var list = new SourceList<PhotoDto>();
                list.AddRange(ret.Photos);
                Photos = list;
            }
            catch(ApiException e)
            {
                if(e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    Error404 = true;
                    //await new MessageDialog("No more result").ShowAsync();
                    return;
                }
                await new MessageDialog(e.Message).ShowAsync();
            }
            catch (Exception e)
            {
                await new MessageDialog(e.Message).ShowAsync();
            }
        }
        private string _id;
        
        private string _name;
        private int _age;
        private string _job;
        private string _school;
        private string _description;
        private Gender _gender;
        private SourceList<PhotoDto> _photos;

        public string Name { get => _name; set => this.RaiseAndSetIfChanged(ref _name, value); }
        public int Age { get => _age; set => this.RaiseAndSetIfChanged(ref _age, value); }
        public string Job { get => _job; set => this.RaiseAndSetIfChanged(ref _job, value); }
        public string School { get => _school; set => this.RaiseAndSetIfChanged(ref _school, value); }
        public string Description { get => _description; set => this.RaiseAndSetIfChanged(ref _description, value); }
        public Gender Gender { get => _gender; set => this.RaiseAndSetIfChanged(ref _gender, value); }

        public SourceList<PhotoDto> Photos { get => _photos; set => this.RaiseAndSetIfChanged(ref _photos, value); }


    }
}
