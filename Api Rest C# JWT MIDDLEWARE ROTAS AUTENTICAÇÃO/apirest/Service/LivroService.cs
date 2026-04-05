namespace Livraria.Services;

using apirest.repository;
using SistemaLivros.Models;

public class LivroService
{
    private readonly LivroRepository _repo;

    public LivroService(LivroRepository repo) => _repo = repo;

    public async Task<List<Livro>> FindAll(string? titulo)
    {
        var livros = await _repo.ListarTodos();
        if (!string.IsNullOrEmpty(titulo))
            return livros.Where(l => l.Titulo.Contains(titulo, StringComparison.OrdinalIgnoreCase)).ToList();

        return livros;
    }

    public Task<Livro?> FindByPk(int id) => _repo.BuscarPorId(id);

    public async Task<Livro> Create(Livro livro) => await _repo.Criar(livro);

    public async Task<bool> Update(int id, Livro dados)
    {
        var livro = await _repo.BuscarPorId(id);
        if (livro == null) return false;

        livro.Titulo = dados.Titulo;
        livro.Autor = dados.Autor;
        livro.Ano = dados.Ano;

        await _repo.Atualizar(livro);
        return true;
    }

    public async Task<bool> Delete(int id)
    {
        var existe = await _repo.Deletar(id);
        if (!existe) return false;

        await _repo.Deletar(id);
        return true;
    }

    public async Task<bool> Patch(int id, Livro dadosParciais)
    {
        var livro = await _repo.BuscarPorId(id);
        if (livro == null) return false;

        // Só atualiza se o campo não for nulo ou padrão
        if (!string.IsNullOrEmpty(dadosParciais.Titulo)) livro.Titulo = dadosParciais.Titulo;
        if (!string.IsNullOrEmpty(dadosParciais.Autor)) livro.Autor = dadosParciais.Autor;
        if (dadosParciais.Ano > 0) livro.Ano = dadosParciais.Ano;

        await _repo.Atualizar(livro);
        return true;
    }
}