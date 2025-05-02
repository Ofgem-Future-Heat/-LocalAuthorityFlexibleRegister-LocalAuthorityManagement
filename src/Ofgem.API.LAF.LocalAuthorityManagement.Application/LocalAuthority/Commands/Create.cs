using MediatR;

namespace Ofgem.API.LAF.LocalAuthorityManagement.Application.LocalAuthority.Commands;

public class Create : IRequest<Ofgem.LAF.SharedLibrary.Models.LocalAuthority>
{
    public Guid LocalAuthorityId { get; set; }
    public string? Name { get; init; }
    public string? OnsCode { get; init; }
    public string? Email { get; init; }
}