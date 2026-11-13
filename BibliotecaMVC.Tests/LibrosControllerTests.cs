using BibliotecaMVC.Controllers;
using BibliotecaMVC.Data;
using BibliotecaMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace BibliotecaMVC.Tests
{
    public class LibrosControllerTests
    {
        // Crea un controlador con un logger simulado.
        private static LibrosController CreateController(BibliotecaContext context)
        {
            var logger = new Mock<ILogger<LibrosController>>();

            return new LibrosController(context, logger.Object);
        }

        // Crea una base de datos en memoria para aislar cada prueba.
        private static BibliotecaContext CreateContext(string? databaseName = null)
        {
            var options = new DbContextOptionsBuilder<BibliotecaContext>()
                .UseInMemoryDatabase(databaseName ?? Guid.NewGuid().ToString())
                .Options;

            return new BibliotecaContext(options);
        }

        // 1. INDEX: devuelve la lista de libros registrados.
        [Fact]
        public async Task Index_DevuelveLaListaDeLibros()
        {
            await using var context = CreateContext();

            context.Libros.AddRange(
                new Libro
                {
                    Titulo = "Cien años de soledad",
                    Autor = "Gabriel García Márquez",
                    Categoria = "Novela",
                    Precio = 20.00m,
                    Disponible = true,
                    Imagen = "libro1.jpg"
                },
                new Libro
                {
                    Titulo = "1984",
                    Autor = "George Orwell",
                    Categoria = "Distopía",
                    Precio = 15.00m,
                    Disponible = true,
                    Imagen = "libro2.jpg"
                }
            );

            await context.SaveChangesAsync();

            var controller = CreateController(context);

            var result = await controller.Index(null, null);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<Libro>>(viewResult.Model);

            Assert.Equal(2, model.Count);
        }

        // 2. INDEX: devuelve una lista vacía cuando no hay libros.
        [Fact]
        public async Task Index_SinLibrosRegistrados_DevuelveListaVacia()
        {
            await using var context = CreateContext();
            var controller = CreateController(context);

            var result = await controller.Index(null, null);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<Libro>>(viewResult.Model);

            Assert.Empty(model);
        }

        // 3. DETAILS: devuelve los detalles de un libro existente.
        [Fact]
        public async Task Details_ConLibroExistente_DevuelveElLibro()
        {
            await using var context = CreateContext();

            var libro = new Libro
            {
                Titulo = "El principito",
                Autor = "Antoine de Saint-Exupéry",
                Categoria = "Fábula",
                Precio = 12.50m,
                Disponible = true,
                Imagen = "principito.jpg"
            };

            context.Libros.Add(libro);
            await context.SaveChangesAsync();

            var controller = CreateController(context);

            var result = await controller.Details(libro.Id);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Libro>(viewResult.Model);

            Assert.Equal("El principito", model.Titulo);
            Assert.Equal(libro.Id, model.Id);
        }

        // 4. DETAILS: devuelve NotFound cuando el libro no existe.
        [Fact]
        public async Task Details_ConLibroInexistente_DevuelveNotFound()
        {
            await using var context = CreateContext();
            var controller = CreateController(context);

            var result = await controller.Details(999);

            Assert.IsType<NotFoundResult>(result);
        }

        // 5. CREATE: guarda un libro válido y redirige al índice.
        [Fact]
        public async Task Create_ConLibroValido_GuardaElLibroYRedirigeAlIndice()
        {
            await using var context = CreateContext();
            var controller = CreateController(context);

            var libro = new Libro
            {
                Titulo = "Don Quijote de la Mancha",
                Autor = "Miguel de Cervantes",
                Categoria = "Clásico",
                Precio = 25.00m,
                Disponible = true,
                Imagen = "quijote.jpg"
            };

            var result = await controller.Create(libro);

            var redirect = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal(nameof(LibrosController.Index), redirect.ActionName);
            Assert.Equal(1, await context.Libros.CountAsync());

            var libroGuardado = await context.Libros.FirstAsync();

            Assert.Equal("Don Quijote de la Mancha", libroGuardado.Titulo);
            Assert.Equal("Miguel de Cervantes", libroGuardado.Autor);
        }

        // 6. CREATE: no guarda el libro cuando el modelo es inválido.
        [Fact]
        public async Task Create_ConModeloInvalido_NoGuardaYDevuelveLaVista()
        {
            await using var context = CreateContext();
            var controller = CreateController(context);

            controller.ModelState.AddModelError("Titulo", "El título es obligatorio.");

            var libro = new Libro
            {
                Titulo = "",
                Autor = "Autor de prueba",
                Categoria = "Prueba",
                Precio = 10.00m,
                Disponible = true,
                Imagen = "prueba.jpg"
            };

            var result = await controller.Create(libro);

            var viewResult = Assert.IsType<ViewResult>(result);

            Assert.Same(libro, viewResult.Model);
            Assert.Equal(0, await context.Libros.CountAsync());
        }

        // 7. CREATE: comprueba diferentes libros mediante Theory e InlineData.
        [Theory]
        [InlineData("1984", "George Orwell", "Distopía", 15.00)]
        [InlineData("Drácula", "Bram Stoker", "Terror", 18.50)]
        [InlineData("Orgullo y prejuicio", "Jane Austen", "Romance", 12.75)]
        public async Task Create_ConDiferentesLibros_GuardaLosDatosEsperados(
            string titulo,
            string autor,
            string categoria,
            double precio)
        {
            await using var context = CreateContext();
            var controller = CreateController(context);

            var libro = new Libro
            {
                Titulo = titulo,
                Autor = autor,
                Categoria = categoria,
                Precio = (decimal)precio,
                Disponible = true,
                Imagen = "prueba.jpg"
            };

            var result = await controller.Create(libro);

            var redirect = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal(nameof(LibrosController.Index), redirect.ActionName);

            var libroGuardado = await context.Libros.SingleAsync();

            Assert.Equal(titulo, libroGuardado.Titulo);
            Assert.Equal(autor, libroGuardado.Autor);
            Assert.Equal(categoria, libroGuardado.Categoria);
            Assert.Equal((decimal)precio, libroGuardado.Precio);
        }

        // 8. EDIT GET: muestra el formulario de edición de un libro existente.
        [Fact]
        public async Task Edit_ConLibroExistente_DevuelveLaVista()
        {
            await using var context = CreateContext();

            var libro = new Libro
            {
                Titulo = "Libro original",
                Autor = "Autor original",
                Categoria = "Novela",
                Precio = 10.00m,
                Disponible = true,
                Imagen = "original.jpg"
            };

            context.Libros.Add(libro);
            await context.SaveChangesAsync();

            var controller = CreateController(context);

            var result = await controller.Edit(libro.Id);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Libro>(viewResult.Model);

            Assert.Equal(libro.Id, model.Id);
            Assert.Equal("Libro original", model.Titulo);
        }

        // 9. EDIT GET: devuelve NotFound si el libro no existe.
        [Fact]
        public async Task Edit_ConLibroInexistente_DevuelveNotFound()
        {
            await using var context = CreateContext();
            var controller = CreateController(context);

            var result = await controller.Edit(999);

            Assert.IsType<NotFoundResult>(result);
        }

        // 10. EDIT POST: actualiza los datos de un libro existente.
        [Fact]
        public async Task Edit_ConLibroValido_ActualizaElLibro()
        {
            await using var context = CreateContext();

            var libro = new Libro
            {
                Titulo = "Título anterior",
                Autor = "Autor anterior",
                Categoria = "Novela",
                Precio = 10.00m,
                Disponible = true,
                Imagen = "anterior.jpg"
            };

            context.Libros.Add(libro);
            await context.SaveChangesAsync();

            var libroEditado = new Libro
            {
                Id = libro.Id,
                Titulo = "Título actualizado",
                Autor = "Autor actualizado",
                Categoria = "Historia",
                Precio = 20.00m,
                Disponible = false,
                Imagen = "actualizado.jpg"
            };

            var controller = CreateController(context);

            var result = await controller.Edit(libroEditado);

            var redirect = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal(nameof(LibrosController.Index), redirect.ActionName);

            var libroGuardado = await context.Libros.FindAsync(libro.Id);

            Assert.NotNull(libroGuardado);
            Assert.Equal("Título actualizado", libroGuardado.Titulo);
            Assert.Equal("Autor actualizado", libroGuardado.Autor);
            Assert.Equal("Historia", libroGuardado.Categoria);
            Assert.Equal(20.00m, libroGuardado.Precio);
            Assert.False(libroGuardado.Disponible);
            Assert.Equal("actualizado.jpg", libroGuardado.Imagen);
        }

        // 11. EDIT POST: no actualiza los datos si el modelo es inválido.
        [Fact]
        public async Task Edit_ConModeloInvalido_NoActualizaYDevuelveLaVista()
        {
            await using var context = CreateContext();

            var libro = new Libro
            {
                Titulo = "Título original",
                Autor = "Autor original",
                Categoria = "Novela",
                Precio = 10.00m,
                Disponible = true,
                Imagen = "original.jpg"
            };

            context.Libros.Add(libro);
            await context.SaveChangesAsync();

            var libroEditado = new Libro
            {
                Id = libro.Id,
                Titulo = "Título modificado",
                Autor = "Autor modificado",
                Categoria = "Historia",
                Precio = 20.00m,
                Disponible = false,
                Imagen = "modificado.jpg"
            };

            var controller = CreateController(context);
            controller.ModelState.AddModelError("Titulo", "El título no es válido.");

            var result = await controller.Edit(libroEditado);

            var viewResult = Assert.IsType<ViewResult>(result);

            Assert.Same(libroEditado, viewResult.Model);

            var libroGuardado = await context.Libros.FindAsync(libro.Id);

            Assert.NotNull(libroGuardado);
            Assert.Equal("Título original", libroGuardado.Titulo);
            Assert.Equal("Autor original", libroGuardado.Autor);
        }

        // 12. EDIT POST: devuelve NotFound si se intenta actualizar un libro inexistente.
        [Fact]
        public async Task Edit_AlActualizarLibroInexistente_DevuelveNotFound()
        {
            await using var context = CreateContext();
            var controller = CreateController(context);

            var libro = new Libro
            {
                Id = 999,
                Titulo = "Libro inexistente",
                Autor = "Autor de prueba",
                Categoria = "Prueba",
                Precio = 10.00m,
                Disponible = true,
                Imagen = "prueba.jpg"
            };

            var result = await controller.Edit(libro);

            Assert.IsType<NotFoundResult>(result);
        }

        // 13. DELETE GET: muestra la confirmación para eliminar un libro existente.
        [Fact]
        public async Task Delete_ConLibroExistente_DevuelveLaVista()
        {
            await using var context = CreateContext();

            var libro = new Libro
            {
                Titulo = "Libro para eliminar",
                Autor = "Autor de prueba",
                Categoria = "Prueba",
                Precio = 10.00m,
                Disponible = true,
                Imagen = "prueba.jpg"
            };

            context.Libros.Add(libro);
            await context.SaveChangesAsync();

            var controller = CreateController(context);

            var result = await controller.Delete(libro.Id);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Libro>(viewResult.Model);

            Assert.Equal(libro.Id, model.Id);
        }

        // 14. DELETE GET: devuelve NotFound si el libro no existe.
        [Fact]
        public async Task Delete_ConLibroInexistente_DevuelveNotFound()
        {
            await using var context = CreateContext();
            var controller = CreateController(context);

            var result = await controller.Delete(999);

            Assert.IsType<NotFoundResult>(result);
        }

        // 15. DELETE POST: elimina un libro existente y redirige al índice.
        [Fact]
        public async Task Delete_ConLibroExistente_EliminaElLibroYRedirigeAlIndice()
        {
            await using var context = CreateContext();

            var libro = new Libro
            {
                Titulo = "Libro para eliminar",
                Autor = "Autor de prueba",
                Categoria = "Prueba",
                Precio = 10.00m,
                Disponible = true,
                Imagen = "prueba.jpg"
            };

            context.Libros.Add(libro);
            await context.SaveChangesAsync();

            var controller = CreateController(context);

            var result = await controller.Delete(libro);

            var redirect = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal(nameof(LibrosController.Index), redirect.ActionName);
            Assert.Equal(0, await context.Libros.CountAsync());
        }

        // 16. DELETE POST: devuelve NotFound si se intenta eliminar un libro inexistente.
        [Fact]
        public async Task Delete_AlEliminarLibroInexistente_DevuelveNotFound()
        {
            await using var context = CreateContext();
            var controller = CreateController(context);

            var libro = new Libro
            {
                Id = 999,
                Titulo = "Libro inexistente",
                Autor = "Autor de prueba",
                Categoria = "Prueba",
                Precio = 10.00m,
                Disponible = true,
                Imagen = "prueba.jpg"
            };

            var result = await controller.Delete(libro);

            Assert.IsType<NotFoundResult>(result);
        }

        // 17. MOq: verifica que Index registre un mensaje informativo en el logger.
        [Fact]
        public async Task Index_AlConsultarLibros_RegistraInformacionEnElLogger()
        {
            await using var context = CreateContext();

            var loggerMock = new Mock<ILogger<LibrosController>>();
            var controller = new LibrosController(context, loggerMock.Object);

            await controller.Index(null, null);

            loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((state, type) =>
                        state.ToString() == "Consultando lista de libros"),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }
    }
}
