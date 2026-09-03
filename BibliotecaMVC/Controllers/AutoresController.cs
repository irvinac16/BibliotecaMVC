using Microsoft.AspNetCore.Mvc;
using BibliotecaMVC.Models;
using BibliotecaMVC.Services;

namespace BibliotecaMVC.Controllers
{
    public class AutoresController : Controller
    {
        private readonly IAutorService _autorService;

        public AutoresController(IAutorService autorService)
        {
            _autorService = autorService;
        }

        public IActionResult Index()
        {
            var autores = _autorService.ObtenerAutores();
            return View(autores);
        }

        public IActionResult Details(int id)
        {
            var autor = _autorService.ObtenerAutorPorId(id);
            if (autor == null)
            {
                return NotFound();
            }
            return View(autor);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Autor autor)
        {
            if (!ModelState.IsValid)
            {
                return View(autor);
            }
            _autorService.AgregarAutor(autor);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var autor = _autorService.ObtenerAutorPorId(id);
            if (autor == null)
            {
                return NotFound();
            }
            return View(autor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Autor autor)
        {
            if (!ModelState.IsValid)
            {
                return View(autor);
            }
            var existingAutor = _autorService.ObtenerAutorPorId(autor.Id);
            if (existingAutor == null)
            {
                return NotFound();
            }
            _autorService.EditarAutor(autor);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var autor = _autorService.ObtenerAutorPorId(id);
            if (autor == null)
            {
                return NotFound();
            }
            return View(autor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(Autor autor)
        {
            var existingAutor = _autorService.ObtenerAutorPorId(autor.Id);
            if (existingAutor == null)
            {
                return NotFound();
            }
            _autorService.EliminarAutor(autor.Id);
            return RedirectToAction(nameof(Index));
        }
    }
}
