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

        [RelayCommand]
        private async Task LoginAsync()
        {
            Error = await auth.LoginAsync(new LoginDTO { Email = Email, Password = Password });
            if (Error is null)
                await Shell.Current.GoToAsync(nameof(PrincipalCliente));
        }
    }
}
