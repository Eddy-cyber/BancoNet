using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BancoNet.Models
{
    public class Cuenta
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long No_Cuenta { get; set; } // Usar long si es clave primaria

        public string? Tipo { get; set; }
        public long? Saldo { get; set; }
        public string? InfoTarjeta { get; set; }

        [ForeignKey("ClienteId")]
        public Clientes Cliente { get; set; }
        public long ClienteId { get; set; }

        public Tarjeta? Tarjeta { get; set; }
    }
}
