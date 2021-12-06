﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Support.Dtos
{
    public  class ContactSettingsResponse
    {
        public Guid ContactID { get; set; }

        public bool? Blocked { get; set; }

        public bool? SeeStatus { get; set; }

        public string? Wallpaper { get; set; }
    }
}
