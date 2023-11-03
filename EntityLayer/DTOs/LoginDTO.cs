using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.DTOs
{
    public class LoginDTO
    {
        [Required(ErrorMessage ="Kullanıcı Adı Boş Bırakılamaz")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Şifre Boş Bırakılamaz")]
        public string Password { get; set; }
    }
}
