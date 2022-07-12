using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ManagingTextResourcesApp.Models
{
    public class ViewModel
    {
        public IEnumerable<ManagingTextResourcesApp.Models.TextResource> TextResources { get; set; }
        public User User { get; set; }
    }
}