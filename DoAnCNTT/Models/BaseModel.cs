using System.ComponentModel.DataAnnotations;

namespace DoAnCNTT.Models
{
    public class BaseModel
    {
        public int Id { get; set; }
        public string? CreatedById { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm tt}", ApplyFormatInEditMode = true)]
        public DateTime CreatedOn { get; set; }
        public string? ModifiedById { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm tt}", ApplyFormatInEditMode = true)]
        public DateTime? ModifiedOn { get; set;}
        public bool IsDeleted { get; set; }
    }
}
