using MediatR;
using Ofgem.API.LAF.LocalAuthorityManagement.Application.Abstractions;
using Ofgem.API.LAF.LocalAuthorityManagement.Application.LocalAuthority.Queries;
using Ofgem.LAF.SharedLibrary.Models;

namespace Ofgem.API.LAF.LocalAuthorityManagement.Application.LocalAuthority.QueryHandlers;

public class GetFilteredHandler(ILocalAuthorityRepository localAuthorityRepository)
    : IRequestHandler<GetFiltered, PagedResult<Ofgem.LAF.SharedLibrary.Models.LocalAuthority>>
{
    public async Task<PagedResult<Ofgem.LAF.SharedLibrary.Models.LocalAuthority>> Handle(GetFiltered request, CancellationToken cancellationToken)
    {
        var outList = new PagedResult<Ofgem.LAF.SharedLibrary.Models.LocalAuthority>();

        if (request.Filter is null) return outList;

        var pagedResult = await localAuthorityRepository.GetFilteredLocalAuthorities(request.Filter);

        if (pagedResult is null) return outList;

        outList.CurrentPage = pagedResult.CurrentPage;

        outList.PageCount = pagedResult.PageCount;
        outList.PageSize = pagedResult.PageSize;
        outList.RowCount = pagedResult.RowCount;

        foreach (var item in pagedResult.Results)
        {
            outList.Results.Add(item.Pared());
        }

        return outList;
    }
}