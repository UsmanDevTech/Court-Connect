using Application.Content.Queries;
using Domain.Contracts;

namespace AdminPanel.Pages.setting;

public class aboutModel : BasePage
{
    public AboutAppContract content { get; set; }
    public async Task OnGet(CancellationToken cancellationToken)
    {
        content = await Mediator.Send(new GetAboutAppQuery(), cancellationToken);
    }
}
