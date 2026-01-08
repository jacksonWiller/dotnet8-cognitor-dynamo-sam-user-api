using Ardalis.Result;
using MediatR;

namespace Arda9UserApi.Application.Auth.ResendCode;

public class ResendCodeCommand : IRequest<Result<ResendCodeResponse>>
{
    public Guid TenantId { get; set; }
    public string Email { get; set; } = string.Empty;
}