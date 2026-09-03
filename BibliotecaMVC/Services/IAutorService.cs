using BibliotecaMVC.Models;
namespace BibliotecaMVC.Services
{
    public interface IAutorService
    {
        List<Autor> ObtenerAutores();
        Autor? ObtenerAutorPorId(int id);
        void AgregarAutor(Autor autor);
        void EditarAutor(Autor autor);
        void EliminarAutor(int id);
    }
}
