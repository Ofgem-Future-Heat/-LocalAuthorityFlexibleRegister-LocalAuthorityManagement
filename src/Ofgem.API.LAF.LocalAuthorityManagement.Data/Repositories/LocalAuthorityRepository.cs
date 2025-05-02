using ChoETL;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Ofgem.API.LAF.LocalAuthorityManagement.Application.Abstractions;
using Ofgem.API.LAF.LocalAuthorityManagement.Data.Extensions;
using Ofgem.LAF.SharedLibrary.Context;
using Ofgem.LAF.SharedLibrary.Models;

namespace Ofgem.API.LAF.LocalAuthorityManagement.Data.Repositories;

public class LocalAuthorityRepository : ILocalAuthorityRepository
{
    private readonly LafContext _context;

    public LocalAuthorityRepository(LafContext context, IConfiguration configuration)
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

    public async Task<LocalAuthority> Create(LocalAuthority localAuthority)
    {
        _context.LocalAuthorities!.Add(localAuthority);

        await _context.SaveChangesAsync();

        return localAuthority;
    }

    public async Task Delete(Guid localAuthorityId)
    {
        var localAuthority = await _context.LocalAuthorities!.FirstOrDefaultAsync(p => p.LocalAuthorityId == localAuthorityId);

        if (localAuthority is null) { return; }

        _context.LocalAuthorities!.Remove(localAuthority);

        await _context.SaveChangesAsync();
    }


    public async Task<PagedResult<LocalAuthority>?> GetFilteredLocalAuthorities(ProfilesFilter filter)
    {
        if (_context.LocalAuthorities == null)
        {
            return null;
        }

        IQueryable<LocalAuthority> queryableResult;
        if (filter.IncludeSoi)
        {
            queryableResult = _context.LocalAuthorities
                .Include(x => x.StatementOfIntents!.OrderByDescending(x1 => x1.PublishedDate))
                .OrderBy(x => x.Name)
                .AsNoTracking()
                .AsExpandable();
        }
        else
        {
            queryableResult = _context.LocalAuthorities
            .OrderBy(x => x.Name)
            .AsNoTracking()
            .AsExpandable();
        }

        if (!filter.Filter.IsNullOrWhiteSpace())
        {
            queryableResult = queryableResult.Where(x => x.Name!.ToUpper().Contains(filter.Filter.ToUpper()));
        }

        var redactedList = await queryableResult.GetPagedAsync(filter.PageIndex, filter.RecordsPerPage);

        foreach (var la in redactedList.Results)
        {
            if (la.StatementOfIntents is null) continue;

            foreach (var soi in la.StatementOfIntents)
            {
                soi.LocalAuthority = null;
            }
        }

        return await Task.FromResult(redactedList);
    }

    public async Task<LocalAuthority?> Get(Guid localAuthorityId)
    {
        LocalAuthority? localAuthority = await _context.LocalAuthorities!.FirstOrDefaultAsync(p => p.LocalAuthorityId == localAuthorityId);

        return localAuthority;
    }


    public async Task<LocalAuthority?> GetByOnsCode(string onsCode)
    {
        LocalAuthority? localAuthority =
            await _context.LocalAuthorities!
                .Include(x => x.StatementOfIntents!.OrderByDescending(x1 => x1.PublishedDate))
                .ThenInclude(x => x.AssessmentNotes!.OrderByDescending(y => y.CreatedDate))
                .Include(x => x.StatementOfIntents!.OrderByDescending(x3 => x3.PublishedDate))
                .ThenInclude(x => x.DesignatedLas!)
                .ThenInclude(x => x.LocalAuthority)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.OnsCode == onsCode);

        return localAuthority;
    }

    public async Task<LocalAuthority?> GetByName(string name)
    {
        LocalAuthority? localAuthority =
            await _context.LocalAuthorities!
                .Include(x => x.StatementOfIntents!.OrderByDescending(x1 => x1.PublishedDate))
                .ThenInclude(x => x.AssessmentNotes!.OrderByDescending(y => y.CreatedDate))
                .Include(x => x.StatementOfIntents!.OrderByDescending(x3 => x3.PublishedDate))
                .ThenInclude(x => x.DesignatedLas!)
                .ThenInclude(x => x.LocalAuthority)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Name == name);

        return localAuthority;
    }

    public async Task<ICollection<LocalAuthority>> GetAll()
    {
        return await _context.LocalAuthorities!.OrderBy(s => s.Name).ToListAsync();
    }

    public async Task<ICollection<LocalAuthority>> GetAssociatedLocalAuthorities(string onsCode)
    {
        return await _context.LocalAuthorities!
            .Where(x => x.OnsCode == onsCode)
            .Join(_context.StatementOfIntents!, soi => soi.LocalAuthorityId,
                la => la.LocalAuthorityId, (la, soi) => new { la, soi })
            .Join(_context.DesignatedLas!, soi => soi.soi.StatementOfIntentId,
                dla => dla.StatementOfIntentId, (soi, dla) => new { soi, dla })
            .Join(_context.LocalAuthorities!, dla => dla.dla.LocalAuthorityId, la2 => la2.LocalAuthorityId,
                (dla, la2) => la2)
            .ToListAsync();
    }


    public async Task<LocalAuthority?> Update(string name, Guid localAuthorityId)
    {
        var localAuthority = await _context.LocalAuthorities!.FirstOrDefaultAsync(p => p.LocalAuthorityId == localAuthorityId);

        if (localAuthority is null) { return null; }

        localAuthority.Name = name;
        await _context.SaveChangesAsync();

        return localAuthority;

    }

    public async Task<LocalAuthority?> UpdateByOnsCode(string onsCode, string name, string email)
    {
        var localAuthority = await _context.LocalAuthorities!.FirstOrDefaultAsync(
            p => p.OnsCode == onsCode);

        if (localAuthority is null) { return null; }

        localAuthority.Name = name;
        localAuthority.Email = email;
        await _context.SaveChangesAsync();

        return localAuthority;

    }

    private void CreateMockDbValues()
    {
        if (_context.LocalAuthorities!.Any()) return;

        _context.LocalAuthorities!.Add(new LocalAuthority
        {
            LocalAuthorityId = new Guid("FD7FC2D7-5247-45C9-960F-23F416E1DA24"),
            Name = "London",
            OnsCode = "A12345678",
            Email = "me@gov.uk"
        });
        _context.LocalAuthorities.Add(new LocalAuthority
        {
            Name = "Manchester",
            Email = "someone.else@manchester.gov.uk",
            OnsCode = "X22222222"
        });
        _context.LocalAuthorities.Add(new LocalAuthority
        {
            Name = "Oldham",
            OnsCode = "X33333333",
            Email = "mr.blobby@oldham.gov.uk",
        });
        _context.SaveChanges();
    }
}