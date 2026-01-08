using Ardalis.Result;
using MediatR;
using System;

namespace Arda9UserApi.Application.Auth.ForgotPassword;

public class ForgotPasswordCommand : IRequest<Result<ForgotPasswordResponse>>
{
    public Guid TenantId { get; set; }
    public string Email { get; set; } = string.Empty;
}