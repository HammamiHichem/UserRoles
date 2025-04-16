#pragma warning disable CS8618
using Microsoft.AspNetCore.Identity;
using System.Text.Json;
using System.ComponentModel.DataAnnotations.Schema; // <-- Ajouté

namespace UserRoles.Models
{
    public class Users : IdentityUser
    {
        public new string? PhoneNumber { get; set; }
        public string FullName { get; set; }
        public DateTime CreatedAt { get; set; }

        public string? Address { get; set; }

        // 👉 Convertir la liste Skills en JSON pour persistance
        public string? SkillsJson { get; set; }

        [NotMapped]
        public List<string> Skills
        {
            get => string.IsNullOrEmpty(SkillsJson)
                ? new List<string>()
                : JsonSerializer.Deserialize<List<string>>(SkillsJson) ?? new List<string>();
            set => SkillsJson = JsonSerializer.Serialize(value);
        }

        public string? ProfilePicturePath { get; set; }
        public string? CvPdfPath { get; set; }
    }
}
