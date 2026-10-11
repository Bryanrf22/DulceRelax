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
        private const int MinutosPorDefecto = 60;

        private readonly CitaRepository _repository;
        private readonly MasajeRepository _masajes;

        public CitaController(CitaRepository repository, MasajeRepository masajes)
        {
            _repository = repository;
            _masajes = masajes;
        }

        private string? Uid => User.FindFirst("user_id")?.Value;

        private bool EsAdmin =>
            User.FindFirst("admin")?.Value.Equals("true", StringComparison.OrdinalIgnoreCase) == true;

        private static int Minutos(MasajeDTO? masaje) =>
            masaje is { DuracionMinutos: > 0 } ? masaje.DuracionMinutos : MinutosPorDefecto;

        [HttpGet]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> GetByFecha([FromQuery] DateTime fecha)
            => Ok(await _repository.GetByFechaAsync(fecha));

        [HttpGet("disponibilidad")]
        public async Task<IActionResult> GetBloqueos(
            [FromQuery] DateTimeOffset desde, [FromQuery] DateTimeOffset hasta)
        {
            if (hasta <= desde || hasta - desde > TimeSpan.FromDays(7))
                return BadRequest(new { error = "Rango inválido." });

            var bloqueos = await _repository.GetBloqueosAsync(desde.UtcDateTime, hasta.UtcDateTime);
            return Ok(bloqueos.Select(b => new { inicio = b.Inicio, fin = b.Fin }));
        }

        [HttpGet("mias")]
        public async Task<IActionResult> GetMias()
            => Ok(await _repository.GetByUsuarioAsync(Uid));

        [HttpGet("agendadas")]
        [Authorize(Policy = "Admin")]
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
            if (cita is null) return NotFound();
            if (!EsAdmin && cita.UsuarioId != Uid) return Forbid();
            return Ok(cita);
        }

        [HttpPost]
        public async Task<IActionResult> Crear(RegistrarCitaDTO dto)
        {
            if (dto.FechaHora.ToUniversalTime() < DateTime.UtcNow.AddDays(5))
                return BadRequest(new { error = "La cita debe solicitarse con al menos 5 días de anticipación." });

            var masaje = await _masajes.GetById(dto.MasajeId);
            if (masaje is null || !masaje.Disponible)
                return BadRequest(new { error = "El masaje seleccionado no está disponible." });

            dto.UsuarioId = Uid;
            dto.MasajeNombre = masaje.Nombre;

            var id = await _repository.CrearAsync(dto, Minutos(masaje));
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
                dto.MasajeId = cita.MasajeId;
            }

            var masaje = await _masajes.GetById(dto.MasajeId);
            if (masaje is null && dto.MasajeId != cita.MasajeId)
                return BadRequest(new { error = "El masaje seleccionado no existe." });

            dto.MasajeNombre = masaje?.Nombre ?? cita.MasajeNombre;

            var resultado = await _repository.ActualizarAsync(id, dto, Minutos(masaje));
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

            var resultado = await _repository.CambiarEstadoAsync(id, estado);
            return resultado switch
            {
                ResultadoCita.NoEncontrada => NotFound(),
                ResultadoCita.HorarioOcupado => Conflict(new
                {
                    error = "Se cruza con una cita ya confirmada (incluye 1 hora de descanso)."
                }),
                _ => NoContent()
            };
        }
    }
}