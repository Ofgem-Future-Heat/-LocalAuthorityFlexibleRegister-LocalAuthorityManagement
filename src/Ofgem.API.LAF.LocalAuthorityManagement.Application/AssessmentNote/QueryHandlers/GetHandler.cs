using MediatR;
using Ofgem.API.LAF.LocalAuthorityManagement.Application.Abstractions;
using Ofgem.API.LAF.LocalAuthorityManagement.Application.AssessmentNote.Queries;

namespace Ofgem.API.LAF.LocalAuthorityManagement.Application.AssessmentNote.QueryHandlers;

public class GetHandler(IAssessmentNoteRepository assessmentNoteRepository)
    : IRequestHandler<Get, Ofgem.LAF.SharedLibrary.Models.AssessmentNote?>
{
    public async Task<Ofgem.LAF.SharedLibrary.Models.AssessmentNote?> Handle(Get request, CancellationToken cancellationToken)
    {
        Ofgem.LAF.SharedLibrary.Models.AssessmentNote? assessmentNote = await assessmentNoteRepository.Get(request.AssessmentNoteId);
        return assessmentNote;
    }
}