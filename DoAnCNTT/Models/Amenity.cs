namespace DoAnCNTT.Models
{
    public class Amenity : BaseModel
    {
        public string? Name { get; set; }
        public string? IconImage { get; set; }
        public ICollection<PostAmenity> PostAmenities { get; set; } = new List<PostAmenity>();
    }
}
