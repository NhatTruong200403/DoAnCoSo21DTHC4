﻿namespace DoAnCNTT.Models
{
    public class CarType : BaseModel
    {
        public string? Name { get; set; }
        public ICollection<Post> Posts { get; set; } = new List<Post>();
        public ICollection<CarTypeDetail> CarTypeDetail { get; set; } = new List<CarTypeDetail>();
    }
}
