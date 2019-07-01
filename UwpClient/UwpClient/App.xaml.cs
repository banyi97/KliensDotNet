using Autofac;
using Refit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using UwpClient.Models.Services;
using UwpClient.Models.Services.Interfaces;
using UwpClient.ViewModels;
using UwpClient.Views;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Globalization;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace UwpClient
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        /// 

        public static IContainer Container { get; private set; }

        private IContainer ConfigureServices()
        {
            var containerBuilder = new ContainerBuilder();

            containerBuilder.Register(q => RestService.For<IRestApiService>("https://projectuwp.azurewebsites.net"))
                .As<IRestApiService>()
                .SingleInstance();
            containerBuilder.Register(q => new NavigationService((Frame)Window.Current.Content))
                .AsSelf()
                .SingleInstance();

            containerBuilder.Register(q => new FbAuthService()).AsSelf().InstancePerLifetimeScope();
            containerBuilder.RegisterType<LocationService>().AsSelf().SingleInstance();

            containerBuilder.RegisterType<AuthViewModel>().InstancePerDependency();
            containerBuilder.RegisterType<MainPageViewModel>().InstancePerDependency(); 
            containerBuilder.RegisterType<MatchViewModel>().InstancePerDependency();
            containerBuilder.RegisterType<MessageViewModel>().InstancePerDependency();
            containerBuilder.RegisterType<ProfileViewModel>().InstancePerDependency();
            containerBuilder.RegisterType<RegisterViewModel>().InstancePerDependency();
            containerBuilder.RegisterType<SettingViewModel>().InstancePerDependency();
            containerBuilder.RegisterType<SignInViewModel>().InstancePerDependency();

            containerBuilder.Register(q => ApplicationData.Current.LocalSettings.CreateContainer("ProjectUwpClient", ApplicationDataCreateDisposition.Always)).InstancePerDependency();

            var container = containerBuilder.Build();
            return container;
        }

        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
            App.Container = ConfigureServices();

            //var cont = App.Container.Resolve<ApplicationDataContainer>();
            //var lang = cont.Values["appLang"] as string;
            //if(lang == null)
            //{
            //    cont.Values["appLang"] = "en-US";
            //    ApplicationLanguages.PrimaryLanguageOverride = "en-US";
            //}
            //else if(lang == "hu-HU")
            //{
            //    ApplicationLanguages.PrimaryLanguageOverride = "hu-HU";
            //}
            //else
            //    ApplicationLanguages.PrimaryLanguageOverride = "en-US";
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    var cont = App.Container.Resolve<ApplicationDataContainer>();
                    var lang = cont.Values["appLang"] as string;
                    if (lang == null)
                    {
                        cont.Values["appLang"] = "en-US";
                        ApplicationLanguages.PrimaryLanguageOverride = "en-US";
                    }
                    else if (lang == "hu-HU")
                    {
                        ApplicationLanguages.PrimaryLanguageOverride = "hu-HU";
                    }
                    else
                        ApplicationLanguages.PrimaryLanguageOverride = "en-US";
                    rootFrame.Navigate(typeof(Auth), e.Arguments);
                }
                // Ensure the current window is active
                Window.Current.Activate();
            }
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}
