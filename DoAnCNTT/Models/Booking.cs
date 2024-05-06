using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace DoAnCNTT.Models
{
    public class Booking : BaseModel
    {
        public decimal PrePayment { get; set; }
        public decimal Total { get; set; } 
        public decimal FinalValue { get; set; }
        public DateTime RecieveOn { get; set; }
        public DateTime ReturnOn { get; set; }
        public int PostId { get; set; }
        [ValidateNever]
        public Post Post { get; set; } = null!;
        public string? UserId { get; set; }
        [ValidateNever]
        public ApplicationUser User { get; set; } = null!;
        public int PromotionId { get; set; }
        [ValidateNever]
        public Promotion Promotion { get; set; } = null!;
        public int InvoiceId { get; set; }
        [ValidateNever]
        public Invoice Invoice { get; set; } = null!;
    }
}
