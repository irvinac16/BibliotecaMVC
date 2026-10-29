using Microsoft.AspNetCore.Mvc;
using BibliotecaMVC.Models;
using BibliotecaMVC.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace BibliotecaMVC.Controllers
{
    [Authorize]
    public class LibrosController : Controller
    {
        private readonly BibliotecaContext _context;
        private readonly ILogger<LibrosController> _logger;

        public LibrosController(BibliotecaContext context, ILogger<LibrosController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Consultando lista de libros");
            try
            {
                var libros = await _context.Libros.ToListAsync();
                return View(libros);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al consultar lista de libros");
                throw;
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            _logger.LogInformation("Consultando detalles del libro con id {LibroId}", id);
            try
            {
                var libro = await _context.Libros.FirstOrDefaultAsync(l => l.Id == id);
                if (libro == null)
                {
                    _logger.LogWarning("Libro con id {LibroId} no encontrado", id);
                    return NotFound();
                }
                return View(libro);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al consultar detalles del libro con id {LibroId}", id);
                throw;
            }

        }

        [Authorize(Roles = "Administrador,Bibliotecario")]
        public IActionResult Create()
        {
            _logger.LogInformation("Mostrando formulario para crear un nuevo libro");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Bibliotecario")]
        public async Task<IActionResult> Create(Libro libro)
        {
            _logger.LogInformation("Creando nuevo libro");
            if (!ModelState.IsValid)
            {
                _logger.LogError("Modelo de libro no válido: {ModelState}", ModelState);
                return View(libro);
            }

            try
            {
                _context.Libros.Add(libro);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo libro");
                throw;
            }
        }

        [Authorize(Roles = "Administrador,Bibliotecario")]
        public async Task<IActionResult> Edit(int id)
        {
            _logger.LogInformation("Mostrando formulario para editar el libro con id {LibroId}", id);
            try
            {
                var libro = await _context.Libros.FindAsync(id);
                if (libro == null)
                {
                    return NotFound();
                }
                return View(libro);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al consultar detalles del libro con id {LibroId}", id);
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Bibliotecario")]
        public async Task<IActionResult> Edit(Libro libro)
        {
            _logger.LogInformation("Editando el libro con id {LibroId}", libro.Id);
            if (!ModelState.IsValid)
            {
                _logger.LogError("Modelo de libro no válido: {ModelState}", ModelState);
                return View(libro);
            }
            
            try
            {
                var existingLibro = await _context.Libros.FindAsync(libro.Id);
                if (existingLibro == null)
                {
                    _logger.LogWarning("Libro con id {LibroId} no encontrado", libro.Id);
                    return NotFound();
                }

                existingLibro.Titulo = libro.Titulo;
                existingLibro.Autor = libro.Autor;
                existingLibro.Categoria = libro.Categoria;
                existingLibro.Precio = libro.Precio;
                existingLibro.Imagen = libro.Imagen;
                existingLibro.Disponible = libro.Disponible;

                _context.Libros.Update(existingLibro);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al editar el libro con id {LibroId}", libro.Id);
                throw;
            }
        }

        [Authorize(Roles = "Administrador,Bibliotecario")]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("Mostrando formulario para eliminar el libro con id {LibroId}", id);
           try
            {
                var libro = await _context.Libros.FindAsync(id);
                if (libro == null)
                {
                    return NotFound();
                }
                return View(libro);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al mostrar formulario para eliminar el libro con id {LibroId}", id);
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Bibliotecario")]
        public async Task<IActionResult> Delete(Libro libro)
        {
            _logger.LogInformation("Eliminando el libro con id {LibroId}", libro.Id);
            try
            {
                var existingLibro = await _context.Libros.FindAsync(libro.Id);

                if (existingLibro == null)
                {
                    return NotFound();
                }
                _context.Libros.Remove(existingLibro);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el libro con id {LibroId}", libro.Id);
                throw;
            }
        }
    }
}
