using EntityLayer.CustomValidation;
using EntityLayer.Entities;
using System.ComponentModel.DataAnnotations;


namespace EntityLayer.DTOs
{
    public class AddVisitorDTO
    {
        public double? VisitorRoomPrice { get; set; }
        public string? VisitorPhoneNumber { get; set; }
        public DateTime? VisitorBirthDay { get; set; }
        [Required(ErrorMessage = "Başlangıç tarihi seçilmelidir")]
        [DateRange(nameof(VisitorEndDate), ErrorMessage = "Bitiş tarihi başlangıç tarihinden önce olamaz ")]
        public DateTime VisitorStartDate { get; set; }
        [Required(ErrorMessage = "Bitiş tarihi seçilmelidir ")]
        public DateTime VisitorEndDate { get; set; }
        public Currency? VisitorPaymentCurrency { get; set; }
        public string? VisitorDescription { get; set; }
        public Rezervation? VisitorRezervation { get; set; }
        public int? VisitorCount { get; set; }
    }
    public enum Rezervation
    {
        Expedia,
        Booking,
        WalkIn,
        Angel,
        Egypt,
        İran
    }
}

