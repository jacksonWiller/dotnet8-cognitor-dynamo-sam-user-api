using Amazon.CognitoIdentityProvider.Model;
using Arda9UserApi.Application.Extensions;
using Arda9UserApi.Application.Services;
using Ardalis.Result;
using MediatR;

namespace Arda9UserApi.Application.Auth.ConfirmEmail;

public class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand, Result<ConfirmEmailResponse>>
{
    private readonly IAuthService authService;
    private readonly ILogger<ConfirmEmailCommandHandler> logger;

    public ConfirmEmailCommandHandler(IAuthService authService, ILogger<ConfirmEmailCommandHandler> logger)
    {
        this.authService = authService;
        this.logger = logger;
    }

    public async Task<Result<ConfirmEmailResponse>> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await authService.ConfirmSignUpAsync(request.TenantId, request.Email, request.Code);

            return Result.Success(new ConfirmEmailResponse
            {
                Success = true,
                Message = "Email confirmado com sucesso"
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
        catch (UserNotFoundException)
        {
            return ResultError.Error("Usuário não encontrado neste tenant");
        }
        catch (NotAuthorizedException)
        {
            return ResultError.Error("Não autorizado a confirmar este usuário");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error confirming email for user {Email} in tenant {TenantId}", request.Email, request.TenantId);
            return ResultError.Error("Erro ao confirmar email");
        }
    }
}