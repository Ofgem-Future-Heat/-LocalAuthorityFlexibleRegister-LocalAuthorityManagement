using MediatR;
using Ofgem.API.LAF.LocalAuthorityManagement.Application.Abstractions;
using Ofgem.API.LAF.LocalAuthorityManagement.Application.StatementOfIntent.Commands;
using Ofgem.API.LAF.LocalAuthorityManagement.Application.StatementOfIntent.Queries;
using Ofgem.LAF.SharedLibrary.Models;

namespace Ofgem.API.LAF.LocalAuthorityManagement.Application.StatementOfIntent.CommandHandlers;

public class CreateHandler(IMediator mediator, IStatementOfIntentRepository statementOfIntentRepository)
    : IRequestHandler<Create, Result<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent>>
{
    public async Task<Result<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent>> Handle(Create request, CancellationToken cancellationToken)
    {
        if (request.StatementOfIntentLink is null)
            return Result<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent>.Failure("Failed to create the SOI");

        var soi = new Ofgem.LAF.SharedLibrary.Models.StatementOfIntent()
        {
            LocalAuthorityId = request.LocalAuthorityId,
            OnsCode = request.OnsCode,
            VersionNumber = request.VersionNumber,
            CanSubmit = request.CanSubmit,
            Category = request.Category,
            PublishedDate = request.PublishedDate,
            Status = request.Status,
            StatementOfIntentLink = request.StatementOfIntentLink
        };

        if (request.DesignatedLas is { Count: > 0 })
        {
            soi.DesignatedLas = [];

            foreach (var desLaRequested in request.DesignatedLas)
            {
                var getLaQuery = new LocalAuthority.Queries.Get()
                {
                    LocalAuthorityId = desLaRequested
                };

                var localAuthorityResult = await mediator.Send(getLaQuery, cancellationToken);

                if (localAuthorityResult.IsSuccess)
                {
                    var desLa = new DesignatedLA
                    {
                        StatementOfIntent = soi,
                        LocalAuthority = localAuthorityResult.Value
                    };

                    soi.DesignatedLas.Add(desLa);
                }
                else
                {
                    return Result<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent>.Failure($"Local Authority {desLaRequested} does not exist.");
                }
            }
        }

        if (await VersionWithThisPublishedDateAlreadyExistsAsync(request.OnsCode, request.PublishedDate))
        {
            return Result<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent>.Failure("SOI with this date already exists.");
        }

        var soiOut = await statementOfIntentRepository.Create(soi);

        return soiOut is null
            ? Result<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent>.Failure("Failed to create the SOI")
            : Result<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent>.Success(soiOut);

    }

    private async Task<bool> VersionWithThisPublishedDateAlreadyExistsAsync(string onsCode, DateTime publishedDate)
    {
        GetByKeyValues request = new(onsCode, publishedDate);

        var result = await mediator.Send(request);

        return result.IsSuccess;
    }
}