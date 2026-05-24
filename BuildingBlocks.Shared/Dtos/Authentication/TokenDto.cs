namespace BuildingBlocks.Shared.Dtos.Authentication;
public record TokenDto(
    string AccessToken,
    string RefreshToken
);