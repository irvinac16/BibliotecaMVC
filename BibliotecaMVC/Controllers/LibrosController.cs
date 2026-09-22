using Microsoft.AspNetCore.Mvc;
using BibliotecaMVC.Models;
using BibliotecaMVC.Data;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaMVC.Controllers
{
    public class LibrosController : Controller
    {
        private readonly BibliotecaContext _context;

        public LibrosController(BibliotecaContext context)
        {
            _context = context;
        }
        
    public async Task<IActionResult> Index()
        {
            var libros = await _context.Libros.ToListAsync();
            return View(libros);
        }

        public async Task<IActionResult> Details(int id)
        {
            var libro = await _context.Libros.FirstOrDefaultAsync(l => l.Id == id);
            if (libro == null)
            {
                return NotFound();
            }
            return View(libro);

        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Libro libro)
        {
            if (!ModelState.IsValid)
            {
                return View(libro);
            }

            _context.Libros.Add(libro);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var libro = await _context.Libros.FindAsync(id);
            if (libro == null)
            {
                return NotFound();
            }
            return View(libro);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Libro libro)
        {
            if (!ModelState.IsValid)
            {
                return View(libro);
            }
            var existingLibro = await _context.Libros.FindAsync(libro.Id);
            if (existingLibro == null)
            {
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

        public async Task<IActionResult> Delete(int id)
        {
            var libro = await _context.Libros.FindAsync(id);
            if (libro == null)
            {
                return NotFound();
            }
            return View(libro);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Libro libro)
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
    }
}
