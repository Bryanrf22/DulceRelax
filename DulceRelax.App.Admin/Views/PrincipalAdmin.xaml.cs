using DulceRelax.App.Admin.Services;
using DulceRelax.App.Admin.ViewModels;

namespace DulceRelax.App.Admin.Views;

public partial class PrincipalAdmin : ContentPage
{
    private readonly AgendaViewModel _vm;
    private readonly IAuthService _auth;

    public PrincipalAdmin(AgendaViewModel vm, IAuthService auth)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
        _auth = auth;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.CargarCommand.ExecuteAsync(null);
    }

    private async void OnCerrarSesion(object sender, EventArgs e)
    {
        _auth.CerrarSesion();
        await Shell.Current.GoToAsync("//InicioSesion");
    }
}