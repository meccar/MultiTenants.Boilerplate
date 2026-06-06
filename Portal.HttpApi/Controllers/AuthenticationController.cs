using BuildingBlocks.Shared.Constants.Permissions;
using BuildingBlocks.Shared.Dtos.Authentication;
using BuildingBlocks.Shared.Dtos.UserAccount;
using Host.Attributes;
using Identity.Application.Commands.ChangePassword;
using Identity.Application.Commands.CreateUserAccount;
using Identity.Application.Commands.ForgotPassword;
using Identity.Application.Commands.Login;
using Identity.Application.Commands.Logout;
using Identity.Application.Commands.ResetPassword;
using Identity.Domain.Extensions;
using Identity.Domain.Model;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Host.Controllers;

[ApiController]
//[ApiVersion("1.0")]
//[Route("api/v{version:apiVersion}/[controller]")]
[Route("api/[controller]")]
public class AuthenticationController
    : ControllerBase
{
    private readonly ILogger<AuthenticationController> _logger;
    private readonly IMediator _mediator;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthenticationController(
        ILogger<AuthenticationController> logger,
        IMediator mediator,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _logger = logger;
        _mediator = mediator;
        _httpContextAccessor = httpContextAccessor;
    }

    [HttpPost("CreateUserAccount")]
    [RequirePermission(Resources.UserAccount, Actions.Manage)]
    public async Task<ActionResult> CreateUserAccount(
        [FromBody] CreateUserAccountDto createUserAccountDto)
    {
        _logger.LogInformation($"START: {nameof(CreateUserAccount)}");

        CurrentUserModel currentUser = _httpContextAccessor.HttpContext!.GetCurrentUser();
        var result = await _mediator.Send(
            new CreateUserAccountCommand(
                createUserAccountDto, currentUser));

        _logger.LogInformation($"END: {nameof(CreateUserAccount)}");

        return Accepted(result);
    }

    [HttpPost("Login")]
    [AllowAnonymous]
    public async Task<ActionResult> Login(
        [FromBody] LocalLoginDto loginDto)
    {
        _logger.LogInformation($"START: {nameof(Login)}");
        
        var result = await _mediator.Send(
            new LocalAuthenticationCommand(
                loginDto));
        
        _logger.LogInformation($"END: {nameof(Login)}");
        
        return Accepted(result);
    }
    
    [HttpPost("OauthLogin")]
    [AllowAnonymous]
    public async Task<ActionResult> OauthLogin(
        [FromBody] OauthLoginDto loginDto)
    {
        _logger.LogInformation($"START: {nameof(OauthLogin)}");
        
        var result = await _mediator.Send(
            new OAuthAuthenticationCommand(
                loginDto));
        
        _logger.LogInformation($"END: {nameof(OauthLogin)}");
        
        return Accepted(result);
    }
    
    [HttpPost("Logout")]
    [RequirePermission(Resources.UserAccount, Actions.Zero)]
    public async Task<ActionResult> Logout()
    {
        _logger.LogInformation($"START: {nameof(Logout)}");

        var currentUser = _httpContextAccessor.HttpContext!.GetCurrentUser();
        var result = await _mediator.Send(
            new LogoutCommand(
                currentUser));
        
        _logger.LogInformation($"END: {nameof(Logout)}");
        
        return Accepted(result);
    }
    
    [HttpPost("ChangeLoggedInUserPassword")]
    [RequirePermission(Resources.UserAccount, Actions.ResetPassword)]
    public async Task<ActionResult> ChangeLoggedInUserPassword(
        [FromBody] ChangePasswordDto changePasswordDto)
    {
        _logger.LogInformation($"START: {nameof(ChangeLoggedInUserPassword)}");

        var currentUser = _httpContextAccessor.HttpContext!.GetCurrentUser();

        var result = await _mediator.Send(
            new ChangeLoggedInUserPasswordCommand(
                currentUser, changePasswordDto));
        
        _logger.LogInformation($"END: {nameof(ChangeLoggedInUserPassword)}");
        
        return Accepted(result);
    }
    
    [HttpPost("ChangeLoggedOutUserPassword")]
    [RequirePermission(Resources.UserAccount, Actions.ResetPassword)]
    public async Task<ActionResult> ChangeLoggedOutUserPassword(
        [FromBody] ChangePasswordDto changePasswordDto)
    {
        _logger.LogInformation($"START: {nameof(ChangeLoggedOutUserPassword)}");

        var result = await _mediator.Send(
            new ChangeLoggedOutUserPasswordCommand(
                changePasswordDto));
        
        _logger.LogInformation($"END: {nameof(ChangeLoggedOutUserPassword)}");
        
        return Accepted(result);
    }
    
    [HttpPost("ChangePasswordByOwner")]
    [RequirePermission(Resources.UserAccount, Actions.ResetPassword)]
    public async Task<ActionResult> ChangePasswordByOwner(
        [FromBody] ChangePasswordDto changePasswordDto)
    {
        _logger.LogInformation($"START: {nameof(ChangePasswordByOwner)}");

        var currentUser = _httpContextAccessor.HttpContext!.GetCurrentUser();
        
        var result = await _mediator.Send(
            new ChangePasswordByOwnerCommand(
                currentUser, changePasswordDto));
        
        _logger.LogInformation($"END: {nameof(ChangePasswordByOwner)}");
        
        return Accepted(result);
    }
    
    [HttpPost("ForgotPassword")]
    [AllowAnonymous]
    public async Task<ActionResult> ForgotPassword(
        [FromBody] string email)
    {
        _logger.LogInformation($"START: {nameof(ForgotPassword)}");

        var result = await _mediator.Send(
            new ForgotPasswordCommand(
                email));
        
        _logger.LogInformation($"END: {nameof(ForgotPassword)}");
        
        return Accepted(result);
    }
    
    [HttpPost("ResetPassword")]
    [AllowAnonymous]
    public async Task<ActionResult> ResetPassword(
        [FromBody] ResetPasswordDto resetPasswordDto,
        [FromQuery] string token)
    {
        _logger.LogInformation($"START: {nameof(ResetPassword)}");

        var result = await _mediator.Send(
            new ResetPasswordCommand(
                token, resetPasswordDto));
        
        _logger.LogInformation($"END: {nameof(ResetPassword)}");
        
        return Accepted(result);
    }
}
