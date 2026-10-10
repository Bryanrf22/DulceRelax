using DulceRelax.App.Cliente.ViewModels;

namespace DulceRelax.App.Cliente.Views;

public partial class InicioSesionCliente : ContentPage
{
    public InicioSesionCliente(LoginViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    private static bool _sesionRevisada;

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (_sesionRevisada) return;
        _sesionRevisada = true;

        if (BindingContext is LoginViewModel vm)
            await vm.RestaurarSesionCommand.ExecuteAsync(null);
    }
}