using MediatR;
using Ofgem.API.LAF.LocalAuthorityManagement.Application.Abstractions;

namespace Ofgem.API.LAF.LocalAuthorityManagement.Application.StatementOfIntent.CommandHandlers;

public class UpdateByIdHandler(IStatementOfIntentRepository statementOfIntentRepository)
    : IRequestHandler<Commands.UpdateById, Result<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent>>
{
    public async Task<Result<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent>> Handle(Commands.UpdateById request, CancellationToken cancellationToken)
    {

        var soi = await statementOfIntentRepository.UpdateById(request.StatementOfIntent);

        return Result<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent>.Success(soi);
    }
}