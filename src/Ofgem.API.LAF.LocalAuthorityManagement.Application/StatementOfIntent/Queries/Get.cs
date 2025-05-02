using MediatR;

namespace Ofgem.API.LAF.LocalAuthorityManagement.Application.StatementOfIntent.Queries;

public class Get : IRequest<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent>
{
    public Guid StatementOfIntentId { get; init; }
}