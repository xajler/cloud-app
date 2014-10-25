namespace CloudApp.API.Domain.Migrations
{
    using CloudApp.API.Domain.Context;
    using CloudApp.API.Domain.Entities;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using EntityFramework.BulkInsert.Extensions;

    internal sealed class Configuration : DbMigrationsConfiguration<CloudApp.API.Domain.Context.CloudAppContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(CloudApp.API.Domain.Context.CloudAppContext context)
        {
            List<Tag> tags = new List<Tag>
            {
                new Tag { Name = "Judas Priest" },
                new Tag { Name = "Sting" },
                new Tag { Name = "Dire Straits" },
                new Tag { Name = "Iron Maiden" },
                new Tag { Name = "The Cult" },
                new Tag { Name = "Chris Rea" },
                new Tag { Name = "Led Zeppelin" },
                new Tag { Name = "Black Sabbath" }
            };

            context.BulkInsert(tags);
            context.SaveChanges();
        }
    }
}
