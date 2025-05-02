using MediatR;
using Ofgem.API.LAF.LocalAuthorityManagement.Application.Abstractions;
using Ofgem.API.LAF.LocalAuthorityManagement.Application.StatementOfIntent.Commands.v2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ofgem.API.LAF.LocalAuthorityManagement.Application.StatementOfIntent.CommandHandlers.v2
{
    public class SchemeDetailsHandler(IStatementOfIntentRepository statementOfIntentRepository)
        : IRequestHandler<Commands.v2.SchemeDetails, Result<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent?>>
    {
        public async Task<Result<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent?>> Handle(SchemeDetails request, CancellationToken cancellationToken)
        {
            var soi = await statementOfIntentRepository.SchemeDetails(
                request.StatementOfIntentId,
                request.ForScheme,
                request.IsPublishedDateCorrect,
                request.IsProxy5PartOfRoute2);

            return Result<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent?>.Success(soi);
        }
    }
}
