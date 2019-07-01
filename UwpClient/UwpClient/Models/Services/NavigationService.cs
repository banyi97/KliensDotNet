using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace UwpClient.Models.Services
{
    public class NavigationService
    {
        private Frame AppFrame { get; set; }
        public Frame Frame { get; set; }

        public NavigationService(Frame frame)
        {
            AppFrame = frame;
        }

        public void Navigate_App(Type page)
        {
            AppFrame.Navigate(page);
        }

        public void NavigateBack_App()
        {
            AppFrame.GoBack();
        }

        public void Navigate_Main(Type page)
        {
            if (Frame is null)
                return;
            Frame.Navigate(page);
        }
    }
}
