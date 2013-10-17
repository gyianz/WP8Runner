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
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using Windows.Devices.Geolocation;
using Windows.Storage;

namespace WP8Runner
{
  public partial class MainPage : PhoneApplicationPage
  {
      private GeoCoordinateWatcher _watcher;
      private List<GeoCoordinate> coorList;
      private double initialLa;
      private double initialLong;
      private MapPolyline _line;
      private DispatcherTimer _timer;
      private DispatcherTimer _countdown;
      private long _startTime;
      private String fileName = "";
      private int counter2 = 1;
      private double latitude;
      private double longitude;
      private const double rangeSmall = 0.0010;
      private MapLayer layer;
      private String buttonState = "";
      private String dataString = "";
      private int difficulty = 1;
      //private Popup popup = new Popup();
      public MainPage()
      {
          InitializeComponent();

          IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
          // txtInput is a TextBox defined in XAML.
          if (!settings.Contains("difficulty"))
          {
              settings.Add("difficulty", 1);
          }
          else
          {
              difficulty = int.Parse(settings["difficulty"].ToString());
          }

          _watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.High);
          _watcher.Start();
          _watcher.PositionChanged += Watcher_PositionChanged;
          coorList = new List<GeoCoordinate>();
          _timer = new DispatcherTimer();
          _countdown = new DispatcherTimer();
          // create a line which illustrates the run
          _line = new MapPolyline();
          _line.StrokeColor = Colors.Red;
          _line.StrokeThickness = 5;
          //Map.MapElements.Add(_line);
          _timer.Interval = TimeSpan.FromSeconds(1);
          _timer.Tick += Timer_Tick;

      }




      private void restart()
      {
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

      private void StartButton_Click(object sender, RoutedEventArgs e)
      {
          if (StartButton.Content.Equals("Stop"))
          {
              _watcher.Stop();
              _timer.Stop();
              dataString = fileName + "\nDistance  :  " + distanceLabel.Text + "\nPoints  :  " + caloriesLabel.Text + "\nTime  :  " + timeLabel.Text + "\n=";
              setAchievement();
              saveData(sender, e);
              //  _watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.High);
              //_timer = new DispatcherTimer();
              //_watcher.PositionChanged += Watcher_PositionChanged;
              restart();

              start.IsOpen = true;

              //   _timer.Tick += Timer_Tick;
              StartButton.Content = "Start";

              //    writer.Close();

          }
          else
          {
              restart();
              ShowMyLocation();
              Map.MapElements.Add(_line);
              _watcher.Start();
              _timer.Start();
              _startTime = System.Environment.TickCount;
              if (_countdown == null)
              {
                  _countdown = new DispatcherTimer();
                  _countdown.Interval = TimeSpan.FromMilliseconds(100);
                  // _countdown.Tick += new EventHandler(dispatcherTimer_Tick);

                  _countdown.Start();
              }
              StartButton.Content = "Stop";
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

          //create a small circle
          Ellipse myCircle = new Ellipse();
          myCircle.Fill = new SolidColorBrush(Colors.Green);
          myCircle.Height = 20;
          myCircle.Width = 20;
          myCircle.Opacity = 50;

          //create the overlay
          MapOverlay myLocationOverlay = new MapOverlay();
          myLocationOverlay.Content = myCircle;
          myLocationOverlay.PositionOrigin = new Point(0.5, 0.5);
          myLocationOverlay.GeoCoordinate = myGeoCoordinate;

          //create a maplayer to contain the overlay
          layer = new MapLayer();
          layer.Add(myLocationOverlay);

          //add the layer to the map
          Map.Layers.Add(layer);
          // Map.Layers.Remove(layer);

      }

      //ID_CAP_LOCATION
      private double _kilometres;
      private double _speed;
      private long _previousPositionChangeTick;

      private void Watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
      {
          var coord = new GeoCoordinate(e.Position.Location.Latitude, e.Position.Location.Longitude);
          int gained = int.Parse(caloriesLabel.Text.ToString());


          // if (counter == 1) {
          initialLa = e.Position.Location.Latitude;
          initialLong = e.Position.Location.Longitude;
          //    counter++;
          //   }
          
          // WriteToFile(e.Position.Location.Longitude.ToString(), e.Position.Location.Latitude.ToString());
          if (buttonState.Equals("top"))
          {
              if (e.Position.Location.Latitude > latitude)
              {

                  sucessBox.IsOpen = true;
                  gained += 10;
                  caloriesLabel.Text = gained.ToString();
              }
          }
          else if (buttonState.Equals("left"))
          {
              if (e.Position.Location.Longitude < longitude)
              {

                  sucessBox.IsOpen = true;
                  gained += 10;
                  caloriesLabel.Text = gained.ToString();
              }
          }
          else if (buttonState.Equals("right"))
          {
              if (e.Position.Location.Longitude > longitude)
              {

                  sucessBox.IsOpen = true;
                  gained += 10;
                  caloriesLabel.Text = gained.ToString();
              }
          }
          else if (buttonState.Equals("bottom"))
          {
              if (e.Position.Location.Latitude < latitude)
              {

                  sucessBox.IsOpen = true;
                  gained += 10;
                  caloriesLabel.Text = gained.ToString();
              }
          }



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
                      popup.IsOpen = true;
                  }

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

              if (counter2 == 1)
              {
                  counter2++;
                  start.IsOpen = true;
              }
          }
          catch (Exception ex)
          {
              Console.WriteLine(ex);
          }
      }

