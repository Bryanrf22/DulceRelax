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

    private async void OnForgotPasswordClicked(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(RecuperarContraseñaCliente));
    }

    private async void OnRegisterClicked(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(RegistroCliente));
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (_sesionRevisada) return;
        _sesionRevisada = true;

        if (BindingContext is LoginViewModel vm)
            await vm.RestaurarSesionCommand.ExecuteAsync(null);
    }
}