

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ofgem.API.LAF.LocalAuthorityManagement.Api.Abstractions;
using Ofgem.API.LAF.LocalAuthorityManagement.Application;
using Ofgem.API.LAF.LocalAuthorityManagement.Application.Declarations.Queries;
using Ofgem.API.LAF.LocalAuthorityManagement.Application.StatementOfIntent.Commands;
using Ofgem.API.LAF.LocalAuthorityManagement.Application.StatementOfIntent.Commands.v2;
using Ofgem.API.LAF.LocalAuthorityManagement.Application.StatementOfIntent.Queries;
using Ofgem.LAF.SharedLibrary.Enums;
using Ofgem.LAF.SharedLibrary.Models; 
using Ofgem.LAF.SharedLibrary.Extensions;
using static Ofgem.API.LAF.LocalAuthorityManagement.Application.LogEvents;

namespace Ofgem.API.LAF.LocalAuthorityManagement.Api.EndpointDefinitions;

[ExcludeFromCodeCoverage]
public class StatementOfIntentEndpointDefinition : IEndpointDefinition
{

    public void RegisterEndpoints(WebApplication app)
    {
        var statementOfIntents = app.MapGroup("/api/Soi");
        statementOfIntents.MapPost("/", Create).WithOpenApi();
        statementOfIntents.MapGet("/{id}", Get).WithName("GetSoiById").WithOpenApi();
        statementOfIntents.MapGet("/count-at-status/{status}", GetCountAtStatus).WithName("GetCountAtStatus").WithOpenApi();
        statementOfIntents.MapPut("/updateSignOffCheckList", UpdateSignOffChecklist).WithName("UpdateSignOffCheckList").WithOpenApi();
        statementOfIntents.MapPut("/updateStatementOfIntent", UpdateSoi).WithOpenApi();
        statementOfIntents.MapPut("/updateAssessmentCheckList", UpdateAssessmentChecklist).WithName("UpdateAssessmentCheckList").WithOpenApi();
        statementOfIntents.MapPut("/updateRoutes", UpdateRoutes).WithName("UpdateRoutes").WithOpenApi();
        statementOfIntents.MapPost("/soiList", GetSoiList).WithOpenApi();
        statementOfIntents.MapGet("/by-soi-id/{statementOfIntentId}", GetBySoiId).WithName("GetBySoiId").WithOpenApi();
        statementOfIntents.MapPut("/update", UpdateBySoiId).WithName("UpdateBySoiId").WithOpenApi();
        statementOfIntents.MapGet("/GetSoiByOns/{onsCode}/{publishedDate}", GetSoiByOns).WithName("GetSoiByOns").WithOpenApi();


        var statementOfIntentsV2 = app.MapGroup("/api/Soi/v2");
        statementOfIntentsV2.MapGet("/getSoiV2/{statementOfIntentId}", GetBySoiId).WithOpenApi();
        statementOfIntentsV2.MapPost("/", Create).WithOpenApi();
        statementOfIntentsV2.MapPut("/updateStatementOfIntent", UpdateSoi).WithOpenApi();
        statementOfIntentsV2.MapPut("/initialAssessmentCheckList", UpdateInitialAssessmentChecklist).WithOpenApi();
        statementOfIntentsV2.MapPut("/eligibilityRoute2Proxy5", UpdateEligibilityRoute2Proxy5).WithOpenApi();
        statementOfIntentsV2.MapPut("/soiStatusSetting", UpdateSoiStatusSetting).WithOpenApi();
        statementOfIntentsV2.MapPut("/schemeDetails", UpdateSchemeDetails).WithOpenApi();
        statementOfIntentsV2.MapPost("/addSoiLog", AddSoiLog).WithOpenApi();
        statementOfIntentsV2.MapPost("/getFilteredSoiLog", GetFilteredSoiLog).WithOpenApi();
    }

