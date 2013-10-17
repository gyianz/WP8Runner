using Microsoft.Phone.Controls;
using Microsoft.Phone.Maps.Controls;
using Microsoft.Phone.Shell;
using NExtra.Geo;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using Windows.Devices.Geolocation;

namespace WP8Runner
{
  public partial class MainPage : PhoneApplicationPage
  {
      private GeoCoordinateWatcher _watcher;
      private List<GeoCoordinate> coorList;
      private GeoCoordinate initialCoord;
      private MapPolyline _line;
      private DispatcherTimer _timer;
      private long _startTime;
      private String fileName = "";
      private int counter = 1;
      private String dataString = "";
      //private Popup popup = new Popup();
      public MainPage()
      {
          InitializeComponent();
          coorList = new List<GeoCoordinate>();
          _watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.High);
          _timer = new DispatcherTimer();

          // create a line which illustrates the run
          _line = new MapPolyline();
          _line.StrokeColor = Colors.Red;
          _line.StrokeThickness = 5;
          Map.MapElements.Add(_line);

          _watcher.PositionChanged += Watcher_PositionChanged;
          _timer.Interval = TimeSpan.FromSeconds(1);
          _timer.Tick += Timer_Tick;
      }



      private void restart()
      {
          dataString = "";
          _timer.Interval = TimeSpan.FromSeconds(1);
          // _line.Path.Clear();
          _kilometres = 0;
          _speed = 0;
          String time = "00:00:00";
          distanceLabel.Text = string.Format("{0:f2} km", _kilometres);
          caloriesLabel.Text = string.Format("{0:f0}", _kilometres * 65);
          speedLabel.Text = string.Format("{0:f2}", _speed);
          timeLabel.Text = string.Format("{0:f2}", time);
      }

      private void Timer_Tick(object sender, EventArgs e)
      {
          TimeSpan runTime = TimeSpan.FromMilliseconds(System.Environment.TickCount - _startTime);
          timeLabel.Text = runTime.ToString(@"hh\:mm\:ss");
      }

      private void setAchievement()
      {
          IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
          // txtInput is a TextBox defined in XAML.
          if (!settings.Contains("totalPoints"))
          {
              settings.Add("totalPoints", caloriesLabel.Text);
          }
          else
          {
              int totalPoints = int.Parse(settings["mostPoints"].ToString());
              totalPoints = totalPoints + int.Parse(caloriesLabel.Text.ToString());
              settings["mostPoints"] = totalPoints.ToString();
          }

          if (!settings.Contains("furthestRun"))
          {
              settings.Add("furthestRun", distanceLabel.Text);
          }
          else
          {
              int value = settings["furthestRun"].ToString().CompareTo(distanceLabel.Text);
              if (value < 0)
              {
                  settings["furthestRun"] = distanceLabel.Text;
              }
          }

          if (!settings.Contains("longestRun"))
          {
              settings.Add("longestRun", timeLabel.Text);
          }
          else
          {
              int value = settings["longestRun"].ToString().CompareTo(timeLabel.Text);
              if (value < 0)
              {
                  settings["longestRun"] = timeLabel.Text;
              }
          }

          settings.Save();
      }

      private void StartButton_Click(object sender, RoutedEventArgs e)
      {
          if (StartButton.Content.Equals("Stop"))
          {
              
              dataString = fileName+"\nDistance  :  "+distanceLabel.Text+"\nTime  :  "+timeLabel.Text+"\nCalories  :  "+caloriesLabel.Text+"\n=";
              setAchievement();

              _watcher.Stop();
              _timer.Stop();
              saveData(sender,e);
              //  _watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.High);
              //_timer = new DispatcherTimer();
              //_watcher.PositionChanged += Watcher_PositionChanged;
              restart();

              //   _timer.Tick += Timer_Tick;
              StartButton.Content = "Start";

              //    writer.Close();

          }
          else
          {
              _watcher.Start();
              _timer.Start();
              _startTime = System.Environment.TickCount;
              //Console.WriteLine("TEST");
              //fileNameDebug.Text = DateTime.Now.ToString();
              fileName = DateTime.Now.ToString();
              //fileNameDebug.Text = fileName;
              
              StartButton.Content = "Stop";
          }
      }


