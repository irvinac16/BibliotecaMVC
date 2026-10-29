using Microsoft.AspNetCore.Mvc;
using BibliotecaMVC.Models;
using BibliotecaMVC.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace BibliotecaMVC.Controllers
{
    [Authorize]
    public class AutoresController : Controller
    {
        private readonly BibliotecaContext _context;
        private readonly ILogger<AutoresController> _logger;

        public AutoresController(BibliotecaContext context, ILogger<AutoresController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Consultando lista de autores");
            try
            {
                var autores = await _context.Autores.ToListAsync();
                return View(autores);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al consultar lista de autores");
                throw;
            }
            
        }

        public async Task<IActionResult> Details(int id)
        {
            _logger.LogInformation("Consultando detalles del autor con id {AutorId}", id);
            try
            {
                var autor = await _context.Autores.FindAsync(id);
                if (autor == null)
                {
                    _logger.LogWarning("Autor con id {AutorId} no encontrado", id);
                    return NotFound();
                }
                return View(autor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al consultar detalles del autor con id {AutorId}", id);
                throw;
            }
            
        }

        [Authorize(Roles = "Administrador, Bibliotecario")]
        public IActionResult Create()
        {
            _logger.LogInformation("Mostrando formulario para crear un nuevo autor");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador, Bibliotecario")]
        public async Task<IActionResult> Create(Autor autor)
        {
            _logger.LogInformation("Creando un nuevo autor");
            if (!ModelState.IsValid)
            {
                _logger.LogError("Modelo de autor no válido: {ModelState}", ModelState);
                return View(autor);
            }

            try
            {
                _context.Autores.Add(autor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear un nuevo autor");
                throw;
            }
            
        }
        
        [Authorize(Roles = "Administrador, Bibliotecario")]
        public async Task<IActionResult> Edit(int id)
        {
            _logger.LogInformation("Mostrando formulario para editar el autor con id {AutorId}", id);
            try
            {
                var autor = await _context.Autores.FindAsync(id);
                if (autor == null)
                {
                    return NotFound();
                }
                return View(autor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al mostrar formulario para editar el autor con id {AutorId}", id);
                throw;
            }
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador, Bibliotecario")]
        public async Task<IActionResult> Edit(int id, Autor autor)
        {
            _logger.LogInformation("Editando el autor con id {AutorId}", id);
            if (id != autor.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                _logger.LogError("Modelo de autor no válido: {ModelState}", ModelState);
                return View(autor);
            }
            
            try
            {
                var exists = await _context.Autores.AnyAsync(a => a.Id == id);
                if (!exists)
                {
                    _logger.LogWarning("Autor con id {AutorId} no encontrado", id);
                    return NotFound();
                }
                _context.Update(autor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al editar el autor con id {AutorId}", id);
                throw;
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("Eliminando el autor con id {AutorId}", id);
            try
            {
                var autor = await _context.Autores.FindAsync(id);
                if (autor == null)
                {
                    return NotFound();
                }
                _context.Autores.Remove(autor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el autor con id {AutorId}", id);
                throw;
            }
        }
    }
}



