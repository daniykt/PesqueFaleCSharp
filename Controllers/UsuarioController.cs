using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using PesqueFaleCSharp.Data;
using PesqueFaleCSharp.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using PesqueFaleCSharp.Data;
using PesqueFaleCSharp.Models;

namespace PesqueFaleCSharp.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly ILogger<UsuarioController> _logger;
        private readonly AppDbContext _context;
        private readonly PasswordHasher<Pescador> _hasher;

        public UsuarioController(ILogger<UsuarioController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
            _hasher = new PasswordHasher<Pescador>();
        }

        // GET: /Usuario/Perfil
        public IActionResult Perfil()
        {
            return View();
        }

        // GET: /Usuario/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Usuario/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password, bool rememberMe = false)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError(string.Empty, "Email e senha são obrigatórios.");
                return View();
            }

            var user = await _context.Pescadores.FirstOrDefaultAsync(u => u.email == email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Credenciais inválidas.");
                return View();
            }

            var result = _hasher.VerifyHashedPassword(user, user.senha, password);
            if (result == PasswordVerificationResult.Success || result == PasswordVerificationResult.SuccessRehashNeeded)
            {
                _logger.LogInformation("Login válido para {Email}", email);
                // TODO: criar claims/session/cookie aqui
                return RedirectToAction(nameof(Perfil));
            }

            ModelState.AddModelError(string.Empty, "Credenciais inválidas.");
            return View();
        }

        // GET: /Usuario/Registro
        [HttpGet]
        public IActionResult Registro()
        {
            return View();
        }

        // POST: /Usuario/Registro
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Registro(Pescador pescador, string confirmar_senha)
        {
            if (!ModelState.IsValid)
            {
                return View("Login");
            }

            // Verifica se as senhas coincidem
            if (pescador.senha != confirmar_senha)
            {
                TempData["Error"] = "As senhas não coincidem.";
                return View("Login");
            }

            // evita duplicação de email
            if (_context.Pescadores.Any(p => p.email == pescador.email))
            {
                TempData["Error"] = "Já existe um usuário com este email.";
                return View("Login");
            }

            var hasher = new PasswordHasher<Pescador>();
            pescador.senha = hasher.HashPassword(pescador, pescador.senha);

            // Não salva o confirmar_senha no banco
            pescador.confirmar_senha = ""; // ou pode remover esta linha

            _context.Pescadores.Add(pescador);
            _context.SaveChanges();

            _logger.LogInformation("Novo registro: {Email}", pescador.email);

            TempData["Success"] = "Cadastro realizado com sucesso! Faça login para continuar.";
            return RedirectToAction(nameof(Login));
        }

        // GET: /Usuario/Notificacao
        public IActionResult Notificacao()
        {
            return View();
        }

        // GET: /Usuario/Logout
        public IActionResult Logout()
        {
            // TODO: limpar sessão/cookies/claims quando implementar autenticação
            return RedirectToAction(nameof(Login));
        }
    }
}
