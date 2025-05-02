using MediatR;
using Ofgem.API.LAF.LocalAuthorityManagement.Application.Abstractions;

namespace Ofgem.API.LAF.LocalAuthorityManagement.Application.LocalAuthority.QueryHandlers;

public class GetHandler(ILocalAuthorityRepository localAuthorityRepository)
    : IRequestHandler<Queries.Get, Result<Ofgem.LAF.SharedLibrary.Models.LocalAuthority>>
{
    public async Task<Result<Ofgem.LAF.SharedLibrary.Models.LocalAuthority>> Handle(Queries.Get request, CancellationToken cancellationToken)
    {
        Ofgem.LAF.SharedLibrary.Models.LocalAuthority? localAuthority = await localAuthorityRepository.Get(request.LocalAuthorityId);

        return localAuthority is null 
            ? Result<Ofgem.LAF.SharedLibrary.Models.LocalAuthority>.Failure("Did not find this local authority.") 
            : Result<Ofgem.LAF.SharedLibrary.Models.LocalAuthority>.Success(localAuthority);
    }
}