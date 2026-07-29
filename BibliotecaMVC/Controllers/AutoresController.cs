using Microsoft.AspNetCore.Mvc;
using BibliotecaMVC.Models;

namespace BibliotecaMVC.Controllers
{
    public class AutoresController : Controller
    {
        private static List<Autor> _autores = new List<Autor>()
        {
            new Autor
            {
                Id = 1,
                Nombre = "Gabriel",
                Apellido = "García Márquez",
                Nacionalidad = "Colombiano",
                FechaNacimiento = new DateTime(1927, 3, 6),
            },
            new Autor
            {
                Id = 2,
                Nombre = "Isabel",
                Apellido = "Allende",
                Nacionalidad = "Chilena",
                FechaNacimiento = new DateTime(1942, 9, 15),
            },
            new Autor
            {
                Id = 3,
                Nombre = "Mario",
                Apellido = "Vargas Llosa",
                Nacionalidad = "Peruano",
                FechaNacimiento = new DateTime(1936, 4, 11),
            },
            new Autor
            {
                Id = 4,
                Nombre = "Jorge Luis",
                Apellido = "Borges",
                Nacionalidad = "Argentino",
                FechaNacimiento = new DateTime(1899, 8, 9),
            },
            new Autor
            {
                Id = 5,
                Nombre = "Julio",
                Apellido = "Cortázar",
                Nacionalidad = "Argentino",
                FechaNacimiento = new DateTime(1914, 7, 19),
            },
            new Autor
            {
                Id = 6,
                Nombre = "Pablo",
                Apellido = "Neruda",
                Nacionalidad = "Chileno",
                FechaNacimiento = new DateTime(1904, 11, 18),
            },
            new Autor
            {
                Id = 7,
                Nombre = "J.K.",
                Apellido = "Rowling",
                Nacionalidad = "Británica",
                FechaNacimiento = new DateTime(1965, 10, 11),
            },
            new Autor
            {
                Id = 8,
                Nombre = "Stephen",
                Apellido = "King",
                Nacionalidad = "Estadounidense",
                FechaNacimiento = new DateTime(1947, 5, 16),
            },
            new Autor
            {
                Id = 9,
                Nombre = "Haruki",
                Apellido = "Murakami",
                Nacionalidad = "Japonés",
                FechaNacimiento = new DateTime(1949, 3, 7),
            },
            new Autor
            {
                Id = 10,
                Nombre = "Chinua",
                Apellido = "Achebe",
                Nacionalidad = "Nigeriano",
                FechaNacimiento = new DateTime(1930, 1, 22),
            }
        };
        public IActionResult Index()
        {
            return View(_autores);
        }

        public IActionResult Details(int id)
        {
            var autor = _autores.FirstOrDefault(x => x.Id == id);
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

            if (_autores.Any())
            {
                autor.Id = _autores.Max(x => x.Id) + 1;
            }
            else
            {
                autor.Id = 1;
            }
            _autores.Add(autor);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var autor = _autores.FirstOrDefault(x => x.Id == id);
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
            var existingAutor = _autores.FirstOrDefault(x => x.Id == autor.Id);
            if (existingAutor == null)
            {
                return NotFound();
            }
            existingAutor.Nombre = autor.Nombre;
            existingAutor.Apellido = autor.Apellido;
            existingAutor.Nacionalidad = autor.Nacionalidad;
            existingAutor.FechaNacimiento = autor.FechaNacimiento;
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var autor = _autores.FirstOrDefault(x => x.Id == id);
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
            var existingAutor = _autores.FirstOrDefault(x => x.Id == autor.Id);

            if (existingAutor == null)
            {
                return NotFound();
            }
            _autores.Remove(existingAutor);
            return RedirectToAction(nameof(Index));
        }
    }
}
