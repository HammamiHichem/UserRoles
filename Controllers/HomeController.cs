using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserRoles.Models;
using UserRoles.ViewModels;

namespace UserRoles.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<Users> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        // ✅ Constructeur unique et correct
        public HomeController(ILogger<HomeController> logger, UserManager<Users> userManager, RoleManager<IdentityRole> roleManager)
        {
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
        }

         public async Task<IActionResult> Index()
    {
        if (User.Identity.IsAuthenticated)
        {
            var user = await _userManager.GetUserAsync(User);

            // Logique de sécurité : fallback sur UserName si FullName est vide
            if (user != null)
            {
                ViewBag.FullName = string.IsNullOrWhiteSpace(user.FullName)
                    ? user.UserName
                    : user.FullName;
            }
        }

        return View();
    }

        // ✅ Page UserCV (liste des utilisateurs)
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UserCV()
        {
            var users = await _userManager.Users.ToListAsync();
            return View(users);
        }

        // ✅ Page Admin pour gérer les rôles
        [Authorize(Roles = "Admin")]
public async Task<IActionResult> ManageRoles()
{
    var users = await _userManager.Users.ToListAsync();
    var model = new List<EditUserRolesViewModel>();

    foreach (var user in users)
    {
        var viewModel = new EditUserRolesViewModel
        {
            UserId = user.Id,
            UserEmail = user.Email ?? string.Empty,
            Roles = await GetUserRolesAsCheckboxes(user)
        };

        model.Add(viewModel);
    }

    return View(model);
}

public class AccountController : Controller
{
    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }
}
public IActionResult AccessDenied(string returnUrl)
{
    ViewBag.ReturnUrl = returnUrl;
    return View();
}

private async Task<List<RoleCheckBox>> GetUserRolesAsCheckboxes(Users user)
{
    var roles = await _roleManager.Roles
        .Where(r => r.Name != null)
        .ToListAsync();

    var userRoles = await _userManager.GetRolesAsync(user);

    return roles.Select(role => new RoleCheckBox
    {
        RoleName = role.Name!,
        IsSelected = userRoles.Contains(role.Name!)
    }).ToList();
}

// Action pour contacter un utilisateur
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public IActionResult ContactUser()
    {
        // Logique pour contacter un utilisateur, par exemple en envoyant un email
        // Tu peux ajouter un service d'email ou d'autres logiques ici

        // Exemple simple : rediriger vers une page de confirmation ou envoyer un email
        return RedirectToAction("ContactConfirmation", "Home");
    }

    // Action pour la confirmation de l'envoi du message
    public IActionResult ContactConfirmation()
    {
        return View();  // Tu peux créer une vue pour informer l'administrateur que le message a été envoyé
    }

        // ✅ Afficher l'interface de modification des rôles
        [Authorize(Roles = "Admin")]
public async Task<IActionResult> EditUserRoles(string id)
{
    var user = await _userManager.FindByIdAsync(id);
    if (user == null) return NotFound();

    var model = new EditUserRolesViewModel
    {
        UserId = user.Id,
        UserEmail = user.Email!
    };

    var roles = await _roleManager.Roles
        .Where(r => r.Name != null)
        .ToListAsync();

    foreach (var role in roles)
    {
        model.Roles.Add(new RoleCheckBox
        {
            RoleName = role.Name!,
            IsSelected = await _userManager.IsInRoleAsync(user, role.Name!)
        });
    }

    return View(model);
}


        // ✅ Traiter la soumission du formulaire de modification des rôles
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditUserRoles(EditUserRolesViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null) return NotFound();

            var currentRoles = await _userManager.GetRolesAsync(user);
            var selectedRoles = model.Roles.Where(x => x.IsSelected).Select(x => x.RoleName).ToList();

            var result = await _userManager.RemoveFromRolesAsync(user, currentRoles.Except(selectedRoles));
            if (!result.Succeeded) return View(model);

            result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(currentRoles));
            if (!result.Succeeded) return View(model);

            return RedirectToAction("ManageRoles");
        }

        // ✅ Page utilisateur classique
        [Authorize(Roles = "User")]
public async Task<IActionResult> UserPage()
{
    var user = await _userManager.GetUserAsync(User);
    if (user == null)
    {
        return RedirectToAction("Login", "Account"); // Redirige si l'utilisateur n'est pas trouvé
    }

    var fullName = user.FullName ?? "Guest";
    ViewData["UserName"] = fullName;

    return View(user);  // Passe l'objet user à la vue
}

[HttpGet]
[Authorize]
public async Task<IActionResult> UserProfile()
{
    var user = await _userManager.GetUserAsync(User);
    if (user == null) return NotFound();

    var model = new UserProfileViewModel
    {
        FullName = user.FullName,
        PhoneNumber = user.PhoneNumber,
        Address = user.Address,
        SelectedSkills = user.Skills,
        ProfilePictureUrl = user.ProfilePicturePath,
        CvPdfUrl = user.CvPdfPath
    };

    return View(model);
}


[HttpPost]
[Authorize]
public async Task<IActionResult> UserProfile(UserProfileViewModel model)
{
    var user = await _userManager.GetUserAsync(User);
    if (user == null) return NotFound();

    user.PhoneNumber = model.PhoneNumber;
    user.FullName = model.FullName;
    user.Address = model.Address;
    user.Skills = model.SelectedSkills;

    // Upload photo
    if (model.ProfilePicture != null)
    {
        var fileName = Path.GetFileName(model.ProfilePicture.FileName);
        var filePath = Path.Combine("wwwroot/uploads/photos", fileName);
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await model.ProfilePicture.CopyToAsync(stream);
        }
        user.ProfilePicturePath = "/uploads/photos/" + fileName;
    }

    // Upload CV PDF
    if (model.CvPdf != null)
    {
        var fileName = Path.GetFileName(model.CvPdf.FileName);
        var filePath = Path.Combine("wwwroot/uploads/cvs", fileName);
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await model.CvPdf.CopyToAsync(stream);
        }
        user.CvPdfPath = "/uploads/cvs/" + fileName;
    }

    await _userManager.UpdateAsync(user);

    TempData["Success"] = "Votre profil a été mis à jour.";
    return RedirectToAction("UserProfile");
}

[HttpGet]
[Authorize]
public async Task<IActionResult> UserCard()
{
    var user = await _userManager.GetUserAsync(User);
    if (user == null) return NotFound();

    var model = new UserProfileViewModel
    {
        FullName = user.FullName,
        PhoneNumber = user.PhoneNumber,
        Address = user.Address,
        SelectedSkills = user.Skills,
        ProfilePictureUrl = user.ProfilePicturePath,
        CvPdfUrl = user.CvPdfPath
    };

    return View(model);
}


 // Méthode pour afficher le CV d'un utilisateur
    [HttpGet]
    public async Task<IActionResult> UserCvDetails(string userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            return NotFound();
        }

        // Recherche l'utilisateur
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        // Créer un modèle pour afficher le CV
        var model = new UserCvDetailsViewModel
{
    FullName = user.FullName,
    Email = user.Email,
    PhoneNumber = user.PhoneNumber,
    Address = user.Address,
    Skills = user.Skills ?? new List<string>(),
    CvPdfPath = user.CvPdfPath,
    CreatedAt = user.CreatedAt,
    ProfilePicturePath = user.ProfilePicturePath // 👈 Ajoute ceci
};


        return View(model);
    }
        // ✅ Gestion des erreurs
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
