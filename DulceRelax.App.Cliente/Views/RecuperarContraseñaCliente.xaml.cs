namespace DulceRelax.App.Cliente.Views;

public partial class RecuperarContraseñaCliente : ContentPage
{
    public RecuperarContraseñaCliente()
    {
        InitializeComponent();
    }

    private async void OnSendClicked(object sender, EventArgs e)
    {
        // TODO: enviar correo de recuperación (fase posterior)
        MessageLabel.Text = "Se ha enviado un correo de recuperación";

        await Task.Delay(1500);

        await Shell.Current.GoToAsync("..");
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}