using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Web.Domain.Orders;
using Web.Domain.Restaurants;
using Web.Domain.Users;

namespace Web.Data.EF.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("orders");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .HasConversion(
                    x => x.Value,
                    x => new OrderId(x))
                .HasColumnName("id")
                .ValueGeneratedNever();

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            builder.Property(x => x.UserId)
                .HasColumnName("user_id");

            builder.HasOne<Restaurant>()
                .WithMany()
                .HasForeignKey(x => x.RestaurantId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            builder.Property(x => x.RestaurantId)
                .HasColumnName("restaurant_id");

            builder.Property(x => x.Status)
                .HasColumnName("status")
                .IsRequired()
                .HasConversion(new EnumToStringConverter<OrderStatus>());

            builder.OwnsOne(x => x.Address, x =>
            {
                x.Property(y => y.Value).HasColumnName("address");
            });

            builder.Property(x => x.PlacedAt).HasColumnName("placed_at");

            builder.Property(x => x.PaymentIntentId).HasColumnName("payment_intent_id");

            builder.Property(x => x.ConfirmedAt).HasColumnName("confirmed_at");

            builder.HasMany(x => x.Items)
                .WithOne()
                .HasForeignKey("order_id")
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
