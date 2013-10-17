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
using System.IO;

namespace WP8Runner
{
    public partial class Records : PhoneApplicationPage
    {
        public Records()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            try
            {
                using (var store = System.IO.IsolatedStorage.IsolatedStorageFile.GetUserStoreForApplication())
                using (var stream = new IsolatedStorageFileStream("data.txt", FileMode.Open, FileAccess.ReadWrite, store))
                {
                    StreamReader stmReader = new StreamReader(stream);
                    //this.recordBox.Text = stmReader.ReadToEnd();
                    //this.tbxFilename.Text = strFileName;

                    
                    String[] set = stmReader.ReadToEnd().ToString().Split('=');
                    recordList.ItemsSource = set;
                    stmReader.Close();
                }
            }
            catch
            {

            }
        }

       

       

       
    }
}