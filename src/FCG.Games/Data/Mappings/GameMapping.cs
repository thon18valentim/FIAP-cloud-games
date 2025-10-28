using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using FCG.Games.Models;

namespace FCG.Games.Data.Mappings
{
    public class GameMapping : IEntityTypeConfiguration<Game>
    {
        public void Configure(EntityTypeBuilder<Game> builder)
        {
            builder.ToTable("Games");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(p => p.Description)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(p => p.PublisherName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.ReleaseDate)
                .IsRequired()
                .HasColumnType("date");

            builder.Property(p => p.Price)
                .IsRequired()
                .HasPrecision(18, 2)
                .HasColumnType("decimal(18,2)");
        }
    }
}
