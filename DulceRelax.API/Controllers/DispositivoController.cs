using DulceRelax.API.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DulceRelax.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "Admin")]
    public class DispositivoController(DispositivoRepository repo) : ControllerBase
    {
        public record TokenDTO(string Token);

        [HttpPost]
        public async Task<IActionResult> Registrar(TokenDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Token)) return BadRequest();
            await repo.GuardarAsync(dto.Token);
            return NoContent();
        }

        [HttpPost("baja")]
        public async Task<IActionResult> Baja(TokenDTO dto)
        {
            await repo.EliminarAsync(dto.Token);
            return NoContent();
        }
    }
}