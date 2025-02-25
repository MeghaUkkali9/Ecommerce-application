using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace PaymentService.Data;

public class PaymentDbContext : DbContext
{
    public PaymentDbContext(DbContextOptions<PaymentDbContext> options) : base(options) { }

    public DbSet<Payment> Payments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Payment>()
            .ToTable("Payment")
            .HasKey(p => p.PaymentId);

        modelBuilder.Entity<Payment>()
            .Property(p => p.OrderId)
            .HasColumnType("INT");
        
        var paymentStatusConverter = new EnumToStringConverter<PaymentStatus>();
        modelBuilder.Entity<Payment>()
            .Property(e => e.PaymentStatus)
            .HasConversion(paymentStatusConverter);
        
        var paymentMethodConverter = new EnumToStringConverter<PaymentMethod>();
        modelBuilder.Entity<Payment>()
            .Property(e => e.PaymentMethod)
            .HasConversion(paymentMethodConverter);
    }
}