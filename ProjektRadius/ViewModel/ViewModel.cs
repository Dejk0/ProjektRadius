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
    IOrientationSensor orientationSensor;
    IGeolocator geolocator;
    private Stopwatch sw = new Stopwatch();
    private Stopwatch timeWatch = new Stopwatch();

    public List<double> alfaAngle_1 = new List<double>();
    public List<double> betaAngle_1 = new List<double>();
    public List<double> gammaAngle_1 = new List<double>();
    public List<double> velocity_list = new List<double>();
    public List<long> angleTimeListInMS = new List<long>();
    public List<long> velocityTimeListInMS = new List<long>();

    [ObservableProperty]
        public double alfaAngle;
        [ObservableProperty]
        public double betaAngle;
        [ObservableProperty]
        public double gammaAngle;
        [ObservableProperty]
        public double speed;

        public double alfaAngle_base;
        public double betaAngle_base;
        public double gammaAngle_base;
        public ViewModel(IConnectivity connectivity,  IOrientationSensor orientationSensor,  IGeolocator geolocator)
        {
            Title = "ProjektRadius";  
            this.orientationSensor = orientationSensor;
            orientationSensor.ReadingChanged += OrientationSensor_ReadingChanged;
            this.geolocator = geolocator;
            geolocator.PositionChanged += Geolocator_PositionChanged;
        }

        private void Geolocator_PositionChanged(object? sender, PositionEventArgs e)
        {
            Speed = e.Position.Speed;
      velocity_list.Add(e.Position.Speed);
      velocityTimeListInMS.Add(timeWatch.ElapsedMilliseconds);
        }

        public void ToEulerAngles(float x, float y, float z, float w)
        {
          double alpha = Alfa_Calculating(x, y, z, w);
          double beta = Beta_Calculating(x, y, z, w);
          double gamma = Gamma_Calculating(x, y, z, w);      
          UpdateTheAngles(alpha,beta,gamma);
      if (sw.ElapsedMilliseconds > 50)
      {
        sw.Restart();
        alfaAngle_1.Add(Math.Round(AlfaAngle,3));
        betaAngle_1.Add(Math.Round(BetaAngle, 3));
        gammaAngle_1.Add(Math.Round(GammaAngle, 3));
        angleTimeListInMS.Add(timeWatch.ElapsedMilliseconds);
      }
    }
    private void UpdateTheAngles(double alpha, double beta, double gamma)
    {
      AlfaAngle = alpha * 180.0 / Math.PI;
      BetaAngle = beta * 180.0 / Math.PI;
      GammaAngle = gamma * 180.0 / Math.PI;
    }

    private double Alfa_Calculating(float x, float y, float z, float w)
    {
      double sinr_cosp = 2 * (w * x + y * z);
      double cosr_cosp = 1 - 2 * (x * x + y * y);
      double alpha = Math.Atan2(sinr_cosp, cosr_cosp);
      return alpha;
    }
    private double Beta_Calculating(float x, float y, float z, float w)
    {
      double sinp = 2 * (w * y - z * x);
      double beta;
      if (Math.Abs(sinp) >= 1)
        beta = Math.CopySign(Math.PI / 2, sinp);
      else
        beta = Math.Asin(sinp);
      return beta;
    }
    private double Gamma_Calculating(float x, float y, float z, float w)
    {
      double siny_cosp = 2 * (w * z + x * y);
      double cosy_cosp = 1 - 2 * (y * y + z * z);
      double gamma = Math.Atan2(siny_cosp, cosy_cosp);
      return gamma;
    }

    private void OrientationSensor_ReadingChanged(object? sender, OrientationSensorChangedEventArgs e)
        {
            ToEulerAngles(e.Reading.Orientation.X, e.Reading.Orientation.Y, e.Reading.Orientation.Z, e.Reading.Orientation.W);
        }
    [RelayCommand]
    async Task StartRecordingAsync()
    {
      sw.Start();
      timeWatch.Start();
      while (true)
      {
        if (IsNotBusy)
        {
          if (orientationSensor.IsMonitoring)
          {
            sw.Stop();
            orientationSensor.Stop();
            geolocator.StopListeningAsync();
            break;
          }
          else
          {
            sw.Restart();
            orientationSensor.Start(sensorSpeed: SensorSpeed.Fastest);
            geolocator.StartListeningAsync(TimeSpan.FromMilliseconds(100),0.01);
            break;
          }
        }
      }
    }
    
    [RelayCommand]
    public async Task ExportToCsvAsync()
    {
      try
      {
        CreatingAnglesCSV();
        CreatingVelocitysCSV();
        await Application.Current.MainPage.DisplayAlert("Success", $"File saved to downloadsPath", "OK");
       }
       catch (Exception ex)
       {
                await Application.Current.MainPage.DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
       }
     }
    private void CreatingVelocitysCSV()
    {       
      string filePath = "velocitys.csv";
      var downloadsPath = Path.Combine("/storage/emulated/0/Download/", filePath);
      using (StreamWriter sw = new StreamWriter(downloadsPath))
      {
        sw.WriteLine("Time;Velocity");
        for (int row = 0; row < velocityTimeListInMS.Count; row++)
        {
          var line = string.Join(";",
              velocityTimeListInMS[row].ToString(),
              velocity_list[row].ToString()
          );
          sw.WriteLine(line);
        }
      }
    }
    private void CreatingAnglesCSV()
    {
      string filePath = "angles.csv";
      var downloadsPath = Path.Combine("/storage/emulated/0/Download/", filePath);
      using (StreamWriter sw = new StreamWriter(downloadsPath))
      {
        sw.WriteLine("Time;AlfaAngle;BetaAngle;GammaAngle");
        for (int row = 0; row < angleTimeListInMS.Count; row++)
        {
          var line = string.Join(";",
              angleTimeListInMS[row].ToString(),
              alfaAngle_1[row].ToString(),
              betaAngle_1[row].ToString(),
              gammaAngle_1[row].ToString()
          );
          sw.WriteLine(line);
        }
      }
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
