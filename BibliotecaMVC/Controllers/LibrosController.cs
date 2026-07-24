using Microsoft.AspNetCore.Mvc;
using BibliotecaMVC.Models;

namespace BibliotecaMVC.Controllers
{
    public class LibrosController : Controller
    {
        public IActionResult Index()
        {
            List<Libro> libros = new List<Libro>()
            {
                new Libro
                {
                    Id = 1,
                    Titulo = "Cien años de soledad",
                    Autor = "Gabriel García Márquez",
                    Categoria = "Novela",
                    Precio = 19.99m,
                    Disponible = true
                },
                new Libro
                {
                    Id = 2,
                    Titulo = "El amor en los tiempos del cólera",
                    Autor = "Gabriel García Márquez",
                    Categoria = "Novela",
                    Precio = 14.99m,
                    Disponible = false
                },
                new Libro
                {
                    Id = 3,
                    Titulo = "La casa de los espíritus",
                    Autor = "Isabel Allende",
                    Categoria = "Novela",
                    Precio = 17.99m,
                    Disponible = true
                },
                new Libro
                {
                    Id = 4,
                    Titulo = "Pedro Páramo",
                    Autor = "Juan Rulfo",
                    Categoria = "Novela",
                    Precio = 12.99m,
                    Disponible = false
                },
                new Libro
                {
                    Id = 5,
                    Titulo = "Rayuela",
                    Autor = "Julio Cortázar",
                    Categoria = "Novela",
                    Precio = 15.99m,
                    Disponible = true
                },
                new Libro
                {
                    Id = 6,
                    Titulo = "Ficciones",
                    Autor = "Jorge Luis Borges",
                    Categoria = "Cuento",
                    Precio = 9.99m,
                    Disponible = true
                },
                new Libro
                {
                    Id = 7,
                    Titulo = "El Aleph",
                    Autor = "Jorge Luis Borges",
                    Categoria = "Cuento",
                    Precio = 11.99m,
                    Disponible = false
                },
                new Libro
                {
                    Id = 8,
                    Titulo = "Los detectives salvajes",
                    Autor = "Roberto Bolaño",
                    Categoria = "Novela",
                    Precio = 18.99m,
                    Disponible = true
                },
                new Libro
                {
                    Id = 9,
                    Titulo = "2666",
                    Autor = "Roberto Bolaño",
                    Categoria = "Novela",
                    Precio = 21.99m,
                    Disponible = false
                },
                new Libro
                {
                    Id = 10,
                    Titulo = "El túnel",
                    Autor = "Ernesto Sabato",
                    Categoria = "Novela",
                    Precio = 13.99m,
                    Disponible = true
                }
             };

            ViewBag.Nombre = "Juan Marco";
            ViewBag.Libros = libros;

            return View(libros);
        }
    }
}
