# Arda9UserApi - AWS Lambda com .NET 8, Cognito e DynamoDB

Este projeto é um exemplo de aplicação serverless utilizando **AWS Lambda**, **AWS Cognito**, **DynamoDB** e **.NET 8**, seguindo padrões de arquitetura como **CQRS** (Command Query Responsibility Segregation) e **Clean Architecture**.

## 📋 Visão Geral do Projeto

A **Arda9UserApi** é uma API RESTful completa para gerenciamento de usuários e autenticação, implementada como uma função AWS Lambda exposta através do **AWS HTTP API Gateway**. A aplicação utiliza:

- **AWS Cognito** para autenticação e gerenciamento de usuários (registro, login, tokens JWT)
- **DynamoDB** como banco de dados NoSQL para persistência de dados
- **MediatR** para implementação do padrão CQRS
- **FluentValidation** para validação de comandos
- **AutoMapper** para mapeamento de objetos
- **Ardalis.Result** para tratamento de resultados

## 🏗️ Arquitetura do Projeto

A aplicação segue os princípios de **Clean Architecture** com as seguintes camadas:

### Controllers (`/Controllers/`)
- Pontos de entrada da API
- Recebem requisições HTTP e delegam para o MediatR
- Exemplos: `AuthController`, `UsersController`

