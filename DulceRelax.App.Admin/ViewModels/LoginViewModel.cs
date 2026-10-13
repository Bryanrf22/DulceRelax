using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DulceRelax.App.Admin.Services;
using DulceRelax.Shared.DTOs;

namespace DulceRelax.App.Admin.ViewModels
{
    public partial class LoginViewModel(IAuthService auth) : ObservableObject
    {
        [ObservableProperty] private string email = "";
        [ObservableProperty] private string password = "";
        [ObservableProperty] private string? error;
        [ObservableProperty] private bool recordarme;

        [RelayCommand]
        private async Task LoginAsync()
        {
            Error = await auth.LoginAsync(new LoginDTO { Email = Email.Trim(), Password = Password }, Recordarme);
            if (Error is null)
            {
                Password = "";
                await Shell.Current.GoToAsync("//Principal");
            }
        }

        [RelayCommand]
        private async Task RestaurarSesionAsync()
        {
            if (await auth.RestaurarSesionAsync())
                await Shell.Current.GoToAsync("//Principal");
        }
    }
}