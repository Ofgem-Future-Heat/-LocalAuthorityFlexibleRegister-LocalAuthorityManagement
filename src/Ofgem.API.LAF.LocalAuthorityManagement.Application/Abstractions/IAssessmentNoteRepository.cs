namespace Ofgem.API.LAF.LocalAuthorityManagement.Application.Abstractions;

public interface IAssessmentNoteRepository
{
    Task<Ofgem.LAF.SharedLibrary.Models.AssessmentNote> Create(Ofgem.LAF.SharedLibrary.Models.AssessmentNote assessmentNote);

    Task<Ofgem.LAF.SharedLibrary.Models.AssessmentNote?> Get(Guid assessmentNoteId);

    Task Delete(Guid assessmentNoteId);
}
    