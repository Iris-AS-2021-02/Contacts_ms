﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Support.Dtos
{
    public class User
    {
        public int UserID { get; set; }

        [RegularExpression(@"^\+(?:[0-9]●?){6,14}[0-9]$", ErrorMessage = "Cell phone number format error.")]
        public string Phone { get; set; }

        public string Name { get; set; }
    }
}