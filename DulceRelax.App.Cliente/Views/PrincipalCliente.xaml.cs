namespace DulceRelax.App.Cliente.Views;

public partial class PrincipalCliente : ContentPage
{
    public PrincipalCliente()
    {
        InitializeComponent();
    }

    private async void OnRelaxClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(ReservaMasajeCliente));
    }

    private async void OnDeepClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(ReservaMasajeCliente));
    }

    private async void OnHomeClicked(object sender, EventArgs e)
    {
        // Si ya está en Principal no hace nada; desde otra vista vuelve a ella.
        if (Shell.Current.CurrentPage is not PrincipalCliente)
        {
            await Shell.Current.GoToAsync("..");
        }
    }

    private async void OnProfileTapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(PerfilCliente));
    }

    private async void OnCalendarTapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(MisCitasCliente));
    }
}