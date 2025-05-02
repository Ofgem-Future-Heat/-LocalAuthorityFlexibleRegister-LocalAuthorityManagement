using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Ofgem.API.LAF.LocalAuthorityManagement.Application.Abstractions;

namespace Ofgem.API.LAF.LocalAuthorityManagement.Application.StatementOfIntent.CommandHandlers.v2
{
    public class AddSoiLogHandler(IStatementOfIntentRepository statementOfIntentRepository)
        : IRequestHandler<Commands.v2.AddSoiLog, Result<string>>
    {
        public async Task<Result<string>> Handle(Commands.v2.AddSoiLog request, CancellationToken cancellationToken)
        {

            var soi = await statementOfIntentRepository.AddSoiLog(request.StatementOfIntentLog);

            return Result<string>.Success(soi);
        }
    }
}
