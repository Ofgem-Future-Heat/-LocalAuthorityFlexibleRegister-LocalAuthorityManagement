using MediatR;

namespace Ofgem.API.LAF.LocalAuthorityManagement.Application.StatementOfIntent.Commands
{
    public class UpdateSignOffChecklist : IRequest<Result<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent>>
    {
        public Guid StatementOfIntentId { get; set; }
        public string? OnsCode { get; init; }
        public string? VersionNumber { get; init; }
        public bool? IsSignOffLaOfficerResponsibleStatement { get; init; }
        public bool? IsSignOffResponsiblePersonSigned { get; init; }

    }
}
