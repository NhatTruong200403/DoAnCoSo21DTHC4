using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace DoAnCNTT.Models
{
    public class Invoice : BaseModel
    {
        public decimal Total { get; set; }
        public DateTime ReturnOn {  get; set; }
        public int BookingId { get; set; }
        [ValidateNever]
        public Booking Booking { get; set; } = null!;
    }
}
