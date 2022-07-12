using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ManagingTextResourcesApp.Models
{
    public class TextResource
    {
        [Key]
        public int Id { get; set; }
        [Index("Unique", 1, IsUnique = true)]
        public int UserId { get; set; }
        [Index("Unique", 2, IsUnique = true)]
        [Display(Name = "Tytuł")]
        [StringLength(50, ErrorMessage = "Maksymalna długość tytułu to 50 znaków!")]
        public string Title { get; set; }
        [Display(Name = "Tekst")]
        [StringLength(1000, ErrorMessage = "Maksymalna długość tekstu to 1000 znaków!")]
        public string Text { get; set; }
        public string EditHashCode { get; set; } = "";
    }
}