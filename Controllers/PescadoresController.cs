using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using PesqueFaleCSharp.Data;
using PesqueFaleCSharp.Models;
using System.Threading.Tasks;

namespace PesqueFaleCSharp.Controllers
{
    public class PescadoresController : Controller
    {
        private readonly AppDbContext _context;

        public PescadoresController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /admin/Pescadores
        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Gerenciar Pescadores";
            return View(await _context.Pescadores.ToListAsync());
        }

        // GET: /admin/Pescadores/Create
        public IActionResult Create()
        {
            ViewData["Title"] = "Novo Pescador";
            return View();
        }

        // POST: /admin/Pescadores/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("nome,email,senha,confirmar_senha")] Pescador pescador)
        {
            if (ModelState.IsValid)
            {
                // Hash da senha antes de salvar
                var hasher = new PasswordHasher<Pescador>();
                pescador.senha = hasher.HashPassword(pescador, pescador.senha);

                // confirmar_senha é [NotMapped] — opcional limpar
                pescador.confirmar_senha = null;

                _context.Add(pescador);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(pescador);
        }

        // GET: /admin/Pescadores/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pescador = await _context.Pescadores.FindAsync(id);
            if (pescador == null)
            {
                return NotFound();
            }
            ViewData["Title"] = "Editar Pescador";
            // Limpa o campo de senha para não vazar o hash/valor anterior
            pescador.senha = string.Empty;
            pescador.confirmar_senha = string.Empty;
            return View(pescador);
        }

        // POST: /admin/Pescadores/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id_pescador,nome,email,senha,confirmar_senha")] Pescador pescador)
        {
            if (id != pescador.id_pescador)
            {
                return NotFound();
            }

            // Se não informou nova senha, remover validação de senha/confirmar (permitir editar sem tocar senha)
            if (string.IsNullOrWhiteSpace(pescador.senha))
            {
                ModelState.Remove(nameof(Pescador.senha));
                ModelState.Remove(nameof(Pescador.confirmar_senha));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingPescador = await _context.Pescadores.AsNoTracking().FirstOrDefaultAsync(p => p.id_pescador == id);

                    if (existingPescador == null)
                    {
                        return NotFound();
                    }

                    if (string.IsNullOrWhiteSpace(pescador.senha))
                    {
                        // mantém hash existente
                        pescador.senha = existingPescador.senha;
                    }
                    else
                    {
                        // novo hash para nova senha
                        var hasher = new PasswordHasher<Pescador>();
                        pescador.senha = hasher.HashPassword(pescador, pescador.senha);
                    }

                    // confirmar_senha não é persistido
                    pescador.confirmar_senha = null;

                    _context.Update(pescador);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Pescadores.Any(e => e.id_pescador == pescador.id_pescador))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(pescador);
        }

        // GET: /admin/Pescadores/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pescador = await _context.Pescadores
                .FirstOrDefaultAsync(m => m.id_pescador == id);
            if (pescador == null)
            {
                return NotFound();
            }
            ViewData["Title"] = "Excluir Pescador";
            return View(pescador);
        }

        // POST: /admin/Pescadores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pescador = await _context.Pescadores.FindAsync(id);
            if (pescador != null)
            {
                _context.Pescadores.Remove(pescador);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
