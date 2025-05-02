using MediatR;
using Ofgem.API.LAF.LocalAuthorityManagement.Application.Abstractions;

namespace Ofgem.API.LAF.LocalAuthorityManagement.Application.StatementOfIntent.CommandHandlers
{
    public class UpdateAssessmentChecklistHandler(IStatementOfIntentRepository statementOfIntentRepository)
        : IRequestHandler<Commands.UpdateAssessmentChecklist, Result<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent>>
    {
        public async Task<Result<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent>> Handle(Commands.UpdateAssessmentChecklist request, CancellationToken cancellationToken)
        {
            var statementOfIntent = await statementOfIntentRepository.UpdateAssessmentChecklist(request);

            return statementOfIntent is null 
                ? Result<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent>.Failure("Failed to update the checklist.") 
                : Result<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent>.Success(statementOfIntent);
        }
    }
}
