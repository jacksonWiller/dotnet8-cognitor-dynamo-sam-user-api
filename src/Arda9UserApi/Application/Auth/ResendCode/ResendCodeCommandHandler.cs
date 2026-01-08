using Amazon.CognitoIdentityProvider.Model;
using Arda9UserApi.Application.Extensions;
using Arda9UserApi.Application.Services;
using Ardalis.Result;
using MediatR;

namespace Arda9UserApi.Application.Auth.ResendCode;

public class ResendCodeCommandHandler : IRequestHandler<ResendCodeCommand, Result<ResendCodeResponse>>
{
    private readonly IAuthService authService;
    private readonly ILogger<ResendCodeCommandHandler> logger;

    public ResendCodeCommandHandler(IAuthService authService, ILogger<ResendCodeCommandHandler> logger)
    {
        this.authService = authService;
        this.logger = logger;
    }

    public async Task<Result<ResendCodeResponse>> Handle(ResendCodeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await authService.ResendConfirmationCodeAsync(request.TenantId, request.Email);

            return Result.Success(new ResendCodeResponse
            {
                Success = true,
                Message = "Código de confirmação reenviado"
            });
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
        catch (CodeDeliveryFailureException)
        {
            return ResultError.Error("Falha ao enviar código. Verifique seu email");
        }
        catch (TooManyRequestsException)
        {
            return ResultError.Error("Muitas tentativas. Tente novamente mais tarde");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error resending confirmation code to {Email} in tenant {TenantId}", request.Email, request.TenantId);
            return ResultError.Error("Erro ao reenviar código");
        }
    }
}