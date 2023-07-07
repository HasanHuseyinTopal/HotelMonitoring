using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.DTOs
{
    public class ReportsDTO
    {
        [Required(ErrorMessage ="Gönderici ismi girilmelidir")]
        public Report Report { get; set; }
        public List<Report> ReportList { get; set; }
    }
}
