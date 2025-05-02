using MediatR;

namespace Ofgem.API.LAF.LocalAuthorityManagement.Application.LocalAuthority.Commands;

public class Delete : IRequest<Result<Unit>>
{
    public Guid LocalAuthorityId { get; init; }
}