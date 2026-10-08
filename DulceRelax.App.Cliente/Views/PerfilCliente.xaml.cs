namespace DulceRelax.App.Cliente.Views;

public partial class PerfilCliente : ContentPage
{
    public PerfilCliente()
    {
        InitializeComponent();

        // TODO: cargar datos reales del usuario desde la API (fase posterior)
        EmailEntry.Text = "cliente@gmail.com";
        PasswordEntry.Text = "123456";
        NombreEntry.Text = "Nombre del cliente";
        TelefonoEntry.Text = "6769-4201";
        DireccionEntry.Text = "De los helados El pepe 67m al sur pegado a donde vivia doña Mary";
    }

    private async void OnEditClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(PerfilClienteEdit));
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}