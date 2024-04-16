using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdminPanel.Pages.account;

public class logoutModel : PageModel
{
    private readonly SignInManager<ApplicationUser> _signInManager;

    public logoutModel(SignInManager<ApplicationUser> signInManager)
    {
        _signInManager = signInManager;
    }

    public async Task<IActionResult> OnPostAsync()
    {

        await _signInManager.SignOutAsync();
        return RedirectToPage("/account/login");
    }
}
