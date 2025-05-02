using MediatR;

namespace Ofgem.API.LAF.LocalAuthorityManagement.Application.LocalAuthority.Queries;

public class GetByName : IRequest<Result<Ofgem.LAF.SharedLibrary.Models.LocalAuthority>>
{
    public string? Name { get; init; }
}