using BuildingBlocks.Application.Commands.CreateUserAccount;
using BuildingBlocks.Attributes;
using BuildingBlocks.Shared.Dtos.UserAccount;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BuildingBlocks.Controllers;

[ApiController]
//[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize(Policy = "InvoiceManagers")]
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
    [RequirePermission("invoices:write")]
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
