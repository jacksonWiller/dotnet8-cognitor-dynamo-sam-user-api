using Ardalis.Result;
using MediatR;
using System;

namespace Arda9UserApi.Application.Auth.RefreshToken;

public class RefreshTokenCommand : IRequest<Result<RefreshTokenResponse>>
{
    public Guid TenantId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}