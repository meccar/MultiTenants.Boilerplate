using BuildingBlocks.Application.Commands.CreateUserAccount;
using BuildingBlocks.Shared.Dtos.UserAccount;

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Portal.HttpApi.Controllers;

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
    [AllowAnonymous]
    //[ApiValidationFilter]
    public async Task<ActionResult> CreateUserAccount(
        [FromBody] CreateUserAccountDto createUserAccountDto)
    {
        _logger.LogInformation("START: CreateUserAccount");

        var loginResponse = await _mediator.Send(
            new CreateUserAccountCommand(
                createUserAccountDto));

        _logger.LogInformation("END: CreateUserAccount");

        return Created();
    }

}
