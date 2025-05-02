using MediatR;
using Ofgem.API.LAF.LocalAuthorityManagement.Application.Abstractions;
using Ofgem.API.LAF.LocalAuthorityManagement.Application.Declarations.Queries;

namespace Ofgem.API.LAF.LocalAuthorityManagement.Application.LocalAuthority.QueryHandlers;

public class GetByOnsCodeHandler(IMediator mediator, ILocalAuthorityRepository localAuthorityRepository)
    : IRequestHandler<Queries.GetByOnsCode, Result<Ofgem.LAF.SharedLibrary.Models.LocalAuthority>>
{
    public async Task<Result<Ofgem.LAF.SharedLibrary.Models.LocalAuthority>> Handle(Queries.GetByOnsCode request, CancellationToken cancellationToken)
    {
        Ofgem.LAF.SharedLibrary.Models.LocalAuthority? localAuthority = await localAuthorityRepository.GetByOnsCode(request.OnsCode!);

        if (localAuthority == null)
        {
            return Result<Ofgem.LAF.SharedLibrary.Models.LocalAuthority>.Failure("Local Authority not found.");
        }

        // augment the soi with the HasDeclarations information
        foreach (var soi in localAuthority.StatementOfIntents!)
        {
            GetCountUsingSoiId countRequest = new()
            {
                StatementOfIntentId = soi.StatementOfIntentId,
            };

            var countResult = mediator.Send(countRequest, cancellationToken);

            soi.HasDeclarations = countResult.Result.Value > 0;

            soi.LocalAuthority = null; // remove the nesting as we only require the 1st level
        }

        return Result<Ofgem.LAF.SharedLibrary.Models.LocalAuthority>.Success(localAuthority);
    }
}