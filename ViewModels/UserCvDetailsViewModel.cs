#pragma warning disable CS8618
using System;
using System.Collections.Generic;

namespace UserRoles.ViewModels
{
    public class UserCvDetailsViewModel
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public List<string> Skills { get; set; } = new List<string>();
        public string ProfilePicturePath { get; set; }
        public string CvPdfPath { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
