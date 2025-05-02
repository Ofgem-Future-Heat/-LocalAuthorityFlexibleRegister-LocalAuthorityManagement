using MediatR;
using Ofgem.API.LAF.LocalAuthorityManagement.Application.Abstractions;

namespace Ofgem.API.LAF.LocalAuthorityManagement.Application.LocalAuthority.QueryHandlers;

public class GetAllHandler(ILocalAuthorityRepository localAuthorityRepository)
    : IRequestHandler<Queries.GetAll, ICollection<Ofgem.LAF.SharedLibrary.Models.LocalAuthority>>
{
    public async Task<ICollection<Ofgem.LAF.SharedLibrary.Models.LocalAuthority>> Handle(Queries.GetAll request, CancellationToken cancellationToken)
    {
        return await localAuthorityRepository.GetAll();
    }
}
