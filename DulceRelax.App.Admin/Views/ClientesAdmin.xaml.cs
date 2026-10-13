using DulceRelax.App.Admin.ViewModels;

namespace DulceRelax.App.Admin.Views;

public partial class ClientesAdmin : ContentPage
{
    private readonly ClientesViewModel _vm;

    public ClientesAdmin(ClientesViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.CargarCommand.ExecuteAsync(null);
    }
}