﻿using Microsoft.AspNetCore.Identity;

namespace THweb.Areas.Identity.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public string Address { get; set; }
    }
}