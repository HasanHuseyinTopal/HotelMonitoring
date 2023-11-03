using EntityLayer.CustomValidation;
using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.DTOs
{
    public class UpdateVisitorDTO
    {
        public UpdateVisitorDTO()
        {
            VisitorProperties = new();
        }
        [Key]
        public int VisitorId { get; set; }
        public int VisitorState { get; set; }
        public List<VisitorProperty>? VisitorProperties { get; set; }
        public double? VisitorRoomPrice { get; set; }
        public double? VisitorTotalRoomPrice { get; set; }
        public string? VisitorPhoneNumber { get; set; }

        [Required(ErrorMessage = "Başlangıç tarihi seçilmelidir")]
        [DateRange(nameof(VisitorEndDate), ErrorMessage = "Bitiş tarihi başlangıç tarihinden önce olamaz ")]
        public DateTime VisitorStartDate { get; set; }
        [Required(ErrorMessage = "Bitiş tarihi seçilmelidir ")]
        public DateTime VisitorEndDate { get; set; }
        [Required(ErrorMessage = "Oda numarası seçilmelidir")]
        public int VisitorRoomNumber { get; set; }
        public Currency? VisitorPaymentCurrency { get; set; }
        public string? VisitorDescription { get; set; }
        public int? VisitorCount { get; set; }
        public string? VisitorRezervation { get; set; }
        public bool VisitorPaymentIsDone { get; set; }
        public DateTime? VisitorAddedDate { get; set; }
        public bool VisitorDontChangeRoom { get; set; }
        public string? VisitorFileUrl { get; set; }
        public int? VisitorPreviusId { get; set; }
        public int? VisitorNextId { get; set; }

    }
}
