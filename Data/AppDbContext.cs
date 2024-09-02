using Microsoft.EntityFrameworkCore;

namespace BancoNet.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Clientes> Clientes { get; set; }
        public DbSet<Cuenta> Cuentas { get; set; }
        public DbSet<Tarjeta> Tarjetas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Clientes>()
                .HasMany(c => c.Cuentas)
                .WithOne(c => c.Cliente)
                .HasForeignKey(c => c.ClienteId);

            modelBuilder.Entity<Cuenta>()
                .HasOne(c => c.Tarjeta)
                .WithOne(t => t.Cuenta)
                .HasForeignKey<Tarjeta>(t => t.No_Cuenta);
        }
    }
}
