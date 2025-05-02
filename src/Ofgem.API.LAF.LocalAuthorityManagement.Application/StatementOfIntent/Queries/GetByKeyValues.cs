using MediatR;

namespace Ofgem.API.LAF.LocalAuthorityManagement.Application.StatementOfIntent.Queries;

public class GetByKeyValues(string onsCode, DateTime publishedDate)
    : IRequest<Result<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent>>
{
    public string OnsCode { get; init; } = onsCode;

    public DateTime PublishedDate { get; init; } = publishedDate;
}