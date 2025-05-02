using MediatR;
using Ofgem.API.LAF.LocalAuthorityManagement.Application.Abstractions;

namespace Ofgem.API.LAF.LocalAuthorityManagement.Application.StatementOfIntent.CommandHandlers
{
    public class UpdateRoutesHandler(IStatementOfIntentRepository statementOfIntentRepository)
        : IRequestHandler<Commands.UpdateRoutes, Result<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent>>
    {
        public async Task<Result<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent>> Handle(Commands.UpdateRoutes request, CancellationToken cancellationToken)
        {
            var statementOfIntent = await statementOfIntentRepository.UpdateRoutes(
              request);
            if (statementOfIntent is null)
                return Result<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent>.Failure("Failed to update routes");
            return Result<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent>.Success(statementOfIntent);
        }
    }
}
