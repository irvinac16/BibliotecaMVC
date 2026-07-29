using System.ComponentModel.DataAnnotations;

namespace BibliotecaMVC.Models
{
    public class Autor
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        [StringLength(50)]

        public string Nacionalidad { get; set; }
        [DataType(DataType.Date)]
        public DateTime FechaNacimiento { get; set; }
    }
}
