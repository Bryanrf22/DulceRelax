using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DulceRelax.App.Cliente.Services;
using DulceRelax.App.Cliente.Views;
using DulceRelax.Shared.DTOs;

namespace DulceRelax.App.Cliente.ViewModels
{
    public partial class RegistroViewModel(IAuthService auth) : ObservableObject
    {
        [ObservableProperty] private string email = "";
        [ObservableProperty] private string password = "";
        [ObservableProperty] private string nombre = "";
        [ObservableProperty] private string telefono = "";
        [ObservableProperty] private string direccion = "";
        [ObservableProperty] private string? error;

        [RelayCommand]
        private async Task RegistrarAsync()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password) ||
                string.IsNullOrWhiteSpace(Nombre) || string.IsNullOrWhiteSpace(Telefono) || string.IsNullOrWhiteSpace(Direccion))
            {
                Error = "Completa todos los campos.";
                return;
            }
            if (Password.Length < 6)
            {
                Error = "La contraseña debe tener al menos 6 caracteres.";
                return;
            }
            
            var correo = Email.Trim();

            Error = await auth.RegistrarAsync(new RegistrarUsuarioDTO
            {
                Correo = correo,
                Password = Password,
                NombreCompleto = Nombre.Trim(),
                NumTelefono = Telefono.Trim(),
                Direccion = Direccion.Trim()
            });
            if (Error is not null) return;

            Error = await auth.LoginAsync(new LoginDTO { Email = correo, Password = Password });
            if (Error is null)
                await Shell.Current.GoToAsync(nameof(PrincipalCliente));
        }

    }
}
