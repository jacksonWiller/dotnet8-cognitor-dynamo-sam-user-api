using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.SecretsManager;
using Arda9UserApi.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace ServerlessAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAmazonCognitoIdentityProvider cognitoClient;
    private readonly AwsCognitoConfig cognitoConfig;

    public AuthController(
        IAmazonCognitoIdentityProvider cognitoClient,
        IAmazonSecretsManager secretsManager,
        IOptions<AwsCognitoConfig> cognitoConfig
        )
    {
        this.cognitoClient = cognitoClient;
        this.cognitoConfig = cognitoConfig.Value;
    }

    /// <summary>
    /// Realiza login do usu�rio no AWS Cognito
    /// </summary>
    /// <returns>Token de autentica��o</returns>
    /// <response code="200">Login realizado com sucesso</response>
    /// <response code="400">Par�metros inv�lidos</response>
    /// <response code="401">Credenciais inv�lidas</response>
    [HttpGet]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> LoginAsync()
    {
        try
        {
            var userPoolId = cognitoConfig.UserPoolId;
            var clientId = cognitoConfig.ClientId;
            var clientSecret = cognitoConfig.ClientSecret;

            if (string.IsNullOrEmpty(userPoolId) || string.IsNullOrEmpty(clientId))
            {
                throw new InvalidOperationException("Configura��es do Cognito n�o encontradas");
            }

            var email = "jacksonwillerduarte@gmail.com";
            var senha = "Abc@1234566";

            // Use os par�metros recebidos
            var secretHash = ComputeSecretHash(email, clientId, clientSecret);

            // Para usu�rios n�o-admin, use InitiateAuthRequest
            var authRequest = new InitiateAuthRequest
            {
                ClientId = clientId,
                AuthFlow = AuthFlowType.USER_PASSWORD_AUTH,
                AuthParameters = new Dictionary<string, string>
                {
                    ["USERNAME"] = email,
                    ["PASSWORD"] = senha,
                    ["SECRET_HASH"] = secretHash
                }
            };

            var authResponse = await cognitoClient.InitiateAuthAsync(authRequest);

            return Ok(new
            {
                Success = true,
                AccessToken = authResponse.AuthenticationResult.AccessToken,
                IdToken = authResponse.AuthenticationResult.IdToken,
                RefreshToken = authResponse.AuthenticationResult.RefreshToken,
                ExpiresIn = authResponse.AuthenticationResult.ExpiresIn,
                TokenType = authResponse.AuthenticationResult.TokenType
            });
        }
        catch (NotAuthorizedException)
        {
            return Unauthorized(new { Success = false, Message = "Email ou senha incorretos" });
        }
        catch (UserNotConfirmedException)
        {
            return BadRequest(new { Success = false, Message = "Usu�rio n�o confirmado. Verifique seu email." });
        }
        catch (InvalidParameterException ex)
        {
            return BadRequest(new { Success = false, Message = "Par�metros inv�lidos", Error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Success = false, Message = "Erro interno do servidor", Error = ex.Message });
        }
    }

    private static string ComputeSecretHash(string username, string clientId, string clientSecret)
    {
        var message = username + clientId;
        var key = Encoding.UTF8.GetBytes(clientSecret);
        var messageBytes = Encoding.UTF8.GetBytes(message);

        using var hmac = new HMACSHA256(key);
        var hash = hmac.ComputeHash(messageBytes);
        return Convert.ToBase64String(hash);
    }
}