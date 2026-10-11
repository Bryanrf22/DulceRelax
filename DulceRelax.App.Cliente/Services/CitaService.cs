using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using DulceRelax.Shared.DTOs;

namespace DulceRelax.App.Cliente.Services
{
    public interface ICitaService
    {
        Task<string?> CrearAsync(RegistrarCitaDTO dto);
        Task<string?> ActualizarAsync(string id, ActualizarCitaDTO dto);
        Task<string?> CancelarAsync(string id);
        Task<(List<RespuestaCitaDTO> Lista, string? Error)> ObtenerMiasAsync();
    }

    public class CitaService(HttpClient http) : ICitaService
    {
        public async Task<string?> CrearAsync(RegistrarCitaDTO dto)
        {
            try
            {
                var res = await http.PostAsJsonAsync("api/cita", dto);
                return await ResultadoAsync(res, "No se pudo crear la cita");
            }
            catch (HttpRequestException) { return "Sin conexión con el servidor"; }
            catch (TaskCanceledException) { return "Tiempo de espera agotado"; }
        }

        public async Task<string?> ActualizarAsync(string id, ActualizarCitaDTO dto)
        {
            try
            {
                var res = await http.PutAsJsonAsync($"api/cita/{id}", dto);
                return await ResultadoAsync(res, "No se pudo actualizar la cita");
            }
            catch (HttpRequestException) { return "Sin conexión con el servidor"; }
            catch (TaskCanceledException) { return "Tiempo de espera agotado"; }
        }

        public async Task<string?> CancelarAsync(string id)
        {
            try
            {
                var res = await http.PatchAsync($"api/cita/{id}/estado?estado=Cancelada", null);
                return await ResultadoAsync(res, "No se pudo cancelar la cita");
            }
            catch (HttpRequestException) { return "Sin conexión con el servidor"; }
            catch (TaskCanceledException) { return "Tiempo de espera agotado"; }
        }

        public async Task<(List<RespuestaCitaDTO> Lista, string? Error)> ObtenerMiasAsync()
        {
            try
            {
                var res = await http.GetAsync("api/cita/mias");
                if (res.StatusCode == HttpStatusCode.Unauthorized) return ([], "Sesión expirada");
                if (!res.IsSuccessStatusCode) return ([], "No se pudieron cargar tus citas");

                var lista = await res.Content.ReadFromJsonAsync<List<RespuestaCitaDTO>>() ?? [];
                return (lista, null);
            }
            catch (HttpRequestException) { return ([], "Sin conexión con el servidor"); }
            catch (TaskCanceledException) { return ([], "Tiempo de espera agotado"); }
        }

        static async Task<string?> ResultadoAsync(HttpResponseMessage res, string porDefecto)
        {
            if (res.IsSuccessStatusCode) return null;

            return res.StatusCode switch
            {
                HttpStatusCode.Unauthorized => "Sesión expirada",
                HttpStatusCode.Forbidden => "No tienes permiso para hacer esto",
                _ => await LeerError(res, porDefecto)
            };
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
    }
}