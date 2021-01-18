using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebTests.TestData
{
    [Table("menu_categories")]
    public record MenuCategory
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Column("name")]
        public string Name { get; set; }

        [Column("menu_id")]
        public int MenuId { get; set; }

        [ForeignKey(nameof(MenuId))]
        public Menu Menu { get; set; }

        public List<MenuItem> Items { get; set; } = new();
    }
}
