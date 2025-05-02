using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Ofgem.API.LAF.LocalAuthorityManagement.Application.Abstractions;

namespace Ofgem.API.LAF.LocalAuthorityManagement.Application.StatementOfIntent.QueryHandlers
{
    public class GetSoiByOnsHandler(IStatementOfIntentRepository statementOfIntentRepository) :
    IRequestHandler<Queries.GetSoiByOns, Result<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent?>>
    {
        public async Task<Result<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent?>> Handle(Queries.GetSoiByOns request, CancellationToken cancellationToken)
        {
            Ofgem.LAF.SharedLibrary.Models.StatementOfIntent? statementOfIntent =
                await statementOfIntentRepository.GetSoiByOnsCode(request.OnsCode, request.PublishedDate);

            if (statementOfIntent == null)
            {
                return Result<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent?>.Failure("Statement of Intent not found.");
            }

            return Result<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent?>.Success(statementOfIntent);
        }
    }
}
