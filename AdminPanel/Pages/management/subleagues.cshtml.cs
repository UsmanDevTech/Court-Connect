using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdminPanel.Pages.management;

[Authorize]
public class subleaguesModel : PageModel
{
    public void OnGet(int subleagueId)
    {
        this.subleagueId = subleagueId;
    }
    public int subleagueId { get; set; }
}
