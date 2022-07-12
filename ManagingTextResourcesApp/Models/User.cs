using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ManagingTextResourcesApp.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        [Display(Name = "Nazwa użytkownika")]
        [StringLength(50, ErrorMessage = "Maksymalna długość nazwy użytkownika to 50 znaków!")]
        public string Name { get; set; }
        [Display(Name = "Hasło")]
        [StringLength(50, ErrorMessage = "Maksymalna długość hasła to 50 znaków!")]
        public string Password { get; set; }
        public string status { get; set; }
    }
}