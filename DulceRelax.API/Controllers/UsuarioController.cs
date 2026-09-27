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
        public async Task<IActionResult> Registrar(RegistrarDTO dto)
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
                    Correo = dto.Correo
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
                return Unauthorized(new { error = "Correo o contraseña incorrectos."});

            var resultado = await response.Content.ReadFromJsonAsync<JsonElement>();
            var idToken = resultado.GetProperty("idToken").GetString();

            return Ok(new { idToken });

        }


    }
}
