using MediatR;
using Ofgem.API.LAF.LocalAuthorityManagement.Application.Abstractions;

namespace Ofgem.API.LAF.LocalAuthorityManagement.Application.StatementOfIntent.CommandHandlers
{
    public class UpdateSignOffChecklistHandler(IStatementOfIntentRepository statementOfIntentRepository)
        : IRequestHandler<Commands.UpdateSignOffChecklist, Result<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent>>
    {
        public async Task<Result<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent>> Handle(Commands.UpdateSignOffChecklist request, CancellationToken cancellationToken)
        {
            var statementOfIntent = await statementOfIntentRepository.UpdateSignOffChecklist(
              request);
            if(statementOfIntent is null) return Result<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent>.Failure("Failed to find an soi to update.");

            return Result<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent>.Success(statementOfIntent);
        }
    }
}
