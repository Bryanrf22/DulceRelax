using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DulceRelax.App.Admin.Services;
using DulceRelax.Shared.DTOs;

namespace DulceRelax.App.Admin.ViewModels
{
    public partial class LoginViewModel(
        IAuthService auth, NotificationService notificaciones, PushService push) : ObservableObject
    {
        [ObservableProperty] private string email = "";
        [ObservableProperty] private string password = "";
        [ObservableProperty] private string? error;
        [ObservableProperty] private bool recordarme;

        [RelayCommand]
        private async Task LoginAsync()
        {
            try
            {
                Error = await auth.LoginAsync(new LoginDTO { Email = Email.Trim(), Password = Password }, Recordarme);
                if (Error is null)
                {
                    Password = "";
                    await ActivarNotificacionesAsync();
                    await Shell.Current.GoToAsync("//Principal");
                }
            }
            catch (Exception)
            {
                Error = "Ocurrió un error inesperado. Inténtalo de nuevo.";
            }
        }

        [RelayCommand]
        private async Task RestaurarSesionAsync()
        {
            try
            {
                if (await auth.RestaurarSesionAsync())
                {
                    await ActivarNotificacionesAsync();
                    await Shell.Current.GoToAsync("//Principal");
                }
            }
            catch { }
        }

        /// <summary>
        /// Pide permisos de notificación y registra este dispositivo para recibir
        /// los avisos de nuevas solicitudes y cancelaciones.
        /// </summary>
        private async Task ActivarNotificacionesAsync()
        {
            try { await notificaciones.PedirPermisosAsync(); }
            catch { }

            await push.IniciarAsync();
        }
    }
}