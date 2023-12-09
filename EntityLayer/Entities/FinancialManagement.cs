using System.ComponentModel.DataAnnotations;


namespace EntityLayer.Entities
{
    public class FinancialManagement
    {
        [Key]
        public int FinancialId { get; set; }
        [Required(ErrorMessage ="Miktar girilmelidir")]
        public double? FinancialPayment { get; set; }
        [Required(ErrorMessage = "Bölme seçilmelidir")]
        public FinancialIssuer? FinancialIssuer { get; set; }
        [Required(ErrorMessage = "Para birimi seçilmelidir")]
        public Currency FinancialCurrency { get; set; }
        [Required(ErrorMessage = "Açıklama yazılmalıdır")]
        public string? FinancialDescripton { get; set; }
        public DateTime? FinancialDate { get; set; } = DateTime.Now.Date.AddHours(DateTime.Now.Hour).AddMinutes(DateTime.Now.Minute);
        public int FinancialUpdateCount { get; set; } = 0;
    }
    public enum FinancialIssuer
    {
        Resepsiyon,
        Müdüriyet,
        AnaKasa
    }
}
