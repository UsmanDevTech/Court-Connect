using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdminPanel.Pages.management;

[Authorize]
public class usersModel : PageModel
{
    public void OnGet()
    {
    }
}
