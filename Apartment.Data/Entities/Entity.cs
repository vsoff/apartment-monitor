using System;
using System.ComponentModel.DataAnnotations;

namespace Apartment.Data.Entities
{
    public class Entity
    {
        [Key]
        public int Id { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
        public bool IsDeleted { get; set; }
    }
}