using MediatR;

namespace Ofgem.API.LAF.LocalAuthorityManagement.Application.AssessmentNote.Commands;

public class Delete : IRequest<Result<Unit>>
{
    public Guid AssessmentNoteId { get; init; }

}