      private void btn_continue_Click(object sender, RoutedEventArgs e)
      {
          // _timer.Stop();
          StartButton.IsEnabled = false;
          popup.IsOpen = false;
          dataString = fileName + "\nDistance  :  " + distanceLabel.Text + "\nPoints  :  " + caloriesLabel.Text + "\nTime  :  " + timeLabel.Text + "\n=";
          setAchievement();
          restart();
          start.IsOpen = true;
          StartButton.Content = "Start";
      }

      private void current_click(object sender, RoutedEventArgs e)
      {
          ShowMyLocation();
      }

      private void top_clicked(object sender, RoutedEventArgs e)
      {
          // Random rand = new Random();
          try
          {
              IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
              if (IsolatedStorageSettings.ApplicationSettings.Contains("difficulty"))
              {
                  latitude = initialLa + rangeSmall * difficulty;
                  longitude = initialLong;
                  buttonState = "top";
                  createPin(latitude, longitude);
              }
          }
          catch
          {

          }
      }

      private void left_clicked(object sender, RoutedEventArgs e)
      {
         
                  latitude = initialLa;
                  longitude = initialLong - rangeSmall * difficulty;
                  buttonState = "left";
                  createPin(latitude, longitude);
          
      }

      private void right_clicked(object sender, RoutedEventArgs e)
      {
         
                  // Random rand = new Random();
                  latitude = initialLa;
                  longitude = initialLong + rangeSmall * difficulty;
                  buttonState = "right";
                  createPin(latitude, longitude);
         
      }

      private void bottom_clicked(object sender, RoutedEventArgs e)
      {
         
                  latitude = initialLa - rangeSmall * difficulty;
                  longitude = initialLong;
                  buttonState = "bottom";
                  createPin(latitude, longitude);
          
      }



      private void createPin(double latitude, double longitude)
      {
          Ellipse myCircle = new Ellipse();
          myCircle.Fill = new SolidColorBrush(Colors.Blue);
          myCircle.Height = 20;
          myCircle.Width = 20;
          myCircle.Opacity = 50;

          MapOverlay myLocationOverlay = new MapOverlay();
          myLocationOverlay.Content = myCircle;
          myLocationOverlay.PositionOrigin = new Point(0, 0);

          GeoCoordinate myGeoCoordinate = new GeoCoordinate(latitude, longitude);
          myLocationOverlay.GeoCoordinate = myGeoCoordinate;

          //create a maplayer to contain the overlay
          MapLayer layer = new MapLayer();
          layer.Add(myLocationOverlay);
          Map.Layers.Clear();
          Map.MapElements.Clear();
          //add the layer to the map
          Map.Layers.Add(layer);
          StartButton.IsEnabled = true;
          start.IsOpen = false;
          ShowMyLocation();
      }

      private void pointClick(object sender, RoutedEventArgs e)
      {
          StartButton.IsEnabled = false;
          sucessBox.IsOpen = false;
          dataString = fileName + "\nDistance  :  " + distanceLabel.Text + "\nPoints  :  " + caloriesLabel.Text + "\nTime  :  " + timeLabel.Text + "\n=";
          setAchievement();
          saveData(sender, e);
          restart();
          start.IsOpen = true;
          StartButton.Content = "Start";
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
              int totalPoints = int.Parse(settings["totalPoints"].ToString());
              totalPoints = totalPoints + int.Parse(caloriesLabel.Text.ToString());
              settings["totalPoints"] = totalPoints.ToString();
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


  }

}