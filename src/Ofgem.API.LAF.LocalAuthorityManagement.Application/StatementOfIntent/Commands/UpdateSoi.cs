using MediatR;
using Ofgem.LAF.SharedLibrary.Enums;

namespace Ofgem.API.LAF.LocalAuthorityManagement.Application.StatementOfIntent.Commands;

public class UpdateSoi : IRequest<Result<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent>>
{
    public required Guid StatementOfIntentId { get; init; }

    public required Guid LocalAuthorityId { get; set; }

    public required string OnsCode { get; set; }

    public DateTime PublishedDate { get; init; }

    public required SoiStatusV2 Status { get; init; }

    public required SoiCategory Category { get; init; }

    public required string VersionNumber { get; init; }

    public bool CanSubmit { get; init; }

    public required string StatementOfIntentLink { get; init; }

    public List<Guid>? DesignatedLas { get; init; }
}