using DulceRelax.App.Admin.ViewModels;

namespace DulceRelax.App.Admin.Views;

public partial class SolicitudesAdmin : ContentPage
{
    private readonly SolicitudesViewModel _vm;

    public SolicitudesAdmin(SolicitudesViewModel vm)
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