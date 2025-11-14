using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Arda9UserApi.Application.DTOs;
using AutoMapper;
using Catalog.Domain.Entities.UserAggregate;

namespace Arda9UserApi.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IDynamoDBContext context;
    private readonly ILogger<UserRepository> logger;
    private readonly IMapper mapper;

    public UserRepository(IDynamoDBContext context, ILogger<UserRepository> logger, IMapper mapper)
    {
        this.context = context;
        this.logger = logger;
        this.mapper = mapper;
    }

    public async Task<bool> CreateAsync(User user)
    {
        try
        {
            var userDto = mapper.Map<UserDto>(user);
            userDto.PK = $"COMPANY#{user.CompanyId}";
            userDto.SK = $"USER#{user.Id}";
            userDto.GSI1PK = $"EMAIL#{user.Email.Value.ToLowerInvariant()}";
            userDto.GSI1SK = $"USER#{user.Id}";
            userDto.CreatedAt = DateTime.UtcNow;
            userDto.UpdatedAt = DateTime.UtcNow;

            await context.SaveAsync(userDto);
            logger.LogInformation("User {UserId} for company {CompanyId} is added", user.Id, user.CompanyId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to persist user to DynamoDB Table");
            return false;
        }

        return true;
    }

    public async Task<bool> DeleteAsync(User user)
    {
        bool result;
        try
        {
            var pk = $"COMPANY#{user.CompanyId}";
            var sk = $"USER#{user.Id}";

            await context.DeleteAsync<UserDto>(pk, sk);
            result = true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to delete user from DynamoDB Table");
            result = false;
        }

        if (result) logger.LogInformation("User {UserId} is deleted", user.Id);

        return result;
    }

    public async Task<bool> UpdateAsync(User user)
    {
        if (user == null) return false;

        try
        {
            var userDto = mapper.Map<UserDto>(user);
            userDto.PK = $"COMPANY#{user.CompanyId}";
            userDto.SK = $"USER#{user.Id}";
            userDto.GSI1PK = $"EMAIL#{user.Email.Value.ToLowerInvariant()}";
            userDto.GSI1SK = $"USER#{user.Id}";
            userDto.UpdatedAt = DateTime.UtcNow;

            await context.SaveAsync(userDto);
            logger.LogInformation("User {UserId} for company {CompanyId} is updated", user.Id, user.CompanyId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to update user from DynamoDB Table");
            return false;
        }

        return true;
    }

    public async Task<User?> GetByIdAsync(Guid companyId, Guid userId)
    {
        try
        {
            var pk = $"COMPANY#{companyId}";
            var sk = $"USER#{userId}";

            var userDto = await context.LoadAsync<UserDto>(pk, sk);

            if (userDto == null) return null;

            return mapper.Map<User>(userDto);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get user from DynamoDB Table");
            return null;
        }
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        try
        {
            var queryConfig = new QueryOperationConfig
            {
                IndexName = "EmailIndex",
                KeyExpression = new Expression
                {
                    ExpressionStatement = "GSI1PK = :email",
                    ExpressionAttributeValues = new Dictionary<string, DynamoDBEntry>
                    {
                        { ":email", $"EMAIL#{email.ToLowerInvariant()}" }
                    }
                },
                Limit = 1
            };

            var search = context.FromQueryAsync<UserDto>(queryConfig);
            var users = await search.GetNextSetAsync();
            var userDto = users.FirstOrDefault();

            if (userDto == null) return null;

            return mapper.Map<User>(userDto);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get user by email from DynamoDB Table");
            return null;
        }
    }

    public async Task<User?> GetByCognitoSubAsync(string cognitoSub)
    {
        try
        {
            var filter = new ScanFilter();
            filter.AddCondition("CognitoSub", ScanOperator.Equal, cognitoSub);
            filter.AddCondition("SK", ScanOperator.BeginsWith, "USER#");

            var scanConfig = new ScanOperationConfig
            {
                Limit = 1,
                Filter = filter
            };

            var queryResult = context.FromScanAsync<UserDto>(scanConfig);
            var users = await queryResult.GetNextSetAsync();
            var userDto = users.FirstOrDefault();

            if (userDto == null) return null;

            return mapper.Map<User>(userDto);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get user by CognitoSub from DynamoDB Table");
            return null;
        }
    }

    public async Task<IList<User>> GetUsersByCompanyAsync(Guid companyId, int limit = 10)
    {
        var result = new List<User>();

        try
        {
            if (limit <= 0) return result;

            var queryConfig = new QueryOperationConfig
            {
                KeyExpression = new Expression
                {
                    ExpressionStatement = "PK = :pk AND begins_with(SK, :sk)",
                    ExpressionAttributeValues = new Dictionary<string, DynamoDBEntry>
                    {
                        { ":pk", $"COMPANY#{companyId}" },
                        { ":sk", "USER#" }
                    }
                },
                Limit = limit
            };

            var search = context.FromQueryAsync<UserDto>(queryConfig);

            do
            {
                var dtos = await search.GetNextSetAsync();
                var users = mapper.Map<List<User>>(dtos);
                result.AddRange(users);
            }
            while (!search.IsDone && result.Count < limit);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to list users from DynamoDB Table");
            return new List<User>();
        }

        return result;
    }
}