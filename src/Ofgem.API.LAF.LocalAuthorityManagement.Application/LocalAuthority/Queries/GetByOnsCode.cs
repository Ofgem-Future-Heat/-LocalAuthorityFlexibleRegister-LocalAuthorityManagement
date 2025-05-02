using MediatR;

namespace Ofgem.API.LAF.LocalAuthorityManagement.Application.LocalAuthority.Queries;

public class GetByOnsCode : IRequest<Result<Ofgem.LAF.SharedLibrary.Models.LocalAuthority>>
{
    public string? OnsCode { get; init; }
}