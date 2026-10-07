namespace DulceRelax.App.Cliente.Views;

public partial class InicioSesionCliente : ContentPage
{
    public InicioSesionCliente()
    {
        InitializeComponent();
    }

    // TODO: validar credenciales contra la API (fase posterior)
    private async void OnLoginClicked(object sender, EventArgs e)
    {
        MessageLabel.Text = string.Empty;

        await Shell.Current.GoToAsync(nameof(PrincipalCliente));
    }

    private void OnButtonPointerEntered(object sender, PointerEventArgs e)
    {
        LoginButton.BackgroundColor = Color.FromArgb("#2F7047");
    }

    private void OnButtonPointerExited(object sender, PointerEventArgs e)
    {
        LoginButton.BackgroundColor = Color.FromArgb("#3F8758");
    }

    private void OnForgotPasswordEntered(object sender, PointerEventArgs e)
    {
        ForgotPasswordLabel.TextColor = Color.FromArgb("#245C38");
    }

    private void OnForgotPasswordExited(object sender, PointerEventArgs e)
    {
        ForgotPasswordLabel.TextColor = Color.FromArgb("#3F8758");
    }

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(RegistroCliente));
    }

    private void OnRegisterEntered(object sender, PointerEventArgs e)
    {
        RegisterLabel.TextColor = Color.FromArgb("#245C38");
    }

    private void OnRegisterExited(object sender, PointerEventArgs e)
    {
        RegisterLabel.TextColor = Color.FromArgb("#3F8758");
    }
}
