using Microsoft.AspNetCore.Mvc;
using BibliotecaMVC.Models;

namespace BibliotecaMVC.Controllers
{
    public class LibrosController : Controller
    {

        public static List<Libro> _libros = new List<Libro>
        {
                new Libro
            {
            Id = 1,
                Titulo = "Cien años de soledad",
                Autor = "Gabriel García Márquez",
                Categoria = "Novela",
                Precio = 19.99m,
                Disponible = true,
                Imagen = "cien-anios.jpg"
            },
            new Libro
            {
                Id = 2,
                Titulo = "El amor en los tiempos del cólera",
                Autor = "Gabriel García Márquez",
                Categoria = "Novela",
                Precio = 14.99m,
                Disponible = false,
                Imagen = "amor-tiempos-de-colera.jpg"
            },
            new Libro
            {
                Id = 3,
                Titulo = "La casa de los espíritus",
                Autor = "Isabel Allende",
                Categoria = "Novela",
                Precio = 17.99m,
                Disponible = true,
                Imagen = "casa-de-los-espiritus.jpg"
            },
            new Libro
            {
                Id = 4,
                Titulo = "Pedro Páramo",
                Autor = "Juan Rulfo",
                Categoria = "Novela",
                Precio = 12.99m,
                Disponible = false,
                Imagen = "pedro-paramo-juan-rulfo.jpg"
            },
                new Libro
                {
                    Id = 5,
                    Titulo = "Rayuela",
                    Autor = "Julio Cortázar",
                    Categoria = "Novela",
                    Precio = 15.99m,
                    Disponible = true,
                    Imagen = "rayuela.jpg"
                }
    };

    public IActionResult Index()
        {
            return View(_libros);
        }

        public IActionResult Details(int id)
        {
            var libro = _libros.FirstOrDefault(l => l.Id == id);
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
        public IActionResult Create(Libro libro)
        {
            if (!ModelState.IsValid)
            {
                return View(libro);
            }
            if (_libros.Any())
            {
                libro.Id = _libros.Max(l => l.Id) + 1;
            }
            else
            {
                libro.Id = 1;
            }
            _libros.Add(libro);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var autor = _libros.FirstOrDefault(x => x.Id == id);
            if (autor == null)
            {
                return NotFound();
            }
            return View(autor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Libro libro)
        {
            if (!ModelState.IsValid)
            {
                return View(libro);
            }
            var existingLibro = _libros.FirstOrDefault(x => x.Id == libro.Id);
            if (existingLibro == null)
            {
                return NotFound();
            }
            existingLibro.Titulo = libro.Titulo;
            existingLibro.Autor = libro.Autor;
            existingLibro.Categoria = libro.Categoria;
            existingLibro.Precio = libro.Precio;
            existingLibro.Disponible = libro.Disponible;
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var libro = _libros.FirstOrDefault(x => x.Id == id);
            if (libro == null)
            {
                return NotFound();
            }
            return View(libro);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(Libro libro)
        {
            var existingLibro = _libros.FirstOrDefault(x => x.Id == libro.Id);

            if (existingLibro == null)
            {
                return NotFound();
            }
            _libros.Remove(existingLibro);
            return RedirectToAction(nameof(Index));
        }
    }
}
