namespace DoAnCNTT.Models
{
    public class Payment : BaseModel
    {
        public string? Name { get; set; }
        public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    }
}
