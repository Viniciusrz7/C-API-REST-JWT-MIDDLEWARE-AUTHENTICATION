using Microsoft.AspNetCore.Mvc;
using Livraria.Services;
using SistemaLivros.Models;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Annotations;

namespace Livraria.Controllers;

[ApiController]
[Route("api/livros")]
[SwaggerTag("Catálogo de Livros da Aplicação")]
public class LivroController : ControllerBase
{
    private readonly LivroService _service;

    public LivroController(LivroService service) => _service = service;

    [AllowAnonymous]
    [HttpGet]
    [SwaggerOperation(Summary = "Buscar todos os livros", Description = "Retorna todos os livros cadastrados com um filtro opcional por título. Pode ser acessado sem token.")]
    [SwaggerResponse(200, "Livros retornados com sucesso")]
    [SwaggerResponse(500, "Erro interno de comunicação com o banco")]
    public async Task<ActionResult> GetAll([FromQuery] string? titulo)
    {
        try
        {
            var data = await _service.FindAll(titulo);
            return Ok(data);
        }
        catch (Exception ex)
        {
            var msg = !string.IsNullOrEmpty(ex.Message) ? ex.Message : "Erro ao buscar livros";
            return StatusCode(500, new { message = msg });
        }
    }

    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Buscar livro por ID", Description = "Busca os detalhes completos de um livro específico mediante ao seu código identificador.")]
    [SwaggerResponse(200, "Livro encontrado com sucesso")]
    [SwaggerResponse(404, "Nenhum livro encontrado com o ID especificado")]
    public async Task<ActionResult> GetOne(int id)
    {
        try
        {
            var data = await _service.FindByPk(id);
            if (data != null) return Ok(data);
            return NotFound(new { message = $"Livro com id {id} não encontrado" });
        }
        catch (Exception ex)
        {
            var msg = !string.IsNullOrEmpty(ex.Message) ? ex.Message : "Erro ao buscar livro";
            return StatusCode(500, new { message = msg });
        }
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Criar novo livro", Description = "Ação para cadastrar um novo exemplar para a loja no banco de dados.")]
    [SwaggerResponse(201, "Livro criado com sucesso")]
    [SwaggerResponse(400, "Dados inválidos presentes no corpo da requisição")]
    [SwaggerResponse(401, "Não autenticado: Requer JWT válido")]
    public async Task<ActionResult> Create([FromBody] Livro livro)
    {
        try
        {
            if (string.IsNullOrEmpty(livro.Titulo))
                return BadRequest(new { message = "Título é obrigatório" });

            var data = await _service.Create(livro);
            return CreatedAtAction(nameof(GetOne), new { id = data.Id }, data);
        }
        catch (Exception ex)
        {
            var msg = !string.IsNullOrEmpty(ex.Message) ? ex.Message : "Erro ao criar livro";
            return StatusCode(500, new { message = msg });
        }
    }

    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Substituir dados do livro", Description = "Atualiza de forma integral todas as colunas e dados do livro existente ou retorna erro se não existir.")]
    [SwaggerResponse(204, "Livro atualizado no banco de sucesso")]
    [SwaggerResponse(404, "Livro alvo não existe no banco de dados")]
    [SwaggerResponse(401, "Não autenticado: Requer JWT válido")]
    public async Task<ActionResult> Update(int id, [FromBody] Livro livro)
    {
        try
        {
            var sucesso = await _service.Update(id, livro);
            if (sucesso) return NoContent();
            return NotFound(new { message = $"Livro com id {id} não encontrado" });
        }
        catch (Exception ex)
        {
            var msg = !string.IsNullOrEmpty(ex.Message) ? ex.Message : "Erro ao atualizar livro";
            return StatusCode(500, new { message = msg });
        }
    }

    [HttpPatch("{id}")]
    [SwaggerOperation(Summary = "Atualizar atributos isolados do livro", Description = "Pode-se informar apenas 1 campo no JSON para ele efetuar a ação separadamente. Útil para campos dinâmicos.")]
    [SwaggerResponse(204, "Livro atualizado no banco de sucesso")]
    [SwaggerResponse(404, "Livro alvo não existe no banco de dados")]
    [SwaggerResponse(401, "Não autenticado: Requer JWT válido")]
    public async Task<ActionResult> Patch(int id, [FromBody] Livro dados)
    {
        try
        {
            var sucesso = await _service.Patch(id, dados);
            if (sucesso) return NoContent();
            return NotFound(new { message = "Livro não encontrado" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [Authorize(Roles = "AGENTE")]
    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Apagar livro", Description = "Ação perigosa que apaga do banco de dados um exemplar. Requer a claim do Perfil ser: 'AGENTE'.")]
    [SwaggerResponse(204, "Livro deletado com êxito!")]
    [SwaggerResponse(404, "Livro já não existe!")]
    [SwaggerResponse(403, "Acesso Negado (Apenas admins ou Agentes)")]
    [SwaggerResponse(401, "Não autenticado: Requer JWT válido")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            var sucesso = await _service.Delete(id);
            if (sucesso) return NoContent();
            return NotFound(new { message = $"Livro com id {id} não encontrado para exclusão" });
        }
        catch (Exception ex)
        {
            var msg = !string.IsNullOrEmpty(ex.Message) ? ex.Message : "Erro ao deletar livro";
            return StatusCode(500, new { message = msg });
        }
    }
}