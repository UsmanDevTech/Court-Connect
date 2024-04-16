using Application.Common.Exceptions;
using Application.Common.Models;
using Application.Content.Queries;
using Application.Services.Command.Notification;
using Application.Services.Commands;
using Application.Services.Queries;
using Domain.Contracts;
using Domain.Generics;
using Domain.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.V1;
[ApiVersion("1.0")]
public class sharedController : BaseController
{
    [AllowAnonymous]
    [HttpPost(Routes.Shared.upload_base64_file), DisableRequestSizeLimit]
    public ResponseKey UploadFile([FromBody] ResponseKey source)
    {
        try
        {
            if (string.IsNullOrEmpty(source.key))
                throw new NotFoundException("No file attached.");

            var folderName = Path.Combine(@"wwwroot", "Images");
            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
            
            //Check if directory exist
            if (!Directory.Exists(pathToSave))
                Directory.CreateDirectory(pathToSave); //Create directory if it doesn't exist

            string imageName = "Base" + DateTime.Now.Ticks.ToString() + source.key.GetBase64FileType();

            //set the image path
            string imgPath = Path.Combine(pathToSave, imageName);

            byte[] imageBytes = Convert.FromBase64String(source.key);

            System.IO.File.WriteAllBytes(imgPath, imageBytes);
            //Get Base Url From Request
            var baseUrl = $"https://{this.Request.Host}{this.Request.PathBase}";
            //Final Return Url
            var dbPath = $"{baseUrl}/Images/{imageName}";

            return new ResponseKey { key= dbPath };
        }
        catch(Exception ex)
        {
            throw new CustomInvalidOperationException(ex.GetBaseException().Message);
        }
        
    }
    
    [AllowAnonymous]
    [HttpGet(Routes.Shared.get_terms)]
    public async Task<GenericAppDocumentContract> GetTermsAsync(CancellationToken token)
    {
        return await Mediator.Send(new GetUserGuideDocQuery(Domain.Enum.AppContentTypeEnum.Terms), token);
    }

    [AllowAnonymous]
    [HttpGet(Routes.Shared.get_policy)]
    public async Task<GenericAppDocumentContract> GetPolicyAsync(CancellationToken token)
    {
        return await Mediator.Send(new GetUserGuideDocQuery(Domain.Enum.AppContentTypeEnum.PrivacyPolicy), token);
    }

    [HttpGet(Routes.Shared.about_application)]
    public async Task<AboutAppContract> AboutApplicationAsync(CancellationToken token)
    {
        return await Mediator.Send(new GetAboutAppQuery(), token);
    }

    [HttpPost(Routes.Shared.get_notifications)]
    public async Task<PaginationResponseBase<List<NotificationContract>>> GetNotificationsAsync([FromBody] GetNotificationQuery getNotification, CancellationToken token)
    {
        return await Mediator.Send(getNotification, token);
    }

    [HttpPut(Routes.Shared.read_all_notifications)]
    public async Task<Result> ReadAllNotificationsAsync(CancellationToken token)
    {
        return await Mediator.Send(new ReadUserAllNotificationsCommand(), token);
    }

    [HttpPut(Routes.Shared.read_single_notifications)]
    public async Task<Result> ReadSingleNotificationsAsync(ReadSingleNotificationCommand command, CancellationToken token)
    {
        return await Mediator.Send(command, token);
    }
}
