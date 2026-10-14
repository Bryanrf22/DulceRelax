using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using DulceRelax.Shared.DTOs;

namespace DulceRelax.App.Admin.Services
{
    public interface IMasajeService
    {
        Task<(List<MasajeDTO> Lista, string? Error)> ObtenerTodosAsync();
        Task<string?> CrearAsync(RegistrarMasajeDTO dto);
        Task<string?> ActualizarAsync(string id, ActualizarMasajeDTO dto);
    }

    public class MasajeService(HttpClient http) : IMasajeService
    {
        public async Task<(List<MasajeDTO> Lista, string? Error)> ObtenerTodosAsync()
        {
            try
            {
                var res = await http.GetAsync("api/masaje");
                if (res.StatusCode == HttpStatusCode.Unauthorized) return ([], "Sesión expirada");
                if (!res.IsSuccessStatusCode) return ([], "No se pudieron cargar los masajes");

                var lista = await res.Content.ReadFromJsonAsync<List<MasajeDTO>>() ?? [];
                return (lista, null);
            }
            catch (HttpRequestException) { return ([], "Sin conexión con el servidor"); }
            catch (TaskCanceledException) { return ([], "Tiempo de espera agotado"); }
        }

        public async Task<string?> CrearAsync(RegistrarMasajeDTO dto)
        {
            try
            {
                var res = await http.PostAsJsonAsync("api/masaje/registrar", dto);
                return await ResultadoAsync(res, "No se pudo crear el masaje");
            }
            catch (HttpRequestException) { return "Sin conexión con el servidor"; }
            catch (TaskCanceledException) { return "Tiempo de espera agotado"; }
        }

        public async Task<string?> ActualizarAsync(string id, ActualizarMasajeDTO dto)
        {
            try
            {
                var res = await http.PutAsJsonAsync($"api/masaje/{id}", dto);
                return await ResultadoAsync(res, "No se pudo actualizar el masaje");
            }
            catch (HttpRequestException) { return "Sin conexión con el servidor"; }
            catch (TaskCanceledException) { return "Tiempo de espera agotado"; }
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