using MediatR;
using Ofgem.API.LAF.LocalAuthorityManagement.Application.Abstractions;

namespace Ofgem.API.LAF.LocalAuthorityManagement.Application.StatementOfIntent.QueryHandlers;

public class GetCountAtStatusHandler(IStatementOfIntentRepository statementOfIntentRepository)
    : IRequestHandler<Queries.GetCountAtStatus, int>
{
    public async Task<int> Handle(Queries.GetCountAtStatus request, CancellationToken cancellationToken)
    {
        int SoiCount = await statementOfIntentRepository.GetCountAtStatus(request.Status);

        return SoiCount;
    }
}