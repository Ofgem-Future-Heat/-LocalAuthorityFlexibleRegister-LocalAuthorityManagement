using MediatR;
using Ofgem.LAF.SharedLibrary.Enums;
using Ofgem.LAF.SharedLibrary.Models;

namespace Ofgem.API.LAF.LocalAuthorityManagement.Application.StatementOfIntent.Queries
{
    public class GetFiltered : IRequest<PagedResult<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent>?>
    {
        public string BaseLocalAuthority { get; set; }
        public string[] LAs { get; set; } = Array.Empty<string>();
        public SoiStatusV2[] Status { get; set; } = Array.Empty<SoiStatusV2>();
        public int RecordsPerPage { get; set; } = 50;
        public int PageIndex { get; set; } = 0;
    }
}
