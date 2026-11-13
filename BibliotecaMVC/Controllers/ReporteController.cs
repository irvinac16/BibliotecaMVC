using Microsoft.AspNetCore.Mvc;
using BibliotecaMVC.Data;
using BibliotecaMVC.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ClosedXML.Excel;

namespace BibliotecaMVC.Controllers
{
    [Authorize]
    public class ReportesController : Controller
    {
        private readonly BibliotecaContext _context;

        public ReportesController(BibliotecaContext context)
        {
            _context = context;
        }

        private async Task<List<Libro>> ObtenerLibrosFiltrados(string? autor, string? categoria)
        {
            var consulta = _context.Libros.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(autor))
            {
                consulta = consulta.Where(l => l.Autor.Contains(autor));
            }

            if (!string.IsNullOrWhiteSpace(categoria))
            {
                consulta = consulta.Where(l => l.Categoria.Contains(categoria));
            }

            return await consulta.OrderBy(l => l.Titulo).ToListAsync();
        }

        public async Task<IActionResult> Reporte(string? autor, string? categoria)
        {
            var libros = await ObtenerLibrosFiltrados(autor, categoria);
            return File(GenerarReporte(libros, autor, categoria), "application/pdf");
        }

        public async Task<IActionResult> DescargarPDF(string? autor, string? categoria)
        {
            var libros = await ObtenerLibrosFiltrados(autor, categoria);
            return File(GenerarReporte(libros, autor, categoria), "application/pdf", "ReporteLibros.pdf");
        }
       

