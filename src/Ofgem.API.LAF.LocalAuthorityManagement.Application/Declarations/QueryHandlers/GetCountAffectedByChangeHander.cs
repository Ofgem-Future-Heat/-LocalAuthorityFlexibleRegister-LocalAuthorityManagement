using MediatR;
using Ofgem.API.LAF.LocalAuthorityManagement.Application.Abstractions;
using Ofgem.API.LAF.LocalAuthorityManagement.Application.Declarations.Queries;

namespace Ofgem.API.LAF.LocalAuthorityManagement.Application.Declarations.QueryHandlers;

public class GetCountAffectedByChangeHandler(IDeclarationsRepository declarationsRepository)
    : IRequestHandler<GetCountAffectedByChange, Result<int>>
{
    public Task<Result<int>> Handle(GetCountAffectedByChange request, CancellationToken cancellationToken)
    {
        var count = declarationsRepository.GetCountAffectedByChange(request.OnsCode);

        return Task.FromResult(Result<int>.Success(count));
    }
}