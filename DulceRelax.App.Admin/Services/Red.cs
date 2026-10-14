using System.Net;

namespace DulceRelax.App.Admin.Services
{
    public static class Red
    {
        public const string SinConexion = "Sin conexión con el servidor";
        public const string TiempoAgotado = "Tiempo de espera agotado";

        public static bool EsError(string? error) => error is SinConexion or TiempoAgotado;

        public static bool NoDisponible(HttpStatusCode codigo) =>
            codigo is HttpStatusCode.BadGateway or HttpStatusCode.ServiceUnavailable
                or HttpStatusCode.GatewayTimeout;
    }
}