        public async Task<IActionResult> ExportarExcel(string? autor, string? categoria)
        {
            var libros = await ObtenerLibrosFiltrados(autor, categoria);

            using var workbook = new XLWorkbook();
            var hoja = workbook.Worksheets.Add("Reporte de Libros");

            hoja.Range("A1:F1").Merge();
            hoja.Cell("A1").Value = "BIBLIOTECA UNIVERSITARIA";
            hoja.Cell("A1").Style.Font.Bold = true;
            hoja.Cell("A1").Style.Font.FontSize = 18;
            hoja.Cell("A1").Style.Font.FontColor = XLColor.White;
            hoja.Cell("A1").Style.Fill.BackgroundColor = XLColor.DarkBlue;
            hoja.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            hoja.Range("A2:F2").Merge();
            hoja.Cell("A2").Value = "Reporte de libros";
            hoja.Cell("A2").Style.Font.Bold = true;
            hoja.Cell("A2").Style.Font.FontSize = 14;
            hoja.Cell("A2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            hoja.Range("A3:F3").Merge();
            hoja.Cell("A3").Value = $"Fecha de generación: {DateTime.Now:dd/MM/yyyy HH:mm}";
            hoja.Cell("A3").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            hoja.Range("A4:F4").Merge();
            hoja.Cell("A4").Value = $"Total de libros registrados: {libros.Count}";
            hoja.Cell("A4").Style.Font.Bold = true;
            hoja.Cell("A4").Style.Fill.BackgroundColor = XLColor.LightBlue;

            hoja.Range("A5:F5").Merge();

            hoja.Cell("A5").Value =
                $"Filtros aplicados: Autor: " +
                $"{(string.IsNullOrWhiteSpace(autor) ? "Todos" : autor)} | " +
                $"Categoría: " +
                $"{(string.IsNullOrWhiteSpace(categoria) ? "Todas" : categoria)}";

            hoja.Cell("A5").Style.Font.Italic = true;
            hoja.Cell("A5").Style.Font.FontColor = XLColor.DarkBlue;

            string[] encabezados =
            {
                "ID", "Título", "Autor", "Categoría", "Precio", "Estado"
            };

            for (int i = 0; i < encabezados.Length; i++)
            {
                hoja.Cell(6, i + 1).Value = encabezados[i];
            }

            var encabezado = hoja.Range("A6:F6");
            encabezado.Style.Font.Bold = true;
            encabezado.Style.Font.FontColor = XLColor.White;
            encabezado.Style.Fill.BackgroundColor = XLColor.DarkBlue;
            encabezado.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            int fila = 7;

            foreach (var libro in libros)
            {
                hoja.Cell(fila, 1).Value = libro.Id;
                hoja.Cell(fila, 2).Value = libro.Titulo ?? "";
                hoja.Cell(fila, 3).Value = libro.Autor ?? "";
                hoja.Cell(fila, 4).Value = libro.Categoria ?? "";
                hoja.Cell(fila, 5).Value = libro.Precio;
                hoja.Cell(fila, 6).Value = libro.Disponible ? "Disponible" : "Prestado";

                if (fila % 2 == 0)
                {
                    hoja.Range(fila, 1, fila, 6) .Style.Fill.BackgroundColor = XLColor.LightGray;
                }
                fila++;
            }

            if (fila > 7)
            {
                hoja.Range($"E7:E{fila - 1}") .Style.NumberFormat.Format = "$#,##0.00";
            }

            var tabla = hoja.Range($"A6:F{Math.Max(6, fila - 1)}");

            tabla.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            tabla.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            hoja.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            return File(stream.ToArray(),"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet","ReporteLibros.xlsx");
        }

        private static byte[] GenerarReporte(
            IReadOnlyCollection<Libro> libros, string? autor, string? categoria)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var fechaGeneracion = DateTime.Now;

            return Document.Create(document =>
            {
                document.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(35);
                    page.PageColor(Colors.Grey.Lighten5);

                    page.Header().Column(column =>
                    {
                        column.Item()
                            .Text("BIBLIOTECA UNIVERSITARIA")
                            .FontSize(11)
                            .Bold()
                            .FontColor(Colors.Blue.Darken2);

                        column.Item()
                            .Text("Reporte de libros")
                            .FontSize(26)
                            .Bold()
                            .FontColor(Colors.Grey.Darken3);

                        column.Item()
                            .Text($"Generado el {fechaGeneracion:dd/MM/yyyy HH:mm}")
                            .FontSize(10)
                            .FontColor(Colors.Grey.Darken1);

                        column.Item()
                            .PaddingTop(12)
                            .LineHorizontal(2)
                            .LineColor(Colors.Blue.Medium);
                    });

                    page.Content().PaddingVertical(20).Column(column =>
                    {
                        column.Item()
                            .Background(Colors.Blue.Darken2)
                            .Padding(12)
                            .Row(row =>
                            {
                                row.RelativeItem()
                                    .Text("Total de libros registrados")
                                    .FontSize(11)
                                    .Bold()
                                    .FontColor(Colors.White);

                                row.ConstantItem(50)
                                    .AlignRight()
                                    .Text(libros.Count.ToString())
                                    .FontSize(18)
                                    .Bold()
                                    .FontColor(Colors.White);
                            });

                        column.Item()
                            .PaddingTop(10)
                            .Text($"Filtros aplicados: Autor: " +
                                  $"{(string.IsNullOrWhiteSpace(autor) ? "Todos" : autor)} | " +
                                  $"Categoría: " +
                                  $"{(string.IsNullOrWhiteSpace(categoria) ? "Todas" : categoria)}")
                            .FontSize(9)
                            .FontColor(Colors.Grey.Darken2);

                        column.Item().PaddingTop(14).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(35);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(1.5f);
                                columns.RelativeColumn(1.2f);
                                columns.RelativeColumn(0.9f);
                                columns.RelativeColumn(1);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Background(Colors.Grey.Lighten2)
                                    .Padding(5).Text("ID").FontSize(9).Bold();

                                header.Cell().Background(Colors.Grey.Lighten2)
                                    .Padding(5).Text("Título").FontSize(9).Bold();

                                header.Cell().Background(Colors.Grey.Lighten2)
                                    .Padding(5).Text("Autor").FontSize(9).Bold();

                                header.Cell().Background(Colors.Grey.Lighten2)
                                    .Padding(5).Text("Categoría").FontSize(9).Bold();

                                header.Cell().Background(Colors.Grey.Lighten2)
                                    .Padding(5).Text("Precio").FontSize(9).Bold();

                                header.Cell().Background(Colors.Grey.Lighten2)
                                    .Padding(5).Text("Estado").FontSize(9).Bold();
                            });

                            foreach (var libro in libros)
                            {
                                var fondo = libro.Id % 2 == 0
                                    ? Colors.White
                                    : Colors.Grey.Lighten4;

                                table.Cell().Background(fondo).Padding(5)
                                    .Text(libro.Id.ToString()).FontSize(8);

                                table.Cell().Background(fondo).Padding(5)
                                    .Text(libro.Titulo ?? "").FontSize(8);

                                table.Cell().Background(fondo).Padding(5)
                                    .Text(libro.Autor ?? "").FontSize(8);

                                table.Cell().Background(fondo).Padding(5)
                                    .Text(libro.Categoria ?? "").FontSize(8);

                                table.Cell().Background(fondo).Padding(5)
                                    .Text($"${libro.Precio:0.00}").FontSize(8);

                                table.Cell().Background(fondo).Padding(5)
                                    .Text(libro.Disponible ? "Disponible" : "Prestado")
                                    .FontSize(8);
                            }
                        });
                    });

                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.Span("Biblioteca Universitaria - Página ");
                        text.CurrentPageNumber();
                        text.Span(" de ");
                        text.TotalPages();
                    });
                });
            }).GeneratePdf();
        }
    }
}
