using MediatR;

namespace Ofgem.API.LAF.LocalAuthorityManagement.Application.StatementOfIntent.Commands
{
    public class UpdateAssessmentChecklist : IRequest<Result<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent>>
    {
        public Guid StatementOfIntentId { get; set; }

        public string? OnsCode { get; init; }

        public string? VersionNumber { get; init; }

        public bool? IsUserCombinedTemplate { get; init; }

        public bool? IsOfgemLogoPresent { get; init; }

        public bool? IsVersionClear { get; init; }

        public bool? IsAppropriateTitle { get; set; }

        public bool? IsPublishDateCorrect { get; set; }

    }
}
