using DulceRelax.App.Admin.Services;
using DulceRelax.App.Admin.ViewModels;

namespace DulceRelax.App.Admin.Views;

public partial class PrincipalAdmin : ContentPage
{
    private readonly AgendaViewModel _vm;

    public PrincipalAdmin(AgendaViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.CargarCommand.EjecutarSeguroAsync();
    }
}