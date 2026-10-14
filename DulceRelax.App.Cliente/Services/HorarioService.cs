using System.Net;
using System.Net.Http.Json;
using DulceRelax.Shared.DTOs;

namespace DulceRelax.App.Cliente.Services
{
    public interface IHorarioService
    {
        Task<HorarioAtencionDTO?> ObtenerAsync();
    }

    public class HorarioService(HttpClient http) : IHorarioService
    {
        public async Task<HorarioAtencionDTO?> ObtenerAsync()
        {
            try
            {
                var res = await http.GetAsync("api/horario");
                if (res.StatusCode != HttpStatusCode.OK) return null;
                return await res.Content.ReadFromJsonAsync<HorarioAtencionDTO>();
            }
            catch { return null; }
        }
    }
}