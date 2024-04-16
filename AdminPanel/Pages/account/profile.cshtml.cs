using Application.Accounts.Queries;
using Domain.Contracts;

namespace AdminPanel.Pages.account;

public class profileModel : BasePage
{
    public UserProfileInfoDetailContract Profile { get; set; }
    public async Task OnGet()
    {
        Profile=await Mediator.Send(new GetAccountProfileQuery(null));
    }
}
