namespace DulceRelax.App.Cliente.Views;

public partial class PerfilClienteEdit : ContentPage
{
    public PerfilClienteEdit()
    {
        InitializeComponent();

        // TODO: cargar datos reales del usuario desde la API (fase posterior)
        EmailEntry.Text = "cliente@gmail.com";
        PasswordEntry.Text = "123456";
        NombreEntry.Text = "Nombre del cliente";
        TelefonoEntry.Text = "6769-4201";
        DireccionEntry.Text = "De los helados El pepe 67m al sur pegado a donde vivia doña Mary";
    }

    // TODO: guardar cambios en la API (fase posterior)
    private async void OnSaveClicked(object sender, EventArgs e)
    {
        await Shell.Current.Navigation.PopAsync();
    }

    private void OnSaveEntered(object sender, PointerEventArgs e)
    {
        SaveButton.BackgroundColor = Color.FromArgb("#2F7047");
    }

    private void OnSaveExited(object sender, PointerEventArgs e)
    {
        SaveButton.BackgroundColor = Color.FromArgb("#3F8758");
    }
}
