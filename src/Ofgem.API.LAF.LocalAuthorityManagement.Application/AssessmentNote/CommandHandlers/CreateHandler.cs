using MediatR;
using Ofgem.API.LAF.LocalAuthorityManagement.Application.Abstractions;
using Ofgem.API.LAF.LocalAuthorityManagement.Application.AssessmentNote.Commands;

namespace Ofgem.API.LAF.LocalAuthorityManagement.Application.AssessmentNote.CommandHandlers;

public class CreateHandler : IRequestHandler<Create, Ofgem.LAF.SharedLibrary.Models.AssessmentNote>
{
    private readonly IAssessmentNoteRepository _assessmentNoteRepository;
    public CreateHandler(IAssessmentNoteRepository assessmentNoteRepository)
    {
        _assessmentNoteRepository = assessmentNoteRepository;
    }

    public async Task<Ofgem.LAF.SharedLibrary.Models.AssessmentNote> Handle(Create request, CancellationToken cancellationToken)
    {
        var localAuthority = new Ofgem.LAF.SharedLibrary.Models.AssessmentNote()
        {
            StatementOfIntentId = request.StatementOfIntentId,
            Text = request.Text,
            CreatedByName = request.CreatedBy
        };

        return await _assessmentNoteRepository.Create(localAuthority);
    }
}