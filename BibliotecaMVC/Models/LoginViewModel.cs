using System.ComponentModel.DataAnnotations;

namespace BibliotecaMVC.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "El nombre de usuario o el correo electrónico es obligatorio.")]
        [Display(Name = "Nombre de usuario o correo electrónico")]
        public string UsernameOrEmail { get; set; } = string.Empty;
        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
        [Display(Name = "Recordarme")]
        public bool RememberMe { get; set; }
        public string? ReturnUrl { get; set; }
    }
}
