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

            var apiUrl = new Uri("https://jkr8tzhd-7204.use.devtunnels.ms/");

            builder.Services.AddTransient<AuthHandler>();
            builder.Services.AddHttpClient<IAuthService, AuthService>(c => c.BaseAddress = apiUrl)
                .AddHttpMessageHandler<AuthHandler>();
            builder.Services.AddHttpClient<IMasajeService, MasajeService>(c => c.BaseAddress = apiUrl)
                .AddHttpMessageHandler<AuthHandler>();

            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<InicioSesionCliente>();
            builder.Services.AddTransient<RegistroViewModel>();
            builder.Services.AddTransient<RegistroCliente>();
            builder.Services.AddTransient<RecuperarPasswordViewModel>();
            builder.Services.AddTransient<RecuperarContraseñaCliente>();
            builder.Services.AddTransient<PrincipalViewModel>();
            builder.Services.AddTransient<PrincipalCliente>();

            builder.Services.AddHttpClient<ICitaService, CitaService>(c => c.BaseAddress = apiUrl)
                .AddHttpMessageHandler<AuthHandler>();
            builder.Services.AddHttpClient<IUsuarioService, UsuarioService>(c => c.BaseAddress = apiUrl)
                .AddHttpMessageHandler<AuthHandler>();

            builder.Services.AddTransient<ReservaViewModel>();
            builder.Services.AddTransient<ReservaMasajeCliente>();
            builder.Services.AddTransient<MisCitasViewModel>();
            builder.Services.AddTransient<MisCitasCliente>();

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