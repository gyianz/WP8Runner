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
using System.Windows.Media.Imaging;

namespace WP8Runner
{
    public partial class Achievement : PhoneApplicationPage
    {
        public Achievement()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            try
            {
                
                if (IsolatedStorageSettings.ApplicationSettings.Contains("totalPoints"))
                {
                    mostPoint.Text = IsolatedStorageSettings.ApplicationSettings["totalPoints"] as string;
                 
                }
                if (IsolatedStorageSettings.ApplicationSettings.Contains("longestRun"))
                {
                    longestRun.Text = IsolatedStorageSettings.ApplicationSettings["longestRun"] as string;

                }
                if (IsolatedStorageSettings.ApplicationSettings.Contains("furthestRun"))
                {
                    furthestRun.Text = IsolatedStorageSettings.ApplicationSettings["furthestRun"] as string;

                }

                getRank();
            }
            catch
            {

            }
        }

        private void getRank()
        {
            

            try{
                if (IsolatedStorageSettings.ApplicationSettings.Contains("totalPoints"))
                {
                    String points = IsolatedStorageSettings.ApplicationSettings["totalPoints"] as string;
                    int rawPoints = int.Parse(points);

                    if (rawPoints < 100)
                    {
                        rank.Text = "Recruit";
                        BitmapImage newicon = new BitmapImage(new Uri(@"/Assets/Icons/Rank0-Rookie.png", UriKind.Relative));
                        icon.Source = newicon;
                    }
                    else if (rawPoints > 100 && rawPoints < 2000)
                    {
                        rank.Text = "Private";

                        BitmapImage newicon = new BitmapImage(new Uri(@"/Assets/Icons/Rank1-Squaddie.png", UriKind.Relative));
                        icon.Source = newicon;
                    }
                    else if (rawPoints >= 2000 && rawPoints < 5000)
                    {
                        rank.Text = "Corporal";

                        BitmapImage newicon = new BitmapImage(new Uri(@"/Assets/Icons/Rank2-Corporal.png", UriKind.Relative));
                        icon.Source = newicon;
                    }
                    else if (rawPoints >= 5000 && rawPoints < 12000)
                    {
                        rank.Text = "Sergeant";

                        BitmapImage newicon = new BitmapImage(new Uri(@"/Assets/Icons/Rank3-Sergeant.png", UriKind.Relative));
                        icon.Source = newicon;
                    }
                    else if (rawPoints >= 12000 && rawPoints < 20000)
                    {
                        rank.Text = "Captain";

                        BitmapImage newicon = new BitmapImage(new Uri(@"/Assets/Icons/Rank4-Lieutenant.png", UriKind.Relative));
                        icon.Source = newicon;
                    }
                    else if (rawPoints >= 20000 && rawPoints < 30000)
                    {
                        rank.Text = "Major";

                        BitmapImage newicon = new BitmapImage(new Uri(@"/Assets/Icons/Rank5-Captain.png", UriKind.Relative));
                        icon.Source = newicon;
                    }
                }
            }
            catch{

            }
            
        }

    }
}