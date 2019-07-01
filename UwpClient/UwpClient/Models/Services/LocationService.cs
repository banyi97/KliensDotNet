using Autofac;
using SharedLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UwpClient.Models.Services.Interfaces;
using Windows.Devices.Geolocation;
using Windows.Storage;
using Windows.UI.Core;

namespace UwpClient.Models.Services
{
    public class LocationService
    {
        public LocationService(IRestApiService apiService)
        {
            _apiService = apiService;
        }
        public Geoposition Geoposition { get; private set; }
        private readonly IRestApiService _apiService;

        public async Task GetAccess()
        {
            var accessStatus = await Geolocator.RequestAccessAsync();

            switch (accessStatus)
            {
                case GeolocationAccessStatus.Allowed:

                    // If DesiredAccuracy or DesiredAccuracyInMeters are not set (or value is 0), DesiredAccuracy.Default is used.
                    Geolocator geolocator = new Geolocator(); //{ ReportInterval = 1000 * 60 * 5 }; // 5 min -> 1000 milisec * 60 sec * 5 min

                    // Subscribe to the StatusChanged event to get updates of location status changes.

                    // Carry out the operation.
                    Geoposition = await geolocator.GetGeopositionAsync();
                    try
                    {
                        var cont = App.Container.Resolve<ApplicationDataContainer>();
                        await _apiService.UpdateLocation(new LocationDto
                        {
                            Latitude = Geoposition.Coordinate.Point.Position.Latitude,
                            Longitude = Geoposition.Coordinate.Point.Position.Longitude
                        }, cont.Values["AuthToken"] as string);
                    }
                    catch (Exception)
                    {
                        
                    }
                    
                    break;

                case GeolocationAccessStatus.Denied:
                    break;

                case GeolocationAccessStatus.Unspecified:
                    break;
            }
        }
        private async void OnPositionChanged(Geolocator sender, PositionChangedEventArgs e)
        {
            var pos = await sender.GetGeopositionAsync();
            if (pos.Coordinate.Point.Position.Latitude == Geoposition.Coordinate.Point.Position.Latitude &&
                pos.Coordinate.Point.Position.Longitude == Geoposition.Coordinate.Point.Position.Longitude)
                return;
            try
            {
                var cont = App.Container.Resolve<ApplicationDataContainer>();
                await _apiService.UpdateLocation(new LocationDto
                {
                    Latitude = pos.Coordinate.Point.Position.Latitude,
                    Longitude = pos.Coordinate.Point.Position.Longitude
                }, cont.Values["AuthToken"] as string);
            }
            catch (Exception)
            {
            }
        }
    }
}
