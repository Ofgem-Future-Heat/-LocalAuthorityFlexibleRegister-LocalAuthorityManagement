using Ofgem.API.LAF.LocalAuthorityManagement.Application.StatementOfIntent.Commands;
using Ofgem.API.LAF.LocalAuthorityManagement.Application.StatementOfIntent.Queries;
using Ofgem.LAF.SharedLibrary.Enums;
using Ofgem.LAF.SharedLibrary.Models;

namespace Ofgem.API.LAF.LocalAuthorityManagement.Application.Abstractions;

public interface IStatementOfIntentRepository
{
    Task<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent?> Create(Ofgem.LAF.SharedLibrary.Models.StatementOfIntent statementOfIntent);
    Task<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent?> UpdateSignOffChecklist(UpdateSignOffChecklist signOffChecklistSource);
    Task<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent?> UpdateAssessmentChecklist(UpdateAssessmentChecklist assessmentChecklistSource);
    Task<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent?> UpdateRoutes(UpdateRoutes routesSource);
    Task<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent?> Get(Guid statementOfIntentId);
    Task<int> GetCountAtStatus(int status);
    Task<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent?> GetByKeyValues(string onsCode, DateTime publishedDate);
    Task<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent> UpdateSoi(Ofgem.LAF.SharedLibrary.Models.StatementOfIntent soi);
    Task<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent> UpdateById(Ofgem.LAF.SharedLibrary.Models.StatementOfIntent soi);

    Task<List<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent>?> GetAll(GetFiltered getAllSource);
    Task<PagedResult<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent>?> GetFiltered(GetFiltered filter, List<Guid> las);
    Task<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent?> GetByStatementOfIntentId(Guid statementOfIntent);

    Task<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent?> GetSoiByOnsCode(string? onsCode, DateTime publishedDate);

    Task<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent?> InitialAssessmentChecklist(Ofgem.LAF.SharedLibrary.Models.StatementOfIntent soi);

    Task<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent?> EligibilityRoute2Proxy5(Guid statementOfIntentId, bool? isProxy5SchemePresent, bool? isDescriptionNiceNg6Recommendation2);

    Task<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent?> SoiStatusSetting(Ofgem.LAF.SharedLibrary.Models.StatementOfIntent soi);

    Task<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent?> SchemeDetails(Guid statementOfIntentId, ForSchemeEnum forScheme, bool? isPublishedDateCorrect, bool? isProxy5PartOfRoute2);

    Task<string> AddSoiLog(Ofgem.LAF.SharedLibrary.Models.StatementOfIntentLog soiLog);

    Task<PagedResult<StatementOfIntentLog>> GetFilteredSoiLogs(UploadFilter filter);
}