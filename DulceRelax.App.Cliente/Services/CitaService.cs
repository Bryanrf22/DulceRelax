using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using DulceRelax.Shared.DTOs;

namespace DulceRelax.App.Cliente.Services
{
    public interface ICitaService
    {
        Task<string?> CrearAsync(RegistrarCitaDTO dto);
    }

    public class CitaService(HttpClient http) : ICitaService
    {
        public async Task<string?> CrearAsync(RegistrarCitaDTO dto)
        {
            try
            {
                var res = await http.PostAsJsonAsync("api/cita", dto);
                if (res.IsSuccessStatusCode) return null;
                if (res.StatusCode == HttpStatusCode.Unauthorized) return "Sesión expirada";
                return await LeerError(res);
            }
            catch (HttpRequestException) { return "Sin conexión con el servidor"; }
            catch (TaskCanceledException) { return "Tiempo de espera agotado"; }
        }

        static async Task<string> LeerError(HttpResponseMessage res)
        {
            try
            {
                var json = await res.Content.ReadFromJsonAsync<JsonElement>();
                return json.GetProperty("error").GetString() ?? "No se pudo crear la cita";
            }
            catch { return "No se pudo crear la cita"; }
        }
    }
}