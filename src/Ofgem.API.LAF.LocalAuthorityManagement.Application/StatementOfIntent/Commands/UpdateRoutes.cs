using MediatR;

namespace Ofgem.API.LAF.LocalAuthorityManagement.Application.StatementOfIntent.Commands
{
    public class UpdateRoutes : IRequest<Result<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent>>
    {
        public Guid StatementOfIntentId { get; set; }

        public string? OnsCode { get; init; }

        public string? VersionNumber { get; init; }

        public bool? IsUserCombinedTemplate { get; set; }

        public bool? IsOfgemLogoPresent { get; set; }

        public bool? IsVersionClear { get; set; }

        public bool? IsRoute1Accurate { get; init; }

        public bool? IsRoute1SapBandsCorrect { get; init; }

        public bool? IsRoute2Accurate { get; init; }

        public bool? IsRoute2SapBandsCorrect { get; init; }

        public bool? IsProxy5excluded { get; init; }

        public bool? IsProxy5notexcluded { get; init; }

        public bool? IsProxy5Named { get; init; }

        public bool? IsProxy1nad3CannotUsedTogether { get; init; }

        public bool? IsProxy7CannotCombi5or6 { get; init; }

        public bool? IsRoute3Accurate { get; init; }

        public bool? IsRoute3SapBandsCorrect { get; init; }

        public bool? IsRoute4Accurate { get; init; }

        public bool? IsRoute4SapBandsCorrect { get; init; }

        public bool? IsRoute4JointSoIOnlyUseECO4 { get; init; }
    }
}
