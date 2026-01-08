using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Arda9UserApi.Application.Helpers;
using Arda9UserApi.Configuration;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace Arda9UserApi.Application.Services;

public interface IAuthService
{
    string ComputeSecretHash(string username);
    Task<SignUpResponse> RegisterUserAsync(Guid tenantId, string email, string password, string name, string? phoneNumber);
    Task ConfirmSignUpAsync(Guid tenantId, string email, string code);
    Task ResendConfirmationCodeAsync(Guid tenantId, string email);
    Task<InitiateAuthResponse> LoginAsync(Guid tenantId, string email, string password);
    Task<InitiateAuthResponse> RefreshTokenAsync(Guid tenantId, string email, string refreshToken);
    Task SendForgotPasswordCodeAsync(Guid tenantId, string email);
    Task ConfirmForgotPasswordAsync(Guid tenantId, string email, string code, string newPassword);
    Task ChangePasswordAsync(string accessToken, string oldPassword, string newPassword);
    Task GlobalSignOutAsync(string accessToken);
    Task<GetUserResponse> GetUserAsync(string accessToken);
}

public class AuthService : IAuthService
{
    private readonly IAmazonCognitoIdentityProvider cognitoClient;
    private readonly AwsCognitoConfig cognitoConfig;
    private readonly ILogger<AuthService> logger;

    public AuthService(
        IAmazonCognitoIdentityProvider cognitoClient,
        IOptions<AwsCognitoConfig> cognitoConfig,
        ILogger<AuthService> logger)
    {
        this.cognitoClient = cognitoClient;
        this.cognitoConfig = cognitoConfig.Value;
        this.logger = logger;
    }

    public string ComputeSecretHash(string username)
    {
        var message = username + cognitoConfig.ClientId;
        var key = Encoding.UTF8.GetBytes(cognitoConfig.ClientSecret);
        var messageBytes = Encoding.UTF8.GetBytes(message);

        using var hmac = new HMACSHA256(key);
        var hash = hmac.ComputeHash(messageBytes);
        return Convert.ToBase64String(hash);
    }

    public async Task<SignUpResponse> RegisterUserAsync(Guid tenantId, string email, string password, string name, string? phoneNumber)
    {
        var username = CognitoUsername.Build(tenantId, email);
        var secretHash = ComputeSecretHash(username);
        var normalizedEmail = email.Trim().ToLowerInvariant();

        var signUpRequest = new SignUpRequest
        {
            ClientId = cognitoConfig.ClientId,
            SecretHash = secretHash,
            Username = username,
            Password = password,
            UserAttributes = new List<AttributeType>
            {
                new() { Name = "email", Value = normalizedEmail },
                new() { Name = "name", Value = name },
                new() { Name = "custom:tenantId", Value = tenantId.ToString() }
            }
        };

        if (!string.IsNullOrEmpty(phoneNumber))
        {
            signUpRequest.UserAttributes.Add(new AttributeType
            {
                Name = "phone_number",
                Value = phoneNumber
            });
        }

        var response = await cognitoClient.SignUpAsync(signUpRequest);
        logger.LogInformation("User {Username} registered successfully in tenant {TenantId} with UserSub {UserSub}", 
            username, tenantId, response.UserSub);
        
        return response;
    }

    public async Task ConfirmSignUpAsync(Guid tenantId, string email, string code)
    {
        var username = CognitoUsername.Build(tenantId, email);
        var secretHash = ComputeSecretHash(username);

        var confirmRequest = new ConfirmSignUpRequest
        {
            ClientId = cognitoConfig.ClientId,
            SecretHash = secretHash,
            Username = username,
            ConfirmationCode = code
        };

        await cognitoClient.ConfirmSignUpAsync(confirmRequest);
        logger.LogInformation("User {Username} confirmed successfully", username);
    }

