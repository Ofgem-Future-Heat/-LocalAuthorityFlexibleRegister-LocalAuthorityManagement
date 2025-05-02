using MediatR;

namespace Ofgem.API.LAF.LocalAuthorityManagement.Application.AssessmentNote.Commands;

public class Create : IRequest<Ofgem.LAF.SharedLibrary.Models.AssessmentNote>
{
    public Guid StatementOfIntentId { get; init; }

    public string? Text { get; init; }

    public string? CreatedBy { get; init; }
}