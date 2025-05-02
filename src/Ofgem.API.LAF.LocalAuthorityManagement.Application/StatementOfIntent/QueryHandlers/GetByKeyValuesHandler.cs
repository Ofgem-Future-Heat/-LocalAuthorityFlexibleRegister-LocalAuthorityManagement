using MediatR;
using Ofgem.API.LAF.LocalAuthorityManagement.Application.Abstractions;

namespace Ofgem.API.LAF.LocalAuthorityManagement.Application.StatementOfIntent.QueryHandlers;

public class GetByKeyValuesHandler(IStatementOfIntentRepository statementOfIntentRepository)
    : IRequestHandler<Queries.GetByKeyValues, Result<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent>>
{
    public async Task<Result<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent>> Handle(Queries.GetByKeyValues request, CancellationToken cancellationToken)
    {
        Ofgem.LAF.SharedLibrary.Models.StatementOfIntent? statementOfIntent = await statementOfIntentRepository
            .GetByKeyValues(request.OnsCode, request.PublishedDate);

        if(statementOfIntent is null) { return Result<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent>
            .Failure($"SOI does not exist for {request.OnsCode},{request.PublishedDate}");}
        return Result<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent>.Success(statementOfIntent);
    }
}