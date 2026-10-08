namespace DulceRelax.App.Cliente.Views;

public partial class MisCitasCliente : ContentPage
{
    public MisCitasCliente()
    {
        InitializeComponent();
        // TODO: cargar citas del usuario desde la API (fase posterior)
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}