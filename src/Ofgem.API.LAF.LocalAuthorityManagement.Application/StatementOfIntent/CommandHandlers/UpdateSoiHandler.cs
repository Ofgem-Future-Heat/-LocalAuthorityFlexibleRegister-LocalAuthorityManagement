using MediatR;
using Ofgem.API.LAF.LocalAuthorityManagement.Application.Abstractions;

namespace Ofgem.API.LAF.LocalAuthorityManagement.Application.StatementOfIntent.CommandHandlers;

public class UpdateSoiHandler(IMediator mediator, IStatementOfIntentRepository statementOfIntentRepository)
    : IRequestHandler<Commands.UpdateSoi, Result<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent>>
{
    public async Task<Result<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent>> Handle(Commands.UpdateSoi request, CancellationToken cancellationToken)
    {
        var getQuery = new Queries.Get
        {
            StatementOfIntentId = request.StatementOfIntentId
        };

        var existingSoi = await mediator.Send(getQuery, cancellationToken);

        existingSoi.Category = request.Category;
        existingSoi.Status = request.Status;
        existingSoi.StatementOfIntentLink = request.StatementOfIntentLink;
        existingSoi.PublishedDate = request.PublishedDate;
        existingSoi.CanSubmit = request.CanSubmit;
        existingSoi.VersionNumber = request.VersionNumber;

        existingSoi.DesignatedLas = [];

        if (request.DesignatedLas is { Count: > 0 })
        {
            existingSoi.DesignatedLas = [];

            foreach (var desLaRequested in request.DesignatedLas ?? [])
            {
                LocalAuthority.Queries.Get getLaQuery = new()
                {
                    LocalAuthorityId = desLaRequested
                };

                var localAuthorityResult = await mediator.Send(getLaQuery, cancellationToken);

                if (localAuthorityResult.IsSuccess)
                {
                    var desLa =
                        new Ofgem.LAF.SharedLibrary.Models.DesignatedLA()
                        {
                            StatementOfIntent = existingSoi,
                            LocalAuthority = localAuthorityResult.Value
                        };

                    existingSoi.DesignatedLas.Add(desLa);
                }
                else
                {
                    return Result<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent>.Failure($"Local Authority {desLaRequested} was not found.");
                }
            }
        }

        var soi = await statementOfIntentRepository.UpdateSoi(existingSoi);

        return Result<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent>.Success(soi);
    }
}