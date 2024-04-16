using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdminPanel.Pages.management;

[Authorize]
public class leaguesModel : PageModel
{
    public void OnGet()
    {
    }
}
