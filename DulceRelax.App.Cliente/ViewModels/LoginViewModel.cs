using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DulceRelax.App.Cliente.Services;
using DulceRelax.Shared.DTOs;
using DulceRelax.App.Cliente.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DulceRelax.App.Cliente.ViewModels
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
                await Shell.Current.GoToAsync(nameof(PrincipalCliente));
        }

        [RelayCommand]
        private async Task RestaurarSesionAsync()
        {
            if (await auth.RestaurarSesionAsync())
                await Shell.Current.GoToAsync(nameof(PrincipalCliente));
        }
    }
}