    private async Task<IResult> Create(StatementOfIntentCreateRequest soiCreateRequest, IMediator mediator, ILogger<StatementOfIntent> logger)
    {
        logger.LogLafInformation(CreateStatementOfIntent);

        Create createRequest = new()
        {
            LocalAuthorityId = soiCreateRequest.LocalAuthorityId,
            OnsCode = soiCreateRequest.OnsCode ?? string.Empty,
            VersionNumber = soiCreateRequest.VersionNumber ?? string.Empty,
            CanSubmit = soiCreateRequest.CanSubmit,
            DesignatedLas = soiCreateRequest.DesignatedLas ?? [],
            Category = soiCreateRequest.Category,
            PublishedDate = soiCreateRequest.PublishedDate,
            StatementOfIntentLink = soiCreateRequest.StatementOfIntentLink ?? string.Empty,
            Status = soiCreateRequest.Status
        };
        var result = await mediator.Send(createRequest);

        if (result.IsSuccess && (result.Value is not null))
        {
            return Results.CreatedAtRoute("GetSoiById", new { Id = result.Value.StatementOfIntentId }, result.Value.Pared());
        }

        logger.LogLafError(CreateStatementOfIntent, "An SOI with ons code and published date already exists.", soiCreateRequest.OnsCode, soiCreateRequest.PublishedDate.Date);

        ProblemDetails problemDetail = new()
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Conflicting SOI exists.",
            Detail = "Statement of Intent published dates must be unique for Statement of Intent version contained for a Local Authority profile."
        };
        return Results.Problem(problemDetail);
    }

    private async Task<IResult> Get(Guid id, IMediator mediator, ILogger<StatementOfIntent> logger)
    {
        logger.LogLafInformation(GetStatementOfIntent);

        var getLocalAuthorityRequest = new Ofgem.API.LAF.LocalAuthorityManagement.Application.LocalAuthority.Queries.Get { LocalAuthorityId = id };
        var localAuthorityResult = await mediator.Send(getLocalAuthorityRequest);

        if (localAuthorityResult.IsSuccess) return TypedResults.Ok(localAuthorityResult.Value);

        logger.LogError("Local Authority {Id} was not found.", id);

        return Results.Problem(new ProblemDetails
        {
            Status = StatusCodes.Status404NotFound,
            Detail = $"Local Authority {id} was not found."
        });
    }

    private async Task<IResult> UpdateSignOffChecklist(SOISignOffChecklistRequest soiSignOffChecklistRequest,
                                    IMediator mediator,
                                    ILogger<StatementOfIntent> logger)
    {
        logger.LogLafInformation(UpdateStatementOfIntent);

        var updateSignOffChecklistCommand = new UpdateSignOffChecklist
        {
            StatementOfIntentId = soiSignOffChecklistRequest.StatementOfIntentId,
            OnsCode = soiSignOffChecklistRequest.OnsCode,
            VersionNumber = soiSignOffChecklistRequest.VersionNumber,
            IsSignOffLaOfficerResponsibleStatement = soiSignOffChecklistRequest.IsSignOffLaOfficerResponsibleStatement,
            IsSignOffResponsiblePersonSigned = soiSignOffChecklistRequest.IsSignOffResponsiblePersonSigned
        };

        var updateSignOffChecklistResult = await mediator.Send(updateSignOffChecklistCommand);

        if (updateSignOffChecklistResult.IsSuccess) return TypedResults.Ok(updateSignOffChecklistResult);

        var problemDetail =
            $"Failed to update StatementOfIntent Sign Off Checklist for{soiSignOffChecklistRequest.OnsCode}, {soiSignOffChecklistRequest.VersionNumber}";

        logger.LogLafError(UpdateStatementOfIntent, problemDetail);

        return Results.Problem(new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Detail = problemDetail
        });
    }

    private async Task<IResult> UpdateSoi(
        StatementOfIntentUpdateRequest statementOfIntent, IMediator mediator,
        ILogger<StatementOfIntent> logger)
    {
        logger.LogLafInformation(UpdateStatementOfIntent);

        var updateSoiCommand = new UpdateSoi
        {
            OnsCode = statementOfIntent.OnsCode ?? string.Empty,
            StatementOfIntentId = statementOfIntent.StatementOfIntentId,
            LocalAuthorityId = statementOfIntent.LocalAuthorityId,
            VersionNumber = statementOfIntent.VersionNumber ?? string.Empty,
            Category = statementOfIntent.Category,
            Status = statementOfIntent.Status,
            StatementOfIntentLink = statementOfIntent.StatementOfIntentLink ?? string.Empty,
            CanSubmit = statementOfIntent.CanSubmit,
            DesignatedLas = statementOfIntent.CanSubmit ? statementOfIntent.DesignatedLas : [],
            PublishedDate = statementOfIntent.PublishedDate
        };
        
        var result = await mediator.Send(updateSoiCommand);

        if (result.IsSuccess)
        {
            var count = 0;

            var returnedSoi = result.Value?.Pared();

            if (returnedSoi is not null)
            {
                // now get the count affected by this change
                if (returnedSoi.Status == SoiStatusV2.PassedAssessment)
                {
                    var affectedCount =
                        await mediator.Send(new GetCountAffectedByChange { OnsCode = returnedSoi.OnsCode });

                    if (affectedCount.IsSuccess) count = affectedCount.Value;
                }

                StatementOfIntentUpdateResponse response = new()
                {
                    OnsCode = returnedSoi.OnsCode,
                    StatementOfIntentId = returnedSoi.StatementOfIntentId,
                    Status = returnedSoi.Status,
                    CanSubmit = returnedSoi.CanSubmit,
                    Category = returnedSoi.Category,
                    CountOfDeclarationsToBeReprocessed = count,
                    LocalAuthorityId = returnedSoi.LocalAuthorityId,
                    PublishedDate = returnedSoi.PublishedDate,
                    StatementOfIntentLink = returnedSoi.StatementOfIntentLink,
                    VersionNumber = returnedSoi.VersionNumber,
                    DesignatedLas = []
                };

                if (returnedSoi.DesignatedLas == null) return TypedResults.Ok(response);

                foreach (var desLa in returnedSoi.DesignatedLas)
                {
                    response.DesignatedLas.Add(desLa.LocalAuthorityId);
                }

                return TypedResults.Ok(response);
            }
        }


        var problemDetail =
            $"Failed to update StatementOfIntent {statementOfIntent.OnsCode}, {statementOfIntent.VersionNumber}.";

        logger.LogLafInformation(UpdateStatementOfIntent, problemDetail);

        return Results.Problem(new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Detail = problemDetail
        });

    }

    private async Task<IResult> UpdateAssessmentChecklist(SOIAssessmentChecklistRequest assessmentChecklistRequest,
                                 IMediator mediator,
                                 ILogger<StatementOfIntent> logger)
    {
        logger.LogLafInformation(UpdateStatementOfIntent);

        var updateAssessmentChecklistCommand = new UpdateAssessmentChecklist
        {
            StatementOfIntentId = assessmentChecklistRequest.StatementOfIntentId,
            OnsCode = assessmentChecklistRequest.OnsCode,
            VersionNumber = assessmentChecklistRequest.VersionNumber,
            IsUserCombinedTemplate = assessmentChecklistRequest.IsUserCombinedTemplate,
            IsOfgemLogoPresent = assessmentChecklistRequest.IsOfgemLogoPresent,
            IsVersionClear = assessmentChecklistRequest.IsVersionClear,
            IsAppropriateTitle = assessmentChecklistRequest.IsAppropriateTitle,
            IsPublishDateCorrect = assessmentChecklistRequest.IsPublishDateCorrect
        };

        var updateAssessmentChecklistResult = await mediator.Send(updateAssessmentChecklistCommand);

        if (updateAssessmentChecklistResult.IsSuccess)
            return TypedResults.Ok(updateAssessmentChecklistResult.Value);

        var problemDetail =
            $"Failed to update StatementOfIntent Assessment Checklist for{assessmentChecklistRequest.OnsCode}, {assessmentChecklistRequest.VersionNumber}";

        logger.LogLafError(UpdateStatementOfIntent, problemDetail);

        return Results.Problem(new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Detail = problemDetail

        });
    }

    private async Task<IResult> UpdateRoutes(SOIRoutesRequest routesRequest,
                             IMediator mediator,
                             ILogger<StatementOfIntent> logger)
    {
        logger.LogLafInformation(UpdateStatementOfIntent);

        var updateRoutesCommand = new UpdateRoutes
        {
            StatementOfIntentId = routesRequest.StatementOfIntentId,
            OnsCode = routesRequest.OnsCode,
            VersionNumber = routesRequest.VersionNumber,
            IsRoute1Accurate = routesRequest.IsRoute1Accurate,
            IsRoute1SapBandsCorrect = routesRequest.IsRoute1SapBandsCorrect,
            IsRoute2Accurate = routesRequest.IsRoute2Accurate,
            IsRoute2SapBandsCorrect = routesRequest.IsRoute2SapBandsCorrect,
            IsProxy5excluded = routesRequest.IsProxy5excluded,
            IsProxy5notexcluded = routesRequest.IsProxy5notexcluded,
            IsProxy5Named = routesRequest.IsProxy5Named,
            IsProxy1nad3CannotUsedTogether = routesRequest.IsProxy1nad3CannotUsedTogether,
            IsProxy7CannotCombi5or6 = routesRequest.IsProxy7CannotCombi5or6,
            IsRoute3Accurate = routesRequest.IsRoute3Accurate,
            IsRoute3SapBandsCorrect = routesRequest.IsRoute3SapBandsCorrect,
            IsRoute4Accurate = routesRequest.IsRoute4Accurate,
            IsRoute4SapBandsCorrect = routesRequest.IsRoute4SapBandsCorrect,
            IsRoute4JointSoIOnlyUseECO4 = routesRequest.IsRoute4JointSoIOnlyUseECO4
        };

        var updateRoutesResult = await mediator.Send(updateRoutesCommand);

        if (updateRoutesResult.IsSuccess) return TypedResults.Ok(updateRoutesResult.Value);


        var problemDetail =
            "Failed to update routes";

        logger.LogLafError(UpdateStatementOfIntent, problemDetail);

        return Results.Problem(new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Detail = problemDetail

        });
    }


    private async Task<IResult> GetSoiList(SoiFilter soiFilter,
        IMediator mediator,
        ILogger<StatementOfIntent> logger)
    {
        logger.LogLafInformation(StatementOfIntents);

        var getSoiListRequest = new GetFiltered
        {
            BaseLocalAuthority = soiFilter.BaseLocalAuthority,
            LAs = soiFilter.LAs,
            Status = soiFilter.Status,
            PageIndex = soiFilter.PageIndex,
            RecordsPerPage = soiFilter.RecordsPerPage
        };
        var soiListResult = await mediator.Send(getSoiListRequest);

        if (soiListResult is not null) return TypedResults.Ok(soiListResult);

        logger.LogError("Error, Unable to fetch the Statement of intent list.");

        return Results.Problem(new ProblemDetails
        {
            Status = StatusCodes.Status404NotFound,
            Detail = "Unable to fetch the Statement of intent list."
        });
    }

    private async Task<IResult> GetCountAtStatus(int status,
        IMediator mediator,
        ILogger<StatementOfIntent> logger)
    {
        logger.LogLafInformation(CountAtStatus);

        var getSoiCountAtStatus = new GetCountAtStatus
        {
            Status = status
        };
        var countAtStatus = await mediator.Send(getSoiCountAtStatus);

        return TypedResults.Ok(countAtStatus);

    }

    private async Task<IResult> GetBySoiId(Guid statementOfIntentId, IMediator mediator, ILogger<StatementOfIntent> logger)
    {
        logger.LogLafInformation(GetStatementOfIntentById);

        var getStatementIntentByIdRequest = new GetByStatementOfIntentId { StatementOfIntentId = statementOfIntentId };

        var statementOfIntent = await mediator.Send(getStatementIntentByIdRequest);

        if (statementOfIntent.IsSuccess)
        {
            logger.LogInformation("Statement of Intent: {StatementOfIntent}", statementOfIntent.Value);

            var redactedSoi = statementOfIntent.Value?.Pared();

            return TypedResults.Ok(redactedSoi);
        }

        logger.LogError("Error, Unable to fetch the Statement of intent.");

        return Results.Problem(new ProblemDetails
        {
            Status = StatusCodes.Status404NotFound,
            Detail = "Unable to fetch the Statement of intent."
        });
    }

    //public StatementOfIntent Pared(StatementOfIntent source, bool descend = true)
    //{
    //    StatementOfIntent soi = new StatementOfIntent
    //    {
    //        OnsCode = source.OnsCode,
    //        VersionNumber = source.VersionNumber,
    //        CanSubmit = source.CanSubmit,
    //        Category = source.Category,
    //        IsOfgemLogoPresent = source.IsOfgemLogoPresent,
    //        IsRoute1Accurate = source.IsRoute1Accurate,
    //        IsRoute1SapBandsCorrect = source.IsRoute1SapBandsCorrect,
    //        IsRoute2Accurate = source.IsRoute2Accurate,
    //        IsProxy5excluded = source.IsProxy5excluded,
    //        IsProxy5notexcluded = source.IsProxy5notexcluded,
    //        IsProxy5Named = source.IsProxy5Named,
    //        IsProxy1nad3CannotUsedTogether = source.IsProxy1nad3CannotUsedTogether,
    //        IsProxy7CannotCombi5or6 = source.IsProxy7CannotCombi5or6,
    //        IsRoute3Accurate = source.IsRoute3Accurate,
    //        IsRoute3SapBandsCorrect = source.IsRoute3SapBandsCorrect,
    //        IsRoute4Accurate = source.IsRoute4Accurate,
    //        IsRoute4SapBandsCorrect = source.IsRoute4SapBandsCorrect,
    //        IsRoute4JointSoIOnlyUseECO4 = source.IsRoute4JointSoIOnlyUseECO4,
    //        IsSignOffLaOfficerResponsibleStatement = source.IsSignOffLaOfficerResponsibleStatement,
    //        IsSignOffResponsiblePersonSigned = source.IsSignOffResponsiblePersonSigned,
    //        IsUserCombinedTemplate = source.IsUserCombinedTemplate,
    //        IsVersionClear = source.IsVersionClear,
    //        IsAppropriateTitle = source.IsAppropriateTitle,
    //        IsRoute2SapBandsCorrect = source.IsRoute2SapBandsCorrect,
    //        IsPublishDateCorrect = source.IsPublishDateCorrect,



    //        //new questions
    //        IsSoiSameAsWebsite = source.IsSoiSameAsWebsite,
    //        HasMostRecentTemplate = source.HasMostRecentTemplate,
    //        IsPreviousVersionStatusClear = source.IsPreviousVersionStatusClear,
    //        ForScheme = source.ForScheme,
    //        IsPublishedDateCorrect = source.IsPublishedDateCorrect,
    //        IsProxy5PartOfRoute2 = source.IsProxy5PartOfRoute2,
    //        IsProxy5SchemePresent = source.IsProxy5SchemePresent,
    //        IsDescriptionNiceNg6Recomendation2 = source.IsDescriptionNiceNg6Recomendation2,
    //        IsLocalAuthorityNamed = source.IsLocalAuthorityNamed,
    //        IsDelegatedAuthorityNamed = source.IsDelegatedAuthorityNamed,


    //        LocalAuthorityId = source.LocalAuthorityId,
    //        StatementOfIntentId = source.StatementOfIntentId,
    //        StatementOfIntentLink = source.StatementOfIntentLink,
    //        PublishedDate = source.PublishedDate,
    //        Status = source.Status,
    //        CreatedBy = source.CreatedBy,
    //        CreatedDate = source.CreatedDate,
    //        HasDeclarations = source.HasDeclarations,

    //        LocalAuthority = source.LocalAuthority,
    //        DesignatedLas = new List<DesignatedLA>(),
    //        AssessmentNotes = new List<AssessmentNote>(),
    //        InternalNotes = new List<InternalNote>()
    //    };

    //    if (!descend) return soi;

    //    soi.LocalAuthority.DesignatedLas=[];

    //    foreach (var la in source.DesignatedLas ?? new List<DesignatedLA>())
    //    {
    //        soi.DesignatedLas.Add(la.Pared());
    //    }

    //    foreach (var note in source.AssessmentNotes ?? new List<AssessmentNote>())
    //    {
    //        soi.AssessmentNotes.Add(note.Pared());
    //    }

    //    foreach (var note in source.InternalNotes ?? new List<InternalNote>())
    //    {
    //        soi.InternalNotes.Add(note.Pared());
    //    }

    //    return soi;
    //}




    private async Task<IResult> GetSoiByOns(string onsCode, string publishedDate,
        IMediator mediator,
        ILogger<StatementOfIntent> logger)
    {
        logger.LogLafInformation(GetStatementOfIntent);

        var ukCulture = new CultureInfo("en-GB");
        var isDateValid = DateTime.TryParse(publishedDate, ukCulture, DateTimeStyles.None, out var newPublishDate);

        if (!isDateValid)
        {
            logger.LogInformation("Invalid publish date");
            return Results.Problem(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Detail = "Invalid publish date"
            });
        }

        var getStatementOfIntent = new GetSoiByOns
        {
            OnsCode = onsCode,
            PublishedDate = newPublishDate
        };

        var statementOfIntent = await mediator.Send(getStatementOfIntent);

        if (statementOfIntent.IsSuccess)
        {
            logger.LogInformation("Statement of Intent: {StatementOfIntent}", statementOfIntent.Value);
            return TypedResults.Ok(statementOfIntent.Value?.Pared());
        }

        logger.LogError("Error, Unable to fetch the Statement of intent.");

        return Results.Problem(new ProblemDetails
        {
            Status = StatusCodes.Status404NotFound,
            Detail = "Unable to fetch the Statement of intent."
        });
    }

    private async Task<IResult> UpdateBySoiId(HttpRequest httpRequest,
                    [FromBody] StatementOfIntent statementOfIntent,
                    IMediator mediator,
                    ILogger<StatementOfIntent> logger)
    {
        logger.LogLafInformation(UpdateStatementOfIntent);

        if (statementOfIntent.AssessmentNotes?.Count != 0 && statementOfIntent.AssessmentNotes != null)
        {
            foreach (var assessmentNote in statementOfIntent.AssessmentNotes)
            {
                assessmentNote.CreatedDate = DateTime.Now;
                assessmentNote.CreatedByName = httpRequest.Headers["X-UserName"];
            }
        }

        var updateSoiCommand = new UpdateById
        {
            StatementOfIntent = statementOfIntent
        };

        var result = await mediator.Send(updateSoiCommand);

        if (result.IsSuccess)
        {
            return TypedResults.Ok();
        }

        var problemDetail =
            $"Failed to update StatementOfIntent {statementOfIntent.OnsCode}, {statementOfIntent.VersionNumber}.";

        logger.LogLafInformation(UpdateStatementOfIntent, problemDetail);

        return Results.Problem(new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Detail = problemDetail

        });

    }

    private async Task<IResult> UpdateInitialAssessmentChecklist([FromBody] StatementOfIntent statementOfIntent,
        IMediator mediator,
        ILogger<LocalAuthority> logger)
    {
        logger.LogLafInformation(LogEvents.InitialAssessmentChecklist);

        var initialAssessmentChecklistCommand = new InitialAssessmentChecklist
        {
            StatementOfIntent = statementOfIntent
        };

        var result = await mediator.Send(initialAssessmentChecklistCommand);

        if (result.IsSuccess)
            return TypedResults.Ok();

        var problemDetail =
            "Failed to update Initial Assessment Checklist";

        logger.LogLafError(UpdateStatementOfIntent, problemDetail);

        return Results.Problem(new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Detail = problemDetail

        });
    }

    private async Task<IResult> UpdateEligibilityRoute2Proxy5([FromBody] StatementOfIntent soi,
        IMediator mediator,
        ILogger<LocalAuthority> logger)
    {
        logger.LogLafInformation(LogEvents.EligibilityRoute2Proxy5);

        var eligibilityRoute2Proxy5Command = new EligibilityRoute2Proxy5
        {
            StatementOfIntentId = soi.StatementOfIntentId,
            IsProxy5SchemePresent = soi.IsProxy5SchemePresent,
            IsDescriptionNiceNg6Recommendation2 = soi.IsDescriptionNiceNg6Recomendation2
        };

        var result = await mediator.Send(eligibilityRoute2Proxy5Command);

        if (result.IsSuccess)
            return TypedResults.Ok();

        var problemDetail =
            "Failed to update Eligibility for Route 2 Proxy 5";

        logger.LogLafError(UpdateStatementOfIntent, problemDetail);

        return Results.Problem(new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Detail = problemDetail

        });
    }

    private async Task<IResult> UpdateSoiStatusSetting(HttpRequest httpRequest, [FromBody] StatementOfIntent soi,
        IMediator mediator, ILogger<LocalAuthority> logger)
    {
        logger.LogLafInformation(LogEvents.SoiStatusSetting);

        if (soi.AssessmentNotes?.Count != 0 && soi.AssessmentNotes != null)
        {
            foreach (var assessmentNote in soi.AssessmentNotes)
            {
                assessmentNote.CreatedDate = DateTime.Now;
                assessmentNote.CreatedByName = httpRequest.Headers["X-UserName"];
            }
        }

        var updateSoiStatusSettingCommand = new SoiStatusSetting
        {
            StatementOfIntent = soi
        };

        var result = await mediator.Send(updateSoiStatusSettingCommand);

        if (result.IsSuccess)
            return TypedResults.Ok();

        var problemDetail =
            "Failed to update Eligibility for Route 2 Proxy 5";

        logger.LogLafError(UpdateStatementOfIntent, problemDetail);

        return Results.Problem(new ProblemDetails()
        {
            Status = StatusCodes.Status400BadRequest,
            Detail = problemDetail

        });
    }

    private async Task<IResult> UpdateSchemeDetails([FromBody] StatementOfIntent soi,
        IMediator mediator,
        ILogger<LocalAuthority> logger)
    {
        logger.LogLafInformation(LogEvents.SchemeDetails);

        var schemaDetailsCommand = new SchemeDetails()
        {
            StatementOfIntentId = soi.StatementOfIntentId,
            ForScheme = soi.ForScheme,
            IsPublishedDateCorrect = soi.IsPublishedDateCorrect,
            IsProxy5PartOfRoute2 = soi.IsProxy5PartOfRoute2
        };

        var result = await mediator.Send(schemaDetailsCommand);

        if (result.IsSuccess)
            return TypedResults.Ok();

        var problemDetail =
            $"Failed to update Schema Details";

        logger.LogLafError(LogEvents.SchemeDetails, problemDetail);

        return Results.Problem(new ProblemDetails()
        {
            Status = StatusCodes.Status400BadRequest,
            Detail = problemDetail

        });
    }

    private async Task<IResult> AddSoiLog(HttpRequest httpRequest, [FromBody] StatementOfIntentLog statementOfIntentLog,
        IMediator mediator,
        ILogger<LocalAuthority> logger)
    {
        logger.LogLafInformation(LogEvents.AddSoiLog);

        statementOfIntentLog.CreatedByName = httpRequest.Headers["X-UserName"].ToString();

        var soiLogCommand = new AddSoiLog
        {
            StatementOfIntentLog = statementOfIntentLog
        };

        var result = await mediator.Send(soiLogCommand);

        if (result.IsSuccess)
            return TypedResults.Ok();

        var problemDetail =
            "Failed to add soi log";

        logger.LogLafError(UpdateStatementOfIntent, problemDetail);

        return Results.Problem(new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Detail = problemDetail

        });
    }

    private async Task<IResult> GetFilteredSoiLog(Ofgem.LAF.SharedLibrary.Models.UploadFilter filter, IMediator mediator, ILogger<LocalAuthority> logger)
    {
        logger.LogInformation("StatementOfIntentDefinition::GetFilteredSoiLog");

        var getFilteredSoiRequest = new GetFilteredSoiLog
        {
            Filter = filter
        };

        var statementOfIntentLogs = await mediator.Send(getFilteredSoiRequest);

        return TypedResults.Ok(statementOfIntentLogs);
    }
}
