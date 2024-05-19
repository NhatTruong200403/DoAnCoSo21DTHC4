using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace DoAnCNTT.Models
{
    public class Booking : BaseModel
    {
        public decimal PrePayment { get; set; }
        public decimal Total { get; set; } 
        public decimal FinalValue { get; set; }
        //[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime RecieveOn { get; set; }
        //[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime ReturnOn { get; set; }
        public bool IsPay {  get; set; }
        public string? Status {  get; set; }
        public bool IsRequest { get; set; }
        public int PostId { get; set; }
        [ValidateNever]
        public Post Post { get; set; } = null!;
        public string? UserId { get; set; }
        [ValidateNever]
        public ApplicationUser User { get; set; } = null!;
        public int PromotionId { get; set; }
        [ValidateNever]
        public Promotion Promotion { get; set; } = null!;
    }
}
