using MediatR;

namespace Ofgem.API.LAF.LocalAuthorityManagement.Application.StatementOfIntent.Queries
{
    public class GetByStatementOfIntentId : IRequest<Result<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent>>
    {
        public Guid StatementOfIntentId { get; init; }
    }
}
