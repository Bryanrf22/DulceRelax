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

            builder.Services.AddTransient<AppShell>();
            builder.Services.AddHttpClient<ICitaService, CitaService>(c => c.BaseAddress = apiUrl)
                .AddHttpMessageHandler<AuthHandler>();
            builder.Services.AddTransient<AgendaViewModel>();
            builder.Services.AddTransient<SolicitudesViewModel>();
            builder.Services.AddTransient<SolicitudesAdmin>();

            builder.Services.AddHttpClient<IMasajeService, MasajeService>(c => c.BaseAddress = apiUrl)
                .AddHttpMessageHandler<AuthHandler>();
            builder.Services.AddTransient<MasajesViewModel>();
            builder.Services.AddTransient<MasajesAdmin>();
            builder.Services.AddTransient<MasajeFormViewModel>();
            builder.Services.AddTransient<MasajeFormAdmin>();

            builder.Services.AddHttpClient<IUsuarioService, UsuarioService>(c => c.BaseAddress = apiUrl)
                .AddHttpMessageHandler<AuthHandler>();
            builder.Services.AddTransient<ClientesViewModel>();
            builder.Services.AddTransient<ClientesAdmin>();

            builder.Services.AddHttpClient<IHorarioService, HorarioService>(c => c.BaseAddress = apiUrl)
                .AddHttpMessageHandler<AuthHandler>();
            builder.Services.AddTransient<AjustesViewModel>();
            builder.Services.AddTransient<AjustesAdmin>();

            builder.Services.AddSingleton<ICitaCacheService, CitaCacheService>();
            builder.Services.AddTransient<RedHandler>();
            builder.Services.ConfigureHttpClientDefaults(b => b.AddHttpMessageHandler<RedHandler>());

            builder.Services.AddSingleton<LocalDb>();
            builder.Services.AddSingleton<ICuentaOfflineService, CuentaOfflineService>();
            builder.Services.AddSingleton<IClienteCacheService, ClienteCacheService>();
            builder.Services.AddTransient<ISincronizadorService, SincronizadorService>();

            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("MaterialIcons-Regular.ttf", "MaterialIcons");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
