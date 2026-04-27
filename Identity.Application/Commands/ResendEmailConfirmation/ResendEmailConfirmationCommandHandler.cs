using BuildingBlocks.Shared.Utilities;
using Identity.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Commands.ResendEmailConfirmation;

public class ResendEmailConfirmationCommandHandler : IRequestHandler<ResendEmailConfirmationCommand, Result>
{
    // private readonly IEmailSender _emailSender;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ResendEmailConfirmationCommandHandler> _logger;
    private readonly UserManager<UsersEntity> _userManager;
    private readonly SignInManager<UsersEntity> _signInManager;

    public ResendEmailConfirmationCommandHandler(
        // IEmailSender emailSender,
        IConfiguration configuration,
        ILogger<ResendEmailConfirmationCommandHandler> logger,
        UserManager<UsersEntity> userManager,
        SignInManager<UsersEntity> signInManager
    ){
        // _emailSender = emailSender;
        _configuration = configuration;
        _logger = logger;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<Result> Handle(ResendEmailConfirmationCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
            throw new UnauthorizedAccessException();
        
        var tokenResult = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        
        // token format: "userId|confirmationToken"

        // var baseUrl = _configuration["App:BaseUrl"]?.TrimEnd('/') ?? "https://localhost";
        // var callbackUrl = $"{baseUrl}/confirm-email?userId={Uri.EscapeDataString(userId)}&code={Uri.EscapeDataString(code)}";

        // await _emailSender.SendEmailAsync(
        //    request.Email,
        //    "Confirm your email",
        //    $"Please confirm your account by visiting: {callbackUrl}",
        //    cancellationToken);

        _logger.LogInformation("Email confirmation resent to {Email}.", request.Email);
        return Result.Success();
    }
}
