using MediatR;
using Ofgem.API.LAF.LocalAuthorityManagement.Application.Abstractions;
using Ofgem.API.LAF.LocalAuthorityManagement.Application.Declarations.Queries;

namespace Ofgem.API.LAF.LocalAuthorityManagement.Application.Declarations.QueryHandlers;

public class GetCountUsingSoiIdHandler(IDeclarationsRepository declarationsRepository)
    : IRequestHandler<GetCountUsingSoiId, Result<int>>
{
    public async Task<Result<int>> Handle(GetCountUsingSoiId request, CancellationToken cancellationToken)
    {
        int count = await declarationsRepository.GetCountUsingSoiId(request.StatementOfIntentId);

        return Result<int>.Success(count);
    }
}