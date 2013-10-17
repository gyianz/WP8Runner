using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;

namespace WP8Runner
{
    public partial class Options : PhoneApplicationPage
    {

        private int difficulty = 0;

        public Options()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            // txtInput is a TextBox defined in XAML.
            if (!settings.Contains("difficulty"))
            {
                settings.Add("difficulty", difficulty);
            }
            else
            {
                settings["difficulty"] = difficulty;
            }
            settings.Save();

            NavigationService.Navigate(new Uri("/Start.xaml", UriKind.Relative));
        }

        private void easyDiff_Checked(object sender, RoutedEventArgs e)
        {
            difficulty = 1;
        }

        private void mediumDiff_Checked(object sender, RoutedEventArgs e)
        {
            difficulty = 2;
        }

        private void hardDiff_Checked(object sender, RoutedEventArgs e)
        {
            difficulty = 3;
        }

    }
}