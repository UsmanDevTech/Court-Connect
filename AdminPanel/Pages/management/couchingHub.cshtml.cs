using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdminPanel.Pages.management;

[Authorize]
public class couchingHubModel : PageModel
{
    public void OnGet()
    {
    }
}
