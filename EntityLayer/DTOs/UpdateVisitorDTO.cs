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
        [Key]
        public int VisitorId { get; set; }
        public int VisitorState { get; set; }
        [Required(ErrorMessage = "Ziyaretçi adı girilmelidir")]
        public string VisitorName { get; set; }
        public string? VisitorSurName { get; set; } 
        public double? VisitorRoomPrice { get; set; }
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
        public Rezervation? VisitorRezervation { get; set; }
        public bool VisitorPaymentIsDone { get; set; }

    }
}
