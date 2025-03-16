using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Thinktecture.EntityConfigurations;

public class MessageEntityTypeConfiguration : IEntityTypeConfiguration<Message>
{
   public void Configure(EntityTypeBuilder<Message> builder)
   {
      builder.ToTable("Messages");

      builder.HasMany(m => m.States)
             .WithOne()
             .HasForeignKey("MessageId");

      builder.Navigation(m => m.States).AutoInclude();
   }
}
