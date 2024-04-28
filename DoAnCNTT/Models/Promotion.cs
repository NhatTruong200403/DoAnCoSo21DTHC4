using System.ComponentModel.DataAnnotations;

namespace DoAnCNTT.Models
{
    public class Promotion : BaseModel
    {
        public string? Content { get; set; }
        public decimal DiscountValue { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm tt}", ApplyFormatInEditMode = true)]
        public DateTime? ExpiredDate { get; set; }
        public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    }
}
