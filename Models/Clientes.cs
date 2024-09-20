using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BancoNet.Models
{
    [Table("Clientes")]
    public class Clientes
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        
        [MaxLength(100)]
        public string? Nombre { get; set; }
        
        public long? Telefono { get; set; }

        [Column(TypeName = "DATE")]
        public DateTime Nacimiento { get; set; }

        public int Edad
        {
            get
            {
                var today = DateTime.Today;
                var age = today.Year - Nacimiento.Year;
                if (Nacimiento > today.AddYears(-age))
                {
                    age--;
                }

                return age;
            }
        }

        [MaxLength(255)]
        public string? Foto { get; set; }
        
        public ICollection<Cuenta>? Cuentas { get; set; }
    }
}
