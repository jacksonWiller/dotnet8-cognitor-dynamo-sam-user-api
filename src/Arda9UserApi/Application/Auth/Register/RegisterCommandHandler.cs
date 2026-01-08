using Amazon.CognitoIdentityProvider.Model;
using Arda9UserApi.Application.Extensions;
using Arda9UserApi.Application.Services;
using Ardalis.Result;
using MediatR;

namespace Arda9UserApi.Application.Auth.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<RegisterResponse>>
{
    private readonly IAuthService authService;
    private readonly ILogger<RegisterCommandHandler> logger;

    public RegisterCommandHandler(IAuthService authService, ILogger<RegisterCommandHandler> logger)
    {
        this.authService = authService;
        this.logger = logger;
    }

    public async Task<Result<RegisterResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await authService.RegisterUserAsync(
                request.TenantId,
                request.Email,
                request.Password,
                request.Name,
                request.PhoneNumber
            );

            return Result.Success(new RegisterResponse
            {
                Success = true,
                UserSub = response.UserSub,
                UserConfirmed = response.UserConfirmed,
                Message = "Usuário registrado com sucesso. Verifique seu email para confirmação."
            });
        }
        catch (UsernameExistsException)
        {
            return ResultError.Error("Este email já está em uso neste tenant");
        }
        catch (InvalidPasswordException ex)
        {
            return ResultError.Error($"Senha inválida: {ex.Message}");
        }
        catch (InvalidParameterException ex)
        {
            return ResultError.Error($"Parâmetros inválidos: {ex.Message}");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error registering user {Email} in tenant {TenantId}", request.Email, request.TenantId);
            return ResultError.Error("Erro ao registrar usuário");
        }
    }
}