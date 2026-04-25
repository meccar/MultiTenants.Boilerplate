using AutoMapper;
using BuildingBlocks.Core.Abstractions;
using BuildingBlocks.Shared.Dtos;
using BuildingBlocks.Shared.Utilities;
using Identity.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Identity.Application.Queries.GetCurrentUser;

public class GetUserByIdQueryHandler 
    : IRequestHandler<GetUserByIdQuery, Result<UserDto?>>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ITenantProvider _tenantProvider;
    private readonly IMapper _mapper;
    
    public GetUserByIdQueryHandler(
        UserManager<AppUser> userManager,
        ITenantProvider tenantProvider,
        IMapper mapper
    ){
        _userManager = userManager;
        _tenantProvider = tenantProvider;
        _mapper = mapper;
    }

    public async Task<Result<UserDto?>> Handle(
        GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        if (_tenantProvider.GetCurrentTenantId() == null)
            return Result<UserDto?>.Failure("Tenant context not found");

        var userDto = await _userManager.FindByIdAsync(request.UserId);
        return Result<UserDto?>.Success(_mapper.Map<UserDto>(userDto));
    }
}
