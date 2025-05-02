using MediatR;
using Ofgem.API.LAF.LocalAuthorityManagement.Application.Abstractions;
using Ofgem.API.LAF.LocalAuthorityManagement.Application.LocalAuthority.Commands;

namespace Ofgem.API.LAF.LocalAuthorityManagement.Application.LocalAuthority.CommandHandlers;

public class DeleteHandler(ILocalAuthorityRepository localAuthorityRepository) : IRequestHandler<Delete, Result<Unit>>
{
    public async Task<Result<Unit>> Handle(Delete request, CancellationToken cancellationToken)
    {
        await localAuthorityRepository.Delete(request.LocalAuthorityId);
        return Result<Unit>.Success(Unit.Value);
    }
}