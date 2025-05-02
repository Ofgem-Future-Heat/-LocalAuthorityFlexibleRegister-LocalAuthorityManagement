using MediatR;

namespace Ofgem.API.LAF.LocalAuthorityManagement.Application.LocalAuthority.Commands;

public class Update : IRequest<Ofgem.LAF.SharedLibrary.Models.LocalAuthority?>
{
    public Guid LocalAuthorityId { get; init; }
    public string? Name { get; init; }
    public string? OnsCode { get; set; }
    public string? Email { get; set; }
}