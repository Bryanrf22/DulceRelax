using DulceRelax.API.Repositories;
using DulceRelax.Shared.DTOs;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace DulceRelax.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsuarioController : ControllerBase
    {
        private readonly UsuarioRepository _repository;

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public UsuarioController(
            UsuarioRepository repository,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
        {
            _repository = repository;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

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
            try
            {
                var args = new UserRecordArgs
                {
                    Email = dto.Correo,
                    Password = dto.Password,
                    DisplayName = dto.NombreCompleto
                };

                var userRecord = await FirebaseAuth.DefaultInstance.CreateUserAsync(args);

                var usuario = new UsuarioDTO
                {
                    Id = userRecord.Uid,
                    NombreCompleto = dto.NombreCompleto,
                    NumTelefono = dto.NumTelefono,
                    Correo = dto.Correo,
                    Direccion = dto.Direccion
                };
                await _repository.SetAsync(userRecord.Uid, usuario);

                return Ok(new { uid = userRecord.Uid });
            }
            catch (FirebaseAuthException ex) when (ex.AuthErrorCode == AuthErrorCode.EmailAlreadyExists)
            {
                return Conflict(new { error = "Este correo ya está en uso" });
            }
            catch (FirebaseAuthException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }


        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginDTO dto)
        {
            var webApiKey = _configuration["Firebase:WebApiKey"];
            var client = _httpClientFactory.CreateClient();

            var response = await client.PostAsJsonAsync(
                $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={webApiKey}",
                new { email = dto.Email, password = dto.Password, returnSecureToken = true });

            if (!response.IsSuccessStatusCode)
                return Unauthorized(new { error = "Correo o contraseña incorrectos." });

            var resultado = await response.Content.ReadFromJsonAsync<JsonElement>();
            var idToken = resultado.GetProperty("idToken").GetString();

            return Ok(new { idToken });

        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            var uid = User.FindFirst("user_id")?.Value;
            var usuario = await _repository.GetByIdAsync(uid);
            return usuario is null ? NotFound() : Ok(usuario);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var usuario = await _repository.GetByIdAsync(id);
            return usuario is null ? NotFound() : Ok(usuario);
        }

        [HttpPut("me")]
        public async Task<IActionResult> UpdateMe(ActualizarUsuarioDTO dto)
        {
            var uid = User.FindFirst("user_id")?.Value;
            await _repository.UpdateAsync(uid, dto);
            return NoContent();
        }

        [HttpDelete("me")]
        public async Task<IActionResult> DeleteMe()
        {
            var uid = User.FindFirst("user_id")?.Value;
            await _repository.DeleteAsync(uid);
            await FirebaseAuth.DefaultInstance.DeleteUserAsync(uid);
            return NoContent();
        }

        [HttpPost("recuperar-password")]
        [AllowAnonymous]
        public async Task<IActionResult> RecuperarPassword(RecuperarPasswordDTO dto)
        {
            var webApiKey = _configuration["Firebase:WebApiKey"];
            var client = _httpClientFactory.CreateClient();

            var response = await client.PostAsJsonAsync(
                $"https://identitytoolkit.googleapis.com/v1/accounts:sendOobCode?key={webApiKey}",
                new { requestType = "PASSWORD_RESET", email = dto.Email });

            if (response.IsSuccessStatusCode)
                return Ok();

            var detalle = await response.Content.ReadAsStringAsync();
            if (detalle.Contains("EMAIL_NOT_FOUND"))
                return Ok();

            return StatusCode(502, new { error = "No se pudo enviar el correo de recuperación." });
        }
    }
}
