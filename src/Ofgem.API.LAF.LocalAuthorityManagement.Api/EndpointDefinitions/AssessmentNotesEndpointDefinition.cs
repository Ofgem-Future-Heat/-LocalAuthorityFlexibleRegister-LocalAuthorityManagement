using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Ofgem.API.LAF.LocalAuthorityManagement.Api.Abstractions;
using Ofgem.API.LAF.LocalAuthorityManagement.Api.Filters;
using Ofgem.API.LAF.LocalAuthorityManagement.Application.AssessmentNote.Commands;
using Ofgem.API.LAF.LocalAuthorityManagement.Application.AssessmentNote.Queries;
using Ofgem.LAF.SharedLibrary.Models;
using System.Diagnostics.CodeAnalysis;
using Ofgem.API.LAF.LocalAuthorityManagement.Application;
using Ofgem.LAF.SharedLibrary.Extensions;

namespace Ofgem.API.LAF.LocalAuthorityManagement.Api.EndpointDefinitions;

[ExcludeFromCodeCoverage]
public class AssessmentNotesEndpointDefinition : IEndpointDefinition
{
    public void RegisterEndpoints(WebApplication app)
    {
        var assessmentNotes = app.MapGroup("/api/assessment-notes");
        assessmentNotes.MapPost("/", Create)
            .AddEndpointFilter<UserNameFilter>()
            .WithOpenApi();
        assessmentNotes.MapGet("/{assessmentNoteId}", Get).WithName("GetAssessmentNotesById").WithOpenApi();

        assessmentNotes.MapDelete("/{assessmentNoteId}", Delete).WithOpenApi();
    }


    private async Task<IResult> Create(HttpRequest httpRequest, CreateAssessmentNoteRequest request, IMediator mediator, ILogger<StatementOfIntent> logger)
    {
        logger.LogLafInformation(LogEvents.CreateProfile);

        if (request is null) return Results.BadRequest("CreateAssessmentNoteRequest is null");
        if (string.IsNullOrEmpty(request.Text)) return Results.BadRequest("CreateAssessmentNoteRequest Text is empty");

        Create createRequest = new()
        {
            StatementOfIntentId = request.StatementOfIntentId,
            Text = request.Text,
            CreatedBy = httpRequest.Headers["X-UserName"]
        };

        var createdAssessmentNote = await mediator.Send(createRequest);

        return Results.CreatedAtRoute("GetAssessmentNotesById", new { assessmentNoteId = createdAssessmentNote.AssessmentNoteId }, createdAssessmentNote);
    }

    private async Task<IResult> Get(Guid assessmentNoteId, IMediator mediator, ILogger<StatementOfIntent> logger)
    {
        logger.LogLafInformation(LogEvents.GetProfile);

        Get getAssessmentNoteRequest = new() { AssessmentNoteId = assessmentNoteId };
        var assessmentNote = await mediator.Send(getAssessmentNoteRequest);

        return TypedResults.Ok(assessmentNote);
    }

    private async Task<NoContent> Delete(Guid assessmentNoteId, IMediator mediator, ILogger<AssessmentNote> logger)
    {
        logger.LogLafInformation(LogEvents.DeleteProfile);

        Delete deleteAssessmentNoteCommand = new() { AssessmentNoteId = assessmentNoteId };
        await mediator.Send(deleteAssessmentNoteCommand);

        return TypedResults.NoContent();
    }
}

