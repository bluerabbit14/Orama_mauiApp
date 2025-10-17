using Microsoft.Extensions.Logging;
using Orama.Interfaces;
using Orama.Services;
using Orama.ViewModels;
using Orama.Views;

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
            
            //Register ViewModels
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<SignupViewModel>();
            builder.Services.AddTransient<ForgotPasswordViewModel>();
            builder.Services.AddTransient<DashboardViewModel>();
            builder.Services.AddTransient<ProfileViewModel>();
            builder.Services.AddTransient<SettingViewModel>();
            
            //Register Views
            builder.Services.AddTransient<Login>();
            builder.Services.AddTransient<Signup>();
            builder.Services.AddTransient<ForgotPassword>();
            builder.Services.AddTransient<Dashboard>();
            builder.Services.AddTransient<Profile>();
            builder.Services.AddTransient<Setting>();
#endif

            return builder.Build();
        }
    }
}
