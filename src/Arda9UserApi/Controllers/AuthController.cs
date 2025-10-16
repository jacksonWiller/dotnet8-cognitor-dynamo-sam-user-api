using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Runtime.Internal;
using Arda9UserApi.Entities;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace ServerlessAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAmazonCognitoIdentityProvider cognitoClient;

    public AuthController(
        IAmazonCognitoIdentityProvider cognitoClient
        )
    {
        this.cognitoClient = cognitoClient;
    }


    //"Region": "us-east-1",
    //"UserPoolId": "us-east-1_sWDh0AKxR",
    //"ClientId": "3q9eboqr11p66tt34mb9724jrh",
    //"ClientSecret": "1srh7dgqnilo65jc0f8vhngp738fh7ng6rccv2pqhql43otdecs1",
    //"Domain": "https://cognito-idp.us-east-1.amazonaws.com/us-east-1_t1kEvBnO3",
    //"RedirectUri": "https://d84l1y8p4kdic.cloudfront.net"


    //    {
    //  "email": "emanuelly_nair_melo@gmail.com",
    //  "password": "IS1N9ZkfF1@",
    //  "firstName": "Emanuelly",
    //  "lastName": "Nair Mariana Melo",
    //  "phoneNumber": "+5538991860532"
    //}

    /// <summary>
    /// Obtém uma lista de livros com limite especificado
    /// </summary>
    /// <param name="limit">Número máximo de livros a retornar (1-100)</param>
    /// <returns>Lista de livros</returns>
    /// <response code="200">Retorna a lista de livros</response>
    /// <response code="400">Se o limite estiver fora do intervalo válido</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Book>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> LoginAsync()
    {
        try
        {
            var userPoolId = "us-east-1_tg7PHhZle";
            var clientId = "6gdd4f0r9k274c4ilann8k5jm7"; // Corrigido para usar o ClientId dos comentários
            var clientSecret = "9a131vl8cmfo5ovins06c3245fnjmcej81m72dpre5cvlva6477";

            if (string.IsNullOrEmpty(userPoolId) || string.IsNullOrEmpty(clientId))
            {
                throw new InvalidOperationException("Configurações do Cognito não encontradas");
            }

            var email = "jacksonwillerduarte@gmail.com";
            var senha = "Abc@1234566";

            // Use os parâmetros recebidos
            var secretHash = ComputeSecretHash(email, clientId, clientSecret);

            // Para usuários não-admin, use InitiateAuthRequest
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
            return BadRequest(new { Success = false, Message = "Usuário não confirmado. Verifique seu email." });
        }
        catch (InvalidParameterException ex)
        {
            return BadRequest(new { Success = false, Message = "Parâmetros inválidos", Error = ex.Message });
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