using DulceRelax.Shared.DTOs;

namespace DulceRelax.Shared
{
    /// <summary>
    /// Reglas de negocio de una cita. Viven aquí para que la API y las apps usen
    /// exactamente el mismo cálculo y para poder probarlas sin Firestore.
    /// </summary>
    public static class CitaRules
    {
        public const int MinutosPorDefecto = 60;
        public const int DiasAnticipacion = 5;

        public static readonly TimeSpan Descanso = TimeSpan.FromHours(1);
        public static readonly TimeSpan BloqueoMaximo = TimeSpan.FromHours(5);

        /// <summary>Duración efectiva del masaje, con el valor por defecto cuando falta.</summary>
        public static int DuracionMinutos(int duracionMinutos) =>
            duracionMinutos > 0 ? duracionMinutos : MinutosPorDefecto;

        /// <summary>
        /// La cita debe pedirse con al menos <see cref="DiasAnticipacion"/> días de
        /// anticipación. Ambos valores deben estar en la misma zona (UTC o local).
        /// </summary>
        public static bool CumpleAnticipacion(DateTime fechaHora, DateTime ahora) =>
            fechaHora >= ahora.AddDays(DiasAnticipacion);

        /// <summary>
        /// Valida que la cita quepa en el horario de atención. <paramref name="inicio"/>
        /// debe estar en la hora local del negocio.
        /// </summary>
        public static bool EstaDentroDelHorario(
            DateTime inicio, int duracionMinutos, HorarioAtencionDTO? horario)
        {
            if (horario?.Dias is null || horario.Dias.Count == 0) return true;

            if (!horario.Dias.TryGetValue(inicio.DayOfWeek.ToString(), out var dia) || !dia.Activo)
                return false;

            if (!TimeSpan.TryParse(dia.HoraInicio, out var abre) ||
                !TimeSpan.TryParse(dia.HoraFin, out var cierra))
                return false;

            var fin = inicio.AddMinutes(DuracionMinutos(duracionMinutos));
            return fin.Date == inicio.Date
                && inicio.TimeOfDay >= abre
                && fin.TimeOfDay <= cierra;
        }

        /// <summary>
        /// Igual que <see cref="EstaDentroDelHorario"/> pero recibe la cita en UTC y la
        /// convierte a la zona horaria del negocio.
        /// </summary>
        public static bool EstaDentroDelHorarioUtc(
            DateTime inicioUtc, int duracionMinutos, HorarioAtencionDTO? horario, TimeZoneInfo zona)
        {
            var inicio = TimeZoneInfo.ConvertTimeFromUtc(
                DateTime.SpecifyKind(inicioUtc, DateTimeKind.Utc), zona);
            return EstaDentroDelHorario(inicio, duracionMinutos, horario);
        }

        /// <summary>Bloqueo que ocupa una cita: su duración más la hora de descanso.</summary>
        public static TimeSpan Bloqueo(int duracionMinutos) =>
            TimeSpan.FromMinutes(DuracionMinutos(duracionMinutos)) + Descanso;

        public static DateTime FinBloqueo(DateTime inicio, int duracionMinutos) =>
            inicio + Bloqueo(duracionMinutos);

        /// <summary>
        /// Dos citas se cruzan si sus bloques (duración + descanso) se solapan.
        /// Que una empiece justo cuando termina el bloqueo de la otra es válido.
        /// </summary>
        public static bool SeSolapan(
            DateTime inicioA, int duracionA, DateTime inicioB, int duracionB) =>
            inicioA < FinBloqueo(inicioB, duracionB) &&
            inicioB < FinBloqueo(inicioA, duracionA);
    }
}
