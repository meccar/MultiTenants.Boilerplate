using BuildingBlocks.Attributes;
using BuildingBlocks.Shared.Dtos.UserAccount;
using Identity.Application.Commands.CreateUserAccount;
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

    public AuthenticationController(
        ILogger<AuthenticationController> logger,
        IMediator mediator
    )
    {
        _logger = logger;
        _mediator = mediator;
    }

    [HttpPost("CreateUserAccount")]
    [RequirePermission("Account:CreateUserAccount")]
    //[ApiValidationFilter]
    public async Task<ActionResult> CreateUserAccount(
        [FromBody] CreateUserAccountDto createUserAccountDto)
    {
        _logger.LogInformation("START: CreateUserAccount");

        var currentUser = HttpContext.GetCurrentUser();

        var loginResponse = await _mediator.Send(
            new CreateUserAccountCommand(
                createUserAccountDto));

        _logger.LogInformation("END: CreateUserAccount");

        return Created();
    }

}
