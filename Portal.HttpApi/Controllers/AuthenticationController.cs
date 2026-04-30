using BuildingBlocks.Attributes;
using BuildingBlocks.Shared.Dtos.Authentication;
using BuildingBlocks.Shared.Dtos.UserAccount;
using Identity.Application.Commands.ChangePassword;
using Identity.Application.Commands.ConfirmEmail;
using Identity.Application.Commands.CreateUserAccount;
using Identity.Application.Commands.Login;
using Identity.Application.Commands.Logout;
using Identity.Domain.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BuildingBlocks.Controllers;

[ApiController]
//[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
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
    [RequirePermission("Account:CreateUserAccount")]
    //[ApiValidationFilter]
    public async Task<ActionResult> CreateUserAccount(
        [FromBody] CreateUserAccountDto createUserAccountDto)
    {
        _logger.LogInformation($"START: {nameof(CreateUserAccount)}");

        var result = await _mediator.Send(
            new CreateUserAccountCommand(
                createUserAccountDto));

        _logger.LogInformation($"END: {nameof(CreateUserAccount)}");

        return Created();
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
    [RequirePermission("Account:Logout")]
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
    [RequirePermission("Account:ChangeLoggedInUserPassword")]
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
    [RequirePermission("Account:ChangeLoggedOutUserPassword")]
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
    [RequirePermission("Account:ChangePasswordByOwner")]
    public async Task<ActionResult> ChangePasswordByOwner(
        [FromBody] ChangePasswordDto changePasswordDto)
    {
        _logger.LogInformation($"START: {nameof(ChangeLoggedOutUserPassword)}");

        var currentUser = _httpContextAccessor.HttpContext!.GetCurrentUser();
        
        var result = await _mediator.Send(
            new ChangePasswordByOwnerCommand(
                currentUser, changePasswordDto));
        
        _logger.LogInformation($"END: {nameof(ChangeLoggedOutUserPassword)}");
        
        return Accepted(result);
    }
}
