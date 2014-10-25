using CloudApp.API.Domain.Context.Configurations;
using CloudApp.API.Domain.Entities;
using System.Data.Entity;

namespace CloudApp.API.Domain.Context
{
    public class CloudAppContext : DbContext
    {
        public CloudAppContext() : base("CloudAppContext")
        {
        }

        public DbSet<Tag> Tags { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new TagConfiguration());
            base.OnModelCreating(modelBuilder);
        }
    }
}
