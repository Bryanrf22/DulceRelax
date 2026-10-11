using DulceRelax.App.Cliente.ViewModels;

namespace DulceRelax.App.Cliente.Views;

public partial class PrincipalCliente : ContentPage
{
    private readonly PrincipalViewModel _vm;

    public PrincipalCliente(PrincipalViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.CargarCommand.ExecuteAsync(null);
    }

    private async void OnHomeClicked(object sender, EventArgs e)
    {
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