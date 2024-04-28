using Microsoft.AspNetCore.Identity;

namespace DoAnCNTT.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? Name { get; set; }
        public string? License { get; set; }
        public string? Image {  get; set; }
        public DateTime? Birthday { get; set; }
        public ICollection<Post> Posts { get; set; } = new List<Post>();    
        public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    }
}
