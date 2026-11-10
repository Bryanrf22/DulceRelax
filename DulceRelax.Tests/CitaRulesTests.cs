using DulceRelax.Shared;
using DulceRelax.Shared.DTOs;
using Xunit;

namespace DulceRelax.Tests
{
    public class CitaRulesTests
    {
        private static readonly DateTime Ahora = new(2026, 11, 10, 9, 0, 0, DateTimeKind.Utc);

        // Zona fija UTC-6 para no depender de la base de zonas horarias del equipo.
        private static readonly TimeZoneInfo Zona = TimeZoneInfo.CreateCustomTimeZone(
            "Test/UTC-6", TimeSpan.FromHours(-6), "Test/UTC-6", "Test/UTC-6");

        private static HorarioAtencionDTO Horario(
            bool activo, string inicio, string fin, DayOfWeek dia = DayOfWeek.Monday) => new()
        {
            Dias = new Dictionary<string, DiaHorarioDTO>
            {
                [dia.ToString()] = new()
                {
                    Activo = activo,
                    HoraInicio = inicio,
                    HoraFin = fin
                }
            }
        };

        [Theory]
        [InlineData(5, 0, true)]
        [InlineData(4, 23, false)]
        [InlineData(-1, 0, false)]
        public void CumpleAnticipacion_exige_cinco_dias(int dias, int horas, bool esperado)
        {
            var fecha = Ahora.AddDays(dias).AddHours(horas);

            Assert.Equal(esperado, CitaRules.CumpleAnticipacion(fecha, Ahora));
        }

        [Theory]
        [InlineData(0, 60)]
        [InlineData(-15, 60)]
        [InlineData(45, 45)]
        [InlineData(90, 90)]
        public void DuracionMinutos_usa_sesenta_cuando_falta_el_dato(int entrada, int esperado)
        {
            Assert.Equal(esperado, CitaRules.DuracionMinutos(entrada));
        }

        [Fact]
        public void Sin_horario_configurado_cualquier_hora_es_valida()
        {
            Assert.True(CitaRules.EstaDentroDelHorario(
                new DateTime(2026, 11, 9, 3, 0, 0), 60, null));
            Assert.True(CitaRules.EstaDentroDelHorario(
                new DateTime(2026, 11, 9, 3, 0, 0), 60, new HorarioAtencionDTO()));
        }

        [Fact]
        public void Rechaza_un_dia_inactivo()
        {
            var horario = Horario(false, "10:00", "18:00");

            Assert.False(CitaRules.EstaDentroDelHorario(
                new DateTime(2026, 11, 9, 12, 0, 0), 60, horario));
        }

        [Fact]
        public void Acepta_una_cita_dentro_del_horario()
        {
            var horario = Horario(true, "10:00", "18:00");

            Assert.True(CitaRules.EstaDentroDelHorario(
                new DateTime(2026, 11, 9, 10, 0, 0), 60, horario));
        }

        [Fact]
        public void Acepta_una_cita_que_termina_justo_al_cerrar()
        {
            var horario = Horario(true, "10:00", "18:00");

            Assert.True(CitaRules.EstaDentroDelHorario(
                new DateTime(2026, 11, 9, 17, 0, 0), 60, horario));
        }

        [Fact]
        public void Rechaza_una_cita_que_empieza_antes_de_abrir()
        {
            var horario = Horario(true, "10:00", "18:00");

            Assert.False(CitaRules.EstaDentroDelHorario(
                new DateTime(2026, 11, 9, 9, 30, 0), 60, horario));
        }

        [Fact]
        public void Rechaza_una_cita_que_termina_despues_de_cerrar()
        {
            var horario = Horario(true, "10:00", "18:00");

            Assert.False(CitaRules.EstaDentroDelHorario(
                new DateTime(2026, 11, 9, 17, 30, 0), 60, horario));
        }

        [Fact]
        public void Rechaza_una_cita_que_cruza_la_medianoche()
        {
            var horario = Horario(true, "10:00", "23:59");

            Assert.False(CitaRules.EstaDentroDelHorario(
                new DateTime(2026, 11, 9, 23, 30, 0), 60, horario));
        }

        [Theory]
        [InlineData("abre", "cierra")]
        [InlineData("99:99", "18:00")]
        [InlineData("10:00", "99:99")]
        public void Rechaza_un_horario_con_horas_invalidas(string inicio, string fin)
        {
            var horario = Horario(true, inicio, fin);

            Assert.False(CitaRules.EstaDentroDelHorario(
                new DateTime(2026, 11, 9, 12, 0, 0), 60, horario));
        }

        [Fact]
        public void Convierte_la_hora_utc_a_la_zona_del_negocio()
        {
            var horario = Horario(true, "10:00", "18:00");
            var inicioUtc = new DateTime(2026, 11, 9, 16, 0, 0, DateTimeKind.Utc);

            Assert.True(CitaRules.EstaDentroDelHorarioUtc(inicioUtc, 60, horario, Zona));
        }

        [Fact]
        public void Rechaza_la_hora_fuera_de_la_zona_del_negocio()
        {
            var horario = Horario(true, "10:00", "18:00");
            var inicioUtc = new DateTime(2026, 11, 9, 4, 0, 0, DateTimeKind.Utc);

            Assert.False(CitaRules.EstaDentroDelHorarioUtc(inicioUtc, 60, horario, Zona));
        }

        [Fact]
        public void El_bloqueo_incluye_la_hora_de_descanso()
        {
            var inicio = new DateTime(2026, 11, 9, 10, 0, 0);

            Assert.Equal(TimeSpan.FromMinutes(120), CitaRules.Bloqueo(60));
            Assert.Equal(inicio.AddHours(2), CitaRules.FinBloqueo(inicio, 60));
        }

        [Fact]
        public void Dos_citas_seguidas_con_una_hora_de_descanso_no_se_solapan()
        {
            var primera = new DateTime(2026, 11, 9, 10, 0, 0);
            var siguiente = new DateTime(2026, 11, 9, 12, 0, 0);

            Assert.False(CitaRules.SeSolapan(primera, 60, siguiente, 60));
        }

        [Fact]
        public void Una_cita_dentro_de_la_hora_de_descanso_se_solapa()
        {
            var primera = new DateTime(2026, 11, 9, 10, 0, 0);
            var siguiente = new DateTime(2026, 11, 9, 11, 30, 0);

            Assert.True(CitaRules.SeSolapan(primera, 60, siguiente, 60));
        }

        [Fact]
        public void El_solapamiento_es_simetrico()
        {
            var primera = new DateTime(2026, 11, 9, 10, 0, 0);
            var siguiente = new DateTime(2026, 11, 9, 11, 0, 0);

            Assert.True(CitaRules.SeSolapan(primera, 60, siguiente, 60));
            Assert.True(CitaRules.SeSolapan(siguiente, 60, primera, 60));
        }

        [Fact]
        public void Citas_en_dias_distintos_no_se_solapan()
        {
            var primera = new DateTime(2026, 11, 9, 10, 0, 0);
            var siguiente = new DateTime(2026, 11, 10, 10, 0, 0);

            Assert.False(CitaRules.SeSolapan(primera, 60, siguiente, 60));
        }
    }
}
