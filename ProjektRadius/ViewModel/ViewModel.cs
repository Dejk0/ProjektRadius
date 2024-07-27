using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Text;
namespace ProjektRadius.ViewModel
{
    public partial class ViewModel : BaseViewModel
    {
        IConnectivity connectivity;
        IGeolocation geolocation;
        IAccelerometer accelerometer;
        IOrientationSensor orientationSensor;

        public ViewModel(IConnectivity connectivity, IGeolocation geolocation, IAccelerometer accelerometer, IOrientationSensor orientationSensor, IMagnetometer magnetometer )
        {
            Title = "ProjektRadius";
            this.connectivity = connectivity;
            this.geolocation = geolocation;
            this.accelerometer = accelerometer;
            accelerometer.ReadingChanged += Accelerometer_Working;
            this.orientationSensor = orientationSensor;
            orientationSensor.ReadingChanged += OrientationSensor_ReadingChanged;
        }

       
        public void ToEulerAngles(float x, float y,float z, float w)
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
            GammaAngle  = gamma * 180.0 / Math.PI;
            
        }
        private void OrientationSensor_ReadingChanged(object? sender, OrientationSensorChangedEventArgs e)
        {
            ToEulerAngles(e.Reading.Orientation.X, e.Reading.Orientation.Y, e.Reading.Orientation.Z, e.Reading.Orientation.W);

        }
       

        private Stopwatch sw = new Stopwatch();
        private Stopwatch timeWatch = new Stopwatch();

        public List<double> x_accel = new List<double>();
        public List<double> y_accel = new List<double>();
        public List<double> z_accel = new List<double>();
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

        public float xResult_base;
        public float yResult_base;
        public float zResult_base;
        public double alfaAngle_base;
        public double betaAngle_base;
        public double gammaAngle_base;

        public List<double> x_accel_1 = new List<double>();
        public List<double> y_accel_1 = new List<double>();
        public List<double> z_accel_1 = new List<double>();

        private void Accelerometer_Working(object sender, AccelerometerChangedEventArgs args)
        { 
            // Calculate the gravity components based on the current orientation
            var gravity = CalculateGravityComponents(AlfaAngle, BetaAngle );

            // Subtract the gravity components from the accelerometer readings
            double xAcc = (args.Reading.Acceleration.X+gravity.X   ) *9.81f;
            double yAcc = (args.Reading.Acceleration.Y-gravity.Y ) * 9.81f; 
            double zAcc = (args.Reading.Acceleration.Z-gravity.Z  ) * 9.81f;
            x_accel_1.Add(xAcc);
            y_accel_1.Add(yAcc);
            z_accel_1.Add(zAcc);

            if (sw.ElapsedMilliseconds > 50)
            {
                sw.Restart();
                xAcc = Math.Round(x_accel_1.Average(),2);
                x_accel.Add(xAcc);
                x_accel_1.Clear();
                yAcc = Math.Round(y_accel_1.Average(), 2);
                y_accel.Add(yAcc);
                y_accel_1.Clear(); 
                zAcc = Math.Round(z_accel_1.Average(), 2);
                z_accel.Add(zAcc);
                z_accel_1.Clear();
                alfaAngle_1.Add(AlfaAngle);
                betaAngle_1.Add(BetaAngle);
                gammaAngle_1.Add(GammaAngle);
                time.Add(timeWatch.ElapsedMilliseconds);
                XResult = Math.Round(xAcc, 5).ToString();
                YResult = Math.Round(yAcc, 5).ToString();
                ZResult = Math.Round(zAcc, 5).ToString();
            }
              
               
            
        }

        private Vector3 CalculateGravityComponents(double alfa, double beta)
        {
            // Convert angles from degrees to radians
            double alfaRad = alfa * Math.PI / 180.0;
            double betaRad = beta * Math.PI / 180.0;



            double g = 1; // Normalized gravitational acceleration (unitless)
            double gx = g * Math.Sin(betaRad)*Math.Cos(alfaRad);
            double gy = g * Math.Sin(alfaRad)* Math.Cos(betaRad);
            double gz = g * Math.Cos(alfaRad) * Math.Cos(betaRad);

            return new Vector3((float)gx, (float)gy, (float)gz);
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
                    if (accelerometer.IsMonitoring || orientationSensor.IsMonitoring)
                    {
                        accelerometer.Stop();
                        orientationSensor.Stop();
                        break;
                    }
                    else
                    {
                        sw.Restart();
                        accelerometer.Start(sensorSpeed: SensorSpeed.Fastest);
                        orientationSensor.Start(sensorSpeed: SensorSpeed.Fastest);
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
                // Adatok a CSV fájlhoz
                string csvData = "helo";

                // Fájl elérési útja és neve
                string fileName = "test.csv";
                string filePath = fileName;
                var downloadsPath = Path.Combine("/storage/emulated/0/Download/", filePath);

               

                // Fájl mentése
                using (StreamWriter sw = new StreamWriter(downloadsPath))
                {
                    // Fejléc hozzáadása (opcionális)
                    sw.WriteLine("Time;XAccel;YAccel;ZAccel;AlfaAngle;BetaAngle;GammaAngle");

                    for (int row = 0; row < x_accel.Count; row++)
                    {
                        // Sorok összeállítása
                        var line = string.Join(";",
                            time[row].ToString(),
                            x_accel[row].ToString(),
                            y_accel[row].ToString(),
                            z_accel[row].ToString(),
                            alfaAngle_1[row].ToString(),
                            betaAngle_1[row].ToString(),
                            gammaAngle_1[row].ToString()
                        );

                        // Sor írása a fájlba
                        sw.WriteLine(line);
                    }
                }

                // Opcionális: visszajelzés a felhasználónak
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
