using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using apirest.Data;
using SistemaLivros.Models;

namespace apirest.repository;

public class LivroRepository
{
    private readonly AppDbContext _context;

        public LivroRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Livro>> ListarTodos()
        {
            return await _context.Livros.ToListAsync();
        }

        public async Task<Livro?> BuscarPorId(int id)
        {
            return await _context.Livros.FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<Livro> Criar(Livro livro)
        {
            _context.Livros.Add(livro);
            await _context.SaveChangesAsync();
            return livro;
        }

        public async Task<Livro> Atualizar(Livro livro)
        {
            _context.Livros.Update(livro);
            await _context.SaveChangesAsync();
            return livro;
        }

        public async Task<bool> Deletar(int id)
        {
            var livro = await BuscarPorId(id);
            if (livro == null) return false;

        _context.Livros.Remove(livro);
        await _context.SaveChangesAsync();
        return true;
    }
}