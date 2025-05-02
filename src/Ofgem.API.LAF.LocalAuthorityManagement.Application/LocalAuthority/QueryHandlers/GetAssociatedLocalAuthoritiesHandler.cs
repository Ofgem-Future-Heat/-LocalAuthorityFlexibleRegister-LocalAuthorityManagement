using MediatR;
using Ofgem.API.LAF.LocalAuthorityManagement.Application.Abstractions;

namespace Ofgem.API.LAF.LocalAuthorityManagement.Application.LocalAuthority.QueryHandlers;

public class GetAssociatedLocalAuthoritiesHandler(ILocalAuthorityRepository localAuthorityRepository)
    : IRequestHandler<Queries.GetAssociatedLocalAuthorities, ICollection<Ofgem.LAF.SharedLibrary.Models.LocalAuthority>>
{
    public async Task<ICollection<Ofgem.LAF.SharedLibrary.Models.LocalAuthority>> Handle(Queries.GetAssociatedLocalAuthorities request, CancellationToken cancellationToken)
    {
        return request.OnsCode is null
            ? []
            : await localAuthorityRepository.GetAssociatedLocalAuthorities(request.OnsCode);
    }
}
