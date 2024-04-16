using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdminPanel.Pages.management;

[Authorize]
public class matchesModel : PageModel
{
    public void OnGet()
    {
    }
}
