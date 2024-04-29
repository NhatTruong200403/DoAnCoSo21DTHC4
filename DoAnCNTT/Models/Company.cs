namespace DoAnCNTT.Models
{
    public class Company : BaseModel
    {
        public string? Name { get; set; }
        public string? IconImage { get; set; }
        public ICollection<CarTypeDetail> CarTypeDetail { get; set; } = new List<CarTypeDetail>();
    }
}
