using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace DoAnCNTT.Models
{
    public class Rating : BaseModel
    {
        public string? Comment { get; set; }
        public float? Point { get; set; }
        public int PostId { get; set; }
        [ValidateNever]
        public Post Post { get; set; } = null!;
    }
}
