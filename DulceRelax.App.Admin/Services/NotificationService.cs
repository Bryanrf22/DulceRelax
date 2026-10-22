using DulceRelax.Shared.DTOs;
using Plugin.LocalNotification;

public class NotificationService
{
    public Task<bool> PedirPermisosAsync() =>
        LocalNotificationCenter.Current.RequestNotificationPermission(
            new NotificationPermission
            {
                Android = { RequestPermissionToScheduleExactAlarm = true }
            });

    public async Task ReconciliarAsync(IEnumerable<RespuestaCitaDTO> citasConfirmadas)
    {
        var pendientes = await LocalNotificationCenter.Current.GetPendingNotificationList();
        var ids = pendientes.Select(n => n.NotificationId).ToArray();
        if (ids.Length > 0)
            LocalNotificationCenter.Current.Cancel(ids);

        foreach (var cita in citasConfirmadas)
        {
            var fecha = DateTime.SpecifyKind(cita.FechaHora, DateTimeKind.Utc).ToLocalTime();
            var detalle = $"{cita.UsuarioNombre} - {cita.MasajeNombre}";

            await ProgramarAsync(Id(cita.Id, 0), fecha.AddDays(-1), "Cita mañana", detalle);
            await ProgramarAsync(Id(cita.Id, 1), fecha.AddHours(-1), "Cita en 1 hora", detalle);
        }
    }

    public void Cancelar(string citaId) =>
        LocalNotificationCenter.Current.Cancel(Id(citaId, 0), Id(citaId, 1));

    private static async Task ProgramarAsync(int id, DateTime cuando, string titulo, string texto)
    {
        if (cuando <= DateTime.Now) return;

        await LocalNotificationCenter.Current.Show(new NotificationRequest
        {
            NotificationId = id,
            Title = titulo,
            Description = texto,
            Schedule = new NotificationRequestSchedule { NotifyTime = cuando }
        });
    }

    // string.GetHashCode() cambia entre ejecuciones; este hash es estable
    private static int Id(string citaId, int tipo)
    {
        unchecked
        {
            int h = 17;
            foreach (var c in citaId) h = h * 31 + c;
            return ((h & 0x3FFFFFFF) << 1) | tipo;
        }
    }
}