using MediatR;
using Ofgem.API.LAF.LocalAuthorityManagement.Application.Abstractions;

namespace Ofgem.API.LAF.LocalAuthorityManagement.Application.LocalAuthority.QueryHandlers;

public class GetByNameHandler(ILocalAuthorityRepository localAuthorityRepository)
    : IRequestHandler<Queries.GetByName, Result<Ofgem.LAF.SharedLibrary.Models.LocalAuthority>>
{
    public async Task<Result<Ofgem.LAF.SharedLibrary.Models.LocalAuthority>> Handle(Queries.GetByName request, CancellationToken cancellationToken)
    {
        if (request.Name is null)
        {
            return Result<Ofgem.LAF.SharedLibrary.Models.LocalAuthority>.Failure("Local Authority not found.");
        }

        var localAuthority = await localAuthorityRepository.GetByName(request.Name);

        return localAuthority is null 
            ? Result<Ofgem.LAF.SharedLibrary.Models.LocalAuthority>.Failure("Local Authority not found.") 
            : Result<Ofgem.LAF.SharedLibrary.Models.LocalAuthority>.Success(localAuthority);
    }
}