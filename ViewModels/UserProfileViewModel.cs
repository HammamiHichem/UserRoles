using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

public class UserProfileViewModel
{
    [Required(ErrorMessage = "Le nom complet est requis.")]
    public string FullName { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Le numéro de téléphone n'est pas valide.")]
    public string PhoneNumber { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    // ✅ Liste des compétences sélectionnées par l'utilisateur
    public List<string> SelectedSkills { get; set; } = new();

    // ✅ Image de profil (optionnelle)
    [Display(Name = "Photo de profil")]
    public IFormFile? ProfilePicture { get; set; }

    // ✅ Fichier CV PDF (optionnel)
    [Display(Name = "CV au format PDF")]
    [DataType(DataType.Upload)]
    public IFormFile? CvPdf { get; set; }
    public string? ProfilePictureUrl { get; set; }
public string? CvPdfUrl { get; set; }


    // ✅ Liste des compétences disponibles (non modifiable)
    public static List<string> AllSkills => new()
    {
        "C#", "ASP.NET Core", "JavaScript", "React", "Angular", 
        "SQL", "HTML", "CSS", "Python", "Java", "DevOps", "Docker"
    };
}
