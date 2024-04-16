using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdminPanel.Pages.management;

[Authorize]
public class categoryModel : PageModel
{
    public void OnGet()
    {
    }
}
