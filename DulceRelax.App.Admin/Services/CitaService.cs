using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using DulceRelax.Shared.DTOs;

namespace DulceRelax.App.Admin.Services
{
    public interface ICitaService
    {
        Task<(List<RespuestaCitaDTO> Lista, string? Error)> ObtenerPorDiaAsync(DateTime dia);
        Task<(List<RespuestaCitaDTO> Lista, string? Error)> ObtenerPendientesAsync();
        Task<string?> CambiarEstadoAsync(string id, EstadoCita estado);
        Task<(List<RespuestaCitaDTO> Lista, string? Error)> ObtenerAgendadasAsync();
    }

    public class CitaService(HttpClient http) : ICitaService
    {

        public async Task<(List<RespuestaCitaDTO> Lista, string? Error)> ObtenerPorDiaAsync(DateTime dia)
        {
            var desde = new DateTimeOffset(dia.Date);
            var hasta = desde.AddDays(1);
            var url = $"api/cita/rango?desde={Uri.EscapeDataString(desde.ToString("o"))}" +
                      $"&hasta={Uri.EscapeDataString(hasta.ToString("o"))}";
            return await ObtenerListaAsync(url);
        }

        public Task<(List<RespuestaCitaDTO> Lista, string? Error)> ObtenerPendientesAsync()
            => ObtenerListaAsync("api/cita/pendientes");

        public async Task<string?> CambiarEstadoAsync(string id, EstadoCita estado)
        {
            try
            {
                var res = await http.PatchAsync($"api/cita/{id}/estado?estado={estado}", null);
                if (res.IsSuccessStatusCode) return null;

                return res.StatusCode switch
                {
                    HttpStatusCode.Unauthorized => "Sesión expirada",
                    HttpStatusCode.Forbidden => "No tienes permiso para hacer esto",
                    _ => await LeerError(res, "No se pudo cambiar el estado de la cita")
                };
            }
            catch (HttpRequestException) { return "Sin conexión con el servidor"; }
            catch (TaskCanceledException) { return "Tiempo de espera agotado"; }
        }

        private async Task<(List<RespuestaCitaDTO> Lista, string? Error)> ObtenerListaAsync(string url)
        {
            try
            {
                var res = await http.GetAsync(url);
                if (res.StatusCode == HttpStatusCode.Unauthorized) return ([], "Sesión expirada");
                if (Red.NoDisponible(res.StatusCode)) return ([], Red.SinConexion);
                if (!res.IsSuccessStatusCode) return ([], "No se pudieron cargar las citas");

                var lista = await res.Content.ReadFromJsonAsync<List<RespuestaCitaDTO>>() ?? [];
                return (lista, null);
            }
            catch (HttpRequestException) { return ([], Red.SinConexion); }
            catch (TaskCanceledException) { return ([], Red.TiempoAgotado); }
        }

        static async Task<string> LeerError(HttpResponseMessage res, string porDefecto)
        {
            try
            {
                var json = await res.Content.ReadFromJsonAsync<JsonElement>();
                return json.GetProperty("error").GetString() ?? porDefecto;
            }
            catch { return porDefecto; }
        }

        public Task<(List<RespuestaCitaDTO> Lista, string? Error)> ObtenerAgendadasAsync()
        {
            var desde = new DateTimeOffset(DateTime.Today);
            var hasta = desde.AddDays(2);
            return ObtenerListaAsync(
                $"api/cita/agendadas?desde={Uri.EscapeDataString(desde.ToString("o"))}" +
                $"&hasta={Uri.EscapeDataString(hasta.ToString("o"))}");
        }
    }
}