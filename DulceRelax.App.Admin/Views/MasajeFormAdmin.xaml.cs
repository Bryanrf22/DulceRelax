using DulceRelax.App.Admin.ViewModels;

namespace DulceRelax.App.Admin.Views;

public partial class MasajeFormAdmin : ContentPage
{
    public MasajeFormAdmin(MasajeFormViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}