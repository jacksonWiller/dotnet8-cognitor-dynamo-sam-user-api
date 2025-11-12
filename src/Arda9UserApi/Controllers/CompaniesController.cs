using Arda9UserApi.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Arda9UserApi.Application.Companies.CreateCompany;
using Arda9UserApi.Application.Companies.DeleteCompany;
using Arda9UserApi.Application.Companies.GetAllCompanies;
using Arda9UserApi.Application.Companies.GetCompanyById;
using Arda9UserApi.Application.Companies.UpdateCompany;
using Arda9UserApi.Application.Companies.GetCompanyBySlug;

namespace Arda9UserApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
[Produces("application/json")]
public class CompaniesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CompaniesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Obtém uma lista de empresas com limite especificado
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(GetAllCompaniesQueryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Get([FromQuery] GetAllCompaniesQuery query)
    {
        var result = await _mediator.Send(query);
        return result.ToActionResult();
    }

    /// <summary>
    /// Cria uma nova empresa
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CreateCompanyResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] CreateCompanyCommand command)
    {
        var result = await _mediator.Send(command);
        return result.ToActionResult();
    }

    /// <summary>
    /// Obtém uma empresa específica pelo ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(GetCompanyByIdQueryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(Guid id)
    {
        var result = await _mediator.Send(new GetCompanyByIdQuery(id));
        return result.ToActionResult();
    }

    /// <summary>
    /// Obtém uma empresa específica pelo slug
    /// </summary>
    [HttpGet("slug/{slug}")]
    [ProducesResponseType(typeof(GetCompanyBySlugQueryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBySlug(string slug)
    {
        var result = await _mediator.Send(new GetCompanyBySlugQuery(slug));
        return result.ToActionResult();
    }

    /// <summary>
    /// Atualiza uma empresa existente
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(UpdateCompanyResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Put(Guid id, [FromBody] UpdateCompanyCommand command)
    {
        command.Id = id;
        var result = await _mediator.Send(command);
        return result.ToActionResult();
    }

    /// <summary>
    /// Remove uma empresa
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(DeleteCompanyResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _mediator.Send(new DeleteCompanyCommand(id));
        return result.ToActionResult();
    }
}
