using System.ComponentModel.DataAnnotations;


namespace EntityLayer.Entities
{
    public class VisitorProperty
    {
        [Key]
        public int VisitorPropertiyId { get; set; }
        [Required(ErrorMessage ="Ziyaretçi ismi girilmelidir")]
        public string VisitorName { get; set; }
        public string? VisitorSurName { get; set; }
        public bool VisitorActive { get; set; }
        public int VisitorId { get; set; }
        public Visitor Visitor { get; set; }
    }
}
