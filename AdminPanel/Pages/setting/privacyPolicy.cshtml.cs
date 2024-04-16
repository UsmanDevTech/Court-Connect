using Application.Content.Queries;
using Domain.Contracts;
using Domain.Enum;

namespace AdminPanel.Pages.setting;

public class privacyPolicyModel : BasePage
{
    public GenericAppDocumentContract htmlContent { get; set; }
    public async Task OnGet(CancellationToken cancellationToken)
    {
        htmlContent = await Mediator.Send(new GetUserGuideDocQuery(AppContentTypeEnum.Terms), cancellationToken);
    }
}
