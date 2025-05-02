using MediatR;
using Ofgem.API.LAF.LocalAuthorityManagement.Application.Abstractions;

namespace Ofgem.API.LAF.LocalAuthorityManagement.Application.StatementOfIntent.CommandHandlers.v2;

public class SoiStatusSettingHandler(IStatementOfIntentRepository statementOfIntentRepository)
    : IRequestHandler<Commands.v2.SoiStatusSetting, Result<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent?>>
{
    public async Task<Result<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent?>> Handle(Commands.v2.SoiStatusSetting request, CancellationToken cancellationToken)
    {
        var soi = await statementOfIntentRepository.SoiStatusSetting(request.StatementOfIntent);
        return Result<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent?>.Success(soi);
    }
}

