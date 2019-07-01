using Autofac;
using DynamicData;
using ReactiveUI;
using Refit;
using SharedLibrary.Dtos;
using SharedLibrary.Interfaces;
using SharedLibrary.Models;
using System;
using System.Collections.Generic;
using System.IO;
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
    public class ProfileViewModel : ReactiveObject, IUserData
    {
        private readonly NavigationService _navigationService;
        private readonly IRestApiService _apiService;
        public ProfileViewModel(NavigationService navigationService, IRestApiService apiService)
        {
            _navigationService = navigationService ?? throw new NullReferenceException();
            _apiService = apiService ?? throw new NullReferenceException();

           SaveCommand = ReactiveCommand.CreateFromTask(async () =>
           {
               try
               {
                   var cont = App.Container.Resolve<ApplicationDataContainer>();
                   var ret = await _apiService.SetProfileData(new UserDataSettingsDto { Description = this.Description, Job = this.Job, School = this.School }, cont.Values["AuthToken"] as string);
                   Description = ret.Description;
                   Job = ret.Job;
                   School = ret.School;
               }
               catch(Exception e)
               {
                    await new MessageDialog(e.Message).ShowAsync();
               }
           });

           DeleteCommand = ReactiveCommand.CreateFromTask(async () =>
           {
               try
               {
                   if (SelectedImage == null)
                       return;
                   var cont = App.Container.Resolve<ApplicationDataContainer>();
                   var ret = await _apiService.DeletePhoto(SelectedImage.Id, cont.Values["AuthToken"] as string);
                   SelectedImage = null;
                   var list = new SourceList<PhotoDto>();
                   list.AddRange(ret);
                   Photos = list;
                   SetMainImage(ret);
               }
               catch (Exception e)
               {
                   await new MessageDialog(e.Message).ShowAsync();
               }
           });

           SetMainCommand = ReactiveCommand.CreateFromTask(async () =>
           {
               try
               {
                   if (SelectedImage == null)
                       return;
                   var cont = App.Container.Resolve<ApplicationDataContainer>();
                   var ret = await _apiService.SetMainPhoto(SelectedImage.Id, cont.Values["AuthToken"] as string);
                   var list = new SourceList<PhotoDto>();
                   list.AddRange(ret);
                   Photos = list;
                   SetMainImage(ret);
               }
               catch (Exception e)
               {
                   await new MessageDialog(e.Message).ShowAsync();
               }
           });
        }
        public ReactiveCommand<Unit, Unit> SaveCommand { get; }
        public ReactiveCommand<Unit, Unit> DeleteCommand { get; }
        public ReactiveCommand<Unit, Unit> SetMainCommand { get; }

        private string _name;
        private int _age;
        private string _job;
        private string _school;
        private string _description;
        private Gender _gender;
        private SourceList<PhotoDto> _photos;

        private bool _pictures = false;
        public bool Pictures { get => _pictures; set => this.RaiseAndSetIfChanged(ref _pictures, value); }      
        public string Name { get => _name; set => this.RaiseAndSetIfChanged(ref _name, value); }
        public int Age { get => _age; set => this.RaiseAndSetIfChanged(ref _age, value); }
        public string Job { get => _job; set => this.RaiseAndSetIfChanged(ref _job, value); }
        public string School { get => _school; set => this.RaiseAndSetIfChanged(ref _school, value); }
        public string Description { get => _description; set => this.RaiseAndSetIfChanged(ref _description, value); }
        public Gender Gender { get => _gender; set => this.RaiseAndSetIfChanged(ref _gender, value); }

        public SourceList<PhotoDto> Photos { get => _photos; set => this.RaiseAndSetIfChanged(ref _photos, value); }

        public async Task AddPhotos()
        {
            try
            {
                var picker = new Windows.Storage.Pickers.FileOpenPicker();
                picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
                picker.SuggestedStartLocation =
                    Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
                picker.FileTypeFilter.Add(".jpg");
                picker.FileTypeFilter.Add(".jpeg");
                picker.FileTypeFilter.Add(".png");
                var cont = App.Container.Resolve<ApplicationDataContainer>();
                var files = await picker.PickMultipleFilesAsync();
                if (files.Count != 0)
                {
                    foreach (var file in files)
                    {
                        using (var stream = await file.OpenReadAsync())
                        {
                            var streamPart = new StreamPart(stream.AsStreamForRead(), file.Name, file.ContentType);
                            await _apiService.PostFile(streamPart, cont.Values["AuthToken"] as string);
                        }
                    }
                }
                var photos = await _apiService.GetPhotos(cont.Values["AuthToken"] as string);
                var list = new SourceList<PhotoDto>();
                list.AddRange(photos);
                Photos = list;
            }
            catch (Exception e)
            {
                await new MessageDialog("Exception: ### " + e.Source + " " + e.Message).ShowAsync();
            }
        }

        public void ShowPictures()
        {
            Pictures = !Pictures;
        }

        public async Task OnInit()
        {
            var cont = App.Container.Resolve<ApplicationDataContainer>();
            try
            {
                var res = await _apiService.GetMyProfile(cont.Values["AuthToken"] as string);
                Name = res.Name;
                Age = res.Age;
                Job = res.Job;
                School = res.School;
                Description = res.Description;
                var list = new SourceList<PhotoDto>();
                list.AddRange(res.Photos);
                Photos = list;
                SetMainImage(res.Photos);
            }
            catch(Exception e)
            {
                await new MessageDialog(e.Message).ShowAsync();
            }
        }

        private PhotoDto _selectedImage = null;
        public PhotoDto SelectedImage { get => _selectedImage; set => this.RaiseAndSetIfChanged(ref _selectedImage, value); }

        public void ItemClick(object sender, ItemClickEventArgs e)
        {
            var img = e.ClickedItem as PhotoDto;
            SelectedImage = img;
        }

        private void SetMainImage(IEnumerable<PhotoDto> photos)
        {
            foreach (var item in photos)
            {   
                if(item.Main == true)
                {
                    MainImage = item;
                    break;
                }
            }
            MainImage = null;
        }

        private PhotoDto _mainImage = null;
        public PhotoDto MainImage { get => _mainImage; set => this.RaiseAndSetIfChanged(ref _mainImage, value); }
    }
}
