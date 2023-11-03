using EntityLayer.CustomValidation;
using EntityLayer.Entities;
using System.ComponentModel.DataAnnotations;

namespace EntityLayer.DTOs
{
    public class AddVisitorDTO
    {
        public AddVisitorDTO()
        {
            VisitorPropertyDTOs = new List<VisitorProperty>();
        }
        public List<VisitorProperty> VisitorPropertyDTOs { get; set; }
        public double? VisitorRoomPrice { get; set; }
        [Required(ErrorMessage = "Oda numarası seçilmelidir")]
        public int VisitorRoomNumber { get; set; }
        public string? VisitorPhoneNumber { get; set; }
        [Required(ErrorMessage = "Başlangıç tarihi seçilmelidir")]
        [DateRange(nameof(VisitorEndDate), ErrorMessage = "Bitiş tarihi başlangıç tarihinden önce olamaz ")]
        public DateTime VisitorStartDate { get; set; }
        [Required(ErrorMessage = "Bitiş tarihi seçilmelidir ")]
        public DateTime VisitorEndDate { get; set; }
        public Currency? VisitorPaymentCurrency { get; set; }
        public string? VisitorDescription { get; set; }
        public string? VisitorRezervation { get; set; }
        public double? VisitorTotalRoomPrice { get; set; }
        public int? VisitorCount { get; set; }
        public DateTime? VisitorAddedDate { get; set; } = DateTime.Now;
        public bool VisitorDontChangeRoom { get; set; }
        public string? VisitorFileUrl { get; set; }
        public int? VisitorPreviusId { get; set; } = 0;
    }

}

