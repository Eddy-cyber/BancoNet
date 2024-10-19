using System.ComponentModel.DataAnnotations;

namespace BancoNet.Dtos
{
    public class ClienteDto
    {
        [Required]
        public long Id { get; set; }

        [MaxLength(100)]
        public string? Nombre { get; set; }
        
        [MaxLength(255)]
        public string? Foto { get; set; }
    }
}
