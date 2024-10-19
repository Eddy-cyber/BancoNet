using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BancoNet.Models
{
    public class Cuenta
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long No_Cuenta { get; set; }

        public string? Tipo { get; set; }
        public long? Saldo { get; set; }
        public string? Beneficiarios { get; set; }

        [ForeignKey("ClienteId")]
        public Clientes? Cliente { get; set; }
        public long ClienteId { get; set; }

        public Tarjeta? Tarjeta { get; set; }
    }
}
