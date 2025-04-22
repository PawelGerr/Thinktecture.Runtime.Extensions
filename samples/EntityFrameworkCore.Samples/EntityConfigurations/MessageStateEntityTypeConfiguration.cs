using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Thinktecture.EntityConfigurations;

public class MessageStateEntityTypeConfiguration
   : IEntityTypeConfiguration<MessageState>,
     IEntityTypeConfiguration<MessageState.Parsed>,
     IEntityTypeConfiguration<MessageState.Processed>
{
   public void Configure(EntityTypeBuilder<MessageState> builder)
   {
      builder.ToTable("MessageStates");

      builder.HasKey(s => s.Order);

      builder.Property(s => s.Order).ValueGeneratedOnAdd(); // auto-increment
      builder.Property<Guid>("MessageId"); // FK to the message table (as a shadow property)

      builder
         .HasDiscriminator<string>("Type")
         .HasValue<MessageState.Initial>("Initial")
         .HasValue<MessageState.Parsed>("Parsed")
         .HasValue<MessageState.Processed>("Processed")
         .HasValue<MessageState.Error>("Error");
   }

   public void Configure(EntityTypeBuilder<MessageState.Parsed> builder)
   {
      builder.Property(s => s.CreatedAt).HasColumnName("CreatedAt");
   }

   public void Configure(EntityTypeBuilder<MessageState.Processed> builder)
   {
      builder.Property(s => s.CreatedAt).HasColumnName("CreatedAt");
   }
}
