using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SystemProducts.Models.Usuario
{
    [Table("USUARIOS")]
    public class Usuario
    {
        [Key]
        public int IDUsuario { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        [Required]
        [StringLength(100)]
        public string Apellido { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Correo { get; set; }

        [Required]
        [StringLength(255)]
        public string Contraseña { get; set; }

        [Required]
        [StringLength(50)]
        public string Rol { get; set; } // Cliente o Colaborador

        public bool Estado { get; set; } = true; // Activo por defecto

        public DateTime FechaRegistro { get; set; } = DateTime.Now;
    }
}