using Microsoft.Extensions.Logging;
using DulceRelax.App.Admin.Services;
using DulceRelax.App.Admin.ViewModels;
using DulceRelax.App.Admin.Views;

namespace DulceRelax.App.Admin
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {

            var builder = MauiApp.CreateBuilder();

            var apiUrl = new Uri("https://jkr8tzhd-7204.use.devtunnels.ms/");

            builder.Services.AddTransient<AuthHandler>();
            builder.Services.AddHttpClient<IAuthService, AuthService>(c => c.BaseAddress = apiUrl)
                .AddHttpMessageHandler<AuthHandler>();
            builder.Services.AddHttpClient("refresh", c => c.BaseAddress = apiUrl);

            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<InicioSesionAdmin>();
            builder.Services.AddTransient<PrincipalAdmin>();


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

            return builder.Build();
        }
    }
}
