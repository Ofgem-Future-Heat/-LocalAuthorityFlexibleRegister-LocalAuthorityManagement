using MediatR;
using Ofgem.LAF.SharedLibrary.Models;

namespace Ofgem.API.LAF.LocalAuthorityManagement.Application.StatementOfIntent.Queries
{
    public class GetFilteredSoiLog : IRequest<PagedResult<StatementOfIntentLog>>
    {
        public UploadFilter? Filter { get; set; }
    }
}
