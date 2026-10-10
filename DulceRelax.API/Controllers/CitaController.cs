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

        private string? Uid => User.FindFirst("user_id")?.Value;

        private bool EsAdmin =>
            User.FindFirst("admin")?.Value.Equals("true", StringComparison.OrdinalIgnoreCase) == true;

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
            => Ok(await _repository.GetByUsuarioAsync(Uid));

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
            if (dto.FechaHora.ToUniversalTime() < DateTime.UtcNow.AddDays(5))
                return BadRequest(new { error = "La cita debe solicitarse con al menos 5 días de anticipación." });

            dto.UsuarioId = Uid;

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

            var cita = await _repository.GetByIdAsync(id);
            if (cita is null) return NotFound();

            if (!EsAdmin)
            {
                if (cita.UsuarioId != Uid) return Forbid();

                if (cita.Estado != EstadoCita.Pendiente)
                    return BadRequest(new { error = "Solo se pueden editar citas pendientes." });

                var cambioFecha = Math.Abs((dto.FechaHora.ToUniversalTime() - cita.FechaHora.ToUniversalTime()).TotalMinutes) >= 1;
                if (cambioFecha && dto.FechaHora.ToUniversalTime() < DateTime.UtcNow.AddDays(5))
                    return BadRequest(new { error = "La cita debe solicitarse con al menos 5 días de anticipación." });

                dto.Estado = EstadoCita.Pendiente;
            }

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
            var cita = await _repository.GetByIdAsync(id);
            if (cita is null) return NotFound();

            if (!EsAdmin && (cita.UsuarioId != Uid || estado != EstadoCita.Cancelada))
                return Forbid();

            await _repository.CambiarEstadoAsync(id, estado);
            return NoContent();
        }
    }
}