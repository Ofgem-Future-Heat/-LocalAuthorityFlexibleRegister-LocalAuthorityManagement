using MediatR;
using Ofgem.API.LAF.LocalAuthorityManagement.Application.Abstractions;
using Ofgem.LAF.SharedLibrary.Models;

namespace Ofgem.API.LAF.LocalAuthorityManagement.Application.StatementOfIntent.QueryHandlers
{
    public class GetAllHandler(IStatementOfIntentRepository statementOfIntentRepository, ILocalAuthorityRepository localAuthorityRepository) : IRequestHandler<Queries.GetFiltered, PagedResult<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent>?>
    {
        public async Task<PagedResult<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent>?> Handle(Queries.GetFiltered request, CancellationToken cancellationToken)
        {

            // we need localAuthorityId not ons code when filtering by local authority. So we build that collection here.
            List<Guid> laIds = new List<Guid>();
            foreach (string onsCode in request.LAs)
            {
                Ofgem.LAF.SharedLibrary.Models.LocalAuthority la = await localAuthorityRepository.GetByOnsCode(onsCode);
                laIds.Add(la.LocalAuthorityId);
            }
            PagedResult<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent>? statementOfIntentList = await statementOfIntentRepository.GetFiltered(request, laIds);

            return statementOfIntentList;
        }
    }
}
