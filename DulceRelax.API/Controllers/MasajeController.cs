using DulceRelax.API.Repositories;
using DulceRelax.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace DulceRelax.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MasajeController : ControllerBase
    {
        private readonly MasajeRepository _repository;

        public MasajeController(MasajeRepository repository) => _repository = repository;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var masajes = await _repository.GetAllAsync();
            return Ok(masajes);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var masaje = await _repository.GetById(id);
            return masaje is null ? NotFound() : Ok(masaje);
        }

        [HttpPost("registrar")]
        public async Task<IActionResult> RegistrarAsync(RegistrarMasajeDTO dto)
        {
            var masaje = new MasajeDTO
            {
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                Precio = dto.Precio,
                Disponible = true
            };

            var id = await _repository.CreateAsync(masaje);

            return Ok(new { id });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(string id, ActualizarMasajeDTO dto)
        {
            await _repository.UpdateAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            await _repository.DeleteAsync(id);
            return NoContent();
        }
    }
}