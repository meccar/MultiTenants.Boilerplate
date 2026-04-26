using BuildingBlocks.Shared.Helpers;
using Identity.Domain.Entities;
using Identity.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Queries.GetUserPermissions;

public class GetUserPermissionsQueryHandler
    : IRequestHandler<GetUserPermissionsQuery, AppUser>
{
    private readonly ILogger<GetUserPermissionsQueryHandler> _logger;
    private readonly JwtToken _jwtToken;
    private readonly UserManager<AppUser>  _userManager;
    private readonly IPermissionsRepository  _permissionsRepository;
    
    public GetUserPermissionsQueryHandler(
        ILogger<GetUserPermissionsQueryHandler> logger,
        JwtToken jwtToken,
        UserManager<AppUser> userManager,
        IPermissionsRepository permissionsRepository
    ) {
        _logger = logger;
        _jwtToken = jwtToken;
        _userManager = userManager;
        _permissionsRepository = permissionsRepository;
    }
    public Task<AppUser> Handle(
        GetUserPermissionsQuery request,
        CancellationToken cancellationToken
    ){
        var validateTokenResult = _jwtToken.ValidateToken(request.Token);
        if(!validateTokenResult.IsValid)
            throw new UnauthorizedAccessException();

        var user = _userManager.FindByEmailAsync(validateTokenResult.Username!);
        if(user == null)
            throw new UnauthorizedAccessException();

        return user;
    }
}