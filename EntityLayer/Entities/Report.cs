using System.ComponentModel.DataAnnotations;


namespace EntityLayer.Entities
{
    public class Report
    {
        [Key]
        public int ReportId { get; set; }
        public DateTime ReportDate { get; set; } = DateTime.Now;
        [Required(ErrorMessage ="Mesaj boş bırakılamaz")]
        public string ReportMessage { get; set; }
        [Required(ErrorMessage ="Gönderici boş bırakılamaz")]
        public string Reporter { get; set; } 
    }
}
