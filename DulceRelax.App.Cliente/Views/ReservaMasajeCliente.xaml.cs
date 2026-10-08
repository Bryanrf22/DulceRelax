namespace DulceRelax.App.Cliente.Views;

public partial class ReservaMasajeCliente : ContentPage
{
    public ReservaMasajeCliente()
    {
        InitializeComponent();
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private async void OnReserveClicked(object sender, EventArgs e)
    {
        // TODO: validar datos y registrar cita en la API (fase posterior)
        await Shell.Current.GoToAsync("..");
    }
}