using Autofac;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UwpClient.ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UwpClient.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Message : Page, IViewFor<MessageViewModel>
    {
        public Message()
        {
            this.InitializeComponent();
            ViewModel = App.Container.Resolve<MessageViewModel>();
        }

        public MessageViewModel ViewModel
        {
            get => (MessageViewModel)DataContext;
            set => DataContext = value;
        }
        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (MessageViewModel)value;
        }
    }
}
