using DulceRelax.App.Admin.Services;
using DulceRelax.App.Admin.ViewModels;

namespace DulceRelax.App.Admin.Views;

public partial class AjustesAdmin : ContentPage
{
    private readonly AjustesViewModel _vm;

    public AjustesAdmin(AjustesViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (_vm.Cargado) return;
        await _vm.CargarCommand.EjecutarSeguroAsync();
    }
}