using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace WP8Runner
{
    public partial class Start : PhoneApplicationPage
    {
        public Start()
        {
            InitializeComponent();

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        private void startButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        	NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        private void recordButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Records.xaml", UriKind.Relative));
        }

        private void achievementButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Achievement.xaml", UriKind.Relative));
        }

        private void optionButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Options.xaml", UriKind.Relative));
        }


        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}