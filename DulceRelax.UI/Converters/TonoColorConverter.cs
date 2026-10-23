using System.Globalization;
using DulceRelax.UI.Models;

namespace DulceRelax.UI.Converters;

public sealed class TonoColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var esFondo = string.Equals(parameter as string, "Fondo", StringComparison.OrdinalIgnoreCase);

        var clave = value switch
        {
            TonoEstado.Exito => esFondo ? "SuccessSoft" : "Success",
            TonoEstado.Error => esFondo ? "ErrorSoft" : "Error",
            TonoEstado.Aviso => esFondo ? "WarningSoft" : "WarningText",
            _ => esFondo ? "SurfaceMuted" : "OnSurfaceVariant"
        };

        return Application.Current?.Resources.TryGetValue(clave, out var recurso) == true
               && recurso is Color color
            ? color
            : Colors.Transparent;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