      private void saveData(object sender, EventArgs e)
      {
          try
          {
              using (var store = System.IO.IsolatedStorage.IsolatedStorageFile.GetUserStoreForApplication())
              using (var stream = new IsolatedStorageFileStream("data.txt", FileMode.Append, FileAccess.Write, store))
              {
                  StreamWriter writer = new StreamWriter(stream);
                  writer.Write(dataString);
                  writer.Close();
              }
          }
          catch (Exception)
          {
              MessageBox.Show(dataString);
          }
      }


      private async void ShowMyLocation()
      {
          Geolocator myGeolocator = new Geolocator();
          Geoposition myGeoposition = await myGeolocator.GetGeopositionAsync();
          Geocoordinate myGeocoordinate = myGeoposition.Coordinate;
          GeoCoordinate myGeoCoordinate = CoordinateConverter.ConvertGeocoordinate(myGeocoordinate);
          this.Map.Center = myGeoCoordinate;
          this.Map.ZoomLevel = 18;

          Grid MyGrid = new Grid();
          MyGrid.RowDefinitions.Add(new RowDefinition());
          MyGrid.RowDefinitions.Add(new RowDefinition());
          MyGrid.Background = new SolidColorBrush(Colors.Transparent);

          //create a small circle
          Rectangle MyRectangle = new Rectangle();
          MyRectangle.Fill = new SolidColorBrush(Colors.Black);
          MyRectangle.Height = 20;
          MyRectangle.Width = 20;
          MyRectangle.SetValue(Grid.RowProperty, 0);
          MyRectangle.SetValue(Grid.ColumnProperty, 0);

          //create the overlay
          MapOverlay myLocationOverlay = new MapOverlay();
          myLocationOverlay.Content = MyRectangle;
          myLocationOverlay.PositionOrigin = new Point(0.5, 0.5);
          myLocationOverlay.GeoCoordinate = myGeoCoordinate;

          //create a maplayer to contain the overlay
          MapLayer layer = new MapLayer();
          layer.Add(myLocationOverlay);

          //add the layer to the map
          Map.Layers.Add(layer);
      }

      //ID_CAP_LOCATION
      private double _kilometres;
      private double _speed;
      private long _previousPositionChangeTick;

      private void Watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
      {
          var coord = new GeoCoordinate(e.Position.Location.Latitude, e.Position.Location.Longitude);



          if (counter == 1)
          {
              initialCoord = coord;
              counter++;
          }

          // WriteToFile(e.Position.Location.Longitude.ToString(), e.Position.Location.Latitude.ToString());

          try
          {
              if (_line.Path.Count > 0)
              {
                  GeoCoordinate previousPoint = _line.Path.Last();
                  double distance = coord.GetDistanceTo(previousPoint);
                  double millisPerKilometer = (1000.0 / distance) * (System.Environment.TickCount - _previousPositionChangeTick);
                  _kilometres += distance / 1000.0;
                  TimeSpan runTime = TimeSpan.FromMilliseconds(System.Environment.TickCount - _startTime);
                  _speed = _kilometres / runTime.TotalHours;

                  distanceLabel.Text = string.Format("{0:f2} km", _kilometres);
                  caloriesLabel.Text = string.Format("{0:f0}", _kilometres * 65);
                  speedLabel.Text = string.Format("{0:f2}", _speed);

                  if (_speed > 40)
                  {
                      _timer.Stop();
                      //popup.IsOpen = true;
                  }

                  if (coord.GetDistanceTo(initialCoord) == 0)
                  {

                  }

                  coorList.Add(coord);

                  PositionHandler handler = new PositionHandler();
                  var heading = handler.CalculateBearing(new Position(previousPoint), new Position(coord));
                  Map.SetView(coord, Map.ZoomLevel, heading, MapAnimationKind.Parabolic);

                  ShellTile.ActiveTiles.First().Update(new IconicTileData()
                  {
                      Title = "WP8Runner",
                      WideContent1 = string.Format("{0:f2} km", _kilometres),
                      WideContent2 = string.Format("{0:f0} calories", _kilometres * 65),
                  });



              }
              else
              {
                  Map.Center = coord;
              }

              _line.Path.Add(coord);
              _previousPositionChangeTick = System.Environment.TickCount;
          }
          catch (Exception ex)
          {
              Console.WriteLine(ex);
          }
      }

      private void btn_continue_Click(object sender, RoutedEventArgs e)
      {
          // _timer.Stop();
          //popup.IsOpen = false;
          restart();
          StartButton.Content = "Start";
      }

      private void current_click(object sender, RoutedEventArgs e)
      {
          ShowMyLocation();
      }
  }
}