using Ofgem.API.LAF.LocalAuthorityManagement.Application.Abstractions;
using MediatR;

namespace Ofgem.API.LAF.LocalAuthorityManagement.Application.StatementOfIntent.CommandHandlers.v2
{
    public class EligibilityRoute2Proxy5Handler(IStatementOfIntentRepository statementOfIntentRepository)
        : IRequestHandler<Commands.v2.EligibilityRoute2Proxy5, Result<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent?>>
    {
        public async Task<Result<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent?>> Handle(Commands.v2.EligibilityRoute2Proxy5 request, CancellationToken cancellationToken)
        {

            var soi = await statementOfIntentRepository.EligibilityRoute2Proxy5(
                request.StatementOfIntentId,
                request.IsProxy5SchemePresent, 
                request.IsDescriptionNiceNg6Recommendation2);

            return Result<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent?>.Success(soi);
        }
    }
}
