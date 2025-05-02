using MediatR;
using Ofgem.API.LAF.LocalAuthorityManagement.Application.Abstractions;
using Ofgem.API.LAF.LocalAuthorityManagement.Application.StatementOfIntent.Queries;
using Ofgem.LAF.SharedLibrary.Models;

namespace Ofgem.API.LAF.LocalAuthorityManagement.Application.StatementOfIntent.QueryHandlers;

public class GetFilteredSoiLogHandler(IStatementOfIntentRepository statementOfIntentRepository) : IRequestHandler<GetFilteredSoiLog, PagedResult<StatementOfIntentLog>>
{

    public async Task<PagedResult<StatementOfIntentLog>> Handle(GetFilteredSoiLog request, CancellationToken cancellationToken)
    {

        var pagedResult = await statementOfIntentRepository.GetFilteredSoiLogs(request.Filter!);

        return pagedResult;
    }
}