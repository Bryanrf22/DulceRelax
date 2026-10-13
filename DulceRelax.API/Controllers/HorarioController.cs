using System.Globalization;
using DulceRelax.API.Repositories;
using DulceRelax.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DulceRelax.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class HorarioController(HorarioRepository repository) : ControllerBase
    {
        private static readonly string[] Dias = Enum.GetNames<DayOfWeek>();

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var horario = await repository.GetAsync();
            return horario is null ? NoContent() : Ok(horario);
        }

        [HttpPut]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> Put(HorarioAtencionDTO dto)
        {
            if (dto.Dias is null || Dias.Any(d => !dto.Dias.ContainsKey(d)))
                return BadRequest(new { error = "El horario debe incluir los 7 días." });

            foreach (var (nombre, dia) in dto.Dias)
            {
                if (!Dias.Contains(nombre) || dia is null)
                    return BadRequest(new { error = "Horario inválido." });
                if (!dia.Activo) continue;

                if (!TimeSpan.TryParseExact(dia.HoraInicio, @"hh\:mm", CultureInfo.InvariantCulture, out var inicio) ||
                    !TimeSpan.TryParseExact(dia.HoraFin, @"hh\:mm", CultureInfo.InvariantCulture, out var fin) ||
                    inicio >= fin)
                    return BadRequest(new { error = $"Horario inválido para {nombre}." });
            }

            await repository.SetAsync(dto);
            return NoContent();
        }
    }
}