    public async Task ResendConfirmationCodeAsync(Guid tenantId, string email)
    {
        var username = CognitoUsername.Build(tenantId, email);
        var secretHash = ComputeSecretHash(username);

        var resendRequest = new ResendConfirmationCodeRequest
        {
            ClientId = cognitoConfig.ClientId,
            SecretHash = secretHash,
            Username = username
        };

        await cognitoClient.ResendConfirmationCodeAsync(resendRequest);
        logger.LogInformation("Confirmation code resent to {Username}", username);
    }

    public async Task<InitiateAuthResponse> LoginAsync(Guid tenantId, string email, string password)
    {
        var username = CognitoUsername.Build(tenantId, email);
        var secretHash = ComputeSecretHash(username);

        var authRequest = new InitiateAuthRequest
        {
            ClientId = cognitoConfig.ClientId,
            AuthFlow = AuthFlowType.USER_PASSWORD_AUTH,
            AuthParameters = new Dictionary<string, string>
            {
                ["USERNAME"] = username,
                ["PASSWORD"] = password,
                ["SECRET_HASH"] = secretHash
            }
        };

        var response = await cognitoClient.InitiateAuthAsync(authRequest);
        logger.LogInformation("User {Username} logged in successfully", username);
        
        return response;
    }

    public async Task<InitiateAuthResponse> RefreshTokenAsync(Guid tenantId, string email, string refreshToken)
    {
        var username = CognitoUsername.Build(tenantId, email);
        var secretHash = ComputeSecretHash(username);

        var refreshRequest = new InitiateAuthRequest
        {
            ClientId = cognitoConfig.ClientId,
            AuthFlow = AuthFlowType.REFRESH_TOKEN_AUTH,
            AuthParameters = new Dictionary<string, string>
            {
                ["REFRESH_TOKEN"] = refreshToken,
                ["USERNAME"] = username,
                ["SECRET_HASH"] = secretHash
            }
        };

        var response = await cognitoClient.InitiateAuthAsync(refreshRequest);
        logger.LogInformation("Token refreshed successfully for {Username}", username);
        
        return response;
    }

    public async Task SendForgotPasswordCodeAsync(Guid tenantId, string email)
    {
        var username = CognitoUsername.Build(tenantId, email);
        var secretHash = ComputeSecretHash(username);

        var forgotRequest = new ForgotPasswordRequest
        {
            ClientId = cognitoConfig.ClientId,
            SecretHash = secretHash,
            Username = username
        };

        await cognitoClient.ForgotPasswordAsync(forgotRequest);
        logger.LogInformation("Password reset code sent to {Username}", username);
    }

    public async Task ConfirmForgotPasswordAsync(Guid tenantId, string email, string code, string newPassword)
    {
        var username = CognitoUsername.Build(tenantId, email);
        var secretHash = ComputeSecretHash(username);

        var confirmRequest = new ConfirmForgotPasswordRequest
        {
            ClientId = cognitoConfig.ClientId,
            SecretHash = secretHash,
            Username = username,
            ConfirmationCode = code,
            Password = newPassword
        };

        await cognitoClient.ConfirmForgotPasswordAsync(confirmRequest);
        logger.LogInformation("Password reset successfully for {Username}", username);
    }

    public async Task ChangePasswordAsync(string accessToken, string oldPassword, string newPassword)
    {
        var changeRequest = new ChangePasswordRequest
        {
            AccessToken = accessToken,
            PreviousPassword = oldPassword,
            ProposedPassword = newPassword
        };

        await cognitoClient.ChangePasswordAsync(changeRequest);
        logger.LogInformation("Password changed successfully");
    }

    public async Task GlobalSignOutAsync(string accessToken)
    {
        var signOutRequest = new GlobalSignOutRequest
        {
            AccessToken = accessToken
        };

        await cognitoClient.GlobalSignOutAsync(signOutRequest);
        logger.LogInformation("User logged out successfully");
    }

    public async Task<GetUserResponse> GetUserAsync(string accessToken)
    {
        var getUserRequest = new GetUserRequest
        {
            AccessToken = accessToken
        };

        return await cognitoClient.GetUserAsync(getUserRequest);
    }
}