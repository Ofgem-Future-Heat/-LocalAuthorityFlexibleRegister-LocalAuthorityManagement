using MediatR;
using Ofgem.API.LAF.LocalAuthorityManagement.Application.Abstractions;

namespace Ofgem.API.LAF.LocalAuthorityManagement.Application.StatementOfIntent.QueryHandlers;

public class GetHandler(IStatementOfIntentRepository statementOfIntentRepository)
    : IRequestHandler<Queries.Get, Ofgem.LAF.SharedLibrary.Models.StatementOfIntent?>
{
    public async Task<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent?> Handle(Queries.Get request, CancellationToken cancellationToken)
    {
        Ofgem.LAF.SharedLibrary.Models.StatementOfIntent? statementOfIntent = await statementOfIntentRepository.Get(request.StatementOfIntentId);

        return statementOfIntent;
    }
}