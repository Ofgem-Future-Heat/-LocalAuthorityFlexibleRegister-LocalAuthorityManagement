using MediatR;
using Ofgem.API.LAF.LocalAuthorityManagement.Application.Abstractions;
using Ofgem.LAF.SharedLibrary.Models;

namespace Ofgem.API.LAF.LocalAuthorityManagement.Application.StatementOfIntent.QueryHandlers
{
    public class GetByStatementOfIntentIdHandler(IStatementOfIntentRepository statementOfIntentRepository) :
        IRequestHandler<Queries.GetByStatementOfIntentId, Result<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent>>
    {
        public async Task<Result<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent>> Handle(Queries.GetByStatementOfIntentId request, CancellationToken cancellationToken)
        {
            Ofgem.LAF.SharedLibrary.Models.StatementOfIntent? statementOfIntent = await statementOfIntentRepository.GetByStatementOfIntentId(request.StatementOfIntentId);

            if (statementOfIntent == null)
            {
                return Result<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent>.Failure("Statement of Intent not found.");
            }

            var abc = Result<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent>.Success(statementOfIntent);

            return abc;
        }
    }
}
