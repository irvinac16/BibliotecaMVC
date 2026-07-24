using Microsoft.AspNetCore.Mvc;
using BibliotecaMVC.Models;

namespace BibliotecaMVC.Controllers
{
    public class AutoresController : Controller
    {
        public IActionResult Index()
        {
            List<Autor> autores = new List<Autor>()
            {
                new Autor
                {
                    Id = 1,
                    Nombre = "Gabriel",
                    Apellido = "García Márquez",
                    Nacionalidad = "Colombiano",
                    FechaNacimiento = 1927,
                    Activo = false
                },
                new Autor
                {
                    Id = 2,
                    Nombre = "Isabel",
                    Apellido = "Allende",
                    Nacionalidad = "Chilena",
                    FechaNacimiento = 1942,
                    Activo = true
                },
                new Autor
                {
                    Id = 3,
                    Nombre = "Mario",
                    Apellido = "Vargas Llosa",
                    Nacionalidad = "Peruano",
                    FechaNacimiento = 1936,
                    Activo = true
                },
                new Autor
                {
                    Id = 4,
                    Nombre = "Jorge Luis",
                    Apellido = "Borges",
                    Nacionalidad = "Argentino",
                    FechaNacimiento = 1899,
                    Activo = false
                },
                new Autor
                {
                    Id = 5,
                    Nombre = "Julio",
                    Apellido = "Cortázar",
                    Nacionalidad = "Argentino",
                    FechaNacimiento = 1914,
                    Activo = false
                },
                new Autor
                {
                    Id = 6,
                    Nombre = "Pablo",
                    Apellido = "Neruda",
                    Nacionalidad = "Chileno",
                    FechaNacimiento = 1904,
                    Activo = false
                },
                new Autor
                {
                    Id = 7,
                    Nombre = "J.K.",
                    Apellido = "Rowling",
                    Nacionalidad = "Británica",
                    FechaNacimiento = 1965,
                    Activo = true
                },
                new Autor
                {
                    Id = 8,
                    Nombre = "Stephen",
                    Apellido = "King",
                    Nacionalidad = "Estadounidense",
                    FechaNacimiento = 1947,
                    Activo = true
                },
                new Autor
                {
                    Id = 9,
                    Nombre = "Haruki",
                    Apellido = "Murakami",
                    Nacionalidad = "Japonés",
                    FechaNacimiento = 1949,
                    Activo = true
                },
                new Autor
                {
                    Id = 10,
                    Nombre = "Chinua",
                    Apellido = "Achebe",
                    Nacionalidad = "Nigeriano",
                    FechaNacimiento = 1930,
                    Activo = false
                }
            };

            ViewBag.Nombre = "Juan Marco";
            ViewBag.Autores = autores;

            return View();
        }
    }
}
