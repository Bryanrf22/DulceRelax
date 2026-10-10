using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DulceRelax.App.Cliente.Services;

namespace DulceRelax.App.Cliente.ViewModels
{
    public partial class RecuperarPasswordViewModel(IAuthService auth) : ObservableObject
    {
        [ObservableProperty] private string email = "";
        [ObservableProperty] private string? mensaje;
        [ObservableProperty] private bool esError;

        [RelayCommand]
        private async Task EnviarAsync()
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                EsError = true;
                Mensaje = "Ingresa tu correo.";
                return;
            }

            var error = await auth.RecuperarPasswordAsync(Email.Trim());
            EsError = error is not null;
            Mensaje = error ?? "Si el correo está registrado, recibirás un enlace para restablecer tu contraseña.";

            if (error is null)
            {
                await Task.Delay(2500);
                await Shell.Current.GoToAsync("..");
            }
        }

        [RelayCommand]
        private Task VolverAsync() => Shell.Current.GoToAsync("..");
    }
}