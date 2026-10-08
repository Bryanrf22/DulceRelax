namespace DulceRelax.App.Cliente.Views;

public partial class InicioSesionCliente : ContentPage
{
    public InicioSesionCliente()
    {
        InitializeComponent();
    }

    // TODO: fase posterior - validar credenciales contra la API.
    // Por ahora el acceso es libre, sin validación de credenciales.
    private async void OnLoginClicked(object sender, EventArgs e)
    {
        MessageLabel.Text = string.Empty;

        await Shell.Current.GoToAsync(nameof(PrincipalCliente));
    }

    private async void OnForgotPasswordClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(RecuperarContraseñaCliente));
    }

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(RegistroCliente));
    }
}