using MediatR;

namespace Ofgem.API.LAF.LocalAuthorityManagement.Application.LocalAuthority.Queries;

public class Get : IRequest<Result<Ofgem.LAF.SharedLibrary.Models.LocalAuthority>>
{
    public Guid LocalAuthorityId { get; init; }
}