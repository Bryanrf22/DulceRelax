using DulceRelax.App.Admin.Services;
using DulceRelax.App.Admin.ViewModels;

namespace DulceRelax.App.Admin.Views;

public partial class InicioSesionAdmin : ContentPage
{
    private static bool _sesionRevisada;
    private readonly LoginViewModel _vm;

    public InicioSesionAdmin(LoginViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _vm = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (_sesionRevisada) return;
        _sesionRevisada = true;
        await _vm.RestaurarSesionCommand.EjecutarSeguroAsync();
    }

    private void OnTogglePassword(object sender, TappedEventArgs e)
    {
        PasswordEntry.IsPassword = !PasswordEntry.IsPassword;
        var clave = PasswordEntry.IsPassword ? "IconoVer" : "IconoOcultar";
        if (Application.Current?.Resources.TryGetValue(clave, out var glyph) == true && glyph is string texto)
            PasswordToggle.Text = texto;
    }
}