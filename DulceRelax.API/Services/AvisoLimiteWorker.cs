using DulceRelax.API.Repositories;

namespace DulceRelax.API.Services
{
    /// <summary>
    /// Revisa periódicamente las citas pendientes que se acerzan a su fecha y avisa
    /// a los administradores por push cuando todavía no se han confirmado.
    /// </summary>
    public class AvisoLimiteWorker(
        CitaRepository citas,
        AvisoLimiteRepository avisos,
        PushSender push,
        ILogger<AvisoLimiteWorker> log) : BackgroundService
    {
        private static readonly TimeSpan Intervalo = TimeSpan.FromHours(1);

        /// <summary>
        /// Margen antes de la cita dentro del cual una solicitud sin confirmar
        /// se considera urgente.
        /// </summary>
        public static readonly TimeSpan Umbral = TimeSpan.FromDays(7);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(Intervalo);

            do
            {
                try { await RevisarAsync(); }
                catch (Exception ex) { log.LogError(ex, "Error en aviso de fecha límite"); }
            }
            while (await timer.WaitForNextTickAsync(stoppingToken));
        }

        private async Task RevisarAsync()
        {
            var limite = DateTime.UtcNow + Umbral;
            var pendientes = await citas.GetPendientesAsync();

            foreach (var c in pendientes.Where(c => c.FechaHora.ToUniversalTime() <= limite))
            {
                if (await avisos.ExisteAsync(c.Id)) continue;

                await push.EnviarAAdminsAsync("Solicitud sin confirmar",
                    $"{c.UsuarioNombre} - {c.MasajeNombre} (faltan menos de {Umbral.Days} días)");

                await avisos.MarcarAsync(c.Id);
            }
        }
    }
}
