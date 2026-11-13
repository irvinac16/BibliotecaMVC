using Microsoft.AspNetCore.Mvc;
using BibliotecaMVC.Models;
using BibliotecaMVC.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using QuestPDF.Helpers;
using QuestPDF.Elements;

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

        public async Task<IActionResult> Reporte()
        {
            var autores = await _context.Autores
                .AsNoTracking()
                .OrderBy(a => a.Nombre)
                .ToListAsync();

            return File(GenerarReporte(autores), "application/pdf");
        }

        public async Task<IActionResult> DescargarReporte()
        {
            var autores = await _context.Autores
                .AsNoTracking()
                .OrderBy(a => a.Nombre)
                .ToListAsync();

            return File(GenerarReporte(autores), "application/pdf", "ReporteAutores.pdf");
        }

        private static byte[] GenerarReporte(IReadOnlyCollection<Autor> autores)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            return Document.Create(document =>
            {
                document.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(42); 
                    page.PageColor(Colors.Grey.Lighten5); 
                    page.Header().Column(column => 
                    { column.Item().Text("BIBLIOTECA UNIVERSITARIA")
                        .FontSize(11).
                        Bold().
                        FontColor(Colors.Blue.Darken2); 
                        column.Item().Text("Reporte de autores")
                        .FontSize(26)
                        .Bold()
                        .FontColor(Colors.Grey.Darken3);
                        column.Item().Text($"Listado actualizado el {DateTime.Now:dd/MM/yyyy HH:mm}")
                        .FontSize(10)
                        .FontColor(Colors.Grey.Darken1);
                        column.Item().PaddingTop(14).LineHorizontal(2).LineColor(Colors.Blue.Medium); }); 
                    
                    page.Content().PaddingVertical(22).Column(column => 
                    {
                        column.Item().Background(Colors.Blue.Darken2).Padding(14).Row(row =>
                        {
                            row.RelativeItem().Text("Autores Registrados")
                            .FontSize(11)
                            .Bold()
                            .FontColor(Colors.White);
                            row.ConstantItem(45).AlignRight().Text(autores.Count().ToString())
                            .FontSize(18)
                            .Bold()
                            .FontColor(Colors.White);
                        });

                        column.Item().PaddingTop(14).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(50);
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });
                            table.Header(header =>
                            {
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("ID").FontSize(10).Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Nombre").FontSize(10).Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Apellidos").FontSize(10).Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Nacionalidad").FontSize(10).Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Fecha de Nacimiento").FontSize(10).Bold();
                            });
                            foreach (var autor in autores)
                            {
                                var fondo = autor.Id % 2 == 0
                                    ? Colors.White
                                    : Colors.Grey.Lighten4;
                                table.Cell().Background(fondo).Padding(5).Text(autor.Id.ToString()).FontSize(10);
                                table.Cell().Background(fondo).Padding(5).Text(autor.Nombre).FontSize(10);
                                table.Cell().Background(fondo).Padding(5).Text(autor.Apellido).FontSize(10);
                                table.Cell().Background(fondo).Padding(5).Text(autor.Nacionalidad ?? "No especificada").FontSize(10);
                                table.Cell().Background(fondo).Padding(5).Text(autor.FechaNacimiento.ToString("dd/MM/yyyy")).FontSize(10);
                            }
                        });
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Biblioteca Universitaria - Pagina ");
                        x.CurrentPageNumber();
                        x.Span(" de ");
                        x.TotalPages();
                    });

                });
            }).GeneratePdf();
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



