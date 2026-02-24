using FuelStation.DAL.Configurations.Base;
using FuelStation.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FuelStation.DAL.Configurations;

public class CarConfiguration : BaseEntityTypeConfiguration<Car>
{
    protected override void ConfigureInternal(EntityTypeBuilder<Car> builder)
    {
        builder.HasOne(c => c.User)
            .WithMany()
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
