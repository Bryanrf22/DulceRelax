using DulceRelax.App.Cliente.ViewModels;

namespace DulceRelax.App.Cliente.Views;

public partial class ReservaMasajeCliente : ContentPage
{
    public ReservaMasajeCliente(ReservaViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}