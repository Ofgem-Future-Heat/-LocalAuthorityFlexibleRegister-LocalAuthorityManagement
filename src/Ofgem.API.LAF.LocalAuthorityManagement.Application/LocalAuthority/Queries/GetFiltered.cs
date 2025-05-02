using MediatR;
using Ofgem.LAF.SharedLibrary.Models;

namespace Ofgem.API.LAF.LocalAuthorityManagement.Application.LocalAuthority.Queries;

public class GetFiltered : IRequest<PagedResult<Ofgem.LAF.SharedLibrary.Models.LocalAuthority>>
{
    public ProfilesFilter? Filter { get; init; }
}

