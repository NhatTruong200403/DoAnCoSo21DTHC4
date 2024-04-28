using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace DoAnCNTT.Models
{
    public class Report : BaseModel
    {
        public string? Content { get; set; }
        public int PostId { get; set; }
        [ValidateNever]
        public Post Post { get; set; } = null!;
    }
}
