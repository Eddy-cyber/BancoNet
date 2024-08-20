using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BancoNet.Models
{
    public class Tarjeta
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        
        public long No_Tarjeta { get; set; }
        
        [Column(TypeName = "DATE")]
        public DateTime Vencimiento { get; set; }
        
        [ForeignKey("No_Cuenta")]
        public long No_Cuenta { get; set; }

        public string? Codigo_Seguridad { get; set; }
        public long? Limite_Credito { get; set; }
        
        public Cuenta Cuenta { get; set; }
    }
}
