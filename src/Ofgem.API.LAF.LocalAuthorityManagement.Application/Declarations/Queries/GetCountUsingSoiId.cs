using MediatR;

namespace Ofgem.API.LAF.LocalAuthorityManagement.Application.Declarations.Queries;

public class GetCountUsingSoiId : IRequest<Result<int>>
{
    public Guid StatementOfIntentId { get; init; }
}