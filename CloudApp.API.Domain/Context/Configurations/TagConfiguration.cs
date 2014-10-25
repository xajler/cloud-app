using CloudApp.API.Domain.Entities;
using System.Data.Entity.ModelConfiguration;

namespace CloudApp.API.Domain.Context.Configurations
{
    public class TagConfiguration : EntityTypeConfiguration<Tag>
    {
        public TagConfiguration()
        {
            this.HasKey(x => x.Id);

            this.Property(x => x.Name)
                .HasMaxLength(32)
                .IsRequired();
        }
    }
}
