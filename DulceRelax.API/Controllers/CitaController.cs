using DulceRelax.API.Repositories;
using DulceRelax.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DulceRelax.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CitaController : ControllerBase
    {
        private readonly CitaRepository _repository;

        public CitaController(CitaRepository repository) => _repository = repository;

        [HttpGet]
        public async Task<IActionResult> GetByFecha([FromQuery] DateTime fecha)
            => Ok(await _repository.GetByFechaAsync(fecha));

        [HttpGet("disponibilidad")]
        public async Task<IActionResult> GetOcupados([FromQuery] DateTime fecha)
        {
            var citas = await _repository.GetByFechaAsync(fecha);
            return Ok(citas
                .Where(c => c.Estado != EstadoCita.Cancelada)
                .Select(c => c.FechaHora));
        }

        [HttpGet("mias")]
        public async Task<IActionResult> GetMias()
        {
            var uid = User.FindFirst("user_id")?.Value;
            return Ok(await _repository.GetByUsuarioAsync(uid));
        }

        [HttpGet("agendadas")]
        public async Task<IActionResult> GetAgendadas(
            [FromQuery] DateTimeOffset desde, [FromQuery] DateTimeOffset hasta)
        {
            if (hasta <= desde || hasta - desde > TimeSpan.FromDays(3))
                return BadRequest(new { error = "Rango inválido." });

            return Ok(await _repository.GetConfirmadasPorRangoAsync(desde.UtcDateTime, hasta.UtcDateTime));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var cita = await _repository.GetByIdAsync(id);
            return cita is null ? NotFound() : Ok(cita);
        }

        [HttpPost]
        public async Task<IActionResult> Crear(RegistrarCitaDTO dto)
        {
            if (dto.FechaHora.ToUniversalTime() <= DateTime.UtcNow)
                return BadRequest(new { error = "La fecha debe ser futura." });

            dto.UsuarioId = User.FindFirst("user_id")?.Value;

            var id = await _repository.CrearAsync(dto);
            if (id is null)
                return Conflict(new { error = "Horario no disponible." });

            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(string id, ActualizarCitaDTO dto)
        {
            if (dto.Id != id)
                return BadRequest(new { error = "El Id no coincide." });

            var resultado = await _repository.ActualizarAsync(id, dto);
            return resultado switch
            {
                ResultadoCita.NoEncontrada => NotFound(),
                ResultadoCita.HorarioOcupado => Conflict(new { error = "Horario no disponible." }),
                _ => NoContent()
            };
        }

        [HttpPatch("{id}/estado")]
        public async Task<IActionResult> CambiarEstado(string id, [FromQuery] EstadoCita estado)
        {
            var ok = await _repository.CambiarEstadoAsync(id, estado);
            return ok ? NoContent() : NotFound();
        }
    }
}