using DulceRelax.App.Cliente.ViewModels;

namespace DulceRelax.App.Cliente.Views;

public partial class PerfilClienteEdit : ContentPage
{
    public PerfilClienteEdit(PerfilEditViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}