using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace DoAnCNTT.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? Name { get; set; }
        public string? License { get; set; }
        public string? Image {  get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? Birthday { get; set; }
        public int? ReportPoint { get; set; } = 0;
        public ICollection<Post> Posts { get; set; } = new List<Post>();    
        public ICollection<Booking> Booking { get; set; } = new List<Booking>();
        public ICollection<Rating> Rating { get; set; } = new List<Rating>();
        public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
    }
}
