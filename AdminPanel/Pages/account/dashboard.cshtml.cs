

using Application.Services.Queries;
using Domain.Contracts;

namespace AdminPanel.Pages.account;

public class dashboardModel: BasePage
{
    public DashboardContract counters { get; set; }
    public async Task OnGet(CancellationToken cancellationToken)
    {
        counters = await Mediator.Send(new DashboardStatsQuery(), cancellationToken);
    }
}
