using DulceRelax.App.Admin.ViewModels;

namespace DulceRelax.App.Admin.Views;

public partial class InicioSesionAdmin : ContentPage
{
    public InicioSesionAdmin(LoginViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}