namespace DoAnCNTT.Models
{
    public class Company : BaseModel
    {
        public string? Name { get; set; }
        public string? IconImage { get; set; }
        public ICollection<Post> Posts { get; set; } = new List<Post>();
        public ICollection<CarTypeDetail> CarTypeDetail { get; set; } = new List<CarTypeDetail>();
    }
}
