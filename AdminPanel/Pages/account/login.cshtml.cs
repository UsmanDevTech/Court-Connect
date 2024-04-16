using Domain.Enum;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace AdminPanel.Pages.account;

public class loginModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    public loginModel(UserManager<ApplicationUser> userManger, SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManger;
        _signInManager = signInManager;
    }

    [BindProperty]
    public credential credential { get; set; }
    public async Task<IActionResult> OnGet(string? ReturnUrl)
    {
        this.ViewData["ReturnUrl"] = ReturnUrl;

        if (_signInManager.IsSignedIn(User))
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            var user = await _userManager.FindByIdAsync(userId);

            if (user.LoginRole == UserTypeEnum.Admin)
                return RedirectToPage("/account/dashboard");
            else
                return RedirectToPage("/account/logout");
        }
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        if (credential.ReturnUrl != null && !String.IsNullOrEmpty(credential.ReturnUrl))
            credential.ReturnUrl ??= Url.Content("~/");

        // This doesn't count login failures towards account lockout
        // To enable password failures to trigger account lockout, set lockoutOnFailure: true
        //check username / email
        var user = await _userManager.FindByEmailAsync(credential.Email);
        if (user == null)
        {
            ModelState.AddModelError("credential.Email", "Invalid email or username.");
            return Page();
        }

        if (user.LoginRole != UserTypeEnum.Admin)
        {
            ModelState.AddModelError("", "Access denied: Because the user account is not authorized for dashboard.");
            return Page();
        }

        //check password
        var result = await _userManager.CheckPasswordAsync(user, credential.Password);
        if (!result)
        {
            ModelState.AddModelError("credential.Password", "Invalid Password.");
            return Page();
        }
        //SignIn into application
        await _signInManager.SignInAsync(user, isPersistent: credential.RememberMe, authenticationMethod: "");

        if (Url.IsLocalUrl(credential.ReturnUrl))
            return Redirect(credential.ReturnUrl);

        if (user.LoginRole == UserTypeEnum.Admin)
            return RedirectToPage("/account/dashboard");
        else
            return RedirectToPage("/account/logout");
    }
}
public class credential
{
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;
    [Required]
    public string Email { get; set; } = null!;
    [Required]
    public bool RememberMe { get; set; }
    public string? ReturnUrl { get; set; }
}