using MediatR;
using Ofgem.LAF.SharedLibrary.Enums;

namespace Ofgem.API.LAF.LocalAuthorityManagement.Application.StatementOfIntent.Commands;

public class Create : IRequest<Result<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent>>
{
    public Guid LocalAuthorityId { get; init; }

    public required string OnsCode { get; init; }

    public DateTime PublishedDate { get; init; }

    public SoiStatusV2 Status { get; init; }

    public SoiCategory Category { get; init; }

    public bool CanSubmit { get; init; }

    public required string VersionNumber { get; init; }

    public string? StatementOfIntentLink { get; init; }

    public List<Guid>? DesignatedLas { get; init; }
}