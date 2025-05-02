using MediatR;

namespace Ofgem.API.LAF.LocalAuthorityManagement.Application.AssessmentNote.Queries;

public class Get : IRequest<Ofgem.LAF.SharedLibrary.Models.AssessmentNote>
{
    public Guid AssessmentNoteId { get; init; }
}