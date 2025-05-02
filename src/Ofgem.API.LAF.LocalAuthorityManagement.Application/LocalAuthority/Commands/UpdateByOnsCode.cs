using MediatR;

namespace Ofgem.API.LAF.LocalAuthorityManagement.Application.LocalAuthority.Commands;

public class UpdateByOnsCode : IRequest<Ofgem.LAF.SharedLibrary.Models.LocalAuthority?>
{
    public string? Name { get; init; }
    public string? OnsCode { get; init; }
    public string? Email { get; init; }
}