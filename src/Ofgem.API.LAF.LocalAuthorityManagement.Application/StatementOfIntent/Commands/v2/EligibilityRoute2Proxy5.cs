using MediatR;

namespace Ofgem.API.LAF.LocalAuthorityManagement.Application.StatementOfIntent.Commands.v2
{
    public class EligibilityRoute2Proxy5 : IRequest<Result<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent?>>
    {
        public Guid StatementOfIntentId { get; set; }

        public bool? IsProxy5SchemePresent { get; set; }

        public bool? IsDescriptionNiceNg6Recommendation2 { get; set; }
    }
}
