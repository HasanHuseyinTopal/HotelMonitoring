﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Entities
{
    public class Agency
    {
        [Key]
        public int AgencyId { get; set; }
        [MaxLength(12)]
        public string AgencyName { get; set; }
        public bool AgencyStatus { get; set; }
    }
}