### Application (`/Application/`)
- Contém os handlers CQRS (Commands e Queries)
- **Auth/**: Comandos para autenticação (Login, Register, ForgotPassword, etc.)
- **Users/**: Comandos para gerenciamento de usuários (Create, Update, Delete)
- **DTOs**: Objetos de transferência de dados
- **Services**: Serviços de negócio (ex: `AuthService`)
- **Behaviors**: Pipeline behaviors (ex: `LoggingBehavior`)

### Domain (`/Domain/`)
- Entidades de negócio e value objects
- **Entities/**: Agregados de domínio (ex: UserAggregate)
- **ValueObjects/**: Objetos de valor (Address, Email, Phone, etc.)
- **Enums**: Enumerações de domínio

### Infrastructure (`/Infrastructure/`)
- Implementações técnicas
- **Repositories**: Acesso a dados (ex: `UserRepository`)
- **Mappings**: Configuração do AutoMapper

### Core (`/Core/`)
- Camada de kernel com entidades base e interfaces
- `BaseEntity`, `IEntity`, `IAggregateRoot`

## 🔧 Tecnologias Utilizadas

| Tecnologia | Versão | Propósito |
|------------|--------|-----------|
| .NET 8 | 8.0 | Framework principal |
| AWS Lambda | - | Runtime serverless |
| AWS Cognito | - | Autenticação e autorização |
| DynamoDB | - | Banco de dados NoSQL |
| MediatR | 13.1.0 | CQRS e eventos de domínio |
| FluentValidation | 11.9.0 | Validação de entradas |
| AutoMapper | 12.0.1 | Mapeamento de objetos |
| Ardalis.Result | 9.0.0 | Tratamento de resultados |
| AWS SAM | - | Template de deploy |

## 📦 Estrutura de Arquivos

```
dotnet8-cognitor-dynamo-sam/
├── src/
│   └── Arda9UserApi/
│       ├── Program.cs                 # Configuração da aplicação
│       ├── Arda9UserApi.csproj        # Arquivo de projeto .NET
│       ├── Controllers/               # Controllers da API
│       ├── Application/               # Camada de aplicação (CQRS)
│       ├── Domain/                    # Domínio de negócio
│       ├── Infrastructure/            # Infraestrutura (repositórios)
│       ├── Core/                      # Núcleo da aplicação
│       └── Configuration/             # Configurações
├── template.yaml                      # Template AWS SAM
├── samconfig.toml                     # Configuração do SAM CLI
└── README.md                          # Documentação
```

## 🚀 Como Rodar a Aplicação

### Pré-requisitos

Antes de começar, certifique-se de ter instalado:

1. **AWS SAM CLI** - [Documentação de instalação](https://docs.aws.amazon.com/serverless-application-model/latest/developerguide/serverless-sam-cli-install.html)
2. **.NET 8 SDK** - [Download do .NET 8](https://dotnet.microsoft.com/download/dotnet/8.0)
3. **Docker** - [Docker Desktop](https://www.docker.com/products/docker-desktop)
4. **AWS CLI** configurado com suas credenciais

### Configuração Inicial

1. **Configure as variáveis de ambiente do Cognito** no `appsettings.json` ou nas variáveis de ambiente da AWS:

```json
{
  "AwsCognito": {
    "UserPoolId": "us-east-1_xxxxxxxxx",
    "ClientId": "xxxxxxxxxxxxxxxxxxxxx",
    "ClientSecret": "your-client-secret",
    "Region": "us-east-1"
  }
}
```

2. **Crie o User Pool no AWS Cognito** com as seguintes configurações:
   - Fluxo de autenticação: Código de confirmação por email
   - Autenticação multifator (opcional)
   - Políticas de senha adequadas

### Desenvolvimento Local

Para testar localmente sem deploy:

```bash
# 1. Instale as dependências
dotnet restore

# 2. Execute em modo de desenvolvimento
dotnet run
```

A API estará disponível em `https://localhost:5001` (ou a porta configurada).

### Teste Local com SAM CLI

Para testar a função Lambda localmente:

```bash
# Build da aplicação
sam build

# Inicie a API localmente
sam local start-api --port 3000

# Teste uma função específica
sam local invoke NetCodeWebAPIServerless --event events/event.json
```

### Deploy para AWS

Para implantar a aplicação na AWS:

```bash
# 1. Build da aplicação
sam build

# 2. Deploy guiado (primeira vez)
sam deploy --guided

# Responda às perguntas:
# - Stack Name: Nome único da stack (ex: arda9-user-api)
# - AWS Region: Região AWS (ex: us-east-1)
# - Confirm changes before deploy: yes/no
# - Allow SAM CLI IAM role creation: yes
# - Save arguments to samconfig.toml: yes

# 3. Deploy futuro (sem perguntas)
sam deploy
```

### Verificação do Deploy

Após o deploy, você receberá uma URL similar a:

```
https://xxxxxxxxxx.execute-api.us-east-1.amazonaws.com/
```

Acesse o Swagger UI em:
```
https://xxxxxxxxxx.execute-api.us-east-1.amazonaws.com/swagger/
```

## 🧪 Testando a API

### Autenticação

1. **Registre um novo usuário**:
```bash
POST /api/auth/register
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "ComplexPassword123!",
  "firstName": "John",
  "lastName": "Doe",
  "phone": "+1234567890"
}
```

2. **Faça login**:
```bash
POST /api/auth/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "ComplexPassword123!"
}
```

3. **Use o token JWT em requisições protegidas**:
```bash
Authorization: Bearer <seu-token-jwt>
```

### Endpoints Disponíveis

#### Autenticação (`/api/auth`)
- `POST /register` - Registrar novo usuário
- `POST /login` - Fazer login
- `POST /logout` - Fazer logout
- `POST /refresh-token` - Renovar token
- `POST /confirm-email` - Confirmar email
- `POST /resend-code` - Reenviar código de confirmação
- `POST /forgot-password` - Esqueci minha senha
- `POST /reset-password` - Resetar senha
- `GET /get-user-info` - Obter informações do usuário
- `POST /change-password` - Alterar senha

#### Usuários (`/api/users`)
- `GET /{id}` - Obter usuário por ID
- `GET /email/{email}` - Obter usuário por email
- `GET /company/{companyId}` - Obter usuários por empresa
- `POST /` - Criar usuário
- `PUT /{id}` - Atualizar usuário
- `DELETE /{id}` - Deletar usuário

## 📊 Monitoramento e Logs

Para visualizar os logs da função Lambda:

```bash
# Acompanhar logs em tempo real
sam logs -n NetCodeWebAPIServerless --stack-name <seu-stack-name> --tail
```

Os logs são formatados em JSON e incluem:
- Timestamp
- Nível de log
- Mensagem
- Contexto da requisição

## Use the SAM CLI to build and test locally

Build your application with the `sam build` command.

```bash
testeDotnetDeploy$ sam build
```

The SAM CLI installs dependencies defined in `src/ServerlessAPI/ServerlessAPI.csproj`, creates a deployment package, and saves it in the `.aws-sam/build` folder.

Test a single function by invoking it directly with a test event. An event is a JSON document that represents the input that the function receives from the event source. Test events are included in the `events` folder in this project.

Run functions locally and invoke them with the `sam local invoke` command.

```bash
testeDotnetDeploy$ sam local invoke NetCodeWebAPIServerless --event events/event.json
```

The AWS SAM CLI can also emulate your application's API. Use the `sam local start-api` command to run the API locally on port 3000.

```bash
testeDotnetDeploy$ sam local start-api
testeDotnetDeploy$ curl http://localhost:3000/
```

## Add a resource to your application

The application template uses AWS SAM to define application resources. AWS SAM is an extension of AWS CloudFormation with a simpler syntax for configuring common serverless application resources, such as functions, triggers, and APIs. For resources that aren't included in the [AWS SAM specification](https://github.com/awslabs/serverless-application-model/blob/master/versions/2016-10-31.md), you can use the standard [AWS CloudFormation resource types](https://docs.aws.amazon.com/AWSCloudFormation/latest/UserGuide/aws-template-resource-type-ref.html).

Update `template.yaml` to add a dead-letter queue to your application. In the **Resources** section, add a resource named **MyQueue** with the type **AWS::SQS::Queue**. Then add a property to the **AWS::Serverless::Function** resource named **DeadLetterQueue** that targets the queue's Amazon Resource Name (ARN), and a policy that grants the function permission to access the queue.

```yaml
Resources:
  MyQueue:
    Type: AWS::SQS::Queue
  NetCodeWebAPIServerless:
    Type: AWS::Serverless::Function
    Properties:
      CodeUri: ./src/ServerlessAPI/
      Handler: ServerlessAPI::ServerlessAPI.Function::FunctionHandler
      Runtime: dotnet8
      MemorySize: 1042
      DeadLetterQueue:
        Type: SQS
        TargetArn: !GetAtt MyQueue.Arn
      Policies:
        - SQSSendMessagePolicy:
            QueueName: !GetAtt MyQueue.QueueName
```

The dead-letter queue is a location for Lambda to send events that could not be processed. It's only used if you invoke your function asynchronously, but it's useful here to show how you can modify your application's resources and function configuration.

Deploy the updated application.

```bash
testeDotnetDeploy$ sam build
testeDotnetDeploy$ sam deploy
```

Open the [**Applications**](https://console.aws.amazon.com/lambda/home#/applications) page of the Lambda console, and choose your application. When the deployment completes, view the application resources on the **Overview** tab to see the new resource. Then, choose the function to see the updated configuration that specifies the dead-letter queue.

## Fetch, tail, and filter Lambda function logs

To simplify troubleshooting, SAM CLI has a command called `sam logs`. `sam logs` lets you fetch logs generated by your deployed Lambda function from the command line. In addition to printing the logs on the terminal, this command has several nifty features to help you quickly find the bug.

`NOTE`: This command works for all AWS Lambda functions; not just the ones you deploy using SAM.

```bash
testeDotnetDeploy$ sam logs -n NetCodeWebAPIServerless --stack-name testeDotnetDeploy --tail
```

You can find more information and examples about filtering Lambda function logs in the [SAM CLI Documentation](https://docs.aws.amazon.com/serverless-application-model/latest/developerguide/serverless-sam-cli-logging.html).

## 🛠️ Como o Código Foi Implementado

### 1. Configuração Inicial (`Program.cs`)

O ponto de entrada da aplicação configura todos os serviços necessários:

```csharp
// Configuração do AWS Cognito para JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = $"https://cognito-idp.{region}.amazonaws.com/{userPoolId}";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = $"https://cognito-idp.{region}.amazonaws.com/{userPoolId}",
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

// Configuração do MediatR para CQRS
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
});

// Configuração do AutoMapper
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

// Configuração do FluentValidation
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
```

### 2. Padrão CQRS com MediatR

A aplicação utiliza o padrão **CQRS** (Command Query Responsibility Segregation) através da biblioteca **MediatR**:

#### Exemplo de Command (Registro de Usuário):

```csharp
// Command
public record RegisterCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string Phone) : IRequest<RegisterResponse>;

// Handler
public class RegisterCommandHandler : IRequestHandler<RegisterCommand, RegisterResponse>
{
    private readonly AuthService _authService;
    
    public async Task<RegisterResponse> Handle(RegisterCommand command, CancellationToken cancellationToken)
    {
        var response = await _authService.RegisterAsync(command);
        return response;
    }
}
```

#### Exemplo de Query (Obter Usuário):

```csharp
// Query
public record GetUserByIdQuery(string UserId) : IRequest<UserDto?>;

// Handler
public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto?>
{
    private readonly IUserRepository _userRepository;
    
    public async Task<UserDto?> Handle(GetUserByIdQuery query, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(query.UserId);
        return user != null ? _mapper.Map<UserDto>(user) : null;
    }
}
```

### 3. Autenticação com AWS Cognito

O `AuthService` gerena toda a lógica de autenticação:

```csharp
public class AuthService
{
    private readonly AmazonCognitoIdentityProviderClient _cognitoClient;
    private readonly AwsCognitoConfig _config;
    
    public async Task<RegisterResponse> RegisterAsync(RegisterCommand command)
    {
        var request = new AdminCreateUserRequest
        {
            UserPoolId = _config.UserPoolId,
            Username = command.Email,
            TemporaryPassword = command.Password,
            UserAttributes = new List<UserAttribute>
            {
                new UserAttribute { Name = "email", Value = command.Email },
                new UserAttribute { Name = "name", Value = command.FirstName },
                new UserAttribute { Name = "phone_number", Value = command.Phone }
            }
        };
        
        var response = await _cognitoClient.AdminCreateUserAsync(request);
        return new RegisterResponse(response.User.Username);
    }
}
```

### 4. Persistência com DynamoDB

O `UserRepository` implementa o padrão Repository para acesso ao DynamoDB:

```csharp
public class UserRepository : IUserRepository
{
    private readonly DynamoDBContext _dynamoContext;
    
    public async Task<User?> GetByIdAsync(string id)
    {
        return await _dynamoContext.LoadAsync<User>(id);
    }
    
    public async Task<User?> GetByEmailAsync(string email)
    {
        var users = await _dynamoContext.QueryAsync<User>(
            $"arda-user-v1#{email}",
            QueryCondition.FromExpression("PK = :pk")
                .WithValue(":pk", $"USER#{email}")
        );
        return users.FirstOrDefault();
    }
    
    public async Task<User> CreateAsync(User user)
    {
        await _dynamoContext.SaveAsync(user);
        return user;
    }
}
```

### 5. Tratamento de Resultados com Ardalis.Result

A aplicação utiliza o padrão **Result** para tratamento de erros:

```csharp
public async Task<IActionResult> RegisterAsync([FromBody] RegisterCommand command)
{
    var result = await mediator.Send(command);
    
    if (result.IsSuccess)
        return Ok(result.Value);
    
    return BadRequest(new { 
        Success = false, 
        Message = result.Errors.FirstOrDefault() 
    });
}
```

### 6. Validação com FluentValidation

Cada command possui seu próprio validator:

```csharp
public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email é obrigatório")
            .EmailAddress().WithMessage("Email inválido");
        
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Senha é obrigatória")
            .MinimumLength(8).WithMessage("Senha deve ter pelo menos 8 caracteres")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)").WithMessage(
                "Senha deve conter pelo menos uma letra maiúscula, uma minúscula e um número");
        
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Nome é obrigatório");
        
        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Sobrenome é obrigatório");
    }
}
```

### 7. Pipeline Behavior para Logging

Um behavior personalizado adiciona logging automático:

```csharp
public class LoggingBehavior<TRequest, TResponse> 
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
    
    public async Task<TResponse> Handle(
        TRequest request, 
        RequestHandlerDelegate<TResponse> next, 
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Processing command: {Command}", 
            request.GetType().Name);
        
        var stopwatch = Stopwatch.StartNew();
        var response = await next();
        stopwatch.Stop();
        
        _logger.LogInformation(
            "Command {Command} completed in {ElapsedMilliseconds}ms", 
            request.GetType().Name, 
            stopwatch.ElapsedMilliseconds);
        
        return response;
    }
}
```

### 8. Mapeamento com AutoMapper

Configuração de mapeamento entre entidades e DTOs:

```csharp
public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.Address, opt => opt.MapFrom(
                src => src.Address != null ? new AddressDto 
                {
                    Street = src.Address.Street,
                    City = src.Address.City,
                    State = src.Address.State,
                    ZipCode = src.Address.ZipCode
                } : null))
            .ForMember(dest =>Phones, opt => opt.MapFrom(
                src => src.Phones?.Select(p => new PhoneDto { Type = p.Type, Number = p.Number }).ToList()));
        
        CreateMap<UserDto, User>()
            .ForMember(dest => dest.Address, opt => opt.MapFrom(
                src => src.Address != null ? new Address 
                {
                    Street = src.Address.Street,
                    City = src.Address.City,
                    State = src.Address.State,
                    ZipCode = src.Address.ZipCode
                } : null));
    }
}
```

### 9. Template AWS SAM

O arquivo `template.yaml` define a infraestrutura:

```yaml
Resources:
  NetCodeWebAPIServerless:
    Type: AWS::Serverless::Function
    Properties:
      CodeUri: ./src/Arda9UserApi/
      Handler: Arda9UserApi
      Runtime: dotnet8
      MemorySize: 1024
      Environment:
        Variables:
          SAMPLE_TABLE: !Ref SampleTable
      Policies:
        - DynamoDBCrudPolicy:
            TableName: !Ref SampleTable
      Events:
        ProxyResource:
          Type: HttpApi
          Properties:
            PayloadFormatVersion: "2.0"
            Path: /{proxy+}
            Method: ANY

  SampleTable:
    Type: AWS::Serverless::SimpleTable
    Properties:
      TableName: arda-user-v1
      PrimaryKey:
        Name: Id
        Type: String
      ProvisionedThroughput:
        ReadCapacityUnits: 2
        WriteCapacityUnits: 2
```

### 10. Value Objects e Entidades de Domínio

A aplicação segue princípios de **Domain-Driven Design (DDD)**:

```csharp
// Value Object
public class Email
{
    public string Value { get; }
    
    private Email(string value) 
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email é obrigatório");
        
        if (!IsValidEmail(value))
            throw new ArgumentException("Email inválido");
        
        Value = value.ToLower();
    }
    
    public static Email Create(string value) => new Email(value);
    
    private static bool IsValidEmail(string email)
    {
        try {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        } catch {
            return false;
        }
    }
}

// Entity
public class User : BaseEntity, IAggregateRoot
{
    public string Email { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public Address? Address { get; private set; }
    public List<Phone> Phones { get; private set; }
    public UserStatus Status { get; private set; }
    
    // Business methods
    public void UpdateInfo(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }
    
    public void Deactivate()
    {
        Status = UserStatus.Inactive;
    }
}
```

## 🎯 Padrões de Projeto Utilizados

1. **CQRS** (Command Query Responsibility Segregation) - Separação de comandos e consultas
2. **Clean Architecture** - Separação por camadas de responsabilidade
3. **Dependency Injection** - Injeção de dependências para testabilidade
4. **Repository Pattern** - Abstração do acesso a dados
5. **Domain-Driven Design (DDD)** - Entidades, value objects e agregados
6. **Result Pattern** - Tratamento de erros imutável
7. **Validator Pattern** - Validação centralizada com FluentValidation
8. **Mediator Pattern** - Coordenação de operações com MediatR

## 📈 Melhores Práticas Implementadas

- ✅ **Validação de entrada** com FluentValidation
- ✅ **Tratamento de erros** com Result pattern
- ✅ **Logging estruturado** com JSON
- ✅ **Mapeamento de objetos** com AutoMapper
- ✅ **Pipeline behaviors** para cross-cutting concerns
- ✅ **Injeção de dependências** para testabilidade
- ✅ **Princípios SOLID** em toda a arquitetura
- ✅ **Value Objects** para consistência de domínio
- ✅ **Eventos de domínio** para desacoplamento
- ✅ **API Gateway** para exposição segura
- ✅ **JWT tokens** para autenticação stateless

## 🧹 Cleanup

Para remover todos os recursos criados:

```bash
# Delete a stack completa
sam delete --stack-name <seu-stack-name>

# Ou manualmente pelo Console AWS:
# 1. Acesse CloudFormation
# 2. Selecione a stack criada
# 3. Clique em "Delete"
```

**Importante**: Lembre-se de:
- Deletar os usuários do Cognito User Pool
- Deletar a tabela DynamoDB se não for mais necessária
- Remover as políticas IAM criadas

## 📚 Recursos Adicionais

### Documentação Oficial

- [AWS SAM Developer Guide](https://docs.aws.amazon.com/serverless-application-model/latest/developerguide/what-is-sam.html)
- [AWS Cognito Documentation](https://docs.aws.amazon.com/cognito/latest/developerguide/what-is-amazon-cognito.html)
- [DynamoDB Documentation](https://docs.aws.amazon.com/dynamodb/)
- [.NET 8 Documentation](https://learn.microsoft.com/dotnet/csharp/)
- [MediatR Documentation](https://github.com/jbogard/MediatR)
- [FluentValidation Documentation](https://docs.fluentvalidation.net/en/latest/)

### Projetos Relacionados

- [AWS Serverless Application Repository](https://aws.amazon.com/serverless/serverlessrepo/)
- [AWS Lambda Examples](https://github.com/aws-samples/aws-lambda-dotnet)
- [Clean Architecture Template](https://github.com/jasontaylordev/CleanArchitecture)

## 🔍 Troubleshooting

### Problemas Comuns

#### 1. Erro de Autenticação JWT

**Sintoma**: Erro 401 Unauthorized

**Solução**:
- Verifique se o User Pool ID está correto nas configurações
- Certifique-se de que o token JWT não expirou
- Verifique se o domínio do Cognito está habilitado para CORS

#### 2. Erro de Permissão DynamoDB

**Sintoma**: Erro 500 ao acessar dados

**Solução**:
- Verifique as políticas IAM da função Lambda
- Certifique-se de que a tabela DynamoDB existe
- Verifique as permissões de leitura/escrita

#### 3. Erro de Build SAM

**Sintoma**: Falha no `sam build`

**Solução**:
- Verifique se o Docker está rodando
- Certifique-se de que o .NET 8 SDK está instalado
- Limpe o cache: `rm -rf .aws-sam`

#### 4. Erro de Conexão Local

**Sintoma**: Falha ao rodar `sam local start-api`

**Solução**:
- Verifique se as variáveis de ambiente estão configuradas
- Certifique-se de que o Docker tem recursos suficientes
- Verifique a porta não está em uso

## 📝 Changelog

### Versão 1.0.0 (2026-04-20)

- ✅ Implementação inicial da API
- ✅ Autenticação com AWS Cognito
- ✅ CRUD de usuários com DynamoDB
- ✅ Padrão CQRS com MediatR
- ✅ Validação com FluentValidation
- ✅ Documentação completa
- ✅ Deploy com AWS SAM
