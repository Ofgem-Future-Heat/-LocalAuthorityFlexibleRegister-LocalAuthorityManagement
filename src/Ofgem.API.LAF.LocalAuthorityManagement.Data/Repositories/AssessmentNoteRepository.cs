using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Ofgem.API.LAF.LocalAuthorityManagement.Application.Abstractions;
using Ofgem.LAF.SharedLibrary.Context;

namespace Ofgem.API.LAF.LocalAuthorityManagement.Data.Repositories;

public class AssessmentNoteRepository : IAssessmentNoteRepository
{
    private readonly LafContext _context;

    public AssessmentNoteRepository(LafContext context, IConfiguration configuration)
    {
        bool useInMemoryDb;
        _context = context;
        var dbConfiguration = configuration.GetSection("DbConfiguration:UseInMemoryDb");
        if (dbConfiguration.Exists())
        {
            if (string.IsNullOrEmpty(dbConfiguration.Value))
            {
                useInMemoryDb = true;
            }
            else
            {
                useInMemoryDb = bool.Parse(dbConfiguration.Value);
            }
        }
        else
        {
            useInMemoryDb = false;
        }

        if (useInMemoryDb)
        {
            CreateMockDbValues();
        }
    }

    public async Task<Ofgem.LAF.SharedLibrary.Models.AssessmentNote> Create(Ofgem.LAF.SharedLibrary.Models.AssessmentNote assessmentNote)
    {
        ArgumentNullException.ThrowIfNull(_context.AssessmentNotes);

        assessmentNote.CreatedDate = DateTime.Now;

        await _context.AssessmentNotes.AddAsync(assessmentNote);
        await _context.SaveChangesAsync();

        return assessmentNote;
    }
    public async Task<Ofgem.LAF.SharedLibrary.Models.AssessmentNote?> Get(Guid assessmentNoteId)
    {
        ArgumentNullException.ThrowIfNull(_context.AssessmentNotes);

        var assessmentNote = await _context.AssessmentNotes.FirstOrDefaultAsync(p => p.AssessmentNoteId == assessmentNoteId);

        return assessmentNote;
    }

    public async Task Delete(Guid assessmentNoteId)
    {
        ArgumentNullException.ThrowIfNull(_context.AssessmentNotes);

        var target = await _context.AssessmentNotes.SingleAsync(x => x.AssessmentNoteId == assessmentNoteId);

       _context.AssessmentNotes.Remove(target);

        await _context.SaveChangesAsync();
    }

    private void CreateMockDbValues()
    {
        ArgumentNullException.ThrowIfNull(_context.AssessmentNotes);

        if (_context.AssessmentNotes.Any()) return;

        _context.AssessmentNotes.Add(new Ofgem.LAF.SharedLibrary.Models.AssessmentNote()
            {
                StatementOfIntentId = new Guid("A27031A5-D9DD-4F69-8F88-6596414A0EB0"),
                AssessmentNoteId = new Guid("8D04ACA5-6F8B-4BFB-9134-AF286C018D97"),
                Text = "Test Text"
            }
        );

        _context.SaveChanges();
    }

}