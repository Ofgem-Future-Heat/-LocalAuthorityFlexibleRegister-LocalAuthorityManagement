using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Ofgem.API.LAF.LocalAuthorityManagement.Application.Abstractions;
using Ofgem.API.LAF.LocalAuthorityManagement.Application.StatementOfIntent.Commands;
using Ofgem.API.LAF.LocalAuthorityManagement.Application.StatementOfIntent.Queries;
using Ofgem.LAF.SharedLibrary.Context;
using Ofgem.LAF.SharedLibrary.Enums;
using Ofgem.LAF.SharedLibrary.Models;
using LinqKit;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace Ofgem.API.LAF.LocalAuthorityManagement.Data.Repositories;

public class StatementOfIntentRepository : IStatementOfIntentRepository
{
    private readonly LafContext _context;
    private readonly ILogger<StatementOfIntentRepository> _logger;

    public StatementOfIntentRepository(LafContext context,
        IConfiguration configuration,
        ILogger<StatementOfIntentRepository> logger)
    {
        _logger = logger;

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

    public async Task<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent?> Create(Ofgem.LAF.SharedLibrary.Models.StatementOfIntent statementOfIntent)
    {
        await _context.StatementOfIntents!.AddAsync(statementOfIntent);
        await _context.SaveChangesAsync();

        return statementOfIntent;
    }

    public async Task<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent?> Get(Guid statementOfIntentId)
    {
        Ofgem.LAF.SharedLibrary.Models.StatementOfIntent? soi = await _context.StatementOfIntents!.FirstOrDefaultAsync(p => p.StatementOfIntentId == statementOfIntentId);

        return soi;
    }

    public async Task<int> GetCountAtStatus(int status)
    {
        int soiCount = await _context.StatementOfIntents.Where(x => x.Status == (SoiStatusV2)status).CountAsync();

        return soiCount;
    }

    public async Task<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent?> GetByKeyValues(string onsCode, DateTime publishedDate)
    {

        Ofgem.LAF.SharedLibrary.Models.StatementOfIntent? soi = await _context.StatementOfIntents!
            .FirstOrDefaultAsync(p =>
                p.OnsCode == onsCode
                && p.PublishedDate.Date == publishedDate.Date);

        return soi;
    }

    public async Task<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent> UpdateById(Ofgem.LAF.SharedLibrary.Models.StatementOfIntent soi)
    {
        if (_context.DesignatedLas != null)
            await _context.DesignatedLas.Where(s => s.StatementOfIntentId == soi.StatementOfIntentId)
                .ExecuteDeleteAsync();

        if (_context.AssessmentNotes != null)
            await _context.AssessmentNotes.Where(s => s.StatementOfIntentId == soi.StatementOfIntentId)
                .ExecuteDeleteAsync();

        _context.StatementOfIntents!.Update(soi);

        await _context.SaveChangesAsync();

        return soi;
    }

    public async Task<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent> UpdateSoi(Ofgem.LAF.SharedLibrary.Models.StatementOfIntent soi)
    {
        await _context.DesignatedLas!.Where(s => s.StatementOfIntentId == soi.StatementOfIntentId).ExecuteDeleteAsync();

        _context.StatementOfIntents!.Update(soi);

        await _context.SaveChangesAsync();

        return soi;
    }
    public async Task<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent?> UpdateSignOffChecklist(UpdateSignOffChecklist signOffChecklistSource)
    {
        var statementOfIntentTarget
            = await _context.StatementOfIntents!.FirstOrDefaultAsync(p => p.StatementOfIntentId == signOffChecklistSource.StatementOfIntentId);

        if (statementOfIntentTarget is null) { return null; }

        statementOfIntentTarget.IsSignOffLaOfficerResponsibleStatement = signOffChecklistSource.IsSignOffLaOfficerResponsibleStatement;
        statementOfIntentTarget.IsSignOffResponsiblePersonSigned = signOffChecklistSource.IsSignOffResponsiblePersonSigned;

        await _context.SaveChangesAsync();

        return statementOfIntentTarget;
    }

    public async Task<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent?> UpdateAssessmentChecklist(UpdateAssessmentChecklist assessmentChecklistSource)
    {
        var assessmentChecklistTarget
            = await _context.StatementOfIntents!.FirstOrDefaultAsync(p => p.StatementOfIntentId == assessmentChecklistSource.StatementOfIntentId);

        if (assessmentChecklistTarget is null) { return null; }

        assessmentChecklistTarget.IsUserCombinedTemplate = assessmentChecklistSource.IsUserCombinedTemplate;
        assessmentChecklistTarget.IsOfgemLogoPresent = assessmentChecklistSource.IsOfgemLogoPresent;
        assessmentChecklistTarget.IsVersionClear = assessmentChecklistSource.IsVersionClear;
        assessmentChecklistTarget.IsAppropriateTitle = assessmentChecklistSource.IsAppropriateTitle;
        assessmentChecklistTarget.IsPublishDateCorrect = assessmentChecklistSource.IsPublishDateCorrect;

        await _context.SaveChangesAsync();

        return assessmentChecklistTarget;
    }

    public async Task<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent?> UpdateRoutes(UpdateRoutes routesSource)
    {
        var routesTarget
            = await _context.StatementOfIntents!.FirstOrDefaultAsync(p => p.StatementOfIntentId == routesSource.StatementOfIntentId);

        if (routesTarget is null) { return null; }

        routesTarget.IsRoute1Accurate = routesSource.IsRoute1Accurate;
        routesTarget.IsRoute1SapBandsCorrect = routesSource.IsRoute1SapBandsCorrect;
        routesTarget.IsRoute2Accurate = routesSource.IsRoute2Accurate;
        routesTarget.IsRoute2SapBandsCorrect = routesSource.IsRoute2SapBandsCorrect;
        routesTarget.IsProxy5excluded = routesSource.IsProxy5excluded;
        routesTarget.IsProxy5notexcluded = routesSource.IsProxy5notexcluded;
        routesTarget.IsProxy5Named = routesSource.IsProxy5Named;
        routesTarget.IsProxy1nad3CannotUsedTogether = routesSource.IsProxy1nad3CannotUsedTogether;
        routesTarget.IsProxy7CannotCombi5or6 = routesSource.IsProxy7CannotCombi5or6;
        routesTarget.IsRoute3Accurate = routesSource.IsRoute3Accurate;
        routesTarget.IsRoute3SapBandsCorrect = routesSource.IsRoute3SapBandsCorrect;
        routesTarget.IsRoute4Accurate = routesSource.IsRoute4Accurate;
        routesTarget.IsRoute4SapBandsCorrect = routesSource.IsRoute4SapBandsCorrect;
        routesTarget.IsRoute4JointSoIOnlyUseECO4 = routesSource.IsRoute4JointSoIOnlyUseECO4;

        await _context.SaveChangesAsync();

        return routesTarget;
    }

    public async Task<List<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent>?> GetAll(GetFiltered getAllSource)
    {
        try
        {
            var soiList
                = await _context.StatementOfIntents!.ToListAsync();

            if (soiList is null) { return null; }

            return soiList;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<PagedResult<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent>?> GetFiltered(GetFiltered filter, List<Guid> deslas)
    {
        try
        {
            _logger.LogInformation("Start GetFiltered.");
            var queryableResult = _context.StatementOfIntents
                .Include(x => x.DesignatedLas!)
                .ThenInclude(x => x.LocalAuthority)
                .Select(x => new StatementOfIntent()
                {
                    OnsCode = x.OnsCode,
                    VersionNumber = x.VersionNumber,
                    Status = x.Status,
                    IsAppropriateTitle = x.IsAppropriateTitle,
                    IsRoute2SapBandsCorrect = x.IsRoute2SapBandsCorrect,
                    IsPublishDateCorrect = x.IsPublishDateCorrect,
                    CanSubmit = x.CanSubmit,
                    Category = x.Category,
                    CreatedBy = x.CreatedBy,
                    CreatedByName = x.CreatedByName,
                    CreatedDate = x.CreatedDate,
                    DesignatedLas = x.DesignatedLas.Select(t => new DesignatedLA
                    {
                        StatementOfIntentId = t.StatementOfIntentId,
                        LocalAuthorityId = t.LocalAuthorityId,
                        DesignatedLAId = t.DesignatedLAId,
                        LocalAuthority = t.LocalAuthority
                    }).ToList(),
                    HasDeclarations = x.HasDeclarations,
                    IsOfgemLogoPresent = x.IsOfgemLogoPresent,
                    IsProxy1nad3CannotUsedTogether = x.IsProxy1nad3CannotUsedTogether,
                    IsProxy5Named = x.IsProxy5Named,
                    IsProxy5excluded = x.IsProxy5excluded,
                    IsProxy5notexcluded = x.IsProxy5notexcluded,
                    IsProxy7CannotCombi5or6 = x.IsProxy7CannotCombi5or6,
                    IsRoute1Accurate = x.IsRoute1Accurate,
                    IsRoute1SapBandsCorrect = x.IsRoute2SapBandsCorrect,
                    IsRoute2Accurate = x.IsRoute2Accurate,
                    IsRoute3Accurate = x.IsRoute3Accurate,
                    IsRoute4Accurate = x.IsRoute4Accurate,
                    IsRoute3SapBandsCorrect = x.IsRoute3SapBandsCorrect,
                    IsRoute4JointSoIOnlyUseECO4 = x.IsRoute4JointSoIOnlyUseECO4,
                    IsRoute4SapBandsCorrect = x.IsRoute4SapBandsCorrect,
                    IsSignOffLaOfficerResponsibleStatement = x.IsSignOffLaOfficerResponsibleStatement,
                    IsSignOffResponsiblePersonSigned = x.IsSignOffResponsiblePersonSigned,
                    IsUserCombinedTemplate = x.IsUserCombinedTemplate,
                    IsVersionClear = x.IsVersionClear,
                    PublishedDate = x.PublishedDate,
                    StatementOfIntentId = x.StatementOfIntentId,
                    StatementOfIntentLink = x.StatementOfIntentLink,
                    LocalAuthorityId = x.LocalAuthorityId,
                    LocalAuthority = x.LocalAuthority,
                    IsSoiSameAsWebsite = x.IsSoiSameAsWebsite,
                    HasMostRecentTemplate = x.HasMostRecentTemplate,
                    IsPreviousVersionStatusClear = x.IsPreviousVersionStatusClear,
                    ForScheme = x.ForScheme,
                    IsPublishedDateCorrect = x.IsPublishedDateCorrect,
                    IsProxy5PartOfRoute2 = x.IsProxy5PartOfRoute2,
                    IsProxy5SchemePresent = x.IsProxy5SchemePresent,
                    IsDescriptionNiceNg6Recomendation2 = x.IsDescriptionNiceNg6Recomendation2,
                    IsLocalAuthorityNamed = x.IsLocalAuthorityNamed,
                    IsDelegatedAuthorityNamed = x.IsDelegatedAuthorityNamed
                })
                .AsNoTracking()
                .AsExpandable();

            var statusPredicate = PredicateBuilder.New<StatementOfIntent>();
            if (filter.Status.Length > 0)
            {
                foreach (SoiStatusV2 status in filter.Status)
                {
                    statusPredicate = statusPredicate.Or(x => x.Status == status);
                }
                queryableResult = queryableResult.Where(statusPredicate);
            }

            // the top level la filter will deal with the base la.
            // filter on LA list
            if (!string.IsNullOrEmpty(filter.BaseLocalAuthority) || filter.LAs.Length > 0)
            {
                var laPredicate = PredicateBuilder.New<StatementOfIntent>();
                if (!string.IsNullOrEmpty(filter.BaseLocalAuthority))
                {
                    laPredicate.Or(x => x.OnsCode == filter.BaseLocalAuthority);
                }

                if (filter.LAs.Length > 0)
                {
                    string laString = string.Empty;
                    foreach (string la in filter.LAs)
                    {
                        laString = laString + la + ",";
                    }

                    laPredicate.Or(x => x.DesignatedLas
                        .Any(y => laString.Contains(y.LocalAuthority.OnsCode))
                    || laString.Contains(x.OnsCode));
                }

                queryableResult = queryableResult.Where(laPredicate);
            }

            var res = await queryableResult.ToListAsync();

            // only get one soi per la and get the latest version by published date 😌 thank you.
            // removed after discussion - may ask to put it back
            // var res = resPreSort.GroupBy(x => x.OnsCode)
            //    .Select(g => g.OrderByDescending(x => x.PublishedDate).FirstOrDefault());

            var orderedResult = res.OrderBy(y =>
                y switch
                {
                    _ when y.Status == SoiStatusV2.ToBeAssessed => 1,
                    _ when y.Status == SoiStatusV2.FailedAssessment => 2,
                    _ when y.Status == SoiStatusV2.AwaitingSignOff => 3,
                    _ when y.Status == SoiStatusV2.PassedAssessment => 4,
                    _ => 5
                })
                .ThenBy(z => z.PublishedDate).ToList();

            var result = new PagedResult<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent>();
            result.CurrentPage = filter.PageIndex;
            result.PageSize = filter.RecordsPerPage;
            result.RowCount = orderedResult.Count;

            var pageCount = (double)result.RowCount / filter.RecordsPerPage;
            result.PageCount = (int)Math.Ceiling(pageCount);

            var skip = (filter.PageIndex - 1) * filter.RecordsPerPage;
            result.Results = orderedResult.Skip(skip).Take(filter.RecordsPerPage).ToList();
            return result;

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent?> GetByStatementOfIntentId(Guid statementOfIntentId)
    {
        try
        {
            _logger.LogInformation("Start GetStatementOfIntentById.");

            StatementOfIntent? statementOfIntentResult = await _context.StatementOfIntents!
               .Include(x => x.DesignatedLas!)
               .ThenInclude(x => x.LocalAuthority)
               .Select(x => new StatementOfIntent()
               {
                   OnsCode = x.OnsCode,
                   VersionNumber = x.VersionNumber,
                   Status = x.Status,
                   IsAppropriateTitle = x.IsAppropriateTitle,
                   IsRoute2SapBandsCorrect = x.IsRoute2SapBandsCorrect,
                   IsPublishDateCorrect = x.IsPublishDateCorrect,
                   CanSubmit = x.CanSubmit,
                   Category = x.Category,
                   CreatedBy = x.CreatedBy,
                   CreatedByName = x.CreatedByName,
                   CreatedDate = x.CreatedDate,
                   DesignatedLas = x.DesignatedLas!.Select(t => new DesignatedLA
                   {
                       StatementOfIntentId = t.StatementOfIntentId,
                       LocalAuthorityId = t.LocalAuthorityId,
                       DesignatedLAId = t.DesignatedLAId,
                       LocalAuthority = t.LocalAuthority
                   }).ToList(),
                   HasDeclarations = x.HasDeclarations,
                   IsOfgemLogoPresent = x.IsOfgemLogoPresent,
                   IsProxy1nad3CannotUsedTogether = x.IsProxy1nad3CannotUsedTogether,
                   IsProxy5Named = x.IsProxy5Named,
                   IsProxy5excluded = x.IsProxy5excluded,
                   IsProxy5notexcluded = x.IsProxy5notexcluded,
                   IsProxy7CannotCombi5or6 = x.IsProxy7CannotCombi5or6,
                   IsRoute1Accurate = x.IsRoute1Accurate,
                   IsRoute1SapBandsCorrect = x.IsRoute2SapBandsCorrect,
                   IsRoute2Accurate = x.IsRoute2Accurate,
                   IsRoute3Accurate = x.IsRoute3Accurate,
                   IsRoute4Accurate = x.IsRoute4Accurate,
                   IsRoute3SapBandsCorrect = x.IsRoute3SapBandsCorrect,
                   IsRoute4JointSoIOnlyUseECO4 = x.IsRoute4JointSoIOnlyUseECO4,
                   IsRoute4SapBandsCorrect = x.IsRoute4SapBandsCorrect,
                   IsSignOffLaOfficerResponsibleStatement = x.IsSignOffLaOfficerResponsibleStatement,
                   IsSignOffResponsiblePersonSigned = x.IsSignOffResponsiblePersonSigned,
                   IsUserCombinedTemplate = x.IsUserCombinedTemplate,
                   IsVersionClear = x.IsVersionClear,
                   PublishedDate = x.PublishedDate,
                   StatementOfIntentId = x.StatementOfIntentId,
                   StatementOfIntentLink = x.StatementOfIntentLink,
                   LocalAuthorityId = x.LocalAuthorityId,
                   LocalAuthority = x.LocalAuthority,
                   IsSoiSameAsWebsite = x.IsSoiSameAsWebsite,
                   HasMostRecentTemplate = x.HasMostRecentTemplate,
                   IsPreviousVersionStatusClear = x.IsPreviousVersionStatusClear,
                   ForScheme = x.ForScheme,
                   IsPublishedDateCorrect = x.IsPublishedDateCorrect,
                   IsProxy5PartOfRoute2 = x.IsProxy5PartOfRoute2,
                   IsProxy5SchemePresent = x.IsProxy5SchemePresent,
                   IsDescriptionNiceNg6Recomendation2 = x.IsDescriptionNiceNg6Recomendation2,
                   IsLocalAuthorityNamed = x.IsLocalAuthorityNamed,
                   IsDelegatedAuthorityNamed = x.IsDelegatedAuthorityNamed
               })
               .AsNoTracking()
               .FirstOrDefaultAsync(x => x.StatementOfIntentId == statementOfIntentId);

            if (statementOfIntentResult is null) { return null; }

            var assessmentNotes = await _context.AssessmentNotes!
                .Where(x => x.StatementOfIntentId == statementOfIntentId)
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync();

            statementOfIntentResult.AssessmentNotes = assessmentNotes;

            return statementOfIntentResult;

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }


    public async Task<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent?> GetSoiByOnsCode(string? onsCode, DateTime publishedDate)
    {
        try
        {
            _logger.LogInformation("Start GetStatementOfIntentById.");

            Ofgem.LAF.SharedLibrary.Models.StatementOfIntent? soi = await _context.StatementOfIntents!
                .FirstOrDefaultAsync(p =>
                    p.OnsCode == onsCode
                    && p.PublishedDate.Date == publishedDate.Date);

            if (soi is null) { return null; }

            return soi;

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<StatementOfIntent?> InitialAssessmentChecklist(StatementOfIntent soi)
    {
        _logger.LogInformation("Update InitialAssessmentChecklist.");

        var statementOfIntent
        = await _context.StatementOfIntents!.FirstOrDefaultAsync(p => p.StatementOfIntentId == soi.StatementOfIntentId);

        if (statementOfIntent is null) { return null; }

        statementOfIntent.IsLocalAuthorityNamed = soi.IsLocalAuthorityNamed;
        statementOfIntent.IsDelegatedAuthorityNamed = soi.IsDelegatedAuthorityNamed;
        statementOfIntent.IsSoiSameAsWebsite = soi.IsSoiSameAsWebsite;
        statementOfIntent.HasMostRecentTemplate = soi.HasMostRecentTemplate;
        statementOfIntent.IsPreviousVersionStatusClear = soi.IsPreviousVersionStatusClear;

        await _context.SaveChangesAsync();

        return statementOfIntent;
    }

    public async Task<StatementOfIntent?> EligibilityRoute2Proxy5(Guid statementOfIntentId, bool? isProxy5SchemePresent, bool? isDescriptionNiceNg6Recommendation2)
    {
        _logger.LogInformation("Update EligibilityRoute2Proxy5.");

        var statementOfIntent
            = await _context.StatementOfIntents!.FirstOrDefaultAsync(p => p.StatementOfIntentId == statementOfIntentId);

        if (statementOfIntent is null) { return null; }

        statementOfIntent.IsProxy5SchemePresent = isProxy5SchemePresent;
        statementOfIntent.IsDescriptionNiceNg6Recomendation2 = isDescriptionNiceNg6Recommendation2;

        await _context.SaveChangesAsync();

        return statementOfIntent;
    }

    public async Task<StatementOfIntent?> SoiStatusSetting(StatementOfIntent soi)
    {
        _logger.LogInformation("Update SoiStatusSetting.");

        var statementOfIntent
            = await _context.StatementOfIntents!.FirstOrDefaultAsync(p => p.StatementOfIntentId == soi.StatementOfIntentId);

        if (statementOfIntent is null) { return null; }

        statementOfIntent.Status = soi.Status;

        if (soi.AssessmentNotes != null && soi.AssessmentNotes.Count != 0)
        {
            await _context.AssessmentNotes!.Where(s => s.StatementOfIntentId == soi.StatementOfIntentId).ExecuteDeleteAsync();

            foreach (var assessmentNote in soi.AssessmentNotes)
            {
                await _context.AssessmentNotes!.AddAsync(assessmentNote);
            }
        }

        await _context.SaveChangesAsync();

        return statementOfIntent;
    }

    public async Task<StatementOfIntent?> SchemeDetails(Guid statementOfIntentId, ForSchemeEnum forScheme, bool? isPublishedDateCorrect, bool? isProxy5PartOfRoute2)
    {
        _logger.LogInformation("Update SchemeDetails.");

        var statementOfIntent
            = await _context.StatementOfIntents!.FirstOrDefaultAsync(p => p.StatementOfIntentId == statementOfIntentId);

        if (statementOfIntent is null) { return null; }

        statementOfIntent.ForScheme = forScheme;
        statementOfIntent.IsPublishedDateCorrect = isPublishedDateCorrect;
        statementOfIntent.IsProxy5PartOfRoute2 = isProxy5PartOfRoute2;

        await _context.SaveChangesAsync();

        return statementOfIntent;
    }

    public async Task<string> AddSoiLog(StatementOfIntentLog soiLog)
    {
        _logger.LogInformation("Add Soi Log.");

        await _context.StatementOfIntentLogs!.AddAsync(soiLog);

        await _context.SaveChangesAsync();

        return "Success";
    }

    public async Task<PagedResult<StatementOfIntentLog>> GetFilteredSoiLogs(UploadFilter filter)
    {
        try
        {
            _logger.LogInformation("Start GetFilteredSoiLogs.");

            var queryableResult = _context.StatementOfIntentLogs!
                .OrderByDescending(x => x.CreatedDate)
                .AsNoTracking()
                .AsExpandable();

            // filter on date from
            if (!string.IsNullOrWhiteSpace(filter.DateFrom))
            {
                DateTime dateFrom = DateTime.ParseExact(filter.DateFrom, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                queryableResult = queryableResult.Where(x => x.CreatedDate > dateFrom);
            }

            // filter on date to
            if (!string.IsNullOrWhiteSpace(filter.DateTo))
            {
                DateTime dateTo = DateTime.ParseExact(filter.DateTo, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                queryableResult = queryableResult.Where(x => x.CreatedDate < dateTo);
            }

            var result = new PagedResult<Ofgem.LAF.SharedLibrary.Models.StatementOfIntentLog>();

            result.CurrentPage = filter.PageIndex;
            result.PageSize = filter.RecordsPerPage;
            result.RowCount = await queryableResult.CountAsync();

            var pageCount = (double)result.RowCount / filter.RecordsPerPage;
            result.PageCount = (int)Math.Ceiling(pageCount);

            var skip = (filter.PageIndex - 1) * filter.RecordsPerPage;
            result.Results = await queryableResult.Skip(skip).Take(filter.RecordsPerPage).ToListAsync();
            return result;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private void CreateMockDbValues()
    {
        if (_context.StatementOfIntents!.Any()) return;
        _context.StatementOfIntents!.Add(new()
            {
                StatementOfIntentId = new Guid("A27031A5-D9DD-4F69-8F88-6596414A0EB0"),
                OnsCode = "12345678",
                VersionNumber = "Version1",
                StatementOfIntentLink = "https://whatever.com",
                PublishedDate = DateTime.Now,
                LocalAuthorityId = Guid.NewGuid(),
                CanSubmit = false,
                Category = SoiCategory.Complete,
                IsOfgemLogoPresent = true,

            }
        );

        _context.SaveChanges();
    }
}