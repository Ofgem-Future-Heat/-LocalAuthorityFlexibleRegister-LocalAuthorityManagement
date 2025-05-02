using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Ofgem.API.LAF.LocalAuthorityManagement.Application.Abstractions;
using Ofgem.LAF.SharedLibrary.Context;
using Ofgem.LAF.SharedLibrary.Enums;

namespace Ofgem.API.LAF.LocalAuthorityManagement.Data.Repositories;

public class DeclarationsRepository : IDeclarationsRepository
{
    private readonly LafContext _context;

    public DeclarationsRepository(LafContext context, IConfiguration configuration)
    {
        bool useInMemoryDb;
        _context = context;
        var dbConfiguration = configuration.GetSection("DbConfiguration:UseInMemoryDb");
        if (dbConfiguration.Exists())
        {
            useInMemoryDb = string.IsNullOrEmpty(dbConfiguration.Value) || bool.Parse(dbConfiguration.Value);
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

    public async Task<int> GetCountUsingSoiId(Guid statementOfIntentId)
    {
        var count = await _context.Declarations!
            .CountAsync(x => x.StatementOfIntentId == statementOfIntentId
                        && x.RecordStatus == RecordStatus.Current);

        return count;
    }

    public int GetCountAffectedByChange(string onsCode)
    {
        var count = _context.Declarations!
            .Where(x => x.StatementOfIntentId == null
                        && x.RecordStatus == RecordStatus.Current)
            .Count(x => x.Urn!.Substring(0, 9).Contains(onsCode));

        return count;
    }

    private void CreateMockDbValues()
    {
        if (_context.Declarations!.Any()) return;
        _context.Declarations!.Add(new()
        {
            StatementOfIntentId = new Guid("A27031A5-D9DD-4F69-8F88-6596414A0EB0"),
            DeclarationId = new Guid("E0EC46F1-852F-4452-B7E8-2EDB5202BE71"),
            RecordStatus = RecordStatus.Current
        }
        );

        _context.SaveChanges();
    }
}