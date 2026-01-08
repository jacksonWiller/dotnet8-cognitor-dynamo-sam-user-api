using Amazon.CognitoIdentityProvider.Model;
using Arda9UserApi.Application.Extensions;
using Arda9UserApi.Application.Services;
using Ardalis.Result;
using MediatR;

namespace Arda9UserApi.Application.Auth.ResetPassword;

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result<ResetPasswordResponse>>
{
    private readonly IAuthService authService;
    private readonly ILogger<ResetPasswordCommandHandler> logger;

    public ResetPasswordCommandHandler(IAuthService authService, ILogger<ResetPasswordCommandHandler> logger)
    {
        this.authService = authService;
        this.logger = logger;
    }

    public async Task<Result<ResetPasswordResponse>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await authService.ConfirmForgotPasswordAsync(request.TenantId, request.Email, request.Code, request.NewPassword);

            return Result.Success(new ResetPasswordResponse
            {
                Success = true,
                Message = "Senha redefinida com sucesso"
            });
        }
        catch (CodeMismatchException)
        {
            return ResultError.Error("Código de confirmação inválido");
        }
        catch (ExpiredCodeException)
        {
            return ResultError.Error("Código de confirmação expirado");
        }
        catch (InvalidPasswordException ex)
        {
            return ResultError.Error($"Senha inválida: {ex.Message}");
        }
        catch (UserNotFoundException)
        {
            return ResultError.Error("Usuário não encontrado neste tenant");
        }
        catch (InvalidParameterException ex)
        {
            return ResultError.Error($"Parâmetros inválidos: {ex.Message}");
        }
        catch (LimitExceededException)
        {
            return ResultError.Error("Limite de tentativas excedido. Tente novamente mais tarde");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error resetting password for {Email} in tenant {TenantId}", request.Email, request.TenantId);
            return ResultError.Error("Erro ao redefinir senha");
        }
    }
}