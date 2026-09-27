using DulceRelax.API.Repositories;
using DulceRelax.Shared.DTOs;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DulceRelax.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsuarioController : ControllerBase
    {
        private readonly UsuarioRepository _repository;

        public UsuarioController(UsuarioRepository repository) => _repository = repository;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var usuarios = await _repository.GetAllAsync();
            return Ok(usuarios);
        }

        [HttpPost("registrar")]
        [AllowAnonymous]
        public async Task<IActionResult> Registrar(RegistrarUsuarioDTO dto)
        {
            var args = new UserRecordArgs
            {
                Email = dto.Email,
                Password = dto.Password,
                DisplayName = dto.NombreCompleto
            };

            var userRecord = await FirebaseAuth.DefaultInstance.CreateUserAsync(args);

            var usuario = new UsuarioDTO
            {
                Id = userRecord.Uid,
                NombreCompleto = dto.NombreCompleto,
                NumTelefono = dto.NumTelefono
            };
            await _repository.SetAsync(userRecord.Uid, usuario);

            return Ok(new { uid = userRecord.Uid });
        }
    }
}
