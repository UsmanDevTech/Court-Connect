using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdminPanel.Pages.management;

[Authorize]
public class registerOrganizationModel : PageModel
{
    public void OnGet()
    {
    }
}
