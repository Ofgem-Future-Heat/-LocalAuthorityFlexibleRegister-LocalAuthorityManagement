using MediatR;
using Ofgem.API.LAF.LocalAuthorityManagement.Application.Abstractions;
using Ofgem.API.LAF.LocalAuthorityManagement.Application.AssessmentNote.Commands;

namespace Ofgem.API.LAF.LocalAuthorityManagement.Application.AssessmentNote.CommandHandlers;

public class DeleteHandler(IAssessmentNoteRepository assessmentNoteRepository) : IRequestHandler<Delete, Result<Unit>>
{
    public async Task<Result<Unit>> Handle(Delete request, CancellationToken cancellationToken)
    {
        await assessmentNoteRepository.Delete(request.AssessmentNoteId);
        return Result<Unit>.Success(Unit.Value);
    }
}