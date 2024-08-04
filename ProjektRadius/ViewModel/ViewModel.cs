﻿using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Text;
using GeolocatorPlugin.Abstractions;
namespace ProjektRadius.ViewModel
{
    public partial class ViewModel : BaseViewModel
    {
        IGeolocation geolocation;
        IOrientationSensor orientationSensor;
        IGeolocator geolocator;
        private Stopwatch sw = new Stopwatch();
        private Stopwatch timeWatch = new Stopwatch();

        public List<double> alfaAngle_1 = new List<double>();
        public List<double> betaAngle_1 = new List<double>();
        public List<double> gammaAngle_1 = new List<double>();
        public List<long> time = new List<long>();

        [ObservableProperty]
        public string xResult = "";
        [ObservableProperty]
        public string yResult = "";
        [ObservableProperty]
        public string zResult = "";
        [ObservableProperty]
        public double alfaAngle;
        [ObservableProperty]
        public double betaAngle;
        [ObservableProperty]
        public double gammaAngle;
        [ObservableProperty]
        public double speed;

        public float xResult_base;
        public float yResult_base;
        public float zResult_base;
        public double alfaAngle_base;
        public double betaAngle_base;
        public double gammaAngle_base;

        public List<double> speed_list = new List<double>();
    public ViewModel(IConnectivity connectivity, IGeolocation geolocation,  IOrientationSensor orientationSensor,  IGeolocator geolocator)
        {
            Title = "ProjektRadius";            
            this.geolocation = geolocation;
            this.orientationSensor = orientationSensor;
            orientationSensor.ReadingChanged += OrientationSensor_ReadingChanged;
            this.geolocator = geolocator;
            geolocator.PositionChanged += Geolocator_PositionChanged;
        }

        private void Geolocator_PositionChanged(object? sender, PositionEventArgs e)
        {
            Speed = e.Position.Speed*3.6;
        }

        public void ToEulerAngles(float x, float y, float z, float w)
        {
            double sinr_cosp = 2 * (w * x + y * z);
            double cosr_cosp = 1 - 2 * (x * x + y * y);
            double alpha = Math.Atan2(sinr_cosp, cosr_cosp);

            double sinp = 2 * (w * y - z * x);
            double beta;
            if (Math.Abs(sinp) >= 1)
                beta = Math.CopySign(Math.PI / 2, sinp); // use 90 degrees if out of range
            else
                beta = Math.Asin(sinp);

            double siny_cosp = 2 * (w * z + x * y);
            double cosy_cosp = 1 - 2 * (y * y + z * z);
            double gamma = Math.Atan2(siny_cosp, cosy_cosp);
            AlfaAngle = alpha * 180.0 / Math.PI;
            BetaAngle = beta * 180.0 / Math.PI;
            GammaAngle = gamma * 180.0 / Math.PI;
        }
        private void OrientationSensor_ReadingChanged(object? sender, OrientationSensorChangedEventArgs e)
        {
            ToEulerAngles(e.Reading.Orientation.X, e.Reading.Orientation.Y, e.Reading.Orientation.Z, e.Reading.Orientation.W);

        }
    [RelayCommand]
    async Task GetAccelerometerAsync()
    {
      sw.Start();
      timeWatch.Start();

      while (true)
      {
        if (IsNotBusy)
        {
          if (orientationSensor.IsMonitoring)
          {
            
            orientationSensor.Stop();
            break;
          }
          else
          {
            sw.Restart();
            orientationSensor.Start(sensorSpeed: SensorSpeed.Fastest);
            break;
          }
        }
      }
    }


    private void Accelerometer_Working(object sender, AccelerometerChangedEventArgs args)
        { 
            if (sw.ElapsedMilliseconds > 50)
            {
                sw.Restart();
                alfaAngle_1.Add(AlfaAngle);
                betaAngle_1.Add(BetaAngle);
                gammaAngle_1.Add(GammaAngle);
                time.Add(timeWatch.ElapsedMilliseconds);
                speed_list.Add(Speed);
            }
        }         
        [RelayCommand]
        public async Task ExportToCsvAsync()
        {
            try
            {
                string fileName = "test.csv";
                string filePath = fileName;
                var downloadsPath = Path.Combine("/storage/emulated/0/Download/", filePath);               
                using (StreamWriter sw = new StreamWriter(downloadsPath))
                {
                    sw.WriteLine("Time;XAccel;YAccel;ZAccel;AlfaAngle;BetaAngle;GammaAngle");
                    for (int row = 0; row < time.Count; row++)
                    {
                        var line = string.Join(";",
                            time[row].ToString(),
                            alfaAngle_1[row].ToString(),
                            betaAngle_1[row].ToString(),
                            gammaAngle_1[row].ToString()
                        );

                        sw.WriteLine(line);
                    }
                }

                await Application.Current.MainPage.DisplayAlert("Success", $"File saved to {downloadsPath}", "OK");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
        }
        [RelayCommand]
        async Task GetTheLocationAsync()
        {
            Stopwatch watch = Stopwatch.StartNew();

            if (IsBusy)
                return;
            try
            {
                var location = await geolocation.GetLocationAsync();
                if (location == null)
                {
                    watch.Restart();
                    watch.Start();
                    location = await geolocation.GetLocationAsync(
                        new GeolocationRequest
                        {
                            DesiredAccuracy = GeolocationAccuracy.Best,
                            Timeout = TimeSpan.FromSeconds(30),
                        }
                    );
                }
                if (location == null)
                    return;
                await Shell.Current.DisplayAlert("Test done",
                    $"Searching time is {watch.ElapsedMilliseconds}", "OK");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                await Shell.Current.DisplayAlert("Error!", $"Unable to get closest monkeys: {ex.Message}", "OK");
            }
            finally { }
        }
    }

    public struct Vector3
    {
        public float X;
        public float Y;
        public float Z;

        public Vector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }
}
