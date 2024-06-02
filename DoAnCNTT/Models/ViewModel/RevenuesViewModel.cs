namespace DoAnCNTT.Models.ViewModel
{
    public class RevenuesViewModel
    {
        public IEnumerable<Invoice>? Invoices { get; set; }
        public int SelectedDay { get; set; }
        public int SelectedMonth { get; set; }
        public decimal Revenues { get; set; }
    }
}
