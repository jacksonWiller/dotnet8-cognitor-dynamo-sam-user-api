using Amazon.CognitoIdentityProvider.Model;
using Arda9UserApi.Application.Extensions;
using Arda9UserApi.Application.Services;
using Ardalis.Result;
using MediatR;

namespace Arda9UserApi.Application.Auth.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<RefreshTokenResponse>>
{
    private readonly IAuthService authService;
    private readonly ILogger<RefreshTokenCommandHandler> logger;

    public RefreshTokenCommandHandler(IAuthService authService, ILogger<RefreshTokenCommandHandler> logger)
    {
        this.authService = authService;
        this.logger = logger;
    }

    public async Task<Result<RefreshTokenResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await authService.RefreshTokenAsync(request.TenantId, request.Email, request.RefreshToken);

            return Result.Success(new RefreshTokenResponse
            {
                Success = true,
                AccessToken = response.AuthenticationResult.AccessToken,
                IdToken = response.AuthenticationResult.IdToken,
                ExpiresIn = response.AuthenticationResult.ExpiresIn,
                TokenType = response.AuthenticationResult.TokenType
            });
        }
        catch (NotAuthorizedException)
        {
            return ResultError.Error("Refresh token inválido ou expirado");
        }
        catch (UserNotFoundException)
        {
            return ResultError.Error("Usuário não encontrado neste tenant");
        }
        catch (InvalidParameterException ex)
        {
            return ResultError.Error($"Parâmetros inválidos: {ex.Message}");
        }
        catch (TooManyRequestsException)
        {
            return ResultError.Error("Muitas tentativas. Tente novamente mais tarde");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error refreshing token for user {Email} in tenant {TenantId}", request.Email, request.TenantId);
            return ResultError.Error("Erro ao renovar token");
        }
    }
}