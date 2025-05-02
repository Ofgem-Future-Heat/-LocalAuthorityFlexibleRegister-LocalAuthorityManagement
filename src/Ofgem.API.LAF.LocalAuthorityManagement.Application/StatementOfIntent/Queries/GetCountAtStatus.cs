using MediatR;

namespace Ofgem.API.LAF.LocalAuthorityManagement.Application.StatementOfIntent.Queries
{
    public class GetCountAtStatus : IRequest<int>
    {
        public int Status { get; set; }
    }
}
