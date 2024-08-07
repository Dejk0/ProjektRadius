using GeolocatorPlugin;
using GeolocatorPlugin.Abstractions;
using Microsoft.Extensions.Logging;
using ProjektRadius.ViewModel;

namespace ProjektRadius
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            builder.Services.AddSingleton<IConnectivity>(Connectivity.Current);
            builder.Services.AddSingleton<IGeolocation>(Geolocation.Default);
            builder.Services.AddSingleton<IMap>(Map.Default);
            builder.Services.AddSingleton<IOrientationSensor>(OrientationSensor.Default);
            builder.Services.AddSingleton<IGeolocator>(CrossGeolocator.Current);
            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddSingleton<ViewModel.ViewModel>();
            return builder.Build();
        }
    }
}
