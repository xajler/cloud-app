using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CloudApp.API.Domain.Entities
{
    public class Tag
    {
        public Tag()
        {
            this.Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }

        [Index(IsUnique = true)]
        public string Name { get; set; }
    }
}
