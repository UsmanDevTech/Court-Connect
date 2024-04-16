using Application.Accounts.Commands;
using Application.Accounts.Queries;
using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.V1;

[ApiVersion("1.0")]
public class accountController : BaseController
{
    /// <summary>
    /// POST REQUESTS

    private readonly IIdentityService _identityService;

    public accountController(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    [HttpPost(Routes.Account.delete_account)]
    public async Task<Result> DeleteAccountAsync([FromBody] DeleteAccountCommand command, CancellationToken token)
    {
        return await Mediator.Send(command, token);
    }

    [AllowAnonymous]
    [HttpPost(Routes.Account.login)]
    public async Task<ResponseKeyContract> LoginAsync([FromBody] LoginQuery loginCommand, CancellationToken token)
    {
        return await Mediator.Send(loginCommand, token);
    }


    [AllowAnonymous]
    [HttpPost(Routes.Account.register_account)]
    public async Task<Result> RegisterAccountAsync([FromBody] CreateAccountCommand createAccount, CancellationToken token)
    {

        //_identityService.AddTestingUserData(res,token);
        //return new Result(true, Array.Empty<string>());
        return await Mediator.Send(createAccount, token);
    }

    [AllowAnonymous]
    [HttpPost(Routes.Account.send_otp)]
    public async Task<Result> SendOtpAsync([FromBody] SendEmailOtpCommand sendOtp, CancellationToken token)
    {
        return await Mediator.Send(sendOtp, token);
    }

    /// <summary>
    /// PUT REQUESTS

    [AllowAnonymous]
    [HttpPut(Routes.Account.verify_account)]
    public async Task<ResponseKeyContract> VerifyOtpAsync([FromBody] ConfirmEmailCommand verifyAccount, CancellationToken token)
    {
        return await Mediator.Send(verifyAccount, token);
    }

    [AllowAnonymous]
    [HttpPut(Routes.Account.reset_password)]
    public async Task<Result> ResetPasswordAsync([FromBody] ResetPasswordViaEmailCommand resetPassword, CancellationToken token)
    {
        return await Mediator.Send(resetPassword, token);
    }

    
    [HttpPut(Routes.Account.update_profile)]
    public async Task<Result> UpdateProfileAsync([FromBody] UpdateProfileCommand updateProfile, CancellationToken token)
    {
        return await Mediator.Send(updateProfile, token);
    }

    [HttpPut(Routes.Account.logout)]
    public async Task<Result> LogoutProfileAsync(CancellationToken token)
    {
        return await Mediator.Send(new LogoutCommand(), token);
    }

    /// <summary>
    /// Delete REQUESTS

    [HttpPut(Routes.Account.remove_profile_image)]
    public async Task<Result> RemoveProfileImageAsync(CancellationToken token)
    {
        return await Mediator.Send(new RemoveProfileImageCommand(), token);
    }

 
    /// <summary>
    /// GET REQUESTS

    [HttpGet(Routes.Account.get_profile)]
    public async Task<UserProfileInfoDetailContract> GetProfileAsync([FromQuery] string? userId, CancellationToken token)
    {
        return await Mediator.Send(new GetAccountProfileQuery(userId), token);
    }


}
