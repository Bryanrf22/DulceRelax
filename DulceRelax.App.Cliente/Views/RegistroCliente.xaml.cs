using DulceRelax.App.Cliente.ViewModels;

namespace DulceRelax.App.Cliente.Views;

public partial class RegistroCliente : ContentPage
{
    public RegistroCliente(RegistroViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}