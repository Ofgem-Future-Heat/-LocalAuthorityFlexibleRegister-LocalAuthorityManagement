using MediatR;
using Ofgem.API.LAF.LocalAuthorityManagement.Application.Abstractions;

namespace Ofgem.API.LAF.LocalAuthorityManagement.Application.StatementOfIntent.CommandHandlers.v2;

public class InitialAssessmentChecklistHandler(IStatementOfIntentRepository statementOfIntentRepository)
    : IRequestHandler<Commands.v2.InitialAssessmentChecklist, Result<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent?>>
{
    public async Task<Result<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent?>> Handle(Commands.v2.InitialAssessmentChecklist request, CancellationToken cancellationToken)
    {

        var soi = await statementOfIntentRepository.InitialAssessmentChecklist(request.StatementOfIntent);

        return Result<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent?>.Success(soi);
    }
}