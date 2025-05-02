using MediatR;

namespace Ofgem.API.LAF.LocalAuthorityManagement.Application.Declarations.Queries;

public class GetCountAffectedByChange : IRequest<Result<int>>
{
    public string OnsCode { get; init; } = string.Empty;
}