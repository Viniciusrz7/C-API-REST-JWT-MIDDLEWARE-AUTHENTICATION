using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaLivros.Models;
using apirest.Data;

using Swashbuckle.AspNetCore.Annotations;

[ApiController]
[Route("api/auth")]
[SwaggerTag("Rotas de Autenticação")]
public class AuthController : ControllerBase
{

    private readonly AppDbContext _context;

    public AuthController(AppDbContext context) {
        _context = context;
    }

    [HttpPost("registrar")]
    [SwaggerOperation(Summary = "Registrar novo usuário", Description = "Cria um novo usuário e retorna o Token JWT autmaticamente.")]
    [SwaggerResponse(201, "Usuário criado com sucesso")]
    [SwaggerResponse(400, "Dados inválidos ou email já cadastrado")]
    public async Task<ActionResult> Registrar([FromBody] Usuario user)
    {
        try
        {
            // Verifica se o usuário já existe
            var existe = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == user.Email);
            if (existe != null) return BadRequest(new { message = "Email já cadastrado" });

            // 1. Hasheia a senha usando BCrypt antes de salvar
            if (user.Senha != null)
                user.Senha = BCrypt.Net.BCrypt.HashPassword(user.Senha);

            // Salva no banco de dados real
            _context.Usuarios.Add(user);
            await _context.SaveChangesAsync();

            // Gera o token e já retorna para evitar que o usuário precise fazer o login novamente
            var token = TokenService.GerarToken(user);

            return StatusCode(201, new { message = "Usuário criado com sucesso!", token });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpPost("login")]
    [SwaggerOperation(Summary = "Realizar login", Description = "Autentica o usuário e gera o Token JWT para acessar rotas protegidas.")]
    [SwaggerResponse(200, "Login bem-sucedido (Retorna o Token JWT)")]
    [SwaggerResponse(401, "Email ou senha inválidos")]
    public async Task<ActionResult> Login([FromBody] Usuario login)
    {
        try
        {
            // Busca o usuário pelo email
            var usuarioDb = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == login.Email);

            // 2. Valida se usuário existe E se a senha pura bate com o hash salvo no banco
            if (usuarioDb != null && login.Senha != null && usuarioDb.Senha != null &&
                BCrypt.Net.BCrypt.Verify(login.Senha, usuarioDb.Senha))
            {
                var token = TokenService.GerarToken(usuarioDb); // Passa o usuarioDb para o token
                return Ok(new { token });
            }

            return Unauthorized(new { message = "Email ou senha inválidos" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }
}