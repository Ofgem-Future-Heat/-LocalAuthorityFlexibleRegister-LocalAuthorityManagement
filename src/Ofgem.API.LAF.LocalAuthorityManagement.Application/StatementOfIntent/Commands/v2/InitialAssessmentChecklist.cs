using MediatR;

namespace Ofgem.API.LAF.LocalAuthorityManagement.Application.StatementOfIntent.Commands.v2
{
    public class InitialAssessmentChecklist : IRequest<Result<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent?>>
    {
        public Ofgem.LAF.SharedLibrary.Models.StatementOfIntent StatementOfIntent { get; set; }
    }
}
