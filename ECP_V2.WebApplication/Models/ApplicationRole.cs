using Microsoft.AspNet.Identity.EntityFramework;

namespace ECP_V2.WebApplication.Models
{
    public class ApplicationRole : IdentityRole
    {
        public ApplicationRole() : base() { }

        public string Description { get; set; }
        public int? TypeOfRole { get; set; }

    }
}