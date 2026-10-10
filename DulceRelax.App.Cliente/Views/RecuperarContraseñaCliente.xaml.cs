using DulceRelax.App.Cliente.ViewModels;

namespace DulceRelax.App.Cliente.Views;

public partial class RecuperarContraseñaCliente : ContentPage
{
    public RecuperarContraseñaCliente(RecuperarPasswordViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}