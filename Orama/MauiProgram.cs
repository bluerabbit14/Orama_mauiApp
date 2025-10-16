using Microsoft.Extensions.Logging;
using Orama.Interfaces;
using Orama.Services;

namespace Orama
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

            //Register the Service in DI
            builder.Services.AddSingleton<INavigationService, NavigationService>();
#endif

            return builder.Build();
        }
    }
}
