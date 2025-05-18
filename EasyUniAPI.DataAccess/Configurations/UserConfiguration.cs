using EasyUniAPI.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EasyUniAPI.DataAccess.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(u => u.Id)
                .HasMaxLength(26)
                .IsRequired();

            builder.Property(u => u.Gender)
                .IsRequired();

            builder.Property(u => u.BirthDate)
                .IsRequired();

            builder.Property(u => u.Email)
                .HasMaxLength(256)
                .IsRequired();
            builder.HasIndex(u => u.Email)
                .IsUnique();

            builder.Property(u => u.FirstName)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(u => u.LastName)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(u => u.MiddleName)
                .HasMaxLength(200);

            builder.Property(u => u.PhoneNumber)
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(u => u.PasswordSalt)
                .HasMaxLength(256)
                .IsRequired();

            builder.Property(u => u.PasswordHash)
                .HasMaxLength(256)
                .IsRequired();

            builder.Property(u => u.Active)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(u => u.FailedLoginAttempts)
                .IsRequired();

            builder.HasMany(u => u.Roles)
                .WithMany(r => r.Users)
                .UsingEntity<UserRole>(
                    b => b.HasOne(ur => ur.Role).WithMany().OnDelete(DeleteBehavior.NoAction).HasForeignKey(ur => ur.RoleId),
                    b => b.HasOne(ur => ur.User).WithMany().OnDelete(DeleteBehavior.NoAction).HasForeignKey(ur => ur.UserId),
                    b =>
                    {
                        b.HasKey(ur => new { ur.UserId, ur.RoleId });
                        b.ToTable("UserRoles");
                    });
        }
    }
}
