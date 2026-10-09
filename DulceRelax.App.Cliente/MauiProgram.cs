using DulceRelax.App.Cliente.Services;
using DulceRelax.App.Cliente.Views;
using DulceRelax.App.Cliente.ViewModels;
using Microsoft.Extensions.Logging;

namespace DulceRelax.App.Cliente
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder.Services.AddTransient<AuthHandler>();
            builder.Services.AddHttpClient<IAuthService, AuthService>(c =>
                    c.BaseAddress = new Uri("https://jkr8tzhd-7204.use.devtunnels.ms/"))
                .AddHttpMessageHandler<AuthHandler>();

            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<InicioSesionCliente>();

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
