using Ardalis.Result;
using MediatR;
using System;

namespace Arda9UserApi.Application.Auth.Login;

public class LoginCommand : IRequest<Result<LoginResponse>>
{
    public Guid TenantId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}