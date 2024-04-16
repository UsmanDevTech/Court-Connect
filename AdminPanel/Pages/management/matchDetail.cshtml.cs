using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdminPanel.Pages.management;

[Authorize]
public class matchDetailModel : PageModel
{
    public void OnGet(string userId)
    {
         this.userId = userId;
    }
    public string userId { get; set; } = null!;
   